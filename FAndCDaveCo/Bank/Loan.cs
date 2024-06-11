using System;

namespace tesinormed.FAndCDaveCo.Bank;

[ES3Serializable]
public record Loan
{
	[ES3NonSerializable] public static readonly Loan None = new(0, 0);

	public int IssuanceDate;
	[ES3NonSerializable] public int DaysSinceIssuance => StartOfRound.Instance.gameStats.daysSpent - IssuanceDate;

	public int Principal;
	[ES3NonSerializable] public int Interest => (int) (Principal * Plugin.Config.InterestAmount);
	[ES3NonSerializable] public int Total => Principal + Interest;

	public int AmountPaid;
	[ES3NonSerializable] public int AmountUnpaid => Total - AmountPaid;

	public Loan(int issuanceDate, int principal, int amountPaid = 0)
	{
		if (issuanceDate < 0) throw new ArgumentOutOfRangeException(nameof(issuanceDate), "nonnegative number required");
		if (principal < 0) throw new ArgumentOutOfRangeException(nameof(principal), "nonnegative number required");
		if (amountPaid < 0) throw new ArgumentOutOfRangeException(nameof(amountPaid), "nonnegative number required");

		IssuanceDate = issuanceDate;
		Principal = principal;
		AmountPaid = amountPaid;
	}
}
