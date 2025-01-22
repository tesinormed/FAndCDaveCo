using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Cursor;
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
		return CursorElement.Create
		(
			name: $"${policy.Coverage}",
			action: () => ConfirmEverything(policy),
			active: _ => terminal.groupCredits >= Plugin.Instance.State.CalculateTotalPremium(policy),
			selectInactive: true
		);
	}

	public override void Initialization()
	{
		if (StartOfRound.Instance.isChallengeFile)
		{
			LockedNotification(TextElement.Create("You cannot get an insurance policy while doing a challenge moon."));
			return;
		}

		// create cursor element for each tier
		(MainScreen, MainCursorMenu) = Selection(
			prompt: "Select a policy tier.",
			CreatePolicyTierCursorElement(PolicyTier.HighDeductible, "High deductible, low premium"),
			CreatePolicyTierCursorElement(PolicyTier.LowDeductible, "Low deductible, medium premium"),
			CreatePolicyTierCursorElement(PolicyTier.NoDeductible, "No deductible, high premium"),
			CursorElement.Create(name: "Cancel policy", action: ConfirmCancelPolicy, active: _ => Plugin.Instance.State.Policy.Tier != PolicyTier.None, selectInactive: true)
		);
	}

	private void ConfirmCancelPolicy()
	{
		if (Plugin.Instance.State.Policy.Tier == PolicyTier.None)
		{
			Notification(backAction: PreviousScreenAction, TextElement.Create("You do not currently have a policy."));
			return;
		}

		Confirm(
			confirmAction: CancelPolicy,
			declineAction: PreviousScreenAction,
			TextElement.Create("Are you sure you want to cancel your current policy?"),
			TextElement.Create($"Your current policy is {Plugin.Instance.State.Policy.Tier.ToFriendlyString()} (${Plugin.Instance.State.Policy.Coverage}).")
		);
	}

	private void CancelPolicy()
	{
		Plugin.Instance.State.Policy = Policy.None;

		LockedNotification(TextElement.Create("Your policy has been canceled."));
	}

	private void SelectCoverage(PolicyTier policyTier)
	{
		SelectionWithBack(
			prompt: "Select the coverage amount.",
			backAction: PreviousMainScreenAction,
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption00.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption01.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption02.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption03.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption04.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption05.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption06.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption07.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption08.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption09.Value)),
			CreateCoverageCursorElement(policyTier.CreatePolicy(Plugin.Instance.Config.CoverageOption10.Value))
		);
	}

	private void ConfirmEverything(Policy policy)
	{
		// make sure policy is not the same as the current one
		if (policy == Plugin.Instance.State.Policy)
		{
			Notification(backAction: PreviousScreenAction, TextElement.Create("You already have this policy."));
			return;
		}

		// make sure enough credits for initial premium payment
		if (terminal.groupCredits < Plugin.Instance.State.CalculateTotalPremium(policy))
		{
			Notification(backAction: PreviousScreenAction, TextElement.Create("You do not have enough credits to purchase this policy."));
			return;
		}

		Confirm(
			confirmAction: () => SetEverything(policy, Plugin.Instance.State.CalculateTotalPremium(policy)),
			declineAction: PreviousScreenAction,
			TextElement.Create($"Are you sure you want to pick {policy.Tier.ToFriendlyString()} (${policy.Coverage})?"),
			TextElement.Create(policy.DeductiblePercent == 0.00
				? "This policy has no deductibles."
				: $"The deductibles are {(int) (policy.DeductiblePercent * 100)}%, with a minimum of ${policy.DeductibleMinimum} and a maximum of ${policy.DeductibleMaximum}."),
			TextElement.Create($"You will be charged ${Plugin.Instance.State.CalculateTotalPremium(policy)}.")
		);
	}

	private void SetEverything(Policy policy, int cost)
	{
		Plugin.Instance.State.Policy = policy;

		CreditEvents.DeductGroupCredits.SendServer(cost);

		LockedNotification(
			TextElement.Create($"Your policy has been updated to {policy.Tier.ToFriendlyString()} (${policy.Coverage})."),
			TextElement.Create($"You have been charged ${cost}.")
		);
	}
}
