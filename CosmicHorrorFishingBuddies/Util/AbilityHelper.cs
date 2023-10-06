using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmicHorrorFishingBuddies.Util;

public static class AbilityHelper
{
	private static readonly Dictionary<Type, Ability> _abilityCache = new();
	private static readonly Dictionary<Type, string> _abilityNames = new()
	{
		{ typeof(LightAbility), "lights" },
		{ typeof(TeleportAbility), "manifest" },
		{ typeof(AtrophyAbility), "atrophy" },
		{ typeof(BaitAbility), "bait" },
		{ typeof(DeployPotAbility), "pot" },
		{ typeof(TrawlNetAbility), "trawl" },
		{ typeof(BoostAbility), "haste" },
	};

	public static T GetAbility<T>() where T : Ability
	{
		if (!_abilityCache.ContainsKey(typeof(T)))
		{
			_abilityCache.Add(typeof(T), GameManager.Instance.PlayerAbilities.abilityMap[_abilityNames[typeof(T)]]);
		}
		return _abilityCache[typeof(T)] as T;
	}
}
