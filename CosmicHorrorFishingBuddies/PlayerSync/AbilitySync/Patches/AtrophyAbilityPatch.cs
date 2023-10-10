using CosmicHorrorFishingBuddies.HarvestPOISync;
using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
    [HarmonyPatch(typeof(AtrophyAbility))]
    internal static class AtrophyAbilityPatch
    {
		[HarmonyPostfix]
		[HarmonyPatch(nameof(AtrophyAbility.Activate))]
		public static void AtrophyAbility_Activate()
		{
			if (GameManager.Instance.Player.Harvester.CurrentHarvestPOI is PlacedHarvestPOI) return;

			NetworkPlayer.LocalPlayer?.remoteAtrophyAbility?
				.Activate(NetworkHarvestPOIManager.Instance.GetNetworkObject(GameManager.Instance.Player.Harvester.CurrentHarvestPOI).netIdentity);
			NetworkPlayer.LocalPlayer?.CmdPlayOneShot(AudioSync.AudioEnum.ATROPHY, 1f, 1f);
		}

        [HarmonyPostfix]
        [HarmonyPatch(nameof(AtrophyAbility.OnHarvestModeToggled))]
        public static void AtrophyAbility_OnHarvestModeToggled() => NetworkPlayer.LocalPlayer?.remoteAtrophyAbility?.Deactivate();
    }
}
