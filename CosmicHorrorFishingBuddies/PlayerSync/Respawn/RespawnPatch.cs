using CosmicHorrorFishingBuddies.Util;
using HarmonyLib;
using System;

namespace CosmicHorrorFishingBuddies.PlayerSync.Respawn;

[HarmonyPatch(typeof(Player))]
internal class RespawnPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(nameof(Player.Die), new Type[] { })]
	public static bool Player_Die(Player __instance)
	{
		// Will just ignore their death
		if (__instance.IsGodModeEnabled || !__instance.IsAlive)
		{
			return true;
		}

		var destination = GameManager.Instance.Player.PreviousDock?.GetComponentInChildren<DockPOI>()?.dockSlots[0]?.gameObject;
		TeleportPlayer.To(destination);
		GameEvents.Instance.OnTeleportComplete += OnTeleportComplete;

		return false;
	}

	private static void OnTeleportComplete()
	{
		GameEvents.Instance.OnTeleportComplete -= OnTeleportComplete;

		// Tank their sanity
		GameManager.Instance.Player.Sanity.ChangeSanity(-GameManager.Instance.Player.Sanity.CurrentSanity);

		NotificationHelper.ShowNotificationWithColour(NotificationType.DAMAGE_TAKEN, "An unknown force keeps you from death...", DredgeColorTypeEnum.NEGATIVE);
	}
}