using CosmicHorrorFishingBuddies.Util;
using HarmonyLib;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.Respawn;

[HarmonyPatch(typeof(Player))]
internal class RespawnPatch
{
	private static float _cachedAchievementDistance;
	private static GameObject _cachedTeleportDestination;
	private static TeleportAbility _teleportAbility;

	[HarmonyPrefix]
	[HarmonyPatch(nameof(Player.Die), new Type[] { })]
	public static bool Player_Die(Player __instance)
	{
		// Will just ignore their death
		if (__instance.IsGodModeEnabled || !__instance.IsAlive)
		{
			return true;
		}

		// Respawn the player
		_teleportAbility = GameManager.Instance.PlayerAbilities.abilityMap["manifest"] as TeleportAbility;

		// Don't want to give them the achievement for this
		_cachedAchievementDistance = _teleportAbility.achievementDistance;
		_teleportAbility.achievementDistance = float.MaxValue;

		// Go to previous dock
		_cachedTeleportDestination = _teleportAbility.teleportDestinationObject;
		var destination = GameManager.Instance.Player.PreviousDock?.GetComponentInChildren<DockPOI>()?.dockSlots[0]?.gameObject;
		// Paranoid that it could be null, it shouldn't though
		if (destination != null) _teleportAbility.teleportDestinationObject = destination;

		GameEvents.Instance.OnTeleportComplete += Instance_OnTeleportComplete;

		_teleportAbility.Activate();

		return false;
	}

	private static void Instance_OnTeleportComplete()
	{
		if (_teleportAbility != null)
		{
			// Reset achievement distance
			_teleportAbility.achievementDistance = _cachedAchievementDistance;

			// Reset the destination
			_teleportAbility.teleportDestinationObject = _cachedTeleportDestination;

			// Tank their sanity
			GameManager.Instance.Player.Sanity.ChangeSanity(-GameManager.Instance.Player.Sanity.CurrentSanity);

			GameEvents.Instance.OnTeleportComplete -= Instance_OnTeleportComplete;
			_teleportAbility = null;

			NotificationHelper.ShowNotificationWithColour(NotificationType.DAMAGE_TAKEN, "An unknown force keeps you from death...", DredgeColorTypeEnum.NEGATIVE);
		}
	}
}



