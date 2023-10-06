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

	public static Ability GetAbility(Type type)
	{
		if (!_abilityCache.ContainsKey(type))
		{
			_abilityCache.Add(type, GameManager.Instance.PlayerAbilities.abilityMap[_abilityNames[type]]);
		}
		return _abilityCache[type];
	}

	public static T GetAbility<T>() where T : Ability
	{
		return GetAbility(typeof(T)) as T;
	}
}
