using LethalModDataLib.Base;
using LethalNetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tesinormed.FAndCDaveCo.Insurance
{
	[Serializable]
	public class PolicyState : ModDataContainer
	{
		public const string POLICY_NETWORK_IDENTIFIER = "Policy";
		public const string CLAIMS_NETWORK_IDENTIFIER = "Claims";
		public static PolicyState Instance = new();
		public static LethalNetworkVariable<T> CreateLethalNetworkVariable<T>(string identifier, params Action<T>[] onValueChanged)
		{
			LethalNetworkVariable<T> variable = new(identifier);
			foreach (var action in onValueChanged) { variable.OnValueChanged += action; }
			return variable;
		}
		public static readonly LethalNetworkVariable<Policy> SyncedPolicy = CreateLethalNetworkVariable<Policy>(
			identifier: POLICY_NETWORK_IDENTIFIER,
			(data) => { Instance.Policy = data; }
		);
		public static readonly LethalNetworkVariable<Dictionary<int, PolicyClaim>> SyncedClaims = CreateLethalNetworkVariable<Dictionary<int, PolicyClaim>>(
			identifier: CLAIMS_NETWORK_IDENTIFIER,
			(data) => { Instance.Claims = data; }
		);

		public Policy Policy { get; set; } = Policy.NONE;
		public Dictionary<int, PolicyClaim> Claims { get; set; } = [];
		public Dictionary<int, PolicyClaim> ClaimedClaims => Claims
			.Where(pair => pair.Value.Claimed)
			.ToDictionary();
		public Dictionary<int, PolicyClaim> UnclaimedClaims => Claims
			.Where(pair => !pair.Value.Claimed)
			.ToDictionary();

		public PolicyState() { }

		public PolicyState(Policy policy, Dictionary<int, PolicyClaim> claims)
		{
			Policy = policy;
			Claims = claims;
		}

		public double FractionalPremiumIncrease => ClaimedClaims.Count * 0.15;
		public int TotalPremium => (int) (Policy.BasePremium * (1.00 + FractionalPremiumIncrease));

		public void Reset()
		{
			Policy = Policy.NONE;
			Claims = [];
		}

		public static void Resync()
		{
			SyncedPolicy.Value = Instance.Policy;
			SyncedClaims.Value = Instance.Claims;
		}
	}
}
