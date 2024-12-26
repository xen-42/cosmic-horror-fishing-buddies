using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmicHorrorFishingBuddies.Util
{
    internal static class EventHelper
    {
        public static T GetWorldEvent<T>() where T : WorldEvent
        {
            if (_dict.TryGetValue(typeof(T), out var data) && data != null)
            {
                return (T)data;
            }
            else
            {
                var (eventData, worldEvent) = GameManager.Instance.DataLoader.allWorldEvents.Select(x => (x, x.prefab.GetComponent<WorldEvent>())).FirstOrDefault(x => x.Item2 is T);
				worldEvent.worldEventData = eventData;
				var worldEventT = (T)worldEvent;
                _dict[typeof(T)] = worldEventT;
                return worldEventT;
            }
        }

        private static Dictionary<Type, WorldEvent> _dict = new();
    }
}
