using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Extensions;
using UnityEngine;

namespace CosmicHorrorFishingBuddies
{
	public class Loader
	{
		public static void Initialize()
		{
			var gameObject = new GameObject("CosmicHorrorFishingBuddies");
			gameObject.AddComponent<CFBCore>();
			gameObject.DontDestroyOnLoad();
		}
	}
}
