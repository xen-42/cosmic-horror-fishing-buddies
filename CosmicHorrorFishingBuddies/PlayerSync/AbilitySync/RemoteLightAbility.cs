using CosmicHorrorFishingBuddies.AudioSync;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
    internal class RemoteLightAbility : RemoteSyncVarAbility
	{
		public void Start()
		{
			_networkPlayer.RefreshBoatModel.AddListener(RefreshLights);
		}

		public void OnDestroy()
		{
			_networkPlayer.RefreshBoatModel.RemoveListener(RefreshLights);
		}

		protected override void OnToggleRemote(bool active)
		{
			_networkPlayer.RemotePlayOneShot(active ? AudioEnum.LIGHT_ON : AudioEnum.LIGHT_OFF, 0.3f, 1f);
			RefreshLights();
		}

		public void RefreshLights()
		{
			_networkPlayer.CurrentBoatModelProxy.SetLightStrength(IsActive ? 4f : 0f);

			foreach (var light in _networkPlayer.CurrentBoatModelProxy.Lights)
			{
				light.SetActive(IsActive);
			}
			foreach (var lightBeam in _networkPlayer.CurrentBoatModelProxy.LightBeams)
			{
				lightBeam.SetActive(IsActive);
			}
		}
	}
}
