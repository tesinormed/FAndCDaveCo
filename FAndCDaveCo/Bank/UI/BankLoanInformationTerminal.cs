using System.Collections.Generic;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Screen;
using tesinormed.FAndCDaveCo.Extensions;

namespace tesinormed.FAndCDaveCo.Bank.UI;

public class BankLoanInformationTerminal : TerminalApplication
{
	public override void Initialization()
	{
		List<ITextElement> textElements = [];
		if (Plugin.Instance.State.Loan.Principal == 0)
		{
			textElements.Add(TextElement.Create("You do not currently have an unpaid loan."));
		}
		else
		{
			textElements.Add(TextElement.Create($"You currently have an unpaid loan from {Plugin.Instance.State.Loan.DaysSinceIssuance} day(s) ago (day {Plugin.Instance.State.Loan.IssuanceDate})."));
			textElements.Add(TextElement.Create($"The loan is for ${Plugin.Instance.State.Loan.Principal} with ${Plugin.Instance.State.Loan.Interest} added as interest."));
			textElements.Add(TextElement.Create($"Amount paid: ${Plugin.Instance.State.Loan.AmountPaid}, amount left: ${Plugin.Instance.State.Loan.AmountUnpaid}, total: ${Plugin.Instance.State.Loan.Total}."));
			if (Plugin.Instance.State.Loan.DaysSinceIssuance >= Plugin.Instance.Config.PenaltyStartDaysFromIssuance.Value)
			{
				textElements.Add(TextElement.Create($"{(int) (Plugin.Instance.Config.PenaltyAmount.Value * 100)}% of your credits are currently being garnished each day."));
			}
		}

		var screen = BoxedScreen.Create($"{BankTerminal.Title}: Loan information", textElements.ToArray());
		currentScreen = screen;
	}
}
