using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Events;
using tesinormed.FAndCDaveCo.Misc;

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
			if (!GameStatistics.IsInOrbit)
			{
				SwitchScreen(
					BoxedScreen.Create(
						title: TITLE,
						elements: [TextElement.Create("You can only make a claim while you are in orbit.")]
					),
					CursorMenu.Create(startingCursorIndex: 0),
					previous: false
				);
				return;
			}

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

			if (!GameStatistics.IsHostingGame)
			{
				ErrorMessage(
					title: TITLE,
					error: "You are not the host of this instance.",
					backAction: () => SwitchScreen(previousScreen, previousCursorMenu, previous: true)
				);
				return;
			}

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

			if (terminal.groupCredits < deductible)
			{
				ErrorMessage(
					title: TITLE,
					error: "You do not have enough credits to continue with this claim.",
					backAction: () => SwitchScreen(MainScreen, MainCursorMenu, previous: true)
				);
				return;
			}

			Plugin.PolicyState.Claims[day] = new(claim.Value, true);
			Plugin.UpdateState();

			// update credits
			LethalServerMessage<int> deductGroupCredits = new(identifier: CreditEvents.DEDUCT_GROUP_CREDITS_IDENTIFIER);
			deductGroupCredits.SendAllClients(deductible);

			LethalServerMessage<int> spawnGoldBar = new(identifier: CreditEvents.SPAWN_GOLD_BAR_IDENTIFIER);
			spawnGoldBar.SendClient(payout, terminal.roundManager.playersManager.localPlayerController.GetClientId());

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
