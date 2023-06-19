using CosmicHorrorFishingBuddies.Util;
using HarmonyLib;

namespace CosmicHorrorFishingBuddies.Core.Patches;

[HarmonyPatch(typeof(SceneLoader))]
internal static class SceneLoaderPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(nameof(SceneLoader.DoSwitchSceneRequest))]
	public static void SceneLoader_DoSwitchSceneRequest(SceneLoader.SceneSwitchRequest request)
	{
		CFBCore.Instance.SwitchSceneRequested.Invoke(Scenes.StringFromLoadSceneReference(request.loadSceneReference));
	}
}
