using System.Linq;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Cursor;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Extensions;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo.Insurance.UI;

public class PolicyClaimTerminal : InteractiveTerminalApplication
{
	protected override string Title => $"{PolicyTerminal.Title}: Make a claim";

	private CursorElement CreateCursorElement(int day, PolicyClaim claim) => CursorElement.Create
	(
		name: $"Day {day}: ${claim.Value}",
		action: () => ConfirmClaim(day, claim),
		active: _ => true,
		selectInactive: false
	);

	public override void Initialization()
	{
		// no policy
		if (Plugin.PolicyState.Policy.Tier == PolicyTier.None)
		{
			LockedNotification(TextElement.Create("You do not have an active insurance policy."));
			return;
		}

		// no claims
		if (Plugin.PolicyState.UnclaimedClaims.Count == 0)
		{
			LockedNotification(TextElement.Create("You do not have any claims."));
			return;
		}

		// make a cursor element for each claim
		CursorElement[] cursorElements = [];
		cursorElements = Plugin.PolicyState.UnclaimedClaims.Aggregate(cursorElements, (current, pair) => [.. current, CreateCursorElement(pair.Key, pair.Value)]);
		(MainScreen, MainCursorMenu) = Selection(prompt: "Select a pending claim.", cursorElements);
	}

	private void ConfirmClaim(int day, PolicyClaim claim)
	{
		var deductible = Plugin.PolicyState.Policy.CalculateDeductible(claim.Value);
		var payout = Plugin.PolicyState.Policy.CalculatePayout(claim.Value);

		Confirm(
			confirmAction: () => ProcessClaim(day, deductible, payout, claim),
			declineAction: () => ConfirmDeleteClaim(day),
			TextElement.Create($"Are you sure you want to confirm this claim? You will need to pay a deductible of ${deductible}."),
			TextElement.Create($"You will be given back ${payout}.")
		);
	}

	private void ConfirmDeleteClaim(int day)
	{
		Confirm(
			confirmAction: () => DeleteClaim(day),
			declineAction: PreviousMainScreenAction,
			TextElement.Create($"Do you want to delete this claim for day {day}?")
		);
	}

	private void DeleteClaim(int day)
	{
		Plugin.PolicyState.UpdateAndSyncClaims(claims => claims.Remove(day));

		LockedNotification(TextElement.Create($"The claim for day {day} has been deleted."));
	}

	private void ProcessClaim(int day, int deductible, int payout, PolicyClaim claim)
	{
		// make sure enough credits for deductible
		if (terminal.groupCredits < deductible)
		{
			Notification(backAction: PreviousMainScreenAction, TextElement.Create("You do not have enough credits to continue with this claim."));
			return;
		}

		Plugin.PolicyState.UpdateAndSyncClaims(claims => claims[day] = new(claim.Value, claimed: true));

		// deduct credits
		LethalClientMessage<int> deductGroupCredits = new(CreditEvents.DeductGroupCreditsIdentifier);
		deductGroupCredits.SendServer(deductible);

		// spawn bar with value of payout
		LethalClientMessage<int> spawnGoldBar = new(CreditEvents.SpawnGoldBarIdentifier);
		spawnGoldBar.SendServer(payout);

		LockedNotification(
			TextElement.Create($"You have been given a gold bar worth ${payout}."),
			TextElement.Create($"You have been charged ${deductible}."),
			TextElement.Create($"Your premiums have increased by {(int) (Plugin.PolicyState.FractionalPremiumIncrease * 100)}%.")
		);
	}
}
