using InteractiveTerminalAPI.UI;
using tesinormed.FAndCDaveCo.Extensions;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo.Bank.UI;

public class BankLoanGetTerminal : InteractiveTerminalApplication
{
	protected override string Title => $"{BankTerminal.Title}: Take out a loan";

	public override void Initialization()
	{
		if (!(StartOfRound.Instance.inShipPhase && GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom))
		{
			LockedNotification(TextElement.Create("You can only take out a loan while you are in orbit."));
			return;
		}

		if (StartOfRound.Instance.isChallengeFile)
		{
			LockedNotification(TextElement.Create("You cannot take out a loan while doing a challenge moon."));
			return;
		}

		if (Plugin.Instance.State.Loan.Principal != 0)
		{
			LockedNotification(TextElement.Create("You can only take out one loan at a time."));
			return;
		}

		if (TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled <= 0)
		{
			LockedNotification(TextElement.Create("Your quota is already fulfilled."));
			return;
		}

		var principal = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
		Confirm(
			() => GetLoan(principal),
			TextElement.Create($"Are you sure you want to take out a loan for ${principal}? The total cost will be ${principal + Loan.CalculateInterest(principal)}.")
		);
	}

	private void GetLoan(int principal)
	{
		// take out a loan and set quota fulfillment
		CreditEvents.TakeOutLoan.InvokeServer();

		LockedNotification(TextElement.Create($"You have successfully taken out a loan for ${principal}."));
	}
}
