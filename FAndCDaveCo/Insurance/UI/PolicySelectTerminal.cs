using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;
using LethalNetworkAPI;
using System;
using tesinormed.FAndCDaveCo.Events;

namespace tesinormed.FAndCDaveCo.Insurance.UI
{
	public class PolicySelectTerminal : InteractiveTerminalApplication
	{
		private const string TITLE = $"{PolicyTerminal.TITLE}: Selection";

		private CursorElement CreatePolicyTierCursorElement(PolicyTier policyTier, string description) => CursorElement.Create
		(
			name: $"{policyTier.ToFriendlyString()}: {description}",
			action: () => SelectCoverage(((PolicyTier[]) Enum.GetValues(typeof(PolicyTier)))[(int) policyTier]),
			active: (_) => true,
			selectInactive: false
		);
		private CursorElement CreateCoverageCursorElement(Policy policy) => CursorElement.Create
		(
			name: $"${policy.Coverage}",
			action: () => ConfirmEverything(
				new()
				{
					Policy = policy,
					Claims = Plugin.PolicyState.Claims,
				}
			),
			active: (_) => true,
			selectInactive: false
		);
		private IScreen MainScreen { get; set; } = null!;
		private CursorMenu MainCursorMenu { get; set; } = null!;

		public override void Initialization()
		{
			// create cursor element for each tier
			CursorElement[] cursorElements =
			[
				CreatePolicyTierCursorElement(PolicyTier.HIGH_DEDUCTIBLE, "High deductible, low premium"),
				CreatePolicyTierCursorElement(PolicyTier.LOW_DEDUCTIBLE, "Low deductible, medium premium"),
				CreatePolicyTierCursorElement(PolicyTier.NO_DEDUCTIBLE, "No deductible, high premium"),
			];
			var cursorMenu = CursorMenu.Create(startingCursorIndex: 0, elements: cursorElements);

			ITextElement[] textElements =
			[
				TextElement.Create("Select a policy tier."),
				TextElement.Create(" "),
				cursorMenu,
			];
			var screen = BoxedScreen.Create(title: TITLE, elements: textElements);

			MainScreen = screen;
			MainCursorMenu = cursorMenu;
			currentScreen = screen;
			currentCursorMenu = cursorMenu;
		}

		private void SelectCoverage(PolicyTier policyTier)
		{
			var previousScreen = currentScreen;
			var previousCursorMenu = currentCursorMenu;

			// create cursor element for each coverage and back out option
			CursorElement[] cursorElements =
			[
				CreateCoverageCursorElement(policyTier.CreatePolicy(300)),
				CreateCoverageCursorElement(policyTier.CreatePolicy(650)),
				CreateCoverageCursorElement(policyTier.CreatePolicy(1500)),
				CreateCoverageCursorElement(policyTier.CreatePolicy(2750)),
				CreateCoverageCursorElement(policyTier.CreatePolicy(5000)),
				CreateCoverageCursorElement(policyTier.CreatePolicy(10000)),
				CursorElement.Create(
					name: "Back",
					action: () => SwitchScreen(previousScreen, previousCursorMenu, previous: true),
					active: (_) => true,
					selectInactive: false
				)
			];
			var cursorMenu = CursorMenu.Create(startingCursorIndex: 0, elements: cursorElements);

			ITextElement[] textElements =
			[
				TextElement.Create("Select the coverage amount."),
				TextElement.Create(" "),
				cursorMenu,
			];
			var screen = BoxedScreen.Create(title: TITLE, elements: textElements);

			currentCursorMenu = cursorMenu;
			currentScreen = screen;
		}

		private void ConfirmEverything(PolicyState state)
		{
			// make sure policy is not the same as the current one
			if (state.Policy == Plugin.PolicyState.Policy)
			{
				ErrorMessage(
					title: TITLE,
					error: "You already have this policy.",
					backAction: () => SwitchScreen(MainScreen, MainCursorMenu, previous: true)
				);
				return;
			}

			var previousScreen = currentScreen;
			var previousCursorMenu = currentCursorMenu;

			// inform user of deductible percent min and max
			string deductibleText = state.Policy.DeductiblePercent == 0.00
				? "This policy has no deductibles."
				: $"The deductibles are {(int) (state.Policy.DeductiblePercent * 100)}%, with a minimum of ${state.Policy.DeductibleMinimum} and a maximum of ${state.Policy.DeductibleMaximum}.";

			Confirm(
				title: TITLE,
				description: $"Are you sure you want to pick {state.Policy.Tier.ToFriendlyString()} (${state.Policy.Coverage})?\n{deductibleText}\nYou will be charged ${state.TotalPremium}.",
				confirmAction: () => SetEverything(state.Policy, state.TotalPremium),
				declineAction: () => SwitchScreen(previousScreen, previousCursorMenu, previous: true)
			);
			return;
		}

		private void SetEverything(Policy policy, int cost)
		{
			var previousScreen = currentScreen;
			var previousCursorMenu = currentCursorMenu;

			// make sure enough credits for initial premium payment
			if (terminal.groupCredits < cost)
			{
				ErrorMessage(
					title: TITLE,
					error: "You do not have enough credits to purchase this policy.",
					backAction: () => SwitchScreen(MainScreen, MainCursorMenu, previous: true)
				);
				return;
			}

			// update policy
			Plugin.PolicyState.Policy = policy;
			// sync over network
			LethalClientMessage<Policy> updatePolicy = new(identifier: NetworkVariableEvents.UPDATE_POLICY_IDENTIFIER);
			updatePolicy.SendServer(policy);

			LethalClientMessage<int> deductGroupCredits = new(identifier: CreditEvents.DEDUCT_GROUP_CREDITS_IDENTIFIER);
			deductGroupCredits.SendServer(cost);

			ITextElement[] textElements = [
				TextElement.Create($"Your policy has been updated to {policy.Tier.ToFriendlyString()} (${policy.Coverage})."),
				TextElement.Create($"You have been charged ${cost}."),
			];
			var screen = BoxedScreen.Create(title: TITLE, elements: textElements);
			var cursorMenu = CursorMenu.Create(startingCursorIndex: 0);

			SwitchScreen(screen, cursorMenu, previous: false);
		}
	}
}
