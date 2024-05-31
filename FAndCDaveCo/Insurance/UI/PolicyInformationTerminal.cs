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
			if (Plugin.PolicyState.Policy.Tier == PolicyTier.NONE)
			{
				textElements = [TextElement.Create($"You do not currently have a policy with {PolicyTerminal.TITLE}.")];
			}
			else
			{
				textElements = [
					TextElement.Create($"You currently have the {Plugin.PolicyState.Policy.Tier.ToFriendlyString()} policy (${Plugin.PolicyState.Policy.Coverage})."),
					TextElement.Create($"You have made {Plugin.PolicyState.ClaimedClaims.Count} claims recently (within 5 days)."),
				];

				if (Plugin.PolicyState.FractionalPremiumIncrease == 0.00)
				{
					textElements = [.. textElements, TextElement.Create($"You pay ${Plugin.PolicyState.TotalPremium} per day.")];
				}
				// show only if increased premium
				else
				{
					textElements =
					[
						.. textElements,
						TextElement.Create($"You pay ${Plugin.PolicyState.TotalPremium} (+{(int) (Plugin.PolicyState.FractionalPremiumIncrease * 100)}%) per day."),
					];
				}

				// show only if has deductible
				if (Plugin.PolicyState.Policy.DeductiblePercent == 0.00)
				{
					textElements = [.. textElements, TextElement.Create("You do not pay deductibles.")];
				}
				else
				{
					textElements =
					[
						.. textElements,
						TextElement.Create($"Your deductible is currently {(int) (Plugin.PolicyState.Policy.DeductiblePercent * 100)}%."),
						TextElement.Create($"Your deductible has a minimum of ${Plugin.PolicyState.Policy.DeductibleMinimum} and a maximum of ${Plugin.PolicyState.Policy.DeductibleMaximum}."),
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
