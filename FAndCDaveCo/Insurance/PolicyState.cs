using System.Collections.Generic;
using System.Linq;
using LethalModDataLib.Attributes;
using LethalModDataLib.Base;
using tesinormed.FAndCDaveCo.Extensions;

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
}
