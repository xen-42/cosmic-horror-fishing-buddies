using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmicFishingBuddies.Util
{
	internal static class NotificationHelper
	{
		public static void ShowNotificationWithColour(NotificationType notificationType, string text, string colourCode)
		{
			GameEvents.Instance.TriggerNotification(notificationType, string.Concat(new string[]
			{
					"<color=#",
					colourCode,
					">",
					text,
					"</color>"
			}));
		}

		public static void ShowNotificationWithColour(NotificationType notificationType, string text, DredgeColorTypeEnum colour)
		{
			ShowNotificationWithColour(notificationType, text, GameManager.Instance.LanguageManager.GetColorCode(colour));
		}

		public static void ShowNotification(NotificationType notificationType, string text)
		{
			GameEvents.Instance.TriggerNotification(notificationType, text);
		}
	}
}
