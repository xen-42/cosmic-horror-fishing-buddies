using HarmonyLib;

namespace CosmicHorrorFishingBuddies.Save;

[HarmonyPatch(typeof(SaveManager))]
internal static class SaveManagerPatches
{
	public static bool IsSaving { get; private set; }
	public static bool IsSavingSettings { get; private set; }
	public static bool IsLoading { get; private set; }

	[HarmonyPrefix]
	[HarmonyPatch(nameof(SaveManager.Save))]
	public static void SaveManager_Save_Prefix() => IsSaving = true;

	[HarmonyPostfix]
	[HarmonyPatch(nameof(SaveManager.Save))]
	public static void SaveManager_Save_Postfix() => IsSaving = false;

	[HarmonyPrefix]
	[HarmonyPatch(nameof(SaveManager.SaveSettings))]
	public static void SaveManager_SaveSettings_Prefix() => IsSavingSettings = true;

	[HarmonyPostfix]
	[HarmonyPatch(nameof(SaveManager.SaveSettings))]
	public static void SaveManager_SaveSettings_Postfix() => IsSavingSettings = false;

	[HarmonyPrefix]
	[HarmonyPatch(nameof(SaveManager.Load))]
	public static void SaveManager_Load_Prefix() => IsLoading = true;

	[HarmonyPostfix]
	[HarmonyPatch(nameof(SaveManager.Load))]
	public static void SaveManager_Load_Postfix() => IsLoading = false;
}