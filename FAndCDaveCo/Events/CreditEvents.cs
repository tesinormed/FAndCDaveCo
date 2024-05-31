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
			LethalClientMessage<int> deductGroupCredits = new(
				identifier: DEDUCT_GROUP_CREDITS_IDENTIFIER,
				onReceived: (amount) =>
				{
					if ((Plugin.Terminal.groupCredits - amount) < 0)
					{
						Plugin.Logger.LogWarning($"could not deduct {amount} credits; balance would be under 0");
						return;
					}

					Plugin.Terminal.groupCredits -= amount;
					Plugin.Logger.LogDebug($"deducted {amount} credits.");
				}
			);

			LethalClientMessage<int> spawnGoldBar = new(
				identifier: SPAWN_GOLD_BAR_IDENTIFIER,
				onReceived: (value) =>
				{
					var item = StartOfRound.Instance.allItemsList.itemsList.Single(item => item.itemName.ToLower() == "gold bar");
					Vector3 position = GameNetworkManager.Instance.localPlayerController.transform.position;
					var playerController = StartOfRound.Instance.localPlayerController;

					GameObject obj = Object.Instantiate(item.spawnPrefab, position, Quaternion.identity);

					obj.GetComponent<GrabbableObject>().itemProperties.isScrap = true;
					obj.GetComponent<GrabbableObject>().itemProperties.creditsWorth = value;
					obj.GetComponent<GrabbableObject>().SetScrapValue(value);
					playerController.SetItemInElevator(true, true, obj.GetComponent<GrabbableObject>());
					obj.transform.SetParent(GameObject.Find("/Environment/HangarShip").transform, worldPositionStays: true);

					obj.GetComponent<NetworkObject>().Spawn();
					Plugin.Logger.LogDebug($"spawned gold bar with value of {value} at player ID {playerController.GetClientId()}");
				}
			);
		}
	}
}
