using System.Collections;
using LethalNetworkAPI;
using UnityEngine;

namespace tesinormed.FAndCDaveCo.Network;

public static class HUDManagerEvents
{
	public const string InsuranceRenewalSuccessIdentifier = "InsuranceRenewalSuccess";
	public const string InsuranceRenewalFailIdentifier = "InsuranceRenewalFail";
	public const string InsuranceClaimAvailableIdentifier = "ClaimAvailable";
	public const string BankLoanCreditsGarnishedIdentifier = "BankLoanCreditsGarnished";

	public static void Init()
	{
		LethalClientEvent insuranceRenewalSuccess = new(InsuranceRenewalSuccessIdentifier, onReceived: () =>
		{
			HUDManager.Instance.StartCoroutine(DisplayTip(
				"Insurance renewed",
				"Please check the terminal for how much has been deducted.",
				false
			));
			Plugin.Logger.LogDebug("displayed insurance renewal success on HUD");
		});
		LethalClientEvent insuranceRenewalFail = new(
			InsuranceRenewalFailIdentifier,
			() =>
			{
				HUDManager.Instance.StartCoroutine(DisplayTip(
					"Insurance canceled",
					"You do not have sufficient credits.",
					true
				));
				Plugin.Logger.LogDebug("displayed insurance renewal fail on HUD");
			}
		);

		LethalClientEvent insuranceClaimAvailable = new(InsuranceClaimAvailableIdentifier, onReceived: () =>
		{
			HUDManager.Instance.StartCoroutine(DisplayTip(
				"Insurance claim",
				"Confirm the pending claim at the terminal.",
				false
			));
			Plugin.Logger.LogDebug("displayed insurance claim available on HUD");
		});

		LethalClientEvent bankLoanCreditsGarnished = new(BankLoanCreditsGarnishedIdentifier, onReceived: () =>
		{
			HUDManager.Instance.StartCoroutine(DisplayTip(
				"Credits garnished",
				"Due to loan nonpayment, 10% of your credits have been garnished.",
				true
			));
			Plugin.Logger.LogDebug("displayed loan credit garnishing warning on HUD");
		});
	}

	private static IEnumerator DisplayTip(string headerText, string bodyText, bool isWarning)
	{
		yield return new WaitForSeconds(3);
		HUDManager.Instance.DisplayTip(headerText, bodyText, isWarning);
	}
}
