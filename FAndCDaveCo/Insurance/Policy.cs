using System;
using System.Text;

namespace tesinormed.FAndCDaveCo.Insurance;

public record struct Policy
{
	[ES3NonSerializable] public static readonly Policy None = new(PolicyTier.None, 0);

	public PolicyTier Tier;
	public int Coverage;

	public Policy(PolicyTier tier, int coverage)
	{
		if (coverage < 0) throw new ArgumentOutOfRangeException(nameof(coverage), "nonnegative number required");

		Tier = tier;
		Coverage = coverage;
	}

	[ES3NonSerializable] public int BasePremium => Tier switch
	{
		PolicyTier.HighDeductible => (int) Math.Floor(Coverage * Plugin.Instance.Config.EconomicBasePremium.Value),
		PolicyTier.LowDeductible => (int) Math.Floor(Coverage * Plugin.Instance.Config.StandardBasePremium.Value),
		PolicyTier.NoDeductible => (int) Math.Floor(Coverage * Plugin.Instance.Config.BespokeBasePremium.Value),
		_ => 0
	};

	[ES3NonSerializable] public double DeductiblePercent => Tier switch
	{
		PolicyTier.HighDeductible => Plugin.Instance.Config.EconomicDeductible.Value,
		PolicyTier.LowDeductible => Plugin.Instance.Config.StandardDeductible.Value,
		_ => 0.00
	};
	[ES3NonSerializable] public int DeductibleMinimum => Tier switch
	{
		PolicyTier.HighDeductible => (int) Math.Floor(Coverage * Plugin.Instance.Config.EconomicDeductibleMinimum.Value),
		PolicyTier.LowDeductible => (int) Math.Floor(Coverage * Plugin.Instance.Config.StandardDeductibleMinimum.Value),
		_ => 0
	};
	[ES3NonSerializable] public int DeductibleMaximum => Tier switch
	{
		PolicyTier.HighDeductible => (int) Math.Floor(Coverage * Plugin.Instance.Config.EconomicDeductibleMaximum.Value),
		PolicyTier.LowDeductible => (int) Math.Floor(Coverage * Plugin.Instance.Config.StandardDeductibleMaximum.Value),
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

	public override string ToString()
	{
		var builder = new StringBuilder();
		builder.Append($"{nameof(Policy)} {{ ");
		builder.Append($"{nameof(Tier)} = {Tier}, ");
		builder.Append($"{nameof(Coverage)} = {Coverage}");
		builder.Append(" }");
		return builder.ToString();
	}
}

public static class PolicyExtensions
{
	public static Policy CreatePolicy(this PolicyTier policyTier, int coverage) => new(policyTier, coverage);
}
