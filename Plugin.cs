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
using tesinormed.FAndCDaveCo.Misc;

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
		public static LethalNetworkVariable<Policy> SyncedPolicy = LethalNetworkCreator.CreateVariable<Policy>(
			identifier: PolicyState.POLICY_NETWORK_IDENTIFIER,
			(data) =>
			{
				PolicyState.Policy = data;
				Logger.LogDebug($"synced policy state policy {data}");
			}
		);
		public static LethalNetworkVariable<Dictionary<int, PolicyClaim>> SyncedClaims = LethalNetworkCreator.CreateVariable<Dictionary<int, PolicyClaim>>(
			identifier: PolicyState.CLAIMS_NETWORK_IDENTIFIER,
			(data) =>
			{
				PolicyState.Claims = data;
				Logger.LogDebug($"synced policy state claims {data}");
			}
		);

		public static BankState BankState { get; private set; } = null!;
		public static LethalNetworkVariable<Dictionary<int, int>> SyncedLoans = LethalNetworkCreator.CreateVariable<Dictionary<int, int>>(
			identifier: BankState.LOANS_NETWORK_IDENTIFIER,
			(data) =>
			{
				BankState.Loans = data;
				Logger.LogDebug($"synced bank state loans {data}");
			}
		);

		private void Awake()
		{
			Logger = base.Logger;
			Instance = this;

			InitState();
			RegisterTerminal();
			CreditEvents.Init();
			HUDManagerEvents.Init();

			Patch();

			Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
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

				UpdateState();
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
		public static void UpdateState()
		{
			SyncedPolicy.Value = PolicyState.Policy;
			SyncedClaims.Value = PolicyState.Claims;

			SyncedLoans.Value = BankState.Loans;
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
