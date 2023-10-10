using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.PlayerSync;
using TMPro;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Debugging
{
	internal class DebugDisplay : MonoBehaviour
	{
		private TextMeshProUGUI _text;

		public void Awake()
		{
			_text = gameObject.AddComponent<TextMeshProUGUI>();
		}

		public void Update()
		{
			_text.text = $"{(CFBCore.IsHost ? "host" : "client")} - {NetworkPlayer.LocalPlayer?.netId}";
		}
	}
}
