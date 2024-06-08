using LethalModDataLib.Attributes;
using LethalModDataLib.Base;
using LethalModDataLib.Enums;
using tesinormed.FAndCDaveCo.Extensions;

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
}
