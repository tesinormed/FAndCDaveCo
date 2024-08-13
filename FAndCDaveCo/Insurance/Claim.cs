using System;
using System.Text;

namespace tesinormed.FAndCDaveCo.Insurance;

[ES3Serializable]
public record Claim
{
	public int Value;
	public bool Claimed;

	public Claim(int value, bool claimed = false)
	{
		if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "nonnegative number required");

		Value = value;
		Claimed = claimed;
	}

	public override string ToString()
	{
		var builder = new StringBuilder();

		builder.Append($"{nameof(Claim)} {{ ");
		builder.Append($"{nameof(Value)} = {Value}, ");
		builder.Append($"{nameof(Claimed)} = {Claimed}");
		builder.Append(" }");

		return builder.ToString();
	}
}
