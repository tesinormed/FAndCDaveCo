using System;
using System.Collections.Generic;
using System.Linq;
using LethalModDataLib.Features;
using LethalNetworkAPI;
using LethalNetworkAPI.Utils;
using tesinormed.FAndCDaveCo.Bank;
using tesinormed.FAndCDaveCo.Extensions;
using tesinormed.FAndCDaveCo.Insurance;

namespace tesinormed.FAndCDaveCo.Network;

public record State
{
	#region Game
	public static bool DisableDeathCreditPenalty => Plugin.Instance.State.Policy.Tier != PolicyTier.None && Plugin.Instance.Config.GameDisableDeathCreditPenalty.Value;
	#endregion

	#region Insurance
	private readonly LNetworkVariable<Policy> _policy = LNetworkVariable<Policy>.Connect(
		identifier: nameof(Policy),
		writePerms: LNetworkVariableWritePerms.Everyone,
		onValueChanged: (previous, current) => { Plugin.Logger.LogDebug($"{nameof(Policy)} changed from ({previous}) to ({current})"); }
	);
	public Policy Policy
	{
		get => _policy.Value;
		set => _policy.Value = value;
	}

	private readonly LNetworkVariable<Dictionary<int, Claim>?> _claims = LNetworkVariable<Dictionary<int, Claim>?>.Connect(
		identifier: nameof(Claims),
		writePerms: LNetworkVariableWritePerms.Everyone,
		onValueChanged: (previous, current) => { Plugin.Logger.LogDebug($"{nameof(Claims)} changed from ({string.Join(", ", previous ?? [])}) to ({string.Join(", ", current ?? [])})"); }
	);
	public Dictionary<int, Claim> Claims
	{
		get => _claims.Value ?? [];
		set => _claims.Value = value;
	}
	public void MutateClaims(Action<Dictionary<int, Claim>> mutation)
	{
		mutation(Claims);
		_claims.MakeDirty();
	}

	public Dictionary<int, Claim> ClaimedClaims => Claims.Where(pair => pair.Value.Claimed).ToDictionary();
	public Dictionary<int, Claim> UnclaimedClaims => Claims.Where(pair => !pair.Value.Claimed).ToDictionary();

	public double FractionalPremiumIncrease => ClaimedClaims.Count * Plugin.Instance.Config.ClaimPremiumIncrease.Value;
	public int TotalPremium => CalculateTotalPremium(Policy.BasePremium);
	internal int CalculateTotalPremium(Policy policy) => CalculateTotalPremium(policy.BasePremium);
	private int CalculateTotalPremium(int basePremium) => (int) (basePremium * (1.00 + FractionalPremiumIncrease));
	#endregion

	#region Bank
	private readonly LNetworkVariable<Loan> _loan = LNetworkVariable<Loan>.Connect(
		identifier: nameof(Loan),
		writePerms: LNetworkVariableWritePerms.Everyone,
		onValueChanged: (previous, current) => { Plugin.Logger.LogDebug($"{nameof(Loan)} changed from ({previous}) to ({current})"); }
	);
	public Loan Loan
	{
		get => _loan.Value;
		set => _loan.Value = value;
	}
	public void MutateLoan(Action<Loan> mutation)
	{
		mutation(Loan);
		_loan.MakeDirty();
	}
	#endregion

	internal void Save()
	{
		SaveLoadHandler.SaveData(Policy, nameof(Policy));
		SaveLoadHandler.SaveData(Claims, nameof(Claims));

		SaveLoadHandler.SaveData(Loan, nameof(Loan));
	}

	internal void Load()
	{
		if (!LNetworkUtils.IsHostOrServer) return;

		Policy = SaveLoadHandler.LoadData(nameof(Policy), defaultValue: Policy.None);
		Claims = SaveLoadHandler.LoadData<Dictionary<int, Claim>>(nameof(Claims)) ?? [];

		Loan = SaveLoadHandler.LoadData(nameof(Loan), defaultValue: Loan.None);
	}

	internal void Reset()
	{
		Policy = Policy.None;
		Claims = [];

		Loan = Loan.None;
	}

	internal void MakeDirty()
	{
		_policy.MakeDirty();
		_claims.MakeDirty();

		_loan.MakeDirty();
	}
}
