using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Cursor;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Extensions;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo.Insurance.UI;

public class PolicySelectTerminal : InteractiveTerminalApplication
{
	protected override string Title => $"{PolicyTerminal.Title}: Selection";

	private CursorElement CreatePolicyTierCursorElement(PolicyTier policyTier, string description) => CursorElement.Create
	(
		name: $"{policyTier.ToFriendlyString()}: {description}",
		action: () => SelectCoverage(policyTier),
		active: _ => true,
		selectInactive: false
	);
	private CursorElement CreateCoverageCursorElement(Policy policy)
	{
		var policyState = new PolicyState
		{
			Policy = policy,
			Claims = Plugin.PolicyState.Claims
		};

		return CursorElement.Create
		(
			name: $"${policy.Coverage}",
			action: () => ConfirmEverything(policyState),
			active: _ => terminal.groupCredits >= policyState.TotalPremium,
			selectInactive: true
		);
	}

	public override void Initialization()
	{
		if (StartOfRound.Instance.isChallengeFile)
		{
			LockedNotification(TextElement.Create("You cannot get an insurance policy while doing the challenge moon."));
			Plugin.Logger.LogDebug("local player tried to get an insurance policy while doing a challenge moon");
			return;
		}

		// create cursor element for each tier
		(MainScreen, MainCursorMenu) = Selection(
			prompt: "Select a policy tier.",
			CreatePolicyTierCursorElement(PolicyTier.HighDeductible, "High deductible, low premium"),
			CreatePolicyTierCursorElement(PolicyTier.LowDeductible, "Low deductible, medium premium"),
			CreatePolicyTierCursorElement(PolicyTier.NoDeductible, "No deductible, high premium")
		);
	}

	private void SelectCoverage(PolicyTier policyTier)
	{
		SelectionWithBack(
			prompt: "Select the coverage amount.",
			backAction: PreviousMainScreenAction,
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption00)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption01)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption02)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption03)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption04)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption05)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption06)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption07)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption08)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption09)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Config.CoverageOption10))
		);
	}

	private void ConfirmEverything(PolicyState state)
	{
		// make sure policy is not the same as the current one
		if (state.Policy == Plugin.PolicyState.Policy)
		{
			Notification(backAction: PreviousScreenAction, TextElement.Create("You already have this policy."));
			return;
		}

		// make sure enough credits for initial premium payment
		if (terminal.groupCredits < state.TotalPremium)
		{
			Notification(backAction: PreviousScreenAction, TextElement.Create("You do not have enough credits to purchase this policy."));
			return;
		}

		Confirm(
			confirmAction: () => SetEverything(state.Policy, state.TotalPremium),
			declineAction: PreviousScreenAction,
			TextElement.Create($"Are you sure you want to pick {state.Policy.Tier.ToFriendlyString()} (${state.Policy.Coverage})?"),
			TextElement.Create(state.Policy.DeductiblePercent == 0.00
				? "This policy has no deductibles."
				: $"The deductibles are {(int) (state.Policy.DeductiblePercent * 100)}%, with a minimum of ${state.Policy.DeductibleMinimum} and a maximum of ${state.Policy.DeductibleMaximum}."),
			TextElement.Create($"You will be charged ${state.TotalPremium}.")
		);
	}

	private void SetEverything(Policy policy, int cost)
	{
		// update policy
		Plugin.PolicyState.Policy = policy;
		// sync over network
		LethalClientMessage<Policy> updatePolicy = new(NetworkVariableEvents.UpdatePolicyIdentifier);
		updatePolicy.SendServer(policy);

		LethalClientMessage<int> deductGroupCredits = new(CreditEvents.DeductGroupCreditsIdentifier);
		deductGroupCredits.SendServer(cost);

		LockedNotification(
			TextElement.Create($"Your policy has been updated to {policy.Tier.ToFriendlyString()} (${policy.Coverage})."),
			TextElement.Create($"You have been charged ${cost}.")
		);
	}
}
