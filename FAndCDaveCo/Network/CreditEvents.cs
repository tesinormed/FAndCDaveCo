using System;
using System.Linq;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Bank;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace tesinormed.FAndCDaveCo.Network;

public static class CreditEvents
{
	public static LNetworkMessage<int> DeductGroupCredits = null!;

	public static LNetworkMessage<int> SpawnGoldBar = null!;
	internal static LNetworkMessage<Tuple<NetworkObjectReference, int>> SyncItemScrap = null!;

	public static LNetworkEvent TakeOutLoan = null!;

	internal static void Init()
	{
		DeductGroupCredits = LNetworkMessage<int>.Connect("DeductGroupCredits", onServerReceived: (amount, _) =>
		{
			// make sure credits won't be negative
			if (Plugin.Terminal.groupCredits < amount)
			{
				Plugin.Terminal.groupCredits = 0;
				Plugin.Logger.LogError($"could not completely deduct {amount} credits, balance set to zero");
				return;
			}

			// deduct credits
			Plugin.Terminal.groupCredits -= amount;
			// sync credits
			Plugin.Terminal.SyncGroupCreditsServerRpc(Plugin.Terminal.groupCredits, Plugin.Terminal.numberOfItemsInDropship);

			Plugin.Logger.LogDebug($"deducted {amount} credits");
		});

		SpawnGoldBar = LNetworkMessage<int>.Connect("SpawnGoldBar", onServerReceived: (value, client) =>
		{
			// find item
			var item = StartOfRound.Instance.allItemsList.itemsList.Single(item => item.itemName.ToLower() == "gold bar");

			// get player controller
			var playerController = client.GetPlayerController();
			if (playerController == null)
			{
				Plugin.Logger.LogError($"could not get player controller for client {client}");
				return;
			}

			// spawn the item
			var gameObject = Object.Instantiate(item.spawnPrefab, playerController.transform.position, Quaternion.identity, StartOfRound.Instance.elevatorTransform);
			var grabbableObject = gameObject.GetComponent<GrabbableObject>();
			// set scrap value
			grabbableObject.SetScrapValue(value);
			// mark as inside the ship
			playerController.SetItemInElevator(droppedInShipRoom: true, droppedInElevator: true, grabbableObject);
			// spawn object
			gameObject.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);

			Plugin.Logger.LogDebug($"spawned gold bar with value of {value} at client {playerController.GetClientId()}");

			// sync over the network
			SyncItemScrap.SendClients(new(gameObject.GetComponent<NetworkObject>(), value));
		});

		SyncItemScrap = LNetworkMessage<Tuple<NetworkObjectReference, int>>.Connect("SyncItemScrapValue", onClientReceived: value =>
		{
			GameObject gameObject = value.Item1;
			var grabbableObject = gameObject.GetComponent<GrabbableObject>();
			grabbableObject.SetScrapValue(value.Item2);
			GameNetworkManager.Instance.localPlayerController.SetItemInElevator(droppedInShipRoom: true, droppedInElevator: true, grabbableObject);
		});

		TakeOutLoan = LNetworkEvent.Connect("TakeOutLoan",
			onServerReceived: _ =>
			{
				Loan loan = new(issuanceDate: StartOfRound.Instance.gameStats.daysSpent, principal: TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled);
				Plugin.Instance.State.Loan = loan;
				Plugin.Logger.LogDebug($"took out a loan for {loan.Principal}");

				TimeOfDay.Instance.quotaFulfilled = TimeOfDay.Instance.profitQuota;

				TakeOutLoan.InvokeClients();
			},
			onClientReceived: () =>
			{
				TimeOfDay.Instance.quotaFulfilled = TimeOfDay.Instance.profitQuota;
				TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
				StartOfRound.Instance.AutoSaveShipData();
			}
		);
	}
}
