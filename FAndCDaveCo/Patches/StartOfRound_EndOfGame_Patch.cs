using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Bank;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Network;
using static HarmonyLib.AccessTools;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace tesinormed.FAndCDaveCo.Patches;

[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.EndOfGame))]
public static class StartOfRound_EndOfGame_Patch
{
	public static void Prefix(ref StartOfRound __instance)
	{
		// make sure we're the server and it's not a challenge moon
		if (__instance is { IsServer: true, isChallengeFile: false })
		{
			// garbage collect old claims
			foreach (var keyValuePair in Plugin.PolicyState.Claims.Where(pair => StartOfRound.Instance.gameStats.daysSpent - pair.Key > Plugin.Config.ClaimRetentionDays))
			{
				Plugin.PolicyState.UpdateAndSyncClaims(claims => claims.Remove(keyValuePair.Key));
				Plugin.Logger.LogInfo($"deleted old claim from day {keyValuePair.Key}: {keyValuePair.Value}");
			}

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
					Plugin.PolicyState.SetAndSyncPolicy(Policy.None);

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

				// credit garnishment
				LethalClientMessage<int> deductGroupCredits = new(CreditEvents.DeductGroupCreditsIdentifier);
				deductGroupCredits.SendServer(amountGarnished);
				Plugin.Logger.LogDebug($"garnished ${amountGarnished} from group credits due to loan nonpayment");

				if (Plugin.BankState.Loan.AmountUnpaid - amountGarnished == 0)
				{
					Plugin.BankState.SetAndSyncLoan(Loan.None);
					Plugin.Logger.LogDebug("loan fully paid off due to garnishment");
				}
				else
				{
					Plugin.BankState.UpdateAndSyncLoan(loan => loan.AmountPaid += amountGarnished);
					Plugin.Logger.LogDebug($"loan amount paid increased by {amountGarnished}");
				}

				// notify all of the credits garnishing
				LethalServerMessage<int> bankLoanCreditsGarnished = new(HUDManagerEvents.BankLoanCreditsGarnishedIdentifier);
				bankLoanCreditsGarnished.SendAllClients(amountGarnished);
			}
		}
	}

	[HarmonyPatch(MethodType.Enumerator)]
	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
	{
		var codeMatcher = new CodeMatcher(instructions, generator)
			.SearchForward(instruction => instruction.Calls(Method(typeof(HUDManager), nameof(HUDManager.ApplyPenalty))))
			.SearchBack(instruction => instruction.LoadsField(Field(typeof(StartOfRound), nameof(StartOfRound.isChallengeFile))))
			.SearchForward(instruction => instruction.opcode == OpCodes.Brtrue);
		var label = codeMatcher.Operand;

		return codeMatcher
			.Advance(1)
			.InsertAndAdvance(
				new(OpCodes.Call, PropertyGetter(typeof(PolicyState), nameof(PolicyState.DisableDeathCreditPenalty))),
				new(OpCodes.Brtrue, label)
			)
			.InstructionEnumeration();
	}

	public static IEnumerator Postfix(IEnumerator values, StartOfRound __instance)
	{
		// return all of the previous values
		do
		{
			yield return values.Current;
		} while (values.MoveNext());

		// make sure we're the server and it's not a challenge moon
		if (__instance is { IsServer: true, isChallengeFile: false })
		{
			// make sure we aren't getting fired
			if (!(TimeOfDay.Instance.timeUntilDeadline <= 0.0))
			{
				// run all of the queued HUD tips
				LethalServerEvent runQueuedHudTips = new(HUDManagerEvents.RunQueuedHudTipsIdentifier);
				runQueuedHudTips.InvokeAllClients();
			}
		}
	}
}
