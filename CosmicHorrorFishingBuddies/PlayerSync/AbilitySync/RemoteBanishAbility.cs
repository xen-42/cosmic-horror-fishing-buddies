using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using CosmicHorrorFishingBuddies.Util;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteBanishAbility : RemoteRPCAbility
	{
		public override Type AbilityType => typeof(BanishAbility);

		public GameObject banishEffect;
		private ParticleSystem _vfxParticles;
		public AudioSource banishAudioSource;

		public void Start()
		{
			_vfxParticles = banishEffect.GetComponent<ParticleSystem>();
		}

		[ClientRpc(includeOwner = false)]
		protected override void OnTriggerAbility(bool active)
		{
			banishEffect.SetActive(active);
			if (active)
			{
				var main = _vfxParticles.main;
				main.startLifetime = AbilityHelper.GetAbility<BanishAbility>().abilityData.duration;
				banishAudioSource.Play();
			}
			else
			{
				banishAudioSource.Stop();
			}
		}
	}
}