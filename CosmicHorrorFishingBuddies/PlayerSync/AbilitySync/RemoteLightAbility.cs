using CosmicHorrorFishingBuddies.AudioSync;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
    internal class RemoteLightAbility : RemoteSyncVarAbility
	{
		public void Start()
		{
			_networkPlayer.remotePlayerBoatGraphics.RefreshBoatModel.AddListener(RefreshLights);
		}

		public void OnDestroy()
		{
			_networkPlayer.remotePlayerBoatGraphics.RefreshBoatModel.RemoveListener(RefreshLights);
		}

		protected override void OnToggleRemote(bool active)
		{
			_networkPlayer.RemotePlayOneShot(active ? AudioEnum.LIGHT_ON : AudioEnum.LIGHT_OFF, 0.3f, 1f);
			RefreshLights();
		}

		public void RefreshLights()
		{
			_networkPlayer.remotePlayerBoatGraphics.CurrentBoatModelProxy.SetLightStrength(IsActive ? 4f : 0f);

			foreach (var light in _networkPlayer.remotePlayerBoatGraphics.CurrentBoatModelProxy.Lights)
			{
				light.SetActive(IsActive);
			}
			foreach (var lightBeam in _networkPlayer.remotePlayerBoatGraphics.CurrentBoatModelProxy.LightBeams)
			{
				lightBeam.SetActive(IsActive);
			}
		}
	}
}
