using System.Collections.Generic;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Screen;
using tesinormed.FAndCDaveCo.Extensions;

namespace tesinormed.FAndCDaveCo.Insurance.UI;

public class PolicyInformationTerminal : TerminalApplication
{
	public override void Initialization()
	{
		List<ITextElement> textElements = [];
		if (Plugin.Instance.State.Policy.Tier == PolicyTier.None)
		{
			textElements.Add(TextElement.Create("You do not currently have an insurance policy."));
		}
		else
		{
			textElements.Add(TextElement.Create($"You currently have the {Plugin.Instance.State.Policy.Tier.ToFriendlyString()} policy (${Plugin.Instance.State.Policy.Coverage})."));
			textElements.Add(TextElement.Create($"You have made {Plugin.Instance.State.ClaimedClaims.Count} claim(s) recently (within {Plugin.Instance.Config.ClaimRetentionDays.Value} days)."));
			textElements.Add(Plugin.Instance.State.FractionalPremiumIncrease == 0.00
				? TextElement.Create($"You pay ${Plugin.Instance.State.TotalPremium} per day.")
				// show only if premium is increased
				: TextElement.Create($"You pay ${Plugin.Instance.State.TotalPremium} (+{(int) (Plugin.Instance.State.FractionalPremiumIncrease * 100)}%) per day."));
			// show only if there is a deductible
			textElements.Add(Plugin.Instance.State.Policy.DeductiblePercent != 0.00
				? TextElement.Create($"Your deductible is currently {(int) (Plugin.Instance.State.Policy.DeductiblePercent * 100)}% (minimum: ${Plugin.Instance.State.Policy.DeductibleMinimum}, maximum: ${Plugin.Instance.State.Policy.DeductibleMaximum}).")
				: TextElement.Create("You do not pay deductibles."));
		}

		var screen = BoxedScreen.Create($"{PolicyTerminal.Title}: Policy information", textElements.ToArray());
		currentScreen = screen;
	}
}
