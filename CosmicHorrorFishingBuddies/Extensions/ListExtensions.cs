using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmicHorrorFishingBuddies.Extensions
{
	internal static class ListExtensions
	{
		public static IEnumerable<int> FindIndices<T>(this IList<T> list, Func<T, bool> predicate)
		{
			return list.Where(predicate).Select(list.IndexOf);
		}
	}
}
