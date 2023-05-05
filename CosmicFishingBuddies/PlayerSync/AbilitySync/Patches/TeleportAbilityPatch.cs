using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync.Patches
{
    [HarmonyPatch]
    internal static class TeleportAbilityPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TeleportAbility), nameof(FoghornAbility.Activate))]
        public static void FoghornAbility_Activate() => NetworkPlayer.LocalPlayer?.remoteTeleportAbility?.Toggle(true);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameEvents), nameof(GameEvents.TriggerTeleportComplete))]
        public static void GameEvents_TriggerTeleportComplete() => NetworkPlayer.LocalPlayer?.remoteTeleportAbility?.Toggle(false);
    }
}
