using CosmicHorrorFishingBuddies.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmicHorrorFishingBuddies.Util
{
	internal static class NotificationHelper
	{
		public static void ShowNotificationWithColour(NotificationType notificationType, string text, DredgeColorTypeEnum colour)
		{
			ShowNotificationWithColour(notificationType, text, GameManager.Instance.LanguageManager.GetColorCode(colour));
		}

		public static void ShowNotificationWithColour(NotificationType notificationType, string text, string colourCode)
		{
			ShowNotification(notificationType, $"<color=#{colourCode}>{text}</color>");
		}

		public static void ShowNotification(NotificationType notificationType, string text)
		{
			GameEvents.Instance.TriggerNotification(notificationType, text);
		}
	}
}
