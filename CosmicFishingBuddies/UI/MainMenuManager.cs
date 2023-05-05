using Coffee.UIExtensions;
using CosmicFishingBuddies.Util;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CosmicFishingBuddies.UI
{
	internal class MainMenuManager : MonoBehaviour
	{
		private PopupWindow _popupWindow;
		private SaveSlotWindow _saveSlotWindow;

		private bool _isHost;
		private CFBNetworkManager.TransportType _transportType;
		private string _address = "localhost";

		private TextMeshProUGUI _connectionText;

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
					var grid = options.AddComponent<GridLayoutGroup>();
					grid.cellSize = new Vector2(300, 100);
					grid.spacing = new Vector2(100, 50);
					grid.startAxis = GridLayoutGroup.Axis.Horizontal;
					grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
					grid.childAlignment = TextAnchor.MiddleCenter;
					options.AddComponent<CanvasRenderer>();
					options.transform.parent = panel;
					options.transform.localPosition = Vector2.zero;
					options.transform.localScale = Vector2.one;
					rectTransform.offsetMax = Vector2.zero;
					rectTransform.offsetMin = Vector2.zero;
					rectTransform.sizeDelta = new Vector2(400, 400);
					options.SetActive(true);

					_connectionText = UIHelper.AddLabel(options.transform, "Multiplayer", TextAlignmentOptions.Center).GetComponent<TextMeshProUGUI>();

					var dropDown = UIHelper.AddDropDown(options.transform, "Server Type", "Pick Epic", new(string, Action)[]
					{
						//("Epic", () => OnSelectOption(CFBNetworkManager.TransportType.EPIC))),
						//("Steam", () => OnSelectOption(CFBNetworkManager.TransportType.STEAM))),
						("IP Address", () => OnSelectTransportOption(CFBNetworkManager.TransportType.KCP))
					});

					var startButton = UIHelper.AddButton(options.transform, "Start", OnClickStart);
					startButton.GetComponent<UITransitionEffect>().enabled = false;
					startButton.GetComponent<Button>().colors = _saveSlotWindow.saveSlots[0].selectSlotButton.GetComponent<Button>().colors;

					UIHelper.AddMainMenuButton("Host Game", OnClickHost, 0);
					UIHelper.AddMainMenuButton("Join Game", OnClickJoin, 1);

					GameObject.DestroyImmediate(UIHelper.GetMainMenuButton(UIHelper.MainMenuButton.CONTINUE));
					GameObject.DestroyImmediate(UIHelper.GetMainMenuButton(UIHelper.MainMenuButton.LOAD));
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

			_isHost = true;

			_connectionText.text = "You will host a server!";

			_popupWindow.Show();
		}

		private void OnClickJoin()
		{
			CFBCore.LogInfo("Connecting!");

			_isHost = false;

			_connectionText.text = "You're joining a server!\nWARNING: Your save data will be overwritten.";

			_popupWindow.Show();
		}

		private void OnClickStart()
		{
			CFBCore.LogInfo("Begin!");

			_popupWindow.Hide();

			CFBNetworkManager.Instance.SetConnection(_isHost, _address, _transportType);

			_saveSlotWindow.Show();
		}

		private void OnSelectTransportOption(CFBNetworkManager.TransportType type)
		{
			CFBCore.LogInfo($"Selected server hosting option {type}");


		}
	}
}
