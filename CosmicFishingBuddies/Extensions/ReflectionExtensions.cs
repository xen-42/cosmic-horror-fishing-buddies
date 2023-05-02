using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CosmicFishingBuddies.Extensions
{
	internal static class ReflectionExtensions
	{
		public static void SetValue(this object obj, string fieldName, object value)
		{
			obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
		}
	}
}
