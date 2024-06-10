using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;

namespace tesinormed.FAndCDaveCo;

public class Config : SyncedConfig2<Config>
{
	#region Game
	[SyncedEntryField] public readonly SyncedEntry<bool> GameDisableDeathCreditPenalty;
	#endregion

	#region Insurance
	#region Policies
	#region Economic
	[SyncedEntryField] public readonly SyncedEntry<double> EconomicBasePremium;
	[SyncedEntryField] public readonly SyncedEntry<double> EconomicDeductible;
	[SyncedEntryField] public readonly SyncedEntry<double> EconomicDeductibleMaximum;
	[SyncedEntryField] public readonly SyncedEntry<double> EconomicDeductibleMinimum;
	#endregion

	#region Standard
	[SyncedEntryField] public readonly SyncedEntry<double> StandardBasePremium;
	[SyncedEntryField] public readonly SyncedEntry<double> StandardDeductible;
	[SyncedEntryField] public readonly SyncedEntry<double> StandardDeductibleMaximum;
	[SyncedEntryField] public readonly SyncedEntry<double> StandardDeductibleMinimum;
	#endregion

	#region Bespoke
	[SyncedEntryField] public readonly SyncedEntry<double> BespokeBasePremium;
	#endregion
	#endregion

	#region Coverage
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption00;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption01;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption02;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption03;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption04;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption05;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption06;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption07;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption08;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption09;
	[SyncedEntryField] public readonly SyncedEntry<int> CoverageOption10;
	#endregion

	#region Claims
	[SyncedEntryField] public readonly SyncedEntry<int> ClaimRetentionDays;
	[SyncedEntryField] public readonly SyncedEntry<double> ClaimPremiumIncrease;
	#endregion
	#endregion

	#region Bank
	#region Loan
	#region Penalty
	[SyncedEntryField] public readonly SyncedEntry<int> PenaltyStartDaysFromIssuance;
	[SyncedEntryField] public readonly SyncedEntry<double> PenaltyAmount;
	#endregion

	[SyncedEntryField] public readonly SyncedEntry<double> InterestAmount;
	#endregion
	#endregion

	public Config(ConfigFile configFile) : base(MyPluginInfo.PLUGIN_GUID)
	{
		#region Game
		GameDisableDeathCreditPenalty = configFile.BindSyncedEntry(
			new ConfigDefinition("Game", "DisableDeathCreditPenalty"),
			defaultValue: true,
			new ConfigDescription("Whether to disable the death credit penalty (only if there is a current insurance policy)")
		);
		#endregion

		#region Insurance
		#region Policies
		#region Economic
		EconomicBasePremium = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Economic", "BasePremium"),
			defaultValue: 0.07,
			new ConfigDescription("Initial premiums before any additional penalties due to submitted claims")
		);
		EconomicDeductible = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Economic", "Deductible"),
			defaultValue: 0.25,
			new ConfigDescription("Percent of claim amount that must be paid before a payout is made")
		);
		EconomicDeductibleMinimum = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Economic", "DeductibleMinimum"),
			defaultValue: 0.15,
			new ConfigDescription("Percent of coverage that sets the deductible minimum amount")
		);
		EconomicDeductibleMaximum = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Economic", "DeductibleMaximum"),
			defaultValue: 0.60,
			new ConfigDescription("Percent of coverage that sets the deductible maximum amount")
		);
		#endregion

		#region Standard
		StandardBasePremium = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Standard", "BasePremium"),
			defaultValue: 0.15,
			new ConfigDescription("Initial premiums before any additional penalties due to submitted claims")
		);
		StandardDeductible = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Standard", "Deductible"),
			defaultValue: 0.10,
			new ConfigDescription("Percent of claim amount that must be paid before a payout is made")
		);
		StandardDeductibleMinimum = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Standard", "DeductibleMinimum"),
			defaultValue: 0.10,
			new ConfigDescription("Percent of coverage that sets the deductible minimum amount")
		);
		StandardDeductibleMaximum = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Standard", "DeductibleMaximum"),
			defaultValue: 0.30,
			new ConfigDescription("Percent of coverage that sets the deductible maximum amount")
		);
		#endregion

		#region Bespoke
		BespokeBasePremium = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Policies.Bespoke", "BasePremium"),
			defaultValue: 0.35,
			new ConfigDescription("Initial premiums before any additional penalties due to submitted claims")
		);
		#endregion
		#endregion

		#region Coverage
		CoverageOption00 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option00"),
			defaultValue: 150
		);
		CoverageOption01 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option01"),
			defaultValue: 400
		);
		CoverageOption02 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option02"),
			defaultValue: 800
		);
		CoverageOption03 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option03"),
			defaultValue: 1500
		);
		CoverageOption04 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option04"),
			defaultValue: 2250
		);
		CoverageOption05 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option05"),
			defaultValue: 3600
		);
		CoverageOption06 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option06"),
			defaultValue: 5500
		);
		CoverageOption07 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option07"),
			defaultValue: 8000
		);
		CoverageOption08 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option08"),
			defaultValue: 11125
		);
		CoverageOption09 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option09"),
			defaultValue: 15250
		);
		CoverageOption10 = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Coverage", "Option10"),
			defaultValue: 18000
		);
		#endregion

		#region Claims
		ClaimPremiumIncrease = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Claims", "PremiumIncrease"),
			defaultValue: 0.25,
			new ConfigDescription("Percent of the cumulative premium increase for each submitted claim")
		);
		ClaimRetentionDays = configFile.BindSyncedEntry(
			new ConfigDefinition("Insurance.Claims", "RetentionDays"),
			defaultValue: 10,
			new ConfigDescription("Days that claims are retained in the save (sets the maximum increase for premiums)")
		);
		#endregion
		#endregion

		#region Bank
		#region Loan
		#region Penalty
		PenaltyStartDaysFromIssuance = configFile.BindSyncedEntry(
			new ConfigDefinition("Bank.Loan.Penalty", "StartDaysFromIssuance"),
			defaultValue: 4,
			new ConfigDescription("Start of the late penalty in days from the issuance date")
		);
		PenaltyAmount = configFile.BindSyncedEntry(
			new ConfigDefinition("Bank.Loan.Penalty", "Amount"),
			defaultValue: 0.10,
			new ConfigDescription("Percent of the credits garnished each day")
		);
		#endregion

		InterestAmount = configFile.BindSyncedEntry(
			new ConfigDefinition("Bank.Loan", "InterestAmount"),
			defaultValue: 0.05,
			new ConfigDescription("Percent of the principal added on for interest (only added once)")
		);
		#endregion
		#endregion

		ConfigManager.Register(this);
	}
}
