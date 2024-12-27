using Coffee.UIExtensions;
using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Util;
using EpicTransport;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TransportType = CosmicHorrorFishingBuddies.Core.TransportType;

namespace CosmicHorrorFishingBuddies.UI
{
	[AddToMainMenuScene]
	internal class MainMenuManager : MonoBehaviour
	{
		private PopupWindow _popupWindow;
		private SaveSlotWindow _saveSlotWindow;

		private bool _isHost;
		private TransportType _transportType;
		private string _address = "localhost";

		private TextMeshProUGUI _connectionTitle;
		private TextMeshProUGUI _connectionInfo;
		private TMP_InputField _inputField;

		public void Start()
		{
			try
			{
				_saveSlotWindow = UIHelper.GetMainMenuButton(UIHelper.MainMenuButton.LOAD).GetComponent<LoadGameButton>().saveSlotWindow;

				_popupWindow = UIHelper.AddMainMenuPopupWindow();
				var panel = _popupWindow.container.transform.Find("Panel");

				var options = new GameObject("OptionsList");
				options.SetActive(false);
				var rectTransform = options.AddComponent<RectTransform>();
				var grid = options.AddComponent<GridLayoutGroup>();
				grid.cellSize = new Vector2(500, 80);
				grid.spacing = new Vector2(100, 20);
				grid.startAxis = GridLayoutGroup.Axis.Horizontal;
				grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
				grid.childAlignment = TextAnchor.MiddleCenter;
				options.AddComponent<CanvasRenderer>();
				options.transform.parent = panel;
				options.transform.localPosition = Vector2.zero;
				options.transform.localScale = Vector2.one;
				rectTransform.offsetMax = Vector2.zero;
				rectTransform.offsetMin = Vector2.zero;
				rectTransform.sizeDelta = new Vector2(500, 500);
				options.SetActive(true);

				_connectionTitle = UIHelper.AddLabel(options.transform, "Multiplayer", TextAlignmentOptions.Center).GetComponent<TextMeshProUGUI>();

				var dropDown = UIHelper.AddDropDown(options.transform, "Server Type", "Pick EOS", new (string, Action)[]
				{
						("EOS", () => OnSelectTransportOption(TransportType.EPIC)),
						//("Steam", () => OnSelectOption(CFBNetworkManager.TransportType.STEAM)),
						("IP Address", () => OnSelectTransportOption(TransportType.KCP))
				});
				_transportType = TransportType.EPIC;

				var dropDownLabel = dropDown.transform.Find("LabelContainer");
				dropDownLabel.transform.localPosition = new Vector2(20, 60);
				dropDownLabel.transform.localScale = Vector2.one * 0.8f;

				_connectionInfo = UIHelper.AddLabel(options.transform, "Info", TextAlignmentOptions.Center).GetComponent<TextMeshProUGUI>();

				_inputField = UIHelper.AddInputField(options.transform, _address);

				var startButton = UIHelper.AddButton(options.transform, "Start", OnClickStart);
				startButton.GetComponent<UITransitionEffect>().enabled = false;
				startButton.GetComponent<Button>().colors = _saveSlotWindow.saveSlots[0].selectSlotButton.GetComponent<Button>().colors;

				UIHelper.AddMainMenuButton("Host Game", OnClickHost, 0);
				UIHelper.AddMainMenuButton("Join Game", OnClickJoin, 1);

				GameObject.DestroyImmediate(UIHelper.GetMainMenuButton(UIHelper.MainMenuButton.CONTINUE));
				GameObject.DestroyImmediate(UIHelper.GetMainMenuButton(UIHelper.MainMenuButton.LOAD));

				CFBCore.LogInfo("Altered main menu!");
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

			RefreshConnectionTest();

			_inputField.gameObject.SetActive(false);

			_popupWindow.Show();
		}

		private void OnClickJoin()
		{
			CFBCore.LogInfo("Connecting!");

			_isHost = false;

			RefreshConnectionTest();

			_inputField.gameObject.SetActive(true);

			_popupWindow.Show();
		}

		private void OnClickStart()
		{
			CFBCore.LogInfo("Begin!");

			_popupWindow.Hide(PopupWindow.WindowHideMode.CLOSE);

			CFBNetworkManager.Instance.SetConnection(_isHost, _inputField.text, _transportType);

			_saveSlotWindow.Show();
		}

		private void OnSelectTransportOption(TransportType type)
		{
			CFBCore.LogInfo($"Selected server hosting option {type}");

			_transportType = type;

			RefreshConnectionTest();
		}

		private void RefreshConnectionTest()
		{
			if (_isHost)
			{
				_connectionTitle.text = $"You will host the server!";

				if (_transportType == TransportType.EPIC)
				{
					if (string.IsNullOrEmpty(EOSSDKComponent.LocalUserProductIdString))
					{
						_connectionInfo.text = $"EOS isn't working (unsupported on Epic Games)\nUse IP address instead";
					}
					else
					{
						_connectionInfo.text = $"Connection code (copied to clipboard):\n{EOSSDKComponent.LocalUserProductIdString}";
						GUIUtility.systemCopyBuffer = EOSSDKComponent.LocalUserProductIdString;
					}
				}
				else
				{
					_connectionInfo.text = string.Empty;
				}
			}
			else
			{
				_connectionTitle.text = "You're joining a server!";
				if (_transportType == TransportType.KCP)
				{
					_connectionInfo.text = "Type the IP address below.";
				}
				else if (_transportType == TransportType.EPIC)
				{
					_connectionInfo.text = "Type the connection code below.";
				}
				_connectionInfo.text += "\nWARNING: Your save data will be overwritten.";
			}
		}
	}
}
