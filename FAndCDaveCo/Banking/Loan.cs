using System;

namespace tesinormed.FAndCDaveCo.Banking
{
	[ES3Serializable]
	public record Loan
	{
		public readonly int Amount;
		public int AmountPaid;
		[ES3NonSerializable]
		public int AmountUnpaid => Amount - AmountPaid;

		public readonly double Interest;

		public Loan(int amount, double interest, int amountPaid = 0)
		{
			if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), $"amount cannot be less than zero");
			if (interest < 0) throw new ArgumentOutOfRangeException(nameof(interest), $"interest cannot be less than zero");
			if (amountPaid < 0) throw new ArgumentOutOfRangeException(nameof(amountPaid), $"amount paid cannot be less than zero");

			Amount = amount;
			Interest = interest;
			AmountPaid = amountPaid;
		}
	}
}
