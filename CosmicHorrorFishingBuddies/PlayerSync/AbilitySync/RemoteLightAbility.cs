using CommandTerminal;
using CosmicHorrorFishingBuddies.AudioSync;
using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using CosmicHorrorFishingBuddies.Util;
using Mirror;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteLightAbility : RemoteSyncVarAbility
	{
		public override Type AbilityType => typeof(LightAbility);

		private LightFlickerEffect _lightFlickerEffect;

		public override void Start()
		{
			_networkPlayer.remotePlayerBoatGraphics.RefreshBoatModel.AddListener(RefreshLights);
			_lightFlickerEffect = GetComponentInChildren<LightFlickerEffect>();
			base.Start();
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

		[Command]
		public void FlickerLights() => ClientRpcFlickerLights();


		[ClientRpc]
		private void ClientRpcFlickerLights()
		{
			try
			{
				var flickerLightsWorldEvent = EventHelper.GetWorldEvent<FlickerLightsWorldEvent>();
				_networkPlayer.RemotePlayOneShot(AudioEnum.LIGHT_FLICKER, flickerLightsWorldEvent.flickerVolume, 1f);
				_lightFlickerEffect.BeginFlicker(flickerLightsWorldEvent.flickerCurve, flickerLightsWorldEvent.worldEventData.durationSec, false);
			}
			catch (Exception e)
			{
				CFBCore.LogError(e);
			}
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