using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using InteractiveTerminalAPI.UI;
using LethalModDataLib.Events;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using tesinormed.FAndCDaveCo.Events;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Insurance.UI;

namespace tesinormed.FAndCDaveCo
{
	[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
	[BepInDependency("BMX.LobbyCompatibility")]
	[BepInDependency("WhiteSpike.InteractiveTerminalAPI")]
	[BepInDependency("MaxWasUnavailable.LethalModDataLib")]
	[BepInDependency("LethalNetworkAPI")]
	[LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.Minor)]
	public class Plugin : BaseUnityPlugin
	{
		public static Plugin Instance { get; private set; } = null!;
		internal new static ManualLogSource Logger { get; private set; } = null!;
		internal static Harmony? Harmony { get; set; }
		public static Terminal Terminal { get; internal set; } = null!;

		private void Awake()
		{
			Logger = base.Logger;
			Instance = this;

			InitNetworkVariables();
			RegisterTerminal();
			CreditEvents.Init();
			HUDManagerEvents.Init();

			Patch();

			Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
		}

		private void InitNetworkVariables()
		{
			SaveLoadEvents.PostSaveGameEvent += (_, _) =>
			{
				PolicyState.Instance.Save();
				Logger.LogInfo($"saved policy state {PolicyState.Instance}");
			};
			SaveLoadEvents.PostLoadGameEvent += (_, _) =>
			{
				PolicyState.Instance.Load();
				Logger.LogInfo($"loaded policy state {PolicyState.Instance}");
			};
			SaveLoadEvents.PostDeleteSaveEvent += (_) =>
			{
				PolicyState.Instance.Reset();
				Logger.LogInfo($"reset policy state");
			};
			SaveLoadEvents.PostResetSavedGameValuesEvent += () =>
			{
				PolicyState.Instance.Reset();
				PolicyState.Instance.Save();
				Logger.LogInfo($"reset and saved policy state");
			};
		}

		private void RegisterTerminal()
		{
			InteractiveTerminalManager.RegisterApplication<PolicyInformationTerminal>(["insurance info", "insurance information", "insurance policy"]);
			InteractiveTerminalManager.RegisterApplication<PolicySelectTerminal>(["insurance select", "insurance configure"]);
			InteractiveTerminalManager.RegisterApplication<PolicyClaimTerminal>(["insurance claim", "insurance claims", "insurance make claim"]);
		}

		internal static void Patch()
		{
			Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

			Logger.LogDebug("Patching...");

			Harmony.PatchAll();

			Logger.LogDebug("Finished patching!");
		}

		internal static void Unpatch()
		{
			Logger.LogDebug("Unpatching...");

			Harmony?.UnpatchSelf();

			Logger.LogDebug("Finished unpatching!");
		}
	}
}
