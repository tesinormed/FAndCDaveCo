using System;
using System.Collections;
using System.Collections.Generic;
using LethalNetworkAPI;
using UnityEngine;

namespace tesinormed.FAndCDaveCo.Network;

public static class HUDManagerEvents
{
	public static LNetworkMessage<int> InsuranceRenewalSuccess = null!;
	public static LNetworkEvent InsuranceRenewalFail = null!;

	public static LNetworkEvent InsuranceClaimAvailable = null!;

	public static LNetworkMessage<int> BankLoanCreditsGarnished = null!;

	public static LNetworkEvent RunShowQueuedHudTips = null!;
	internal static readonly List<Action> QueuedHudTips = [];

	internal static void Init()
	{
		InsuranceRenewalSuccess = LNetworkMessage<int>.Connect("InsuranceRenewalSuccess", onClientReceived: value =>
		{
			QueuedHudTips.Add(() => HUDManager.Instance.DisplayTip(
				"Insurance renewed",
				$"{value} credits have been deducted.",
				isWarning: false
			));
			Plugin.Logger.LogDebug("queued insurance renewal success on HUD");
		});
		InsuranceRenewalFail = LNetworkEvent.Connect("InsuranceRenewalFail", onClientReceived: () =>
		{
			QueuedHudTips.Add(() => HUDManager.Instance.DisplayTip(
				"Insurance canceled",
				"You do not have sufficient credits.",
				isWarning: true
			));
			Plugin.Logger.LogDebug("queued insurance renewal fail on HUD");
		});

		InsuranceClaimAvailable = LNetworkEvent.Connect("InsuranceClaimAvailable", onClientReceived: () =>
		{
			QueuedHudTips.Add(() => HUDManager.Instance.DisplayTip(
				"Insurance claim",
				"Confirm the pending claim at the terminal.",
				isWarning: false
			));
			Plugin.Logger.LogDebug("queued insurance claim available on HUD");
		});

		BankLoanCreditsGarnished = LNetworkMessage<int>.Connect("BankLoanCreditsGarnished", onClientReceived: value =>
		{
			QueuedHudTips.Add(() => HUDManager.Instance.DisplayTip(
				"Credits garnished",
				$"Due to loan nonpayment, {(int) (Plugin.Instance.Config.PenaltyAmount.Value * 100)}% of your credits (\u25ae{value}) have been garnished.",
				isWarning: true
			));
			Plugin.Logger.LogDebug("queued loan credit garnishing warning on HUD");
		});

		RunShowQueuedHudTips = LNetworkEvent.Connect("RunShowQueuedHudTips", onClientReceived: () => { HUDManager.Instance.StartCoroutine(ShowQueuedHudTips()); });
	}

	private static IEnumerator ShowQueuedHudTips()
	{
		for (var index = 0; index < QueuedHudTips.Count; index++)
		{
			var tip = QueuedHudTips[index];

			if (index == 0) yield return new WaitForSeconds(3F);
			tip.Invoke();
			Plugin.Logger.LogDebug("displayed queued HUD tip");
			yield return new WaitForSeconds(7.5F);
		}

		QueuedHudTips.Clear();
	}
}
