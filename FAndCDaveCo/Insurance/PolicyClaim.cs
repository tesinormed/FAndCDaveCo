using System;

namespace tesinormed.FAndCDaveCo.Insurance;

[ES3Serializable]
public record PolicyClaim
{
	public readonly bool Claimed;
	public readonly int Value;

	public PolicyClaim(int value, bool claimed = false)
	{
		if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "value cannot be less than zero");

		Value = value;
		Claimed = claimed;
	}
}
