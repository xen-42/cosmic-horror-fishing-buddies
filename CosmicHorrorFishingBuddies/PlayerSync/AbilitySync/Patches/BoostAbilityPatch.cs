using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch]
	internal static class BoostAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.Activate))]
		public static void BoostAbility_Activate()
		{
			NetworkPlayer.LocalPlayer?.CmdPlayOneShot(AudioSync.AudioEnum.HASTE, 1f, 1f);
		}
	}
}
