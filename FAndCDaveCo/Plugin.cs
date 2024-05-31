using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using InteractiveTerminalAPI.UI;
using LethalModDataLib.Events;
using LethalNetworkAPI;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using System.Collections.Generic;
using tesinormed.FAndCDaveCo.Banking;
using tesinormed.FAndCDaveCo.Banking.UI;
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
		public static Terminal Terminal { get; set; } = null!;

		public static PolicyState PolicyState { get; private set; } = null!;
		public static LethalNetworkVariable<Policy> SyncedPolicy = new(identifier: PolicyState.POLICY_NETWORK_IDENTIFIER);
		public static LethalNetworkVariable<Dictionary<int, PolicyClaim>> SyncedClaims = new(identifier: PolicyState.CLAIMS_NETWORK_IDENTIFIER);

		public static BankState BankState { get; private set; } = null!;
		public static LethalNetworkVariable<Dictionary<int, int>> SyncedLoans = new(identifier: BankState.LOANS_NETWORK_IDENTIFIER);

		private void Awake()
		{
			Logger = base.Logger;
			Instance = this;

			InitNetworkVariables();
			InitState();
			RegisterTerminal();
			NetworkVariableEvents.Init();
			CreditEvents.Init();
			HUDManagerEvents.Init();

			Patch();

			Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
		}

		private void InitNetworkVariables()
		{
			SyncedPolicy.OnValueChanged += (data) =>
			{
				PolicyState.Policy = data;
				Logger.LogDebug($"synced policy state policy {data}");
			};
			SyncedClaims.OnValueChanged += (data) =>
			{
				PolicyState.Claims = data;
				Logger.LogDebug($"synced policy state claims {data}");
			};

			SyncedLoans.OnValueChanged += (data) =>
			{
				BankState.Loans = data;
				Logger.LogDebug($"synced bank state loans {data}");
			};
		}
		private void InitState()
		{
			PolicyState = new();
			BankState = new();

			SaveLoadEvents.PostSaveGameEvent += (_, _) =>
			{
				PolicyState.Save();
				BankState.Save();
			};
			SaveLoadEvents.PostLoadGameEvent += (_, _) =>
			{
				PolicyState.Load();
				BankState.Load();
			};
			SaveLoadEvents.PostDeleteSaveEvent += (_) =>
			{
				PolicyState.Reset();
				BankState.Reset();
			};
			SaveLoadEvents.PostResetSavedGameValuesEvent += () =>
			{
				PolicyState.Reset();
				BankState.Reset();

				PolicyState.Save();
				BankState.Save();
			};
		}

		private void RegisterTerminal()
		{
			InteractiveTerminalManager.RegisterApplication<PolicyInformationTerminal>(["insurance info", "insurance information", "insurance policy"]);
			InteractiveTerminalManager.RegisterApplication<PolicySelectTerminal>(["insurance select", "insurance configure"]);
			InteractiveTerminalManager.RegisterApplication<PolicyClaimTerminal>(["insurance claim", "insurance claims", "insurance make claim"]);

			InteractiveTerminalManager.RegisterApplication<BankLoanListTerminal>(["bank loan list", "bank loan info", "bank loans"]);
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
