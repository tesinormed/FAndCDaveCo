using LethalNetworkAPI;
using System.Collections;
using UnityEngine;

namespace tesinormed.FAndCDaveCo.Events
{
	public static class HUDManagerEvents
	{
		public const string INSURANCE_RENEWAL_SUCCESS_IDENTIFIER = "InsuranceRenewalSuccess";
		public const string INSURANCE_RENEWAL_FAIL_IDENTIFIER = "InsuranceRenewalFail";

		public static void Init()
		{
			LethalClientEvent insuranceRenewalSuccess = new(
				identifier: INSURANCE_RENEWAL_SUCCESS_IDENTIFIER,
				onReceived: () =>
				{
					HUDManager.Instance.StartCoroutine(DisplayTip(
						headerText: "Insurance renewed",
						bodyText: "Please check the terminal for how much has been deducted.",
						isWarning: false
					));
					Plugin.Logger.LogDebug($"displayed insurance renewal success on HUD");
				}
			);

			LethalClientEvent insuranceRenewalFail = new(
				identifier: INSURANCE_RENEWAL_FAIL_IDENTIFIER,
				onReceived: () =>
				{
					HUDManager.Instance.StartCoroutine(DisplayTip(
						headerText: "Insurance canceled",
						bodyText: "You do not have sufficient credits.",
						isWarning: true
					));
					Plugin.Logger.LogDebug($"displayed insurance renewal fail on HUD");
				}
		   );
		}
		private static IEnumerator DisplayTip(string headerText, string bodyText, bool isWarning)
		{
			yield return new WaitForSeconds(3);
			HUDManager.Instance.DisplayTip(headerText, bodyText, isWarning);
		}
	}
}
