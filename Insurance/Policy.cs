using System;

namespace tesinormed.FAndCDaveCo.Insurance
{
	[Serializable]
	public record Policy
	{
		public static readonly Policy NONE = new(PolicyTier.NONE, 0);

		public PolicyTier Tier;
		public int Coverage;

		public Policy(PolicyTier tier, int coverage)
		{
			if (coverage < 0) throw new ArgumentOutOfRangeException(nameof(Coverage), $"coverage cannot be less than zero");

			Tier = tier;
			Coverage = coverage;
		}

		public int BasePremium
		{
			get
			{
				return Tier switch
				{
					PolicyTier.HIGH_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.10),
					PolicyTier.LOW_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.20),
					PolicyTier.NO_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.35),
					_ => 0
				};
			}
		}

		public double DeductiblePercent => Tier switch
		{
			PolicyTier.HIGH_DEDUCTIBLE => 0.30,
			PolicyTier.LOW_DEDUCTIBLE => 0.15,
			PolicyTier.NO_DEDUCTIBLE => 0.00,
			_ => 0.00,
		};
		public int DeductibleMinimum => Tier switch
		{
			PolicyTier.HIGH_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.20),
			PolicyTier.LOW_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.10),
			PolicyTier.NO_DEDUCTIBLE => 0,
			_ => 0,
		};
		public int DeductibleMaximum => Tier switch
		{
			PolicyTier.HIGH_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.50),
			PolicyTier.LOW_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.25),
			PolicyTier.NO_DEDUCTIBLE => 0,
			_ => 0,
		};

		public int CalculatePayout(int value)
		{
			if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero");

			return Math.Min(value, Coverage);
		}
		public int CalculateDeductible(int value)
		{
			if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero");

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
}
