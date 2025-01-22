using System;
using System.Text;

namespace tesinormed.FAndCDaveCo.Bank;

public record struct Loan
{
	[ES3NonSerializable] public static readonly Loan None = new(0, 0);

	public int IssuanceDate;
	[ES3NonSerializable] public int DaysSinceIssuance => StartOfRound.Instance.gameStats.daysSpent - IssuanceDate;

	public int Principal;
	[ES3NonSerializable] public int Interest => CalculateInterest(Principal);
	public static int CalculateInterest(int principal) => (int) (principal * Plugin.Instance.Config.InterestAmount.Value);
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

	public override string ToString()
	{
		var builder = new StringBuilder();
		builder.Append($"{nameof(Loan)} {{ ");
		builder.Append($"{nameof(IssuanceDate)} = {IssuanceDate}, ");
		builder.Append($"{nameof(Principal)} = {Principal}, ");
		builder.Append($"{nameof(AmountPaid)} = {AmountPaid}");
		builder.Append(" }");
		return builder.ToString();
	}
}
