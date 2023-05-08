using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Extensions;
using Mirror;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal class RemoteBoatGraphics : NetworkBehaviour
	{
		[SyncVar(hook = nameof(FillPercentHook))]
		private float _fillPercent;

		[SyncVar(hook = nameof(DamageHook))]
		private int _damage;

		[SyncVar(hook = nameof(CriticalDamageHook))]
		private bool _criticalDamage;

		[SyncVar(hook = nameof(UpgradeTierHook))]
		private int _upgradeTier;

		private SyncList<int> _tier1Children = new();
		private SyncList<int> _tier2Children = new();
		private SyncList<int> _tier3Children = new();
		private SyncList<int> _tier4Children = new();

		[Command]
		private void SetActiveChildren(int[] tier1, int[] tier2, int[] tier3, int[] tier4)
		{
			_tier1Children.Clear();
			_tier1Children.AddRange(tier1);

			_tier2Children.Clear();
			_tier2Children.AddRange(tier2);

			_tier3Children.Clear();
			_tier3Children.AddRange(tier3);

			_tier4Children.Clear();
			_tier4Children.AddRange(tier4);

			RpcRefreshActiveChildren();
		}

		[ClientRpc]
		private void RpcRefreshActiveChildren() => RefreshActiveChildren();

		[Command]
		public void SetUpgradeTier(int upgradeTier) => _upgradeTier = upgradeTier;
		[Command]
		public void SetDamage(int damage, bool critical)
		{
			_damage = damage;
			_criticalDamage = critical;
		}
		[Command]
		public void SetFillPercent(float fillPercent) => _fillPercent = fillPercent;

		public void UpgradeTierHook(int prev, int current) => RefreshUpgradeTier();
		public void DamageHook(int prev, int current) => OnRemotePlayerDamageChanged();
		public void CriticalDamageHook(bool prev, bool current) => OnRemotePlayerDamageChanged();
		public void FillPercentHook(float prev, float current) => OnRemoteFillPercentChanged();

		public UnityEvent RefreshBoatModel = new();

		public BoatSubModelToggler[] boatSubModelTogglers;
		public BoatModelProxy[] boatModelProxies;

		public BoatModelProxy CurrentBoatModelProxy { get; private set; }
		public BoatSubModelToggler CurrentBoatSubModelToggler { get; private set; }

		public GameObject wake;

		public void Start()
		{
			try
			{
				if (isOwned)
				{
					GameEvents.Instance.OnUpgradesChanged += OnLocalUpgradesChanged;
					GameEvents.Instance.OnPlayerDamageChanged += OnLocalPlayerDamageChanged;
					GameManager.Instance.SaveData.Inventory.OnContentsUpdated += OnLocalInventoryContentsUpdated;

					// Initial state
					CheckLocalActiveChildren();
					SetUpgradeTier(GetLocalUpgradeTier());
					OnLocalPlayerDamageChanged();
					OnLocalInventoryContentsUpdated();
				}
				else
				{
					RefreshUpgradeTier();
					foreach (var boatSubModelToggler in boatSubModelTogglers)
					{
						GameEvents.Instance.OnPlayerDamageChanged -= boatSubModelToggler.OnPlayerDamageChanged;
						GameEvents.Instance.OnPlayerStatsChanged -= boatSubModelToggler.OnPlayerStatsChanged;
						GameManager.Instance.SaveData.Inventory.OnContentsUpdated -= boatSubModelToggler.RefreshFishContainers;
					}
				}
			}

			catch (Exception e)
			{
				CFBCore.LogError($"Failed to make remote player {e}");
			}
		}

		public void CheckLocalActiveChildren()
		{
			var tier1 = GameManager.Instance.Player._allBoatModelProxies[0].gameObject.GetChildren().FindIndices(x => x.activeInHierarchy).ToArray();
			var tier2 = GameManager.Instance.Player._allBoatModelProxies[1].gameObject.GetChildren().FindIndices(x => x.activeInHierarchy).ToArray();
			var tier3 = GameManager.Instance.Player._allBoatModelProxies[2].gameObject.GetChildren().FindIndices(x => x.activeInHierarchy).ToArray();
			var tier4 = GameManager.Instance.Player._allBoatModelProxies[3].gameObject.GetChildren().FindIndices(x => x.activeInHierarchy).ToArray();

			SetActiveChildren(tier1, tier2, tier3, tier4);
		}

		private int GetLocalUpgradeTier() => Math.Clamp(GameManager.Instance.Player._allBoatModelProxies.IndexOf(GameManager.Instance.Player.BoatModelProxy), 0, 3);

		public void OnDestroy()
		{
			if (isOwned)
			{
				GameEvents.Instance.OnUpgradesChanged -= OnLocalUpgradesChanged;
				GameEvents.Instance.OnPlayerDamageChanged -= OnLocalPlayerDamageChanged;
			}
		}

		private void OnLocalUpgradesChanged(UpgradeData upgradeData)
		{
			if (upgradeData is HullUpgradeData)
			{
				SetUpgradeTier(upgradeData.tier);
			}
			CheckLocalActiveChildren();
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

					RefreshActiveChildren();
					OnRemotePlayerDamageChanged();
					OnRemoteFillPercentChanged();
				}
				catch (Exception e)
				{
					CFBCore.LogError($"Failed to refresh upgrade tier {e}");
				}
			}
		}

		public void OnRemotePlayerDamageChanged()
		{
			if (CurrentBoatSubModelToggler != null)
			{
				CurrentBoatSubModelToggler.hullCriticalEffects.SetActive(_criticalDamage);
				CurrentBoatSubModelToggler.meshFilter.mesh = CurrentBoatModelProxy.damageStateMeshes[_damage];
			}
		}

		public void OnRemoteFillPercentChanged()
		{
			for(int i = 0; i < CurrentBoatSubModelToggler.fishContainers.Length; i++)
			{
				var fishContainer = CurrentBoatSubModelToggler.fishContainers[i];
				fishContainer.gameObject.SetActive(i < Mathf.CeilToInt(_fillPercent * CurrentBoatSubModelToggler.fishContainers.Length));
			}
		}

		public void OnLocalPlayerDamageChanged()
		{
			var toggler = GameManager.Instance.Player.BoatModelProxy.GetComponent<BoatSubModelToggler>();
			_damage = GameManager.Instance.Player.BoatModelProxy.DamageStateMeshes.IndexOf(toggler.meshFilter.mesh);
			_criticalDamage = toggler.hullCriticalEffects.activeInHierarchy;
		}

		public void OnLocalInventoryContentsUpdated()
		{
			float num = GameManager.Instance.SaveData.Inventory.GetFillProportional(ItemSubtype.FISH) + GameManager.Instance.SaveData.Inventory.GetFillProportional(ItemSubtype.TRINKET);
			SetFillPercent(num);
		}

		public void RefreshActiveChildren()
		{
			if (!isOwned)
			{
				RefreshChildrenForTier(boatModelProxies[0], _tier1Children);
				RefreshChildrenForTier(boatModelProxies[1], _tier2Children);
				RefreshChildrenForTier(boatModelProxies[2], _tier3Children);
				RefreshChildrenForTier(boatModelProxies[3], _tier4Children);
			}
		}

		private void RefreshChildrenForTier(BoatModelProxy boatProxy, SyncList<int> children)
		{
			//TODO: Is this deterministic?
			CFBCore.LogInfo($"Updating children on {netId} tier {boatProxy.name} for {PlayerManager.LocalNetID}");

			try
			{
				foreach (Transform child in boatProxy.transform)
				{
					child.gameObject.SetActive(false);
				}

				foreach (var childInd in children)
				{
					boatProxy.transform.GetChild(childInd).gameObject.SetActive(true);
				}
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Couldn't update children : {e}");
			}
		}
	}
}
