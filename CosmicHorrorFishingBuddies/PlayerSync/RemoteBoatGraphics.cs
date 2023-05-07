using CosmicHorrorFishingBuddies.Core;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal class RemoteBoatGraphics : NetworkBehaviour
	{
		[SyncVar(hook = nameof(UpgradeTierHook))]
		private int _upgradeTier;

		[Command]
		public void SetUpgradeTier(int upgradeTier) => _upgradeTier = upgradeTier;

		public void UpgradeTierHook(int prev, int current) => RefreshUpgradeTier();

		public UnityEvent RefreshBoatModel = new();

		public BoatSubModelToggler[] boatSubModelTogglers;
		public BoatModelProxy[] boatModelProxies;

		public BoatModelProxy CurrentBoatModelProxy { get; private set; }
		public BoatSubModelToggler CurrentBoatSubModelToggler { get; private set; }

		public GameObject wake;

		public void Start()
		{
			if (isOwned)
			{
				GameEvents.Instance.OnUpgradesChanged += OnUpgradesChanged;

				// Initial state
				SetUpgradeTier(GetLocalUpgradeTier());
			}
			else
			{
				try
				{
					RefreshUpgradeTier();
				}
				catch (Exception e)
				{
					CFBCore.LogError($"Failed to make remote player {e}");
				}
			}
		}

		private int GetLocalUpgradeTier() => Math.Clamp(GameManager.Instance.Player._allBoatModelProxies.IndexOf(GameManager.Instance.Player.BoatModelProxy), 0, 3);

		public void OnDestroy()
		{
			if (isOwned)
			{
				GameEvents.Instance.OnUpgradesChanged -= OnUpgradesChanged;
			}
		}

		private void OnUpgradesChanged(UpgradeData upgradeData)
		{
			if (upgradeData is HullUpgradeData)
			{
				SetUpgradeTier(upgradeData.tier);
			}
		}

		public void RefreshUpgradeTier()
		{
			if (!isOwned)
			{
				try
				{
					CFBCore.LogInfo($"Player {netId} has ship hull upgrade tier {_upgradeTier}");

					foreach (var boatModel in boatModelProxies)
					{
						boatModel.gameObject.SetActive(false);
						boatModel.enabled = false;
					}
					CurrentBoatModelProxy = boatModelProxies[_upgradeTier];
					CurrentBoatModelProxy.gameObject.SetActive(true);

					foreach (var boatSubModelToggler in boatSubModelTogglers)
					{
						boatSubModelToggler.gameObject.SetActive(false);
						boatSubModelToggler.enabled = false;
					}
					CurrentBoatSubModelToggler = boatSubModelTogglers[_upgradeTier];
					CurrentBoatSubModelToggler.gameObject.SetActive(true);

					RefreshBoatModel?.Invoke();
				}
				catch (Exception e)
				{
					CFBCore.LogError($"Failed to refresh upgrade tier {e}");
				}
			}
		}
	}
}
