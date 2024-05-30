using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Screen;
using System;

namespace tesinormed.FAndCDaveCo.Insurance.UI
{
	public class PolicyInformationTerminal : TerminalApplication
	{
		private const string TITLE = $"{PolicyTerminal.TITLE}: Policy information";

		public override void Initialization()
		{
			ITextElement[] textElements;
			if (PolicyState.Instance.Policy.Tier == PolicyTier.NONE)
			{
				textElements = [TextElement.Create($"You do not currently have a policy with {PolicyTerminal.TITLE}.")];
			}
			else
			{
				textElements = [
					TextElement.Create($"You currently have the {PolicyState.Instance.Policy.Tier.ToFriendlyString()} policy (${PolicyState.Instance.Policy.Coverage})."),
					TextElement.Create($"You have made {PolicyState.Instance.ClaimedClaims.Count} claims recently (within 5 days)."),
				];

				if (PolicyState.Instance.FractionalPremiumIncrease == 0.00)
				{
					textElements = [.. textElements, TextElement.Create($"You pay ${PolicyState.Instance.TotalPremium} per day.")];
				}
				else
				{
					textElements =
					[
						.. textElements,
						TextElement.Create($"You pay ${PolicyState.Instance.TotalPremium} (+{(int) (PolicyState.Instance.FractionalPremiumIncrease * 100)}%) per day."),
					];
				}

				if (PolicyState.Instance.Policy.DeductiblePercent == 0.00)
				{
					textElements = [.. textElements, TextElement.Create("You do not pay deductibles.")];
				}
				else
				{
					textElements =
					[
						.. textElements,
						TextElement.Create($"Your deductible is currently {(int) (PolicyState.Instance.Policy.DeductiblePercent * 100)}%."),
						TextElement.Create($"Your deductible has a minimum of ${PolicyState.Instance.Policy.DeductibleMinimum} and a maximum of ${PolicyState.Instance.Policy.DeductibleMaximum}."),
					];
				}
			}
			var screen = BoxedScreen.Create(title: TITLE, elements: textElements);

			currentScreen = screen;
		}

		protected override string GetApplicationText() => currentScreen.GetText(51);

		protected override Action PreviousScreen() => () => { };
	}
}
