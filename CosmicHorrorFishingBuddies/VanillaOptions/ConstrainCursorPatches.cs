using HarmonyLib;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.VanillaOptions;

[HarmonyPatch]
internal class ConstrainCursorPatches
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(DredgeInputManager), nameof(DredgeInputManager.RefreshCursorLockState))]
	public static void DredgeInputManager_RefreshCursorLockState()
	{
		// Should make this it's own mod
		// When the cursor is invisible it should be confined no matter what (i.e., when driving boat around)
		if (!Cursor.visible && Cursor.lockState == CursorLockMode.None)
		{
			Cursor.lockState = CursorLockMode.Confined;
		}
	}
}
