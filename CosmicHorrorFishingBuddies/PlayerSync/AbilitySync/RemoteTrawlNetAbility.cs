using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using DG.Tweening;
using Mirror;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteTrawlNetAbility : RemoteRPCAbility
	{
		// Have to adjust the rotation of the trawl net - will be unnessecary when rigidbodies work
		private Tween _rotationTween;

		[ClientRpc(includeOwner = false)]
		protected override void OnTriggerAbility(bool active)
		{
			_networkPlayer.remotePlayerBoatGraphics.CurrentBoatModelProxy.TrawlNetAnimator.SetBool("isDeployed", active);
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
	}
}
