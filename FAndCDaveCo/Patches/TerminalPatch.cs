using HarmonyLib;

namespace tesinormed.FAndCDaveCo.Patches;

[HarmonyPatch(typeof(Terminal))]
public static class TerminalPatch
{
	[HarmonyPatch("Awake")]
	[HarmonyPostfix]
	public static void Awake(ref Terminal __instance)
	{
		Plugin.Terminal = __instance;
	}
}
