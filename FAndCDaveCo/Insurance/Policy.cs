using System;

namespace tesinormed.FAndCDaveCo.Insurance;

[ES3Serializable]
public record Policy
{
	[ES3NonSerializable] public static readonly Policy None = new(PolicyTier.None, 0);

	public int Coverage;
	public PolicyTier Tier;

	public Policy(PolicyTier tier, int coverage)
	{
		if (coverage < 0) throw new ArgumentOutOfRangeException(nameof(coverage), "nonnegative number required");

		Tier = tier;
		Coverage = coverage;
	}

	[ES3NonSerializable] public int BasePremium => Tier switch
	{
		PolicyTier.HighDeductible => (int) Math.Floor(Coverage * Plugin.Config.EconomicBasePremium),
		PolicyTier.LowDeductible => (int) Math.Floor(Coverage * Plugin.Config.StandardBasePremium),
		PolicyTier.NoDeductible => (int) Math.Floor(Coverage * Plugin.Config.BespokeBasePremium),
		_ => 0
	};

	[ES3NonSerializable] public double DeductiblePercent => Tier switch
	{
		PolicyTier.HighDeductible => Plugin.Config.EconomicDeductible,
		PolicyTier.LowDeductible => Plugin.Config.StandardDeductible,
		_ => 0.00
	};
	[ES3NonSerializable] public int DeductibleMinimum => Tier switch
	{
		PolicyTier.HighDeductible => (int) Math.Floor(Coverage * Plugin.Config.EconomicDeductibleMinimum),
		PolicyTier.LowDeductible => (int) Math.Floor(Coverage * Plugin.Config.StandardDeductibleMinimum),
		_ => 0
	};
	[ES3NonSerializable] public int DeductibleMaximum => Tier switch
	{
		PolicyTier.HighDeductible => (int) Math.Floor(Coverage * Plugin.Config.EconomicDeductibleMaximum),
		PolicyTier.LowDeductible => (int) Math.Floor(Coverage * Plugin.Config.StandardDeductibleMaximum),
		_ => 0
	};

	public int CalculatePayout(int value)
	{
		if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "nonnegative number required");

		return Math.Min(value, Coverage);
	}

	public int CalculateDeductible(int value)
	{
		if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "nonnegative number required");

		return Math.Clamp(
			(int) Math.Floor(CalculatePayout(value) * DeductiblePercent),
			DeductibleMinimum,
			DeductibleMaximum
		);
	}
}

public static class PolicyExtensions
{
	public static Policy CreatePolicy(this PolicyTier policyTier, int coverage) => new(policyTier, coverage);
}
