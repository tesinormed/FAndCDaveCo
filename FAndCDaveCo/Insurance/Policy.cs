using System;

namespace tesinormed.FAndCDaveCo.Insurance
{
	[ES3Serializable]
	public record Policy
	{
		[ES3NonSerializable]
		public static readonly Policy NONE = new(PolicyTier.NONE, 0);

		public PolicyTier Tier;
		public int Coverage;

		public Policy(PolicyTier tier, int coverage)
		{
			if (coverage < 0) throw new ArgumentOutOfRangeException(nameof(coverage), $"coverage cannot be less than zero");

			Tier = tier;
			Coverage = coverage;
		}

		[ES3NonSerializable]
		public int BasePremium
		{
			get
			{
				return Tier switch
				{
					PolicyTier.HIGH_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.10), // 10% of coverage
					PolicyTier.LOW_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.20), // 20% of coverage
					PolicyTier.NO_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.35), // 35% of coverage
					_ => 0
				};
			}
		}

		[ES3NonSerializable]
		public double DeductiblePercent => Tier switch
		{
			PolicyTier.HIGH_DEDUCTIBLE => 0.30, // 30%
			PolicyTier.LOW_DEDUCTIBLE => 0.15, // 15%
			PolicyTier.NO_DEDUCTIBLE => 0.00, // 0%
			_ => 0.00,
		};
		[ES3NonSerializable]
		public int DeductibleMinimum => Tier switch
		{
			PolicyTier.HIGH_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.20), // 20% of coverage
			PolicyTier.LOW_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.10), // 10% of coverage
			PolicyTier.NO_DEDUCTIBLE => 0,
			_ => 0,
		};
		[ES3NonSerializable]
		public int DeductibleMaximum => Tier switch
		{
			PolicyTier.HIGH_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.50), // 50% of coverage
			PolicyTier.LOW_DEDUCTIBLE => (int) Math.Floor(Coverage * 0.25), // 25% of coverage
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

			// deductible percent times payout, clamped between minimum and maximum
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
