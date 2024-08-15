using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using DG.Tweening;
using System;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteTrawlNetAbility : RemoteSyncVarAbility
	{
		// Have to adjust the rotation of the trawl net - will be unnessecary when rigidbodies work
		private Tween _rotationTween;

		public override Type AbilityType => typeof(TrawlNetAbility);

		protected override void OnToggleRemote(bool active)
		{
			// Temporary fix that I will never fix haha
			// RunWhen bc there's an NRE otherwise idk
			Delay.RunWhen(
				() => _networkPlayer?.remotePlayerBoatGraphics?.CurrentBoatModelProxy?.GetTrawlNetAnimator() != null,
				() =>
				{
					// Have to wait a frame else initial state isn't properly received
					Delay.FireOnNextUpdate(() =>
					{
						try
						{
							_networkPlayer.remotePlayerBoatGraphics.CurrentBoatModelProxy.GetTrawlNetAnimator().SetBool("isDeployed", active);
							var trawlNet = _networkPlayer.remotePlayerBoatGraphics.CurrentBoatModelProxy.transform.Find("TrawlNet/TrawlArmature/TrawlArm/Net");
							if (active)
							{
								_networkPlayer.RemotePlayOneShot(AudioSync.AudioEnum.TRAWL_ACTIVATE, 1f, 1f);
								_rotationTween?.Kill();
								_rotationTween = trawlNet.DOLocalRotate(new UnityEngine.Vector3(90, 0, 0), 1f);
							}
							else
							{
								_networkPlayer.RemotePlayOneShot(AudioSync.AudioEnum.TRAWL_END, 1f, 1f);
								_rotationTween?.Kill();
								_rotationTween = trawlNet.DOLocalRotate(new UnityEngine.Vector3(0, 0, 0), 1f);
								_rotationTween.SetDelay(2f);
							}
						}
						catch (Exception ex)
						{
							CFBCore.LogError(ex);
						}
					});
				}
			);
		}
	}
}