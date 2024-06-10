using HarmonyLib;

namespace tesinormed.FAndCDaveCo.Patches;

[HarmonyPatch(typeof(HUDManager))]
public static class HUDManagerPatch
{
	[HarmonyPatch("ApplyPenalty")]
	[HarmonyPrefix]
	public static bool ApplyPenalty()
	{
		return !Plugin.Config.GameDisableDeathCreditPenalty;
	}

	[HarmonyPatch("Awake")]
	[HarmonyPostfix]
	public static void Awake(ref HUDManager __instance)
	{
		if (Plugin.Config.GameDisableDeathCreditPenalty)
		{
			__instance.statsUIElements.penaltyTotal.transform.parent.parent.gameObject.SetActive(false);
		}
	}
}
