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
			var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			field.SetValue(obj, value);
		}

		public static object RunMethod(this object obj, string methodName, params object[] args)
		{
			var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			return method.Invoke(obj, args);
		}
	}
}
