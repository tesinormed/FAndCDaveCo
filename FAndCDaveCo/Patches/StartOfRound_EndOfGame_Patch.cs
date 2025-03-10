using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using tesinormed.FAndCDaveCo.Bank;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Network;
using static HarmonyLib.AccessTools;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace tesinormed.FAndCDaveCo.Patches;

[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.EndOfGame))]
internal static class StartOfRound_EndOfGame_Patch
{
	public static void Prefix(ref StartOfRound __instance)
	{
		// make sure we're the server and it's not a challenge moon
		if (__instance is not { IsServer: true, isChallengeFile: false }) return;

		// garbage collect old claims
		foreach (var pair in Plugin.Instance.State.Claims.Where(pair => StartOfRound.Instance.gameStats.daysSpent - pair.Key > Plugin.Instance.Config.ClaimRetentionDays.Value))
		{
			Plugin.Instance.State.MutateClaims(claims => claims.Remove(pair.Key));
			Plugin.Logger.LogDebug($"deleted old claim from day {pair.Key}");
		}

		// make sure there's a policy
		if (Plugin.Instance.State.Policy != Policy.None)
		{
			// make sure that the credits are sufficient for the premium payment
			if (Plugin.Terminal.groupCredits >= Plugin.Instance.State.TotalPremium)
			{
				// deduct premium payment
				CreditEvents.DeductGroupCredits.SendServer(Plugin.Instance.State.TotalPremium);

				// notify everyone about the successful renewal
				HUDManagerEvents.InsuranceRenewalSuccess.SendClients(Plugin.Instance.State.TotalPremium);

				Plugin.Logger.LogDebug($"insurance successfully renewed with premium of {Plugin.Instance.State.TotalPremium}");
			}
			else
			{
				// cancel policy
				Plugin.Instance.State.Policy = Policy.None;

				// notify everyone about the failed renewal
				HUDManagerEvents.InsuranceRenewalFail.InvokeClients();

				Plugin.Logger.LogDebug("insurance failed to renew");
			}
		}

		// check if there's an unpaid loan and if it's been more than the set amount of days
		if (Plugin.Instance.State.Loan.AmountUnpaid > 0 && Plugin.Instance.State.Loan.DaysSinceIssuance >= Plugin.Instance.Config.PenaltyStartDaysFromIssuance.Value)
		{
			var amountGarnished = System.Math.Min(
				(int) (Plugin.Terminal.groupCredits * Plugin.Instance.Config.PenaltyAmount.Value),
				Plugin.Instance.State.Loan.AmountUnpaid
			);

			// credit garnishment
			CreditEvents.DeductGroupCredits.SendServer(amountGarnished);
			Plugin.Logger.LogDebug($"garnished ${amountGarnished} from group credits due to loan nonpayment");

			if (Plugin.Instance.State.Loan.AmountUnpaid - amountGarnished == 0)
			{
				Plugin.Instance.State.Loan = Loan.None;
				Plugin.Logger.LogDebug("loan fully paid off due to garnishment");
			}
			else
			{
				Plugin.Instance.State.Loan = Plugin.Instance.State.Loan with { AmountPaid = Plugin.Instance.State.Loan.AmountPaid + amountGarnished };
				Plugin.Logger.LogDebug($"loan amount paid increased by {amountGarnished}");
			}

			// notify everyone about the credits garnishing
			HUDManagerEvents.BankLoanCreditsGarnished.SendClients(amountGarnished);
		}
	}

	[HarmonyPatch(MethodType.Enumerator)]
	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
	{
		// search for the right part of the method to insert our instructions into
		var codeMatcher = new CodeMatcher(instructions, generator)
			// search forwards until we see the method to apply the penalty
			.SearchForward(instruction => instruction.Calls(Method(typeof(HUDManager), nameof(HUDManager.ApplyPenalty))))
			// search backwards until we see the check for if it's a challenge file
			.SearchBack(instruction => instruction.LoadsField(Field(typeof(StartOfRound), nameof(StartOfRound.isChallengeFile))))
			// search forwards until we see the comparison to true
			.SearchForward(instruction => instruction.opcode == OpCodes.Brtrue);
		var label = codeMatcher.Operand;

		return codeMatcher
			.Advance(1)
			.InsertAndAdvance(
				// tesinormed.FAndCDaveCo.State.DisableDeathCreditPenalty == true
				new(OpCodes.Call, PropertyGetter(typeof(State), nameof(State.DisableDeathCreditPenalty))),
				new(OpCodes.Brtrue, label)
			)
			.InstructionEnumeration();
	}

	public static IEnumerator Postfix(IEnumerator values, StartOfRound __instance)
	{
		// return all previous values
		do
		{
			yield return values.Current;
		} while (values.MoveNext());

		// make sure we're the server and it's not a challenge moon
		if (__instance is not { IsServer: true, isChallengeFile: false }) yield break;
		// make sure we aren't getting fired
		if (TimeOfDay.Instance.timeUntilDeadline <= 0.0) yield break;

		// show all queued HUD tips
		HUDManagerEvents.RunShowQueuedHudTips.InvokeClients();
	}
}
