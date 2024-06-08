using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalModDataLib.Events;
using LethalNetworkAPI;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using tesinormed.FAndCDaveCo.Bank;
using tesinormed.FAndCDaveCo.Bank.UI;
using tesinormed.FAndCDaveCo.Extensions;
using tesinormed.FAndCDaveCo.Insurance;
using tesinormed.FAndCDaveCo.Insurance.UI;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("BMX.LobbyCompatibility")]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI")]
[BepInDependency("MaxWasUnavailable.LethalModDataLib")]
[BepInDependency("LethalNetworkAPI")]
[BepInDependency("com.sigurd.csync", "5.0.0")]
[LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.Minor)]
public class Plugin : BaseUnityPlugin
{
	internal new static ManualLogSource Logger { get; private set; } = null!;
	public static Plugin Instance { get; private set; } = null!;
	internal new static Config Config { get; private set; } = null!;

	internal static Harmony? Harmony { get; set; }

	public static Terminal Terminal { get; set; } = null!;

	public static PolicyState PolicyState { get; private set; } = null!;
	public static readonly LethalNetworkVariable<Policy> SyncedPolicy = new(nameof(PolicyState.Policy));
	public static readonly LethalNetworkVariable<Dictionary<int, PolicyClaim>> SyncedClaims = new(nameof(PolicyState.Claims));

	public static BankState BankState { get; private set; } = null!;
	public static readonly LethalNetworkVariable<Loan> SyncedLoan = new(nameof(BankState.Loan));

	private void Awake()
	{
		Logger = base.Logger;
		Instance = this;
		Config = new Config(base.Config);

		InitNetworkVariables();
		InitState();
		RegisterTerminal();
		NetworkVariableEvents.Init();
		CreditEvents.Init();
		HUDManagerEvents.Init();

		Patch();

		Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} ({MyPluginInfo.PLUGIN_NAME}) version {MyPluginInfo.PLUGIN_VERSION} loaded");
	}

	private void InitNetworkVariables()
	{
		SyncedPolicy.OnValueChanged += data =>
		{
			PolicyState.Policy = data;
			Logger.LogDebug($"synced policy state policy {data}");
		};
		SyncedClaims.OnValueChanged += data =>
		{
			PolicyState.Claims = data;
			Logger.LogDebug($"synced policy state claims {data}");
		};
		SyncedLoan.OnValueChanged += data =>
		{
			BankState.Loan = data;
			Logger.LogDebug($"synced bank state loan {data}");
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
		SaveLoadEvents.PostDeleteSaveEvent += _ =>
		{
			PolicyState = new();
			BankState = new();
		};
		SaveLoadEvents.PostResetSavedGameValuesEvent += () =>
		{
			PolicyState = new();
			BankState = new();

			PolicyState.Save();
			BankState.Save();
		};
	}

	private void RegisterTerminal()
	{
		InteractiveTerminalManagerExtensions.RegisterApplication<PolicyInformationTerminal>("insurance info", "insurance information", "insurance policy");
		InteractiveTerminalManagerExtensions.RegisterApplication<PolicySelectTerminal>("insurance select", "insurance get");
		InteractiveTerminalManagerExtensions.RegisterApplication<PolicyClaimTerminal>("insurance claim", "insurance claims", "insurance make claim");

		InteractiveTerminalManagerExtensions.RegisterApplication<BankLoanInformationTerminal>("bank loan info", "bank loan information");
		InteractiveTerminalManagerExtensions.RegisterApplication<BankLoanGetTerminal>("bank loan get", "bank loan");
		InteractiveTerminalManagerExtensions.RegisterApplication<BankLoanPaymentTerminal>("bank loan pay", "bank loan payment");
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
