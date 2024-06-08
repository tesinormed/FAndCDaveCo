using System;

namespace tesinormed.FAndCDaveCo.Insurance;

public enum PolicyTier : byte
{
	None,
	HighDeductible,
	LowDeductible,
	NoDeductible
}

public static class PolicyTierExtensions
{
	public static string ToFriendlyString(this PolicyTier policyTier) => policyTier switch
	{
		PolicyTier.HighDeductible => "Economic",
		PolicyTier.LowDeductible => "Standard",
		PolicyTier.NoDeductible => "Bespoke",
		PolicyTier.None => "No Policy?",
		_ => throw new ArgumentOutOfRangeException(nameof(policyTier), $"unexpected PolicyTier: {policyTier}")
	};
}
