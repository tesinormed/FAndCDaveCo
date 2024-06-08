using System;
using System.Collections;
using System.Linq;
using HarmonyLib;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Bank;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo.Patches;

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
			// garbage collect old claims
			foreach (var keyValuePair in Plugin.PolicyState.Claims.Where(pair => StartOfRound.Instance.gameStats.daysSpent - pair.Key > Plugin.Config.ClaimRetentionDays))
			{
				Plugin.PolicyState.Claims.Remove(keyValuePair.Key);
				Plugin.Logger.LogInfo($"deleted old claim from day {keyValuePair.Key}: {keyValuePair.Value}");
			}

			// sync over network
			Plugin.SyncedClaims.Value = Plugin.PolicyState.Claims;

			// make sure there's a policy
			if (Plugin.PolicyState.Policy != Policy.None)
			{
				// make sure that the credits are sufficient for the premium payment
				if (Plugin.Terminal.groupCredits >= Plugin.PolicyState.TotalPremium)
				{
					// deduct premium payment
					LethalClientMessage<int> deductGroupCredits = new(CreditEvents.DeductGroupCreditsIdentifier);
					deductGroupCredits.SendServer(Plugin.PolicyState.TotalPremium);

					// notify all of the successful renewal
					LethalServerMessage<int> insuranceRenewalSuccess = new(HUDManagerEvents.InsuranceRenewalSuccessIdentifier);
					insuranceRenewalSuccess.SendAllClients(Plugin.PolicyState.TotalPremium);

					Plugin.Logger.LogDebug($"insurance successfully renewed with premium of {Plugin.PolicyState.TotalPremium}");
				}
				else
				{
					// cancel policy
					Plugin.PolicyState.Policy = Policy.None;
					// sync over network
					Plugin.SyncedPolicy.Value = Policy.None;

					// notify all of the failed renewal
					LethalServerEvent insuranceRenewalFail = new(HUDManagerEvents.InsuranceRenewalFailIdentifier);
					insuranceRenewalFail.InvokeAllClients();

					Plugin.Logger.LogDebug("insurance failed to renew");
				}
			}

			// check if there's an unpaid loan and if it's been more than the set amount of days
			if (Plugin.BankState.Loan.AmountUnpaid > 0 && Plugin.BankState.Loan.DaysSinceIssuance >= Plugin.Config.PenaltyStartDaysFromIssuance)
			{
				var amountGarnished = Math.Min(
					(int) (Plugin.Terminal.groupCredits * Plugin.Config.PenaltyAmount),
					Plugin.BankState.Loan.AmountUnpaid
				);

				// garnish 10% of credits
				LethalClientMessage<int> deductGroupCredits = new(CreditEvents.DeductGroupCreditsIdentifier);
				deductGroupCredits.SendServer(amountGarnished);
				Plugin.Logger.LogDebug($"garnished ${amountGarnished} from group credits due to loan nonpayment");

				// add to paid amount of loan
				Plugin.BankState.Loan.AmountPaid += amountGarnished;

				// check if loan has been fully paid
				if (Plugin.BankState.Loan.AmountUnpaid == 0)
				{
					Plugin.BankState.Loan = Loan.None;
					Plugin.Logger.LogDebug("loan fully paid off due to garnishment");
				}

				// sync over network
				LethalClientMessage<Loan> updateLoan = new(NetworkVariableEvents.UpdateLoanIdentifier);
				updateLoan.SendServer(Plugin.BankState.Loan);

				// notify all of the credits garnishing
				LethalServerMessage<int> bankLoanCreditsGarnished = new(HUDManagerEvents.BankLoanCreditsGarnishedIdentifier);
				bankLoanCreditsGarnished.SendAllClients(amountGarnished);
			}
		}
	}
}
