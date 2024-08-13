using HarmonyLib;
using LethalNetworkAPI.Utils;
using Unity.Netcode;

namespace tesinormed.FAndCDaveCo.Patches;

[HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.Initialize))]
internal static class NetworkManager_Initialize_Patch
{
	public static void Postfix(ref NetworkManager __instance)
	{
		if (LNetworkUtils.IsHostOrServer) __instance.OnClientConnectedCallback += _ => { Plugin.Instance.State.MakeDirty(); };
	}
}
