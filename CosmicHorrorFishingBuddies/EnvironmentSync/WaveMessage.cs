using CosmicHorrorFishingBuddies.Messaging;

namespace CosmicHorrorFishingBuddies.EnvironmentSync;

public class WaveMessage : CFBMessage<(float steepness, float wavelength, float wavespeed, float[] waveDirections)>
{
	public WaveMessage((float steepness, float wavelength, float wavespeed, float[] waveDirections) data) : base(data) { }

	public override void OnReceiveRemote()
	{
		GameManager.Instance.WaveController.steepness = Data.steepness;
		GameManager.Instance.WaveController.wavelength = Data.wavelength;
		GameManager.Instance.WaveController.speed = Data.wavespeed;
		GameManager.Instance.WaveController.waveDirections = Data.waveDirections;

		// Call awake to have it set all these values
		GameManager.Instance.WaveController.Awake();
	}
}
