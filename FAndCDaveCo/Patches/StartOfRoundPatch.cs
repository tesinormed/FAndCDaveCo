using HarmonyLib;
using LethalNetworkAPI;
using System.Collections;
using tesinormed.FAndCDaveCo.Events;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Misc;

namespace tesinormed.FAndCDaveCo.Patches
{
	[HarmonyPatch(typeof(StartOfRound))]
	public static class StartOfRoundPatch
	{
		[HarmonyPatch("EndOfGame")]
		[HarmonyPostfix]
		public static IEnumerator EndOfGame(IEnumerator values, StartOfRound __instance)
		{
			do
			{
				yield return values.Current;
			} while (values.MoveNext());

			if (__instance.IsServer)
			{
				if (Plugin.PolicyState.Policy != Policy.NONE)
				{
					if (Plugin.Terminal.groupCredits - Plugin.PolicyState.TotalPremium >= 0)
					{
						LethalServerMessage<int> deductGroupCredits = new(identifier: CreditEvents.DEDUCT_GROUP_CREDITS_IDENTIFIER);
						deductGroupCredits.SendAllClients(Plugin.PolicyState.TotalPremium);

						LethalServerEvent insuranceRenewalSuccess = new(identifier: HUDManagerEvents.INSURANCE_RENEWAL_SUCCESS_IDENTIFIER);
						insuranceRenewalSuccess.InvokeAllClients();

						Plugin.Logger.LogDebug($"insurance successfully renewed with premium of {Plugin.PolicyState.TotalPremium}");
					}
					else
					{
						Plugin.PolicyState.Policy = Policy.NONE;
						Plugin.UpdateState();

						LethalServerEvent insuranceRenewalFail = new(identifier: HUDManagerEvents.INSURANCE_RENEWAL_FAIL_IDENTIFIER);
						insuranceRenewalFail.InvokeAllClients();

						Plugin.Logger.LogDebug($"insurance failed to renew");
					}
				}

				foreach (var pair in Plugin.PolicyState.Claims)
				{
					if ((GameStatistics.CurrentDay - pair.Key) > 5)
					{
						Plugin.PolicyState.Claims.Remove(pair.Key);
						Plugin.Logger.LogInfo($"deleted >5 day old claim from day {pair.Key}: {pair.Value}");
					}
				}
				Plugin.UpdateState();
			}
		}
	}
}
