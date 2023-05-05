using CosmicFishingBuddies.Util;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CosmicFishingBuddies.UI
{
	internal class MainMenuManager : MonoBehaviour
	{
		private SaveSlotWindow _saveSlotWindow;
		private PopupWindow _popupWindow;

		public void Awake()
		{
			SceneManager.activeSceneChanged += OnSceneChanged;
		}

		public void OnDestroy()
		{
			SceneManager.activeSceneChanged -= OnSceneChanged;
		}

		private void OnSceneChanged(Scene prev, Scene current)
		{
			try
			{
				if (current.name == Scenes.Title)
				{
					_saveSlotWindow = UIHelper.GetMainMenuButton(UIHelper.MainMenuButton.LOAD).GetComponent<LoadGameButton>().saveSlotWindow;

					_popupWindow = UIHelper.AddMainMenuPopupWindow();
					var panel = _popupWindow.container.transform.Find("Panel");
					
					var options = new GameObject("OptionsList");
					options.SetActive(false);
					var rectTransform = options.AddComponent<RectTransform>();
					options.AddComponent<VerticalLayoutGroup>();
					options.AddComponent<CanvasRenderer>();
					options.transform.parent = panel;
					options.transform.localPosition = Vector2.zero;
					options.transform.localScale = Vector2.one;
					rectTransform.offsetMax = Vector2.zero;
					rectTransform.offsetMin = Vector2.zero;
					rectTransform.sizeDelta = new Vector2(400, 400);
					options.SetActive(true);

					var dropDown = UIHelper.AddDropDown(options.transform, "Server Type", "Pick epic", new(string, Action)[]
					{
						("KCP", () => OnSelectOption("KCP")),
						("Epic", () => OnSelectOption("Epic"))
					});

					var startButton = UIHelper.AddButton(options.transform, "Begin", OnClickBegin);

					UIHelper.AddMainMenuButton("Host Game", OnClickHost, 0);
					UIHelper.AddMainMenuButton("Join Game", OnClickJoin, 1);

					//GameObject.DestroyImmediate(UIHelper.GetMainMenuButton(UIHelper.MainMenuButton.CONTINUE));
					//GameObject.DestroyImmediate(UIHelper.GetMainMenuButton(UIHelper.MainMenuButton.LOAD));
				}
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Couldn't alter main menu: {e}");
			}
		}

		private void OnClickHost()
		{
			CFBCore.LogInfo("Hosting!");

			_popupWindow.Show();
		}

		private void OnClickJoin()
		{
			CFBCore.LogInfo("Connecting!");

			_popupWindow.Show();
		}

		private void OnClickBegin()
		{
			CFBCore.LogInfo("Begin!");


		}

		private void OnSelectOption(string option)
		{
			CFBCore.LogInfo($"Selected server hosting option {option}");
		}
	}
}
