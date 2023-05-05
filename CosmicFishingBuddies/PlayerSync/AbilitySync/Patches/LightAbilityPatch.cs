using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync.Patches
{
    [HarmonyPatch(typeof(LightAbility))]
    internal static class LightAbilityPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LightAbility.Activate))]
        public static void LightAbility_Activate() => NetworkPlayer.LocalPlayer?.remoteLightAbility?.Toggle(true);

        [HarmonyPrefix]
        [HarmonyPatch(nameof(LightAbility.Deactivate))]
        public static void LightAbility_Deactivate() => NetworkPlayer.LocalPlayer?.remoteLightAbility?.Toggle(false);
    }
}
