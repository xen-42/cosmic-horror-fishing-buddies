using CosmicHorrorFishingBuddies.Core;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace CosmicHorrorFishingBuddies
{
    public class Loader
	{
		public static void Initialize()
		{
			var gameObject = new GameObject("CosmicHorrorFishingBuddies");
			gameObject.AddComponent<CFBCore>();
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}
}
