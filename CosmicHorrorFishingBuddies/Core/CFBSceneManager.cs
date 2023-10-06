using CosmicHorrorFishingBuddies.Util;
using CosmicHorrorFishingBuddies.Util.Attributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CosmicHorrorFishingBuddies.Core;

public class CFBSceneManager : MonoBehaviour
{
	public static Action GameSceneLoaded;
	public static Action MainMenuSceneLoaded;

	public void Awake()
	{
		SceneManager.activeSceneChanged += OnSceneChanged;
		OnSceneChanged(default, SceneManager.GetActiveScene());
	}

	public void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnSceneChanged;
	}

	private static void OnSceneChanged(Scene prev, Scene current)
	{
		var sceneManagerObject = new GameObject(nameof(CFBSceneManager));

		switch(current.name)
		{
			case Scenes.Game:
				foreach (var type in TypeHelper.GetTypesWithAttribute(typeof(AddToGameSceneAttribute)))
				{
					sceneManagerObject.AddComponent(type);
				}
				GameSceneLoaded?.Invoke();
				break;

			case Scenes.Title:
				foreach (var type in TypeHelper.GetTypesWithAttribute(typeof(AddToMainMenuSceneAttribute)))
				{
					sceneManagerObject.AddComponent(type);
				}
				MainMenuSceneLoaded?.Invoke();
				break;

		}
	}
}
