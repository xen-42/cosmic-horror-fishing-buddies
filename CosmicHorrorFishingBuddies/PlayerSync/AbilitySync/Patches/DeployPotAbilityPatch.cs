using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch]
	internal static class DeployPotAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(DeployPotAbility), nameof(DeployPotAbility.Activate))]
		public static void DeployPotAbility_Activate()
		{
			//NetworkPlayer.LocalPlayer?.CmdPlayOneShot(AudioSync.AudioEnum.DEPLOY_POT, 1f, 1f);
		}
	}
}
