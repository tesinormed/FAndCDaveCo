using BepInEx.Configuration;

namespace tesinormed.FAndCDaveCo;

public class Config
{
	#region Game
	public readonly ConfigEntry<bool> GameDisableDeathCreditPenalty;
	#endregion

	#region Insurance
	#region Policies
	#region Economic
	public readonly ConfigEntry<double> EconomicBasePremium;
	public readonly ConfigEntry<double> EconomicDeductible;
	public readonly ConfigEntry<double> EconomicDeductibleMaximum;
	public readonly ConfigEntry<double> EconomicDeductibleMinimum;
	#endregion

	#region Standard
	public readonly ConfigEntry<double> StandardBasePremium;
	public readonly ConfigEntry<double> StandardDeductible;
	public readonly ConfigEntry<double> StandardDeductibleMaximum;
	public readonly ConfigEntry<double> StandardDeductibleMinimum;
	#endregion

	#region Bespoke
	public readonly ConfigEntry<double> BespokeBasePremium;
	#endregion
	#endregion

	#region Coverage
	public readonly ConfigEntry<int> CoverageOption00;
	public readonly ConfigEntry<int> CoverageOption01;
	public readonly ConfigEntry<int> CoverageOption02;
	public readonly ConfigEntry<int> CoverageOption03;
	public readonly ConfigEntry<int> CoverageOption04;
	public readonly ConfigEntry<int> CoverageOption05;
	public readonly ConfigEntry<int> CoverageOption06;
	public readonly ConfigEntry<int> CoverageOption07;
	public readonly ConfigEntry<int> CoverageOption08;
	public readonly ConfigEntry<int> CoverageOption09;
	public readonly ConfigEntry<int> CoverageOption10;
	#endregion

	#region Claims
	public readonly ConfigEntry<int> ClaimRetentionDays;
	public readonly ConfigEntry<double> ClaimPremiumIncrease;
	#endregion
	#endregion

	#region Bank
	#region Loan
	#region Penalty
	public readonly ConfigEntry<int> PenaltyStartDaysFromIssuance;
	public readonly ConfigEntry<double> PenaltyAmount;
	#endregion

	public readonly ConfigEntry<double> InterestAmount;
	#endregion
	#endregion

	public Config(ConfigFile configFile)
	{
		#region Game
		GameDisableDeathCreditPenalty = configFile.Bind(
			new ConfigDefinition("Game", "DisableDeathCreditPenalty"),
			true,
			new ConfigDescription("Whether to disable the death credit penalty (only if there is a current insurance policy)")
		);
		#endregion

		#region Insurance
		#region Policies
		#region Economic
		EconomicBasePremium = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Economic", "BasePremium"),
			0.07,
			new ConfigDescription("Initial premiums before any additional penalties due to submitted claims")
		);
		EconomicDeductible = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Economic", "Deductible"),
			0.25,
			new ConfigDescription("Percent of claim amount that must be paid before a payout is made")
		);
		EconomicDeductibleMinimum = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Economic", "DeductibleMinimum"),
			0.15,
			new ConfigDescription("Percent of coverage that sets the deductible minimum amount")
		);
		EconomicDeductibleMaximum = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Economic", "DeductibleMaximum"),
			0.60,
			new ConfigDescription("Percent of coverage that sets the deductible maximum amount")
		);
		#endregion

		#region Standard
		StandardBasePremium = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Standard", "BasePremium"),
			0.15,
			new ConfigDescription("Initial premiums before any additional penalties due to submitted claims")
		);
		StandardDeductible = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Standard", "Deductible"),
			0.10,
			new ConfigDescription("Percent of claim amount that must be paid before a payout is made")
		);
		StandardDeductibleMinimum = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Standard", "DeductibleMinimum"),
			0.10,
			new ConfigDescription("Percent of coverage that sets the deductible minimum amount")
		);
		StandardDeductibleMaximum = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Standard", "DeductibleMaximum"),
			0.30,
			new ConfigDescription("Percent of coverage that sets the deductible maximum amount")
		);
		#endregion

		#region Bespoke
		BespokeBasePremium = configFile.Bind(
			new ConfigDefinition("Insurance.Policies.Bespoke", "BasePremium"),
			0.35,
			new ConfigDescription("Initial premiums before any additional penalties due to submitted claims")
		);
		#endregion
		#endregion

		#region Coverage
		CoverageOption00 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option00"),
			150
		);
		CoverageOption01 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option01"),
			400
		);
		CoverageOption02 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option02"),
			800
		);
		CoverageOption03 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option03"),
			1500
		);
		CoverageOption04 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option04"),
			2250
		);
		CoverageOption05 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option05"),
			3600
		);
		CoverageOption06 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option06"),
			5500
		);
		CoverageOption07 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option07"),
			8000
		);
		CoverageOption08 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option08"),
			11125
		);
		CoverageOption09 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option09"),
			15250
		);
		CoverageOption10 = configFile.Bind(
			new ConfigDefinition("Insurance.Coverage", "Option10"),
			18000
		);
		#endregion

		#region Claims
		ClaimPremiumIncrease = configFile.Bind(
			new ConfigDefinition("Insurance.Claims", "PremiumIncrease"),
			0.25,
			new ConfigDescription("Percent of the cumulative premium increase for each submitted claim")
		);
		ClaimRetentionDays = configFile.Bind(
			new ConfigDefinition("Insurance.Claims", "RetentionDays"),
			10,
			new ConfigDescription("Days that claims are retained in the save (sets the maximum increase for premiums)")
		);
		#endregion
		#endregion

		#region Bank
		#region Loan
		#region Penalty
		PenaltyStartDaysFromIssuance = configFile.Bind(
			new ConfigDefinition("Bank.Loan.Penalty", "StartDaysFromIssuance"),
			4,
			new ConfigDescription("Start of the late penalty in days from the issuance date")
		);
		PenaltyAmount = configFile.Bind(
			new ConfigDefinition("Bank.Loan.Penalty", "Amount"),
			0.10,
			new ConfigDescription("Percent of the credits garnished each day")
		);
		#endregion

		InterestAmount = configFile.Bind(
			new ConfigDefinition("Bank.Loan", "InterestAmount"),
			0.05,
			new ConfigDescription("Percent of the principal added on for interest (only added once)")
		);
		#endregion
		#endregion
	}
}
