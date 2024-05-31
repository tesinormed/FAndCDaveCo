using LethalNetworkAPI;
using System.Collections.Generic;
using tesinormed.FAndCDaveCo.Insurance;

namespace tesinormed.FAndCDaveCo.Events
{
	public static class NetworkVariableEvents
	{
		public const string UPDATE_POLICY_IDENTIFIER = "UpdatePolicy";
		public const string UPDATE_CLAIMS_IDENTIFIER = "UpdateClaims";
		public const string UPDATE_LOANS_IDENTIFIER = "UpdateLoans";

		public static void Init()
		{
			LethalServerMessage<Policy> updatePolicy = new(
				identifier: UPDATE_POLICY_IDENTIFIER,
				onReceived: (value, _) =>
				{
					Plugin.SyncedPolicy.Value = value;
				}
			);

			LethalServerMessage<Dictionary<int, PolicyClaim>> updateClaims = new(
				identifier: UPDATE_CLAIMS_IDENTIFIER,
				onReceived: (value, _) =>
				{
					Plugin.SyncedClaims.Value = value;
				}
			);

			LethalServerMessage<Dictionary<int, int>> updateLoans = new(
				identifier: UPDATE_LOANS_IDENTIFIER,
				onReceived: (value, _) =>
				{
					Plugin.SyncedLoans.Value = value;
				}
			);
		}
	}
}
