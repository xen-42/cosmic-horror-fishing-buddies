using UnityEngine.AddressableAssets;

namespace CosmicHorrorFishingBuddies.Util
{
	internal static class Scenes
	{
		public static string Title = nameof(Title);
		public static string Game = nameof(Game);

		public static string StringFromLoadSceneReference(AssetReference loadSceneReference)
		{
			if (loadSceneReference == GameManager.Instance._sceneLoader.titleSceneReference)
			{
				return Title;
			}
			if (loadSceneReference == GameManager.Instance._sceneLoader.gameSceneReference)
			{
				return Game;
			}
			return null;
		}
	}
}
