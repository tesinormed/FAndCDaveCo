using System;

namespace tesinormed.FAndCDaveCo.Insurance
{
	public enum PolicyTier : byte
	{
		NONE,
		HIGH_DEDUCTIBLE,
		LOW_DEDUCTIBLE,
		NO_DEDUCTIBLE
	}

	public static class PolicyTierExtensions
	{
		public static string ToFriendlyString(this PolicyTier policyTier) => policyTier switch
		{
			PolicyTier.HIGH_DEDUCTIBLE => "Economic",
			PolicyTier.LOW_DEDUCTIBLE => "Standard",
			PolicyTier.NO_DEDUCTIBLE => "Bespoke",
			PolicyTier.NONE => "No Policy?",
			_ => throw new ArgumentOutOfRangeException(nameof(policyTier), $"unexpected PolicyTier: {policyTier}"),
		};
	}
}
