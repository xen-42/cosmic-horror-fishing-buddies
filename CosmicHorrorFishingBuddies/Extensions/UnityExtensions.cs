using AeLa.EasyFeedback.APIs;
using System.Collections.Generic;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Extensions
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

		public static List<GameObject> GetChildren(this GameObject gameObject)
		{
			var children = new List<GameObject>();
			foreach (Transform child in gameObject.transform)
			{
				children.Add(child.gameObject);
			}
			return children;
		}
	}
}
