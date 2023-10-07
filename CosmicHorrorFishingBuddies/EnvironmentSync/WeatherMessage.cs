using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Messaging;

namespace CosmicHorrorFishingBuddies.EnvironmentSync;

/// <summary>
/// Sends the chosen weather index to other players
/// </summary>
internal class WeatherMessage : CFBMessage<int>
{
	public WeatherMessage(int data) : base(data) { }

	public override void OnReceiveRemote()
	{
		if (CFBCore.IsHost)
		{
			var weatherController = GameManager.Instance.WeatherController;
			weatherController.ChangeWeather(weatherController.allWeather[Data]);
		}
	}
}
