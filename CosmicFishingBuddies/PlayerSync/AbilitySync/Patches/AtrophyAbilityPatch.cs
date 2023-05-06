using CosmicFishingBuddies.HarvestPOISync;
using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync.Patches
{
    [HarmonyPatch(typeof(AtrophyAbility))]
    internal static class AtrophyAbilityPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(AtrophyAbility.Activate))]
        public static void AtrophyAbility_Activate() => NetworkPlayer.LocalPlayer?.remoteAtrophyAbility?
			.Activate(NetworkHarvestPOIManager.Instance.GetNetworkObject(GameManager.Instance.Player.Harvester.CurrentHarvestPOI).netIdentity);

        [HarmonyPostfix]
        [HarmonyPatch(nameof(AtrophyAbility.OnHarvestModeToggled))]
        public static void AtrophyAbility_OnHarvestModeToggled() => NetworkPlayer.LocalPlayer?.remoteAtrophyAbility?.Deactivate();
    }
}
