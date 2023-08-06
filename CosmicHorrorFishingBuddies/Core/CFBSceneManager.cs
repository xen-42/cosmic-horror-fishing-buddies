using CosmicHorrorFishingBuddies.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CosmicHorrorFishingBuddies.Core;

public class CFBSceneManager : MonoBehaviour
{
	public static Action GameSceneLoaded;

	public void Awake()
	{
		SceneManager.activeSceneChanged += OnSceneChanged;
	}

	public void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnSceneChanged;
	}

	private static void OnSceneChanged(Scene prev, Scene current)
	{
		var sceneManagerObject = new GameObject(nameof(CFBSceneManager));

		if (current.name == Scenes.Game)
		{
			GameSceneLoaded?.Invoke();
			foreach (var type in TypeHelper.GetTypesWithAttribute(typeof(AddToGameSceneAttribute)))
			{
				sceneManagerObject.AddComponent(type);
			}
		}
	}
}
