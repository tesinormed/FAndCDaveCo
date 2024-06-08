using System.Collections.Generic;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Bank;
using tesinormed.FAndCDaveCo.Insurance;

namespace tesinormed.FAndCDaveCo.Network;

public static class NetworkVariableEvents
{
	public const string UpdatePolicyIdentifier = "UpdatePolicy";
	public const string UpdateClaimsIdentifier = "UpdateClaims";
	public const string UpdateLoanIdentifier = "UpdateLoan";

	public static void Init()
	{
		LethalServerMessage<Policy> updatePolicy = new(UpdatePolicyIdentifier, onReceived: (value, _) => { Plugin.SyncedPolicy.Value = value; });
		LethalServerMessage<Dictionary<int, PolicyClaim>> updateClaims = new(UpdateClaimsIdentifier, onReceived: (value, _) => { Plugin.SyncedClaims.Value = value; });
		LethalServerMessage<Loan> updateLoan = new(UpdateLoanIdentifier, onReceived: (value, _) => { Plugin.SyncedLoan.Value = value; });
	}
}
