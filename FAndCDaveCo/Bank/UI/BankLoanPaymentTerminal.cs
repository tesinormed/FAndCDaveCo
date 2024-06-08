using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Cursor;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Extensions;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo.Bank.UI;

public class BankLoanPaymentTerminal : InteractiveTerminalApplicationExtension
{
	protected override string Title => $"{BankTerminal.Title}: Submit loan payment";

	private CursorElement CreateCursorElement(double amount) => CursorElement.Create
	(
		name: $"{(int) (amount * 100)}%",
		action: () => ConfirmPayLoan((int) (Plugin.BankState.Loan.Total * amount)),
		active: _ => Plugin.Terminal.groupCredits >= (int) (Plugin.BankState.Loan.Total * amount),
		selectInactive: true
	);

	public override void Initialization()
	{
		if (Plugin.BankState.Loan.Principal == 0)
		{
			LockedNotification(TextElement.Create("You can only submit a loan payment if you have a loan."));
			Plugin.Logger.LogDebug("local player tried to submit a loan payment when there was no loan");
			return;
		}

		(MainScreen, MainCursorMenu) = Selection(
			prompt: "Pick an amount to pay towards the loan.",
			CreateCursorElement(0.10),
			CreateCursorElement(0.25),
			CreateCursorElement(0.50),
			CreateCursorElement(0.75),
			CreateCursorElement(1.00)
		);
	}

	private void ConfirmPayLoan(int amount)
	{
		if (Plugin.Terminal.groupCredits < amount)
		{
			Error(backAction: PreviousScreenAction, TextElement.Create($"You do not have enough credits to submit a payment of ${amount} for the loan."));
			return;
		}

		Confirm(
			confirmAction: () => PayLoan(amount),
			declineAction: PreviousScreenAction,
			TextElement.Create($"Are you sure you want to pay ${amount}?")
		);
	}

	private void PayLoan(int amount)
	{
		// deduct credits
		LethalClientMessage<int> deductGroupCredits = new(CreditEvents.DeductGroupCreditsIdentifier);
		deductGroupCredits.SendServer(amount);
		// add to paid amount of loan
		Plugin.BankState.Loan.AmountPaid += amount;
		Plugin.Logger.LogDebug($"paid {amount} towards the current loan");
		// check if loan has been fully paid
		if (Plugin.BankState.Loan.AmountUnpaid == 0)
		{
			Plugin.BankState.Loan = Loan.None;
			LockedNotification(TextElement.Create("You have paid off your loan fully."));
			Plugin.Logger.LogDebug("loan paid off fully; removing");
		}
		else
		{
			LockedNotification(TextElement.Create($"You have paid ${amount}; ${Plugin.BankState.Loan.AmountUnpaid} must still be paid."));
		}

		// sync over network
		LethalClientMessage<Loan> updateLoan = new(NetworkVariableEvents.UpdateLoanIdentifier);
		updateLoan.SendServer(Plugin.BankState.Loan);
	}
}
