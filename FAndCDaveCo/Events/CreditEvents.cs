using LethalNetworkAPI;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace tesinormed.FAndCDaveCo.Events
{
	public static class CreditEvents
	{
		public const string DEDUCT_GROUP_CREDITS_IDENTIFIER = "DeductGroupCredits";
		public const string SPAWN_GOLD_BAR_IDENTIFIER = "SpawnGoldBar";

		public static void Init()
		{
			LethalServerMessage<int> deductGroupCredits = new(
				identifier: DEDUCT_GROUP_CREDITS_IDENTIFIER,
				onReceived: (amount, _) =>
				{
					// make sure credits won't be negative
					if ((Plugin.Terminal.groupCredits - amount) < 0)
					{
						Plugin.Logger.LogError($"could not deduct {amount} credits; balance would be under 0");
						return;
					}

					// deduct credits
					Plugin.Terminal.groupCredits -= amount;
					// sync credits
					Plugin.Terminal.SyncGroupCreditsServerRpc(Plugin.Terminal.groupCredits, Plugin.Terminal.numberOfItemsInDropship);

					Plugin.Logger.LogDebug($"deducted {amount} credits.");
				}
			);

			LethalServerMessage<int> spawnGoldBar = new(
				identifier: SPAWN_GOLD_BAR_IDENTIFIER,
				onReceived: (value, client) =>
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
					Vector3 position = playerController.transform.position;

					GameObject obj = Object.Instantiate(item.spawnPrefab, position, Quaternion.identity);
					// set object properties (scrap, value, location, etc)
					obj.GetComponent<GrabbableObject>().itemProperties.isScrap = true;
					obj.GetComponent<GrabbableObject>().itemProperties.creditsWorth = value;
					obj.GetComponent<GrabbableObject>().SetScrapValue(value);
					playerController.SetItemInElevator(true, true, obj.GetComponent<GrabbableObject>());
					obj.transform.SetParent(GameObject.Find("/Environment/HangarShip").transform, worldPositionStays: true);
					// spawn object
					obj.GetComponent<NetworkObject>().Spawn();

					Plugin.Logger.LogDebug($"spawned gold bar with value of {value} at client {playerController.GetClientId()}");
				}
			);
		}
	}
}
