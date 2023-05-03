using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicFishingBuddies.AudioSync
{
	internal static class AudioClipManager
	{
		public static AudioClip GetClip(AudioEnum audio) => audio switch
		{
			AudioEnum.LIGHT_ON => (GameManager.Instance.PlayerAbilities.abilityMap["lights"] as LightAbility).onSFX,
			AudioEnum.LIGHT_OFF => (GameManager.Instance.PlayerAbilities.abilityMap["lights"] as LightAbility).offSFX,
			_ => null,
		};
	}
}
