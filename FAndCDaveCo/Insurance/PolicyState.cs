using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LethalModDataLib.Attributes;
using LethalModDataLib.Base;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Extensions;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo.Insurance;

public class PolicyState : ModDataContainer
{
	public Policy Policy { get; set; } = Policy.None;
	public Dictionary<int, PolicyClaim> Claims { get; set; } = [];

	[ModDataIgnore] public Dictionary<int, PolicyClaim> ClaimedClaims => Claims.Where(pair => pair.Value.Claimed).ToDictionary();
	[ModDataIgnore] public Dictionary<int, PolicyClaim> UnclaimedClaims => Claims.Where(pair => !pair.Value.Claimed).ToDictionary();

	[ModDataIgnore] public double FractionalPremiumIncrease => ClaimedClaims.Count * Plugin.Config.ClaimPremiumIncrease;
	[ModDataIgnore] public int TotalPremium => (int) (Policy.BasePremium * (1.00 + FractionalPremiumIncrease));

	protected override void PostSave()
	{
		Plugin.Logger.LogDebug($"saved policy state {this}");
	}
	protected override void PostLoad()
	{
		Plugin.Logger.LogDebug($"loaded policy state {this}");

		// sync over network
		Plugin.SyncedPolicy.Value = Policy;
		Plugin.SyncedClaims.Value = Claims;
	}

	public void SetAndSyncPolicy(Policy policy)
	{
		Policy = policy;

		// sync over network
		LethalClientMessage<Policy> updatePolicy = new(NetworkVariableEvents.UpdatePolicyIdentifier);
		updatePolicy.SendServer(policy);
	}

	public void UpdateAndSyncClaims(Action<Dictionary<int, PolicyClaim>> action)
	{
		action.Invoke(Claims);

		// sync over network
		LethalClientMessage<Dictionary<int, PolicyClaim>> updateClaims = new(NetworkVariableEvents.UpdateClaimsIdentifier);
		updateClaims.SendServer(Claims);
	}

	public void ResetAndSync()
	{
		SetAndSyncPolicy(Policy.None);
		UpdateAndSyncClaims(claims => claims.Clear());
	}

	public static bool DisableDeathCreditPenalty => Plugin.PolicyState.Policy.Tier != PolicyTier.None ? Plugin.Config.GameDisableDeathCreditPenalty : false;

	public override string ToString()
	{
		var stringBuilder = new StringBuilder();

		stringBuilder.Append("PolicyState(");
		stringBuilder.Append($"policy: {Policy}");
		if (Claims.Count > 0) stringBuilder.Append($", claims: [{string.Join(", ", Claims)}]");
		stringBuilder.Append(")");

		return stringBuilder.ToString();
	}
}
