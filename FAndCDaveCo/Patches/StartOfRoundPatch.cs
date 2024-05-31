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
			// return all of the previous values
			do
			{
				yield return values.Current;
			} while (values.MoveNext());

			// make sure we're the server
			if (__instance.IsServer)
			{
				// garbage collect >5 day old claims
				foreach (var pair in Plugin.PolicyState.Claims)
				{
					if ((GameStatistics.CurrentDay - pair.Key) > 5)
					{
						Plugin.PolicyState.Claims.Remove(pair.Key);
						Plugin.Logger.LogInfo($"deleted >5 day old claim from day {pair.Key}: {pair.Value}");
					}
				}
				// sync over network
				Plugin.SyncedClaims.Value = Plugin.PolicyState.Claims;

				// make sure there's a policy
				if (Plugin.PolicyState.Policy != Policy.NONE)
				{
					// make sure that the credits are sufficient for the premium payment
					if ((Plugin.Terminal.groupCredits - Plugin.PolicyState.TotalPremium) >= 0)
					{
						// deduct premium payment
						LethalClientMessage<int> deductGroupCredits = new(identifier: CreditEvents.DEDUCT_GROUP_CREDITS_IDENTIFIER);
						deductGroupCredits.SendServer(Plugin.PolicyState.TotalPremium);

						// notify all of the successful renewal
						LethalServerEvent insuranceRenewalSuccess = new(identifier: HUDManagerEvents.INSURANCE_RENEWAL_SUCCESS_IDENTIFIER);
						insuranceRenewalSuccess.InvokeAllClients();

						Plugin.Logger.LogDebug($"insurance successfully renewed with premium of {Plugin.PolicyState.TotalPremium}");
					}
					else
					{
						// cancel policy
						Plugin.PolicyState.Policy = Policy.NONE;
						// sync over network
						Plugin.SyncedPolicy.Value = Policy.NONE;

						// notify all of the failed renewal
						LethalServerEvent insuranceRenewalFail = new(identifier: HUDManagerEvents.INSURANCE_RENEWAL_FAIL_IDENTIFIER);
						insuranceRenewalFail.InvokeAllClients();

						Plugin.Logger.LogDebug($"insurance failed to renew");
					}
				}
			}
		}
	}
}
