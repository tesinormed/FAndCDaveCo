namespace tesinormed.FAndCDaveCo.Misc
{
	public static class GameStatistics
	{
		public static int CurrentDay => StartOfRound.Instance.gameStats.daysSpent;
		public static bool IsHostingGame => GameNetworkManager.Instance.isHostingGame;
		public static bool IsInOrbit => StartOfRound.Instance.inShipPhase && GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom;
	}
}
