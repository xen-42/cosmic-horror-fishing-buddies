using CosmicFishingBuddies.Core;
using CosmicFishingBuddies.Extensions;
using CosmicFishingBuddies.Util;
using Epic.OnlineServices.Platform;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CosmicFishingBuddies.UI
{
    internal class UIHelper	: MonoBehaviour
	{
		public static UIHelper Instance { get; private set; }

		private static TitleController _titleController;

		private static GameObject _windowPrefab;
		private static GameObject _buttonPrefab;
		private static GameObject _dropDownPrefab;
		private static GameObject _labelPrefab;

		private static GameObject _buttonContainer;
		private static GameObject _menuCanvas;

		public enum MainMenuButton
		{
			CONTINUE,
			LOAD,
			SETTINGS,
			CREDITS,
			QUIT
		}

		public void Awake()
		{
			Instance = this;
			SceneManager.activeSceneChanged += OnSceneChanged;
		}

		public void OnDestroy()
		{
			SceneManager.activeSceneChanged -= OnSceneChanged;
		}

		private void CreatePrefabs()
		{
			try
			{
				// BUTTON
				_buttonPrefab = GameObject.Find("Canvases/MenuCanvas/ButtonContainer/Settings").gameObject.InstantiateInactive();
				_buttonPrefab.name = "ButtonPrefab";
				Component.DestroyImmediate(_buttonPrefab.GetComponent<SettingsButton>());
				Component.DestroyImmediate(_buttonPrefab.GetComponentInChildren<LocalizeStringEvent>());

				// WINDOW
				_windowPrefab = GameObject.Find("Canvases/MenuCanvas/SaveSlotWindow").gameObject.InstantiateInactive();
				_windowPrefab.name = "WindowPrefab";
				var saveSlotWindow = _windowPrefab.GetComponent<SaveSlotWindow>();
				var panel = _windowPrefab.transform.Find("Container/Panel");
				for (int i = panel.childCount - 1; i >= 0; i--)
				{
					GameObject.DestroyImmediate(panel.GetChild(i).gameObject);
				}
				var popup = _windowPrefab.AddComponent<PopupWindow>();
				popup.container = _windowPrefab.transform.Find("Container").gameObject;
				popup.openSFX = saveSlotWindow.openSFX;
				popup.closeSFX = saveSlotWindow.closeSFX;
				popup.canvasGroup = saveSlotWindow.canvasGroup;
				popup.windowType = saveSlotWindow.windowType;
				popup.toggleGameUI = saveSlotWindow.toggleGameUI;
				popup.canBeClosedByPlayer = true;
				Component.DestroyImmediate(saveSlotWindow);

				// DROPDOWN
				_dropDownPrefab = GameObject.Find("Canvases/SettingsDialog/TabbedPanelContainer/Panels/AccessibilityPanel/Container/PopupDuration").gameObject.InstantiateInactive();
				_dropDownPrefab.name = "DropDownPrefab";
				var dropdownSettingInput = _dropDownPrefab.GetComponent<DropdownSettingInput>();
				dropdownSettingInput.settingType = SettingType.NONE;

				// LABEL
				_labelPrefab = GameObject.Find("Canvases/SettingsDialog/TabbedPanelContainer/Panels/AccessibilityPanel/Container/PopupDuration/LabelContainer/DropdownLabel").gameObject.InstantiateInactive();
				GameObject.DestroyImmediate(_labelPrefab.GetComponent<LocalizeStringEvent>());
				_labelPrefab.name = "LabelPrefab";
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Failed to create UI prefabs : {e}");
			}
		}

		private void OnSceneChanged(Scene prev, Scene current)
		{
			try
			{
				if (current.name == Scenes.Title)
				{
					_titleController = GameObject.FindObjectOfType<TitleController>();

					_buttonContainer = GameObject.Find("Canvases/MenuCanvas/ButtonContainer").gameObject;
					_menuCanvas = GameObject.Find("Canvases/MenuCanvas").gameObject;

					CreatePrefabs();
				}
			}
			catch(Exception e)
			{
				CFBCore.LogError($"Couldn't create {nameof(UIHelper)} : {e}");
			}
		}

		public static GameObject AddMainMenuButton(string text, Action onClick, int index)
		{
			var newButton = AddButton(_buttonContainer.transform, text, onClick);

			newButton.transform.SetSiblingIndex(index);

			return newButton;
		}

		public static GameObject AddButton(Transform parent, string text, Action onClick)
		{
			var newButton = _buttonPrefab.InstantiateInactive();
			newButton.name = $"{text}_Button";
			newButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
			newButton.GetComponent<BasicButtonWrapper>().OnClick = onClick;

			newButton.transform.parent = parent;
			newButton.transform.localScale = Vector3.one;
			newButton.SetActive(true);

			return newButton;
		}

		public static GameObject GetMainMenuButton(MainMenuButton button) => button switch
		{
			MainMenuButton.CONTINUE => _buttonContainer.GetComponentInChildren<ContinueOrNewButton>().gameObject,
			MainMenuButton.LOAD => _buttonContainer.GetComponentInChildren<LoadGameButton>().gameObject,
			MainMenuButton.SETTINGS => _buttonContainer.GetComponentInChildren<SettingsButton>().gameObject,
			MainMenuButton.CREDITS => _buttonContainer.GetComponentInChildren<CreditsButton>().gameObject,
			MainMenuButton.QUIT => _buttonContainer.GetComponentInChildren<QuitGameButton>().gameObject,
			_ => throw new Exception($"Unsupported main menu button {button}"),
		};

		public static PopupWindow AddMainMenuPopupWindow()
		{
			var newPanel = _windowPrefab.InstantiateInactive();
			newPanel.name = "Window";
			newPanel.transform.parent = _menuCanvas.transform;
			newPanel.transform.localScale = Vector3.one;
			newPanel.transform.localPosition = Vector3.zero;

			var rectTransform = newPanel.GetComponent<RectTransform>();
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.offsetMin = Vector2.zero;

			var popup = newPanel.GetComponent<PopupWindow>();

			newPanel.SetActive(true);

			return popup;
		}

		public static GameObject AddDropDown(Transform parent, string text, string tooltip, (string text, Action action)[] options)
		{
			var newDropdown = _dropDownPrefab.InstantiateInactive();
			newDropdown.name = $"{text}Dropdown";
			newDropdown.transform.parent = parent;
			newDropdown.transform.localScale = Vector2.one;

			var dropdownElement = newDropdown.GetComponentInChildren<TMP_Dropdown>();
			dropdownElement.ClearOptions();
			dropdownElement.AddOptions(options.Select(x => x.text).ToList());
			dropdownElement.onValueChanged.RemoveAllListeners();
			dropdownElement.onValueChanged.AddListener(i =>
			{
				options.Select(x => x.action).ToArray()[i]?.Invoke();
			});

			var label = newDropdown.transform.Find("LabelContainer/DropdownLabel");
			GameObject.DestroyImmediate(label.GetComponent<LocalizeStringEvent>());
			label.GetComponent<TextMeshProUGUI>().text = text;

			newDropdown.SetActive(true);

			return newDropdown;
		}

		public static GameObject AddLabel(Transform parent, string text, TextAlignmentOptions alignment)
		{
			var newLabel = _labelPrefab.InstantiateInactive();
			newLabel.name = $"{text}Label";
			newLabel.transform.parent = parent;
			newLabel.transform.localScale = Vector2.one;
			var tmp = newLabel.GetComponent<TextMeshProUGUI>();
			tmp.text = text;
			tmp.alignment = alignment;

			newLabel.SetActive(true);

			return newLabel;
		}

		public static TMP_InputField AddInputField(Transform parent, string placeholder)
		{
			var newInputField = TMP_DefaultControls.CreateInputField(new TMP_DefaultControls.Resources());
			newInputField.transform.parent = parent;
			newInputField.transform.localPosition = Vector2.zero;
			newInputField.transform.localScale = Vector2.one;
			var inputField = newInputField.GetComponent<TMP_InputField>();
			inputField.pointSize = 32;
			inputField.text = placeholder;

			return inputField;
		}
	}
}
