using System.Linq;
using HarmonyLib;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Network;
using UnityEngine;

namespace tesinormed.FAndCDaveCo.Patches;

[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
internal static class RoundManager_DespawnPropsAtEndOfRound_Patch
{
	public static void Prefix(ref RoundManager __instance)
	{
		// make sure we're on the server
		// check if it's the failure state (all players died)
		// check that there's a current policy
		if (__instance.IsServer && StartOfRound.Instance.allPlayersDead && Plugin.Instance.State.Policy.Tier != PolicyTier.None)
		{
			var totalScrapValue = GameObject.Find("/Environment/HangarShip").GetComponentsInChildren<GrabbableObject>()
				.Where(grabbableObject => grabbableObject.itemProperties.isScrap && grabbableObject is not RagdollGrabbableObject)
				.Sum(grabbableObject => grabbableObject.scrapValue);
			// make sure there was at least some scrap
			if (totalScrapValue > 0)
			{
				var claimDay = StartOfRound.Instance.gameStats.daysSpent - 1;

				// create the claim
				Plugin.Instance.State.MutateClaims(claims => claims[claimDay] = new(totalScrapValue));
				Plugin.Logger.LogInfo($"crew all dead, recorded pending claim with value of {totalScrapValue} for day {claimDay}");

				// notify crew of pending insurance claim
				HUDManagerEvents.InsuranceClaimAvailable.InvokeClients();
			}
		}
	}
}
