using System;
using LethalModDataLib.Base;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Network;

namespace tesinormed.FAndCDaveCo.Bank;

public class BankState : ModDataContainer
{
	public Loan Loan { get; set; } = Loan.None;

	protected override void PostSave()
	{
		Plugin.Logger.LogDebug($"saved bank state {this}");
	}
	protected override void PostLoad()
	{
		Plugin.Logger.LogDebug($"loaded bank state {this}");

		// sync to network
		Plugin.SyncedLoan.Value = Loan;
	}

	public void SetAndSyncLoan(Loan loan)
	{
		Loan = loan;

		// sync over network
		LethalClientMessage<Loan> updateLoan = new(NetworkVariableEvents.UpdateLoanIdentifier);
		updateLoan.SendServer(loan);
	}

	public void UpdateAndSyncLoan(Action<Loan> action)
	{
		action.Invoke(Loan);

		// sync over network
		LethalClientMessage<Loan> updateLoan = new(NetworkVariableEvents.UpdateLoanIdentifier);
		updateLoan.SendServer(Loan);
	}
}
