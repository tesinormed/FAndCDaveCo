using InteractiveTerminalAPI.UI;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Extensions;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo.Bank.UI;

public class BankLoanGetTerminal : InteractiveTerminalApplication
{
	protected override string Title => $"{BankTerminal.Title}: Get a loan";

	public override void Initialization()
	{
		if (!(StartOfRound.Instance.inShipPhase && GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom))
		{
			LockedNotification(TextElement.Create("You can only get a loan while you are in orbit."));
			Plugin.Logger.LogDebug("local player tried to get a loan when not in orbit");
			return;
		}

		if (StartOfRound.Instance.isChallengeFile)
		{
			LockedNotification(TextElement.Create("You cannot get a loan while doing a challenge moon."));
			Plugin.Logger.LogDebug("local player tried to get a loan while doing a challenge moon");
			return;
		}

		if (Plugin.BankState.Loan.Principal != 0)
		{
			LockedNotification(TextElement.Create("You can only have one loan at a time."));
			Plugin.Logger.LogDebug("local player tried to get a loan when there already was a loan");
			return;
		}

		Loan loan = new(issuanceDate: 0 /* ignored */, principal: TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled);
		Confirm(() => GetLoan(loan), TextElement.Create($"Are you sure you want to get a loan for ${loan.Principal}? The total cost will be ${loan.Total}."));
	}

	private void GetLoan(Loan loan)
	{
		// take out a loan and set quota fulfillment
		LethalClientEvent takeOutLoan = new(CreditEvents.TakeOutLoan);
		takeOutLoan.InvokeServer();

		LockedNotification(TextElement.Create($"You have successfully taken out a loan for ${loan.Principal}."));
	}
}
