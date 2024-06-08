using System.Collections.Generic;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Screen;
using tesinormed.FAndCDaveCo.Extensions;

namespace tesinormed.FAndCDaveCo.Bank.UI;

public class BankLoanInformationTerminal : TerminalApplicationExtension
{
	public override void Initialization()
	{
		List<ITextElement> textElements = [];
		if (Plugin.BankState.Loan.Amount == 0)
		{
			textElements.Add(TextElement.Create("You do not currently have an unpaid loan."));
		}
		else
		{
			textElements.Add(TextElement.Create("You currently have an unpaid loan."));
			textElements.Add(TextElement.Create($"The loan is for ${Plugin.BankState.Loan.Amount} (amount paid: ${Plugin.BankState.Loan.AmountPaid}, amount left: ${Plugin.BankState.Loan.AmountUnpaid})."));

			if (Plugin.BankState.Loan.DaysSinceIssuance >= Plugin.Config.PenaltyStartDaysFromIssuance)
				textElements.Add(TextElement.Create($"{(int) (Plugin.Config.PenaltyAmount * 100)}% of your credits are currently being garnished each day."));
		}

		var screen = BoxedScreen.Create($"{BankTerminal.Title}: Loan information", textElements.ToArray());
		currentScreen = screen;
	}
}
