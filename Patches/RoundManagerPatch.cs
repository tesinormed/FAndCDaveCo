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
			if (__instance.IsServer && StartOfRound.Instance.allPlayersDead && PolicyState.Instance.Policy.Tier != PolicyTier.NONE)
			{
				var lootValue = GameObject.Find("/Environment/HangarShip").GetComponentsInChildren<GrabbableObject>()
					.Where(obj => obj.itemProperties.isScrap && obj is not RagdollGrabbableObject)
					.ToList()
					.Sum(scrap => scrap.scrapValue);

				if (lootValue > 0)
				{
					PolicyState.Instance.Claims[GameStatistics.CurrentDay] = new(lootValue);
					PolicyState.Resync();

					Plugin.Logger.LogInfo($"crew all dead, recording pending claim with value of {lootValue} for day {GameStatistics.CurrentDay}");
				}
			}
		}
	}
}
