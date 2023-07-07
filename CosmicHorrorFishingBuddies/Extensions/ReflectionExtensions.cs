using System.Reflection;

namespace CosmicHorrorFishingBuddies.Extensions
{
	internal static class ReflectionExtensions
	{
		public static void SetValue(this object obj, string fieldName, object value)
		{
			var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			field.SetValue(obj, value);
		}

		public static T GetValue<T>(this object obj, string fieldName)
		{
			var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			return (T)field.GetValue(obj);
		}

		public static object RunMethod(this object obj, string methodName, params object[] args)
		{
			var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			return method.Invoke(obj, args);
		}
	}
}
