using UnityEngine;

namespace CosmicFishingBuddies.Extensions
{
	internal static class UnityExtensions
	{
		public static GameObject InstantiateInactive(this GameObject gameObject)
		{
			var wasActive = gameObject.activeSelf;
			gameObject.SetActive(false);
			var newObj = GameObject.Instantiate(gameObject);
			gameObject.SetActive(wasActive);
			return newObj;
		}

		public static GameObject DontDestroyOnLoad(this GameObject gameObject)
		{
			GameObject.DontDestroyOnLoad(gameObject);
			return gameObject;
		}

		public static T GetAddComponent<T>(this GameObject gameObject) where T : Component
		{
			var component = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
			return component;
		}
	}
}
