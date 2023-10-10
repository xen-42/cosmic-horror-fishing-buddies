using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace CosmicHorrorFishingBuddies.AudioSync
{
	[RequireComponent(typeof(AudioSource))]
	internal class AudioMixerGroupFixer : MonoBehaviour
	{
		public void Start()
		{
			GetComponent<AudioSource>().outputAudioMixerGroup = GameObject.FindObjectsOfType<AudioMixerGroup>().First(x => x.name == "WorldSFX");
			Component.Destroy(this);
		}
	}
}
