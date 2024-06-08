using System;
using System.Linq;
using LethalNetworkAPI;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace tesinormed.FAndCDaveCo.Network;

public static class CreditEvents
{
	public const string DeductGroupCreditsIdentifier = "DeductGroupCredits";
	public const string SpawnGoldBarIdentifier = "SpawnGoldBar";
	public const string SyncItemScrapValues = "SyncItemScrapValues";
	public const string SyncQuotaFulfilled = "SyncQuotaFulfilled";

	public static void Init()
	{
		LethalServerMessage<int> deductGroupCredits = new(DeductGroupCreditsIdentifier, onReceived: (amount, _) =>
		{
			// make sure credits won't be negative
			if (Plugin.Terminal.groupCredits - amount < 0)
			{
				Plugin.Logger.LogError($"could not deduct {amount} credits; balance would be under 0");
				return;
			}

			// deduct credits
			Plugin.Terminal.groupCredits -= amount;
			// sync credits
			Plugin.Terminal.SyncGroupCreditsServerRpc(Plugin.Terminal.groupCredits, Plugin.Terminal.numberOfItemsInDropship);

			Plugin.Logger.LogDebug($"deducted {amount} credits.");
		});

		LethalServerMessage<int> spawnGoldBar = new(SpawnGoldBarIdentifier, onReceived: (value, client) =>
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

			// position to spawn at
			var position = playerController.transform.position;

			var gameObject = Object.Instantiate(item.spawnPrefab, position, Quaternion.identity, GameObject.Find("/Environment/HangarShip").transform);
			// set object properties (scrap, value, location, etc)
			gameObject.GetComponent<GrabbableObject>().itemProperties.isScrap = true;
			gameObject.GetComponent<GrabbableObject>().itemProperties.creditsWorth = value;
			gameObject.GetComponent<GrabbableObject>().SetScrapValue(value);
			playerController.SetItemInElevator(true, true, gameObject.GetComponent<GrabbableObject>());
			// spawn object
			gameObject.GetComponent<NetworkObject>().Spawn(true);

			Plugin.Logger.LogDebug($"spawned gold bar with value of {value} at client {playerController.GetClientId()}");

			// sync over the network
			LethalServerMessage<Tuple<NetworkObjectReference, int>> syncItemScrapValues = new(SyncItemScrapValues);
			syncItemScrapValues.SendAllClients(new Tuple<NetworkObjectReference, int>(gameObject.GetComponent<NetworkObject>(), value));
		});

		LethalClientMessage<Tuple<NetworkObjectReference, int>> syncItemScrapValues = new(SyncItemScrapValues, onReceived: value =>
		{
			GameObject obj = value.Item1;
			obj.GetComponent<GrabbableObject>().SetScrapValue(value.Item2);
		});

		LethalServerMessage<int> syncQuotaFulfilledServer = new(SyncQuotaFulfilled, onReceived: (value, _) =>
		{
			LethalServerMessage<int> syncQuotaFulfilled = new(SyncQuotaFulfilled);
			syncQuotaFulfilled.SendAllClients(value);
		});

		LethalClientMessage<int> syncQuotaFulfilled = new(SyncQuotaFulfilled, onReceived: value =>
		{
			TimeOfDay.Instance.quotaFulfilled = value;
			TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
		});
	}
}
