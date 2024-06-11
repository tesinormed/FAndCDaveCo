﻿using System;

namespace tesinormed.FAndCDaveCo.Insurance;

[ES3Serializable]
public record PolicyClaim
{
	public bool Claimed;
	public int Value;

	public PolicyClaim(int value, bool claimed = false)
	{
		if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "nonnegative number required");

		Value = value;
		Claimed = claimed;
	}
}
