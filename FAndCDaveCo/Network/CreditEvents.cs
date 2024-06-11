﻿using System;
using System.Linq;
using LethalNetworkAPI;
using tesinormed.FAndCDaveCo.Bank;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace tesinormed.FAndCDaveCo.Network;

public static class CreditEvents
{
	public const string DeductGroupCreditsIdentifier = "DeductGroupCredits";
	public const string SpawnGoldBarIdentifier = "SpawnGoldBar";
	public const string SyncItemScrapValue = "SyncItemScrapValue";
	public const string TakeOutLoan = "TakeOutLoan";

	public static void Init()
	{
		LethalServerMessage<int> deductGroupCredits = new(DeductGroupCreditsIdentifier, onReceived: (amount, _) =>
		{
			// make sure credits won't be negative
			if (Plugin.Terminal.groupCredits < amount)
			{
				Plugin.Logger.LogError($"could not deduct {amount} credits; balance would be under 0");
				return;
			}

			// deduct credits
			Plugin.Terminal.groupCredits -= amount;
			// sync credits
			Plugin.Terminal.SyncGroupCreditsServerRpc(Plugin.Terminal.groupCredits, Plugin.Terminal.numberOfItemsInDropship);

			Plugin.Logger.LogDebug($"deducted {amount} credits");
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
			LethalServerMessage<Tuple<NetworkObjectReference, int>> syncItemScrapValue = new(SyncItemScrapValue);
			syncItemScrapValue.SendAllClients(new Tuple<NetworkObjectReference, int>(gameObject.GetComponent<NetworkObject>(), value));
		});

		LethalClientMessage<Tuple<NetworkObjectReference, int>> syncItemScrapValue = new(SyncItemScrapValue, onReceived: value =>
		{
			GameObject obj = value.Item1;
			obj.GetComponent<GrabbableObject>().SetScrapValue(value.Item2);
		});

		LethalServerEvent takeOutLoanServer = new(TakeOutLoan, onReceived: _ =>
		{
			Loan loan = new(issuanceDate: StartOfRound.Instance.gameStats.daysSpent, principal: TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled);
			Plugin.BankState.SetAndSyncLoan(loan);
			Plugin.Logger.LogDebug($"took out a loan for {loan.Principal}");

			TimeOfDay.Instance.quotaFulfilled = TimeOfDay.Instance.profitQuota;

			LethalServerEvent syncQuotaFulfilled = new(TakeOutLoan);
			syncQuotaFulfilled.InvokeAllClients();
		});

		LethalClientEvent takeOutLoan = new(TakeOutLoan, onReceived: () =>
		{
			TimeOfDay.Instance.quotaFulfilled = TimeOfDay.Instance.profitQuota;
			TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
			StartOfRound.Instance.AutoSaveShipData();
		});
	}
}
