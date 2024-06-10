using System.Linq;
using HarmonyLib;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Network;
using UnityEngine;

namespace tesinormed.FAndCDaveCo.Patches;

[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
public static class RoundManager_DespawnPropsAtEndOfRound_Patch
{
	public static void Prefix(ref RoundManager __instance)
	{
		// make sure we're on the server
		// check if it's the failure state (all players died)
		// check that there's a current policy
		if (__instance.IsServer && StartOfRound.Instance.allPlayersDead && Plugin.PolicyState.Policy.Tier != PolicyTier.None)
		{
			var lootValue = GameObject.Find("/Environment/HangarShip").GetComponentsInChildren<GrabbableObject>()
				.Where(grabbableObject => grabbableObject.itemProperties.isScrap && grabbableObject is not RagdollGrabbableObject)
				.Sum(grabbableObject => grabbableObject.scrapValue);

			// make sure there was at least some loot
			if (lootValue > 0)
			{
				Plugin.PolicyState.UpdateAndSyncClaims(claims => claims[StartOfRound.Instance.gameStats.daysSpent - 1] = new(lootValue));
				Plugin.Logger.LogInfo($"crew all dead, recording pending claim with value of {lootValue} for day {StartOfRound.Instance.gameStats.daysSpent - 1}");

				// notify crew of pending insurance claim
				LethalServerEvent insuranceClaimAvailable = new(HUDManagerEvents.InsuranceClaimAvailableIdentifier);
				insuranceClaimAvailable.InvokeAllClients();
			}
		}
	}
}
