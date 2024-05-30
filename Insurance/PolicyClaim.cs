using System;

namespace tesinormed.FAndCDaveCo.Insurance
{
	[Serializable]
	public record PolicyClaim
	{
		public readonly int Value;
		public readonly bool Claimed;

		public PolicyClaim(int value, bool claimed = false)
		{
			if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), $"value cannot be less than zero");

			Value = value;
			Claimed = claimed;
		}
	}
}
