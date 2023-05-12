using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal class RemoteVariablePlayerLight : PlayerLight
	{
		private RemoteBoatGraphics _remote;

		public override void RefreshLightStrength()
		{
			Intensity = _remote.LightLumens * lumensIntensityCoefficient;
			Range = _remote.LightRange;
		}

		public static void Replace(VariablePlayerLight localLight, RemoteBoatGraphics parent)
		{
			var remoteLight = localLight.gameObject.AddComponent<RemoteVariablePlayerLight>();
			remoteLight._remote = parent;
			remoteLight.lumensIntensityCoefficient = localLight.lumensIntensityCoefficient;
			remoteLight.light = localLight.light;
			remoteLight.enabled = localLight.enabled;
			GameObject.DestroyImmediate(localLight);

			remoteLight.RefreshLightStrength();
		}
	}
}
