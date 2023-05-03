using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace CosmicFishingBuddies
{
	public class Loader
	{
		public static void Initialize()
		{
			var gameObject = new GameObject("CosmicFishingBuddies");
			gameObject.AddComponent<CFBCore>();
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}
}
