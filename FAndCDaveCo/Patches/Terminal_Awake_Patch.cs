using HarmonyLib;

namespace tesinormed.FAndCDaveCo.Patches;

[HarmonyPatch(typeof(Terminal), nameof(Terminal.Awake))]
internal static class Terminal_Awake_Patch
{
	public static void Postfix(ref Terminal __instance)
	{
		Plugin.Terminal = __instance;
	}
}
