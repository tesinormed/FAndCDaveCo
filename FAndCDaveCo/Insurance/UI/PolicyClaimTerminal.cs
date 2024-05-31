using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using LethalNetworkAPI;
using System.Collections.Generic;
using tesinormed.FAndCDaveCo.Events;

namespace tesinormed.FAndCDaveCo.Insurance.UI
{
	public class PolicyClaimTerminal : InteractiveTerminalApplication
	{
		private const string TITLE = $"{PolicyTerminal.TITLE}: Make a claim";

		private CursorElement CreateCursorElement(int day, PolicyClaim claim) => CursorElement.Create
		(
			name: $"Day {day}: ${claim.Value}",
			action: () => ConfirmClaim(day, claim),
			active: (_) => true,
			selectInactive: false
		);
		private IScreen MainScreen { get; set; } = null!;
		private CursorMenu MainCursorMenu { get; set; } = null!;

		public override void Initialization()
		{
			// no policy
			if (Plugin.PolicyState.Policy.Tier == PolicyTier.NONE)
			{
				SwitchScreen(
					BoxedScreen.Create(
						title: TITLE,
						elements: [TextElement.Create("You do not have an active insurance policy.")]
					),
					CursorMenu.Create(startingCursorIndex: 0),
					previous: false
				);
				return;
			}

			// no claims
			if (Plugin.PolicyState.UnclaimedClaims.Count == 0)
			{
				SwitchScreen(
					BoxedScreen.Create(
						title: TITLE,
						elements: [TextElement.Create("You do not have any claims.")]
					),
					CursorMenu.Create(startingCursorIndex: 0),
					previous: false
				);
				return;
			}

			// make a cursor element for each claim
			CursorElement[] cursorElements = [];
			foreach (var pair in Plugin.PolicyState.UnclaimedClaims)
			{
				cursorElements = [.. cursorElements, CreateCursorElement(pair.Key, pair.Value)];
			}
			var cursorMenu = CursorMenu.Create(startingCursorIndex: 0, elements: cursorElements);

			ITextElement[] textElements =
			[
				TextElement.Create("Select a pending claim."),
				TextElement.Create(" "),
				cursorMenu,
			];
			var screen = BoxedScreen.Create(title: TITLE, elements: textElements);

			MainScreen = screen;
			MainCursorMenu = cursorMenu;
			currentScreen = screen;
			currentCursorMenu = cursorMenu;
		}

		private void ConfirmClaim(int day, PolicyClaim claim)
		{
			var previousScreen = currentScreen;
			var previousCursorMenu = currentCursorMenu;

			var deductible = Plugin.PolicyState.Policy.CalculateDeductible(claim.Value);
			var payout = Plugin.PolicyState.Policy.CalculatePayout(claim.Value);

			Confirm(
				title: TITLE,
				description: $"Are you sure you want to confirm this claim?\nYou will be given back ${payout}.\nYour deductible is ${deductible}.",
				confirmAction: () => ProcessClaim(day, deductible, payout, claim),
				declineAction: () => SwitchScreen(previousScreen, previousCursorMenu, previous: true)
			);
			return;
		}

		private void ProcessClaim(int day, int deductible, int payout, PolicyClaim claim)
		{
			var previousScreen = currentScreen;
			var previousCursorMenu = currentCursorMenu;

			// make sure enough credits for deductible
			if (terminal.groupCredits < deductible)
			{
				ErrorMessage(
					title: TITLE,
					error: "You do not have enough credits to continue with this claim.",
					backAction: () => SwitchScreen(MainScreen, MainCursorMenu, previous: true)
				);
				return;
			}

			// set claims
			Plugin.PolicyState.Claims[day] = new(claim.Value, true);
			// sync over network
			LethalClientMessage<Dictionary<int, PolicyClaim>> updateClaims = new(identifier: NetworkVariableEvents.UPDATE_CLAIMS_IDENTIFIER);
			updateClaims.SendServer(Plugin.PolicyState.Claims);

			LethalClientMessage<int> deductGroupCredits = new(identifier: CreditEvents.DEDUCT_GROUP_CREDITS_IDENTIFIER);
			deductGroupCredits.SendServer(deductible);

			// spawn bar with value of payout
			LethalClientMessage<int> spawnGoldBar = new(identifier: CreditEvents.SPAWN_GOLD_BAR_IDENTIFIER);
			spawnGoldBar.SendServer(payout);

			ITextElement[] textElements = [
				TextElement.Create($"You have been given a gold bar worth ${payout}."),
				TextElement.Create($"You have been charged ${deductible}."),
				TextElement.Create($"Your premiums have increased by {(int) (Plugin.PolicyState.FractionalPremiumIncrease * 100)}%."),
			];
			var screen = BoxedScreen.Create(title: TITLE, elements: textElements);
			var cursorMenu = CursorMenu.Create(startingCursorIndex: 0);

			SwitchScreen(screen, cursorMenu, previous: false);
		}
	}
}
