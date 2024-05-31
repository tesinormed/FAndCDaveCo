using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Screen;
using System;
using System.Linq;

namespace tesinormed.FAndCDaveCo.Banking.UI
{
	public class BankLoanListTerminal : TerminalApplication
	{
		private const string TITLE = $"{BankTerminal.TITLE}: List of loans";

		public override void Initialization()
		{
			ITextElement[] textElements;
			if (Plugin.BankState.Loans.Count == 0)
			{
				textElements = [TextElement.Create($"You do not currently have any loans.")];
			}
			else
			{
				textElements = [
					TextElement.Create($"You currently have {Plugin.BankState.Loans.Count} unpaid loans."),
					TextElement.Create($"These loans total ${Plugin.BankState.Loans.Values.Sum()}."),
				];
			}
			var screen = BoxedScreen.Create(title: TITLE, elements: textElements);

			currentScreen = screen;
		}

		protected override string GetApplicationText() => currentScreen.GetText(51);

		protected override Action PreviousScreen() => () => { };
	}
}
