using System.Collections.Generic;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Screen;
using tesinormed.FAndCDaveCo.Extensions;

namespace tesinormed.FAndCDaveCo.Insurance.UI;

public class PolicyInformationTerminal : TerminalApplicationExtension
{
	public override void Initialization()
	{
		List<ITextElement> textElements = [];
		if (Plugin.PolicyState.Policy.Tier == PolicyTier.None)
		{
			textElements.Add(TextElement.Create($"You do not currently have a policy with {PolicyTerminal.Title}."));
		}
		else
		{
			textElements.Add(TextElement.Create($"You currently have the {Plugin.PolicyState.Policy.Tier.ToFriendlyString()} policy (${Plugin.PolicyState.Policy.Coverage})."));
			textElements.Add(TextElement.Create($"You have made {Plugin.PolicyState.ClaimedClaims.Count} claims recently (within {Plugin.Config.ClaimRetentionDays.Value} days)."));
			textElements.Add(Plugin.PolicyState.FractionalPremiumIncrease == 0.00
				? TextElement.Create($"You pay ${Plugin.PolicyState.TotalPremium} per day.")
				// show only if premium is increased
				: TextElement.Create($"You pay ${Plugin.PolicyState.TotalPremium} (+{(int) (Plugin.PolicyState.FractionalPremiumIncrease * 100)}%) per day."));

			// show only if there is a deductible
			textElements.Add(Plugin.PolicyState.Policy.DeductiblePercent != 0.00
				? TextElement.Create($"Your deductible is currently {(int) (Plugin.PolicyState.Policy.DeductiblePercent * 100)}% (minimum: ${Plugin.PolicyState.Policy.DeductibleMinimum}, maximum: ${Plugin.PolicyState.Policy.DeductibleMaximum}).")
				: TextElement.Create("You do not pay deductibles."));
		}

		var screen = BoxedScreen.Create($"{PolicyTerminal.Title}: Policy information", textElements.ToArray());
		currentScreen = screen;
	}
}
