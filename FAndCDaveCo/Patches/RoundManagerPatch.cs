using HarmonyLib;
using System.Linq;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Misc;
using UnityEngine;

namespace tesinormed.FAndCDaveCo.Patches
{
	[HarmonyPatch(typeof(RoundManager))]
	public static class RoundManagerPatch
	{
		[HarmonyPatch("DespawnPropsAtEndOfRound")]
		[HarmonyPrefix]
		public static void DespawnPropsAtEndOfRound(ref RoundManager __instance)
		{
			// make sure we're on the server
			// check if it's the failure state (all players died)
			// check that there's a current policy
			if (__instance.IsServer && StartOfRound.Instance.allPlayersDead && Plugin.PolicyState.Policy.Tier != PolicyTier.NONE)
			{
				var lootValue = GameObject.Find("/Environment/HangarShip").GetComponentsInChildren<GrabbableObject>()
					.Where(obj => obj.itemProperties.isScrap && obj is not RagdollGrabbableObject)
					.ToList()
					.Sum(scrap => scrap.scrapValue);

				// make sure there was at least some loot
				if (lootValue > 0)
				{
					// update claims
					Plugin.PolicyState.Claims[GameStatistics.CurrentDay] = new(lootValue);
					// sync over network
					Plugin.SyncedClaims.Value = Plugin.PolicyState.Claims;

					Plugin.Logger.LogInfo($"crew all dead, recording pending claim with value of {lootValue} for day {GameStatistics.CurrentDay}");
				}
			}
		}
	}
}
