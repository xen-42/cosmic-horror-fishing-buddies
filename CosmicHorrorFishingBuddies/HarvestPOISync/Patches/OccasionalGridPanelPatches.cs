using HarmonyLib;

namespace CosmicHorrorFishingBuddies.HarvestPOISync.Patches;

[HarmonyPatch(typeof(OccasionalGridPanel))]
public static class OccasionalGridPanelPatches
{
	/// <summary>
	/// Method called when a crab pot is picked up
	/// </summary>
	/// <param name="__instance"></param>
	[HarmonyPostfix]
	[HarmonyPatch(nameof(OccasionalGridPanel.RemoveDeployable))]
	public static void OccasionalGridPanel_RemoveDeployable(OccasionalGridPanel __instance)
	{
		NetworkHarvestPOIManager.Instance.DestroyCrabPot(__instance.harvestPOI as PlacedHarvestPOI);
	}
}
