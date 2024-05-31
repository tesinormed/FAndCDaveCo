using LethalModDataLib.Attributes;
using LethalModDataLib.Base;
using LethalModDataLib.Enums;
using System.Collections.Generic;
using System.Linq;
using tesinormed.FAndCDaveCo.Misc;

namespace tesinormed.FAndCDaveCo.Insurance
{
	[ES3Serializable]
	public class PolicyState : ModDataContainer
	{
		[ModDataIgnore(IgnoreFlags.None)]
		public const string POLICY_NETWORK_IDENTIFIER = "Policy";
		[ModDataIgnore(IgnoreFlags.None)]
		public const string CLAIMS_NETWORK_IDENTIFIER = "Claims";

		public Policy Policy { get; set; } = Policy.NONE;
		public Dictionary<int, PolicyClaim> Claims { get; set; } = [];
		[ModDataIgnore(IgnoreFlags.None)]
		public Dictionary<int, PolicyClaim> ClaimedClaims => Claims
			.Where(pair => pair.Value.Claimed)
			.ToDictionary();
		[ModDataIgnore(IgnoreFlags.None)]
		public Dictionary<int, PolicyClaim> UnclaimedClaims => Claims
			.Where(pair => !pair.Value.Claimed)
			.ToDictionary();

		[ModDataIgnore(IgnoreFlags.None)]
		public double FractionalPremiumIncrease => ClaimedClaims.Count * 0.15;
		[ModDataIgnore(IgnoreFlags.None)]
		public int TotalPremium => (int) (Policy.BasePremium * (1.00 + FractionalPremiumIncrease));

		public PolicyState() { }
		public PolicyState(Policy policy, Dictionary<int, PolicyClaim> claims)
		{
			Policy = policy;
			Claims = claims;
		}

		public void Reset()
		{
			Policy = Policy.NONE;
			Claims = [];
			PostReset();
		}

		protected override void PostSave()
		{
			Plugin.Logger.LogDebug($"saved policy state {this}");
		}
		protected override void PostLoad()
		{
			Plugin.Logger.LogDebug($"loaded policy state {this}");
		}
		private void PostReset()
		{
			Plugin.Logger.LogDebug("reset policy state");
		}
	}
}
