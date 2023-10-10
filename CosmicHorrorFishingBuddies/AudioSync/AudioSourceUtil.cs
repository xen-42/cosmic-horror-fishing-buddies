using CosmicHorrorFishingBuddies.AudioSync;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Util
{
	public static class AudioSourceUtil
	{
		public static AnimationCurve CustomCurve => new(
			new Keyframe(0.0333f, 1f, -30.012f, -30.012f, 0.3333f, 0.3333f),
			new Keyframe(0.0667f, 0.5f, -7.503f, -7.503f, 0.3333f, 0.3333f),
			new Keyframe(0.1333f, 0.25f, -1.8758f, -1.8758f, 0.3333f, 0.3333f),
			new Keyframe(0.2667f, 0.125f, -0.4689f, -0.4689f, 0.3333f, 0.3333f),
			new Keyframe(0.5333f, 0.0625f, -0.1172f, -0.1172f, 0.3333f, 0.3333f),
			new Keyframe(1f, 0f, -0.0333f, -0.0333f, 0.3333f, 0.3333f)
		);

		public static AudioSource MakeSpatialAudio(GameObject go, float minDistance = 20f, float maxDistance = 60f, bool loop = false)
		{
			var audioSource = go.AddComponent<AudioSource>();

			audioSource.minDistance = minDistance;
			audioSource.maxDistance = maxDistance;
			audioSource.spatialBlend = 1;
			audioSource.rolloffMode = AudioRolloffMode.Custom;
			audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, CustomCurve);
			audioSource.loop = loop;
			audioSource.playOnAwake = loop;

			go.AddComponent<AudioMixerGroupFixer>();

			return audioSource;
		}
	}
}
