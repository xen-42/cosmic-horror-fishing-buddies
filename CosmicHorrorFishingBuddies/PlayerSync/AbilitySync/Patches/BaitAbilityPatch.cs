using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch]
	internal static class BaitAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(BaitAbility), nameof(BaitAbility.Activate))]
		public static void BaitAbility_Activate()
		{
			//NetworkPlayer.LocalPlayer?.CmdPlayOneShot(AudioSync.AudioEnum.BAIT, 1f, 1f);
		}
	}
}
