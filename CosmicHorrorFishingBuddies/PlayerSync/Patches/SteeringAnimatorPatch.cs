using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(SteeringAnimator))]
	internal class SteeringAnimatorPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(SteeringAnimator.Update))]
		public static void SteeringAnimatorPatch_Update(SteeringAnimator __instance)
		{
			NetworkPlayer.LocalPlayer.remoteSteeringAnimator.SetSteeringParameters(__instance.moveValX, __instance.moveValY);
		}
	}
}
