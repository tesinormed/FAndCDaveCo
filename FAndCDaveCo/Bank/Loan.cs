using System;

namespace tesinormed.FAndCDaveCo.Bank;

[ES3Serializable]
public record Loan
{
	[ES3NonSerializable] public static readonly Loan None = new(0, 0);

	public int IssuanceDate;
	[ES3NonSerializable] public int DaysSinceIssuance => StartOfRound.Instance.gameStats.daysSpent - IssuanceDate;

	public int Amount;
	public int AmountPaid;
	[ES3NonSerializable] public int AmountUnpaid => Amount - AmountPaid;

	public Loan(int issuanceDate, int amount, int amountPaid = 0)
	{
		if (issuanceDate < 0) throw new ArgumentOutOfRangeException(nameof(issuanceDate), "issuance date cannot be less than zero");
		if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "amount cannot be less than zero");
		if (amountPaid < 0) throw new ArgumentOutOfRangeException(nameof(amountPaid), "amount paid cannot be less than zero");

		IssuanceDate = issuanceDate;
		Amount = amount;
		AmountPaid = amountPaid;
	}
}
