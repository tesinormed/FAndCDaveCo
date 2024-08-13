using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalModDataLib.Events;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using tesinormed.FAndCDaveCo.Bank.UI;
using tesinormed.FAndCDaveCo.Insurance.UI;
using tesinormed.FAndCDaveCo.Network;
using static tesinormed.FAndCDaveCo.Extensions.InteractiveTerminalManager;

namespace tesinormed.FAndCDaveCo;

// mark this as a plugin with the information from the csproj file
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
// declare all of our dependencies
[BepInDependency("BMX.LobbyCompatibility")]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI")]
[BepInDependency("MaxWasUnavailable.LethalModDataLib")]
[BepInDependency("LethalNetworkAPI", MinimumDependencyVersion: "3.0.0")]
[BepInDependency("com.sigurd.csync", MinimumDependencyVersion: "5.0.0")]
// mark this plugin as required on both sides (server and client)
// mark this plugin as requiring the same minor version (Y, where X.Y.Z)
[LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.Minor)]
public class Plugin : BaseUnityPlugin
{
	// boilerplate setup
	public static Plugin Instance { get; private set; } = null!;
	internal new static ManualLogSource Logger { get; private set; } = null!;
	internal static Terminal Terminal { get; set; } = null!; // access to singleton terminal (set in a patch for Terminal.Awake)

	internal new Config Config { get; private set; } = null!;
	public State State { get; private set; } = null!;

	private void Awake()
	{
		// boilerplate initialisation
		Instance = this;
		Logger = base.Logger;

		Config = new(base.Config);
		State = new();
		// set up SaveLoadEvents hooks for automatic saving, loading, deleting, and resetting
		SaveLoadEvents.PostSaveGameEvent += (_, _) =>
		{
			State.Save();
		};
		SaveLoadEvents.PostLoadGameEvent += (_, _) =>
		{
			State.Load();
		};
		SaveLoadEvents.PostDeleteSaveEvent += _ =>
		{
			State.Reset();
			HUDManagerEvents.QueuedHudTips.Clear();
		};
		SaveLoadEvents.PostResetSavedGameValuesEvent += () =>
		{
			State.Reset();
			HUDManagerEvents.QueuedHudTips.Clear();

			State.Save();
		};

		// set up events
		CreditEvents.Init();
		HUDManagerEvents.Init();

		// register terminal applications for the insurance
		RegisterApplication<PolicyInformationTerminal>("insurance info", "insurance information", "insurance policy");
		RegisterApplication<PolicySelectTerminal>("insurance select", "insurance get");
		RegisterApplication<PolicyClaimTerminal>("insurance claim", "insurance claims", "insurance make claim");

		// register terminal applications for the bank
		RegisterApplication<BankLoanInformationTerminal>("bank loan info", "bank loan information");
		RegisterApplication<BankLoanGetTerminal>("bank loan get", "bank loan");
		RegisterApplication<BankLoanPaymentTerminal>("bank loan pay", "bank loan payment");

		// patch any base game classes
		var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

		Logger.LogDebug("patching in progress");
		harmony.PatchAll(Assembly.GetExecutingAssembly());
		Logger.LogDebug("finished patching");

		// notify that loading is complete
		Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} ({MyPluginInfo.PLUGIN_NAME}) version {MyPluginInfo.PLUGIN_VERSION} loaded");
	}
}
