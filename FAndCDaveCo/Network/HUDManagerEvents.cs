using System;
using System.Collections;
using System.Collections.Generic;
using LethalNetworkAPI;
using UnityEngine;

namespace tesinormed.FAndCDaveCo.Network;

public static class HUDManagerEvents
{
	public const string InsuranceRenewalSuccessIdentifier = "InsuranceRenewalSuccess";
	public const string InsuranceRenewalFailIdentifier = "InsuranceRenewalFail";
	public const string InsuranceClaimAvailableIdentifier = "ClaimAvailable";
	public const string BankLoanCreditsGarnishedIdentifier = "BankLoanCreditsGarnished";
	public const string RunQueuedHudTipsIdentifier = "RunQueuedHudTips";

	internal static readonly List<Action> QueuedHudTips = [];

	public static void Init()
	{
		LethalClientMessage<int> insuranceRenewalSuccess = new(InsuranceRenewalSuccessIdentifier, onReceived: value =>
		{
			QueuedHudTips.Add(() => HUDManager.Instance.DisplayTip(
				"Insurance renewed",
				$"\u25ae{value} has been deducted.",
				isWarning: false
			));
			Plugin.Logger.LogDebug("queued insurance renewal success on HUD");
		});
		LethalClientEvent insuranceRenewalFail = new(InsuranceRenewalFailIdentifier, onReceived: () =>
		{
			QueuedHudTips.Add(() => HUDManager.Instance.DisplayTip(
				"Insurance canceled",
				"You do not have sufficient credits.",
				isWarning: true
			));
			Plugin.Logger.LogDebug("queued insurance renewal fail on HUD");
		});

		LethalClientEvent insuranceClaimAvailable = new(InsuranceClaimAvailableIdentifier, onReceived: () =>
		{
			QueuedHudTips.Add(() => HUDManager.Instance.DisplayTip(
				"Insurance claim",
				"Confirm the pending claim at the terminal.",
				isWarning: false
			));
			Plugin.Logger.LogDebug("queued insurance claim available on HUD");
		});

		LethalClientMessage<int> bankLoanCreditsGarnished = new(BankLoanCreditsGarnishedIdentifier, onReceived: value =>
		{
			QueuedHudTips.Add(() => HUDManager.Instance.DisplayTip(
				"Credits garnished",
				$"Due to loan nonpayment, {(int) (Plugin.Config.PenaltyAmount * 100)}% of your credits (\u25ae{value}) have been garnished.",
				isWarning: true
			));
			Plugin.Logger.LogDebug("queued loan credit garnishing warning on HUD");
		});

		LethalClientEvent runQueuedHudTips = new(RunQueuedHudTipsIdentifier, onReceived: () => { HUDManager.Instance.StartCoroutine(RunQueuedHudTips()); });
	}

	private static IEnumerator RunQueuedHudTips()
	{
		for (var index = 0; index < QueuedHudTips.Count; index++)
		{
			var tip = QueuedHudTips[index];

			if (index == 0) yield return new WaitForSeconds(3F);
			tip.Invoke();
			yield return new WaitForSeconds(7.5F);
		}

		QueuedHudTips.RemoveAll(_ => true);
	}
}
