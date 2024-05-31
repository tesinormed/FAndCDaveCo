using LethalModDataLib.Attributes;
using LethalModDataLib.Base;
using LethalModDataLib.Enums;
using System.Collections.Generic;

namespace tesinormed.FAndCDaveCo.Banking
{
	[ES3Serializable]
	public class BankState : ModDataContainer
	{
		[ModDataIgnore(IgnoreFlags.None)]
		public const string LOANS_NETWORK_IDENTIFIER = "Loans";

		public Dictionary<int, int> Loans { get; set; } = [];

		public BankState() { }
		public BankState(Dictionary<int, int> loans)
		{
			Loans = loans;
		}

		public void Reset()
		{
			Loans = [];
			PostReset();
		}

		protected override void PostSave()
		{
			Plugin.Logger.LogDebug($"saved bank state {this}");
		}
		protected override void PostLoad()
		{
			Plugin.Logger.LogDebug($"loaded bank state {this}");
		}
		private void PostReset()
		{
			Plugin.Logger.LogDebug("reset bank state");
		}
	}
}
