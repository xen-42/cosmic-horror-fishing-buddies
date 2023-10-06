using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync;

public static class TeleportPlayer
{
	private static float _cachedAchievementDistance;
	private static GameObject _cachedTeleportDestination;
	private static TeleportAbility _teleportAbility;

	public static void To(GameObject destination)
	{
		// Respawn the player
		_teleportAbility = GameManager.Instance.PlayerAbilities.abilityMap["manifest"] as TeleportAbility;

		// Don't want to give them the achievement for this
		_cachedAchievementDistance = _teleportAbility.achievementDistance;
		_teleportAbility.achievementDistance = float.MaxValue;

		// Go to previous dock
		_cachedTeleportDestination = _teleportAbility.teleportDestinationObject;

		// Paranoid that it could be null, it shouldn't though
		if (destination != null) _teleportAbility.teleportDestinationObject = destination;

		GameEvents.Instance.OnTeleportComplete += OnTeleportComplete;

		_teleportAbility.Activate();
	}

	private static void OnTeleportComplete()
	{
		if (_teleportAbility != null)
		{
			// Reset achievement distance
			_teleportAbility.achievementDistance = _cachedAchievementDistance;

			// Reset the destination
			_teleportAbility.teleportDestinationObject = _cachedTeleportDestination;

			GameEvents.Instance.OnTeleportComplete -= OnTeleportComplete;
			_teleportAbility = null;
		}
	}
}
