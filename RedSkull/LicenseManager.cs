using System;
using System.Drawing;

namespace RedSkullShoot
{
	// Token: 0x02000005 RID: 5
	public class LicenseManager
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00002A85 File Offset: 0x00000C85
		public static bool IsLicenseExpired()
		{
			return DateTime.Now > LicenseManager.expirationDate;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002A96 File Offset: 0x00000C96
		public static DateTime GetExpirationDate()
		{
			return LicenseManager.expirationDate;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002AA0 File Offset: 0x00000CA0
		public static string GetCountdownText()
		{
			TimeSpan timeLeft = LicenseManager.expirationDate - DateTime.Now;
			if (timeLeft.TotalDays >= 1.0)
			{
				return string.Format("⏳ เหลือ {0} วัน {1} ชม. {2} นาที {3} วินาที", new object[]
				{
					timeLeft.Days,
					timeLeft.Hours,
					timeLeft.Minutes,
					timeLeft.Seconds
				});
			}
			if (timeLeft.TotalHours >= 1.0)
			{
				return string.Format("⏳ เหลือ {0} ชม. {1} นาที {2} วินาที", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
			}
			if (timeLeft.TotalMinutes >= 1.0)
			{
				return string.Format("⏳ เหลือ {0} นาที {1} วินาที", timeLeft.Minutes, timeLeft.Seconds);
			}
			if (timeLeft.TotalSeconds > 0.0)
			{
				return string.Format("⏳ เหลือ {0} วินาที", timeLeft.Seconds);
			}
			return "❌ หมดอายุแล้ว";
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002BC0 File Offset: 0x00000DC0
		public static Color GetCountdownColor()
		{
			TimeSpan timeLeft = LicenseManager.expirationDate - DateTime.Now;
			if (timeLeft.TotalDays <= 0.0)
			{
				return Color.Red;
			}
			if (timeLeft.TotalDays <= 7.0)
			{
				return Color.Orange;
			}
			return Color.LightGreen;
		}

		// Token: 0x0400001C RID: 28
		private static readonly DateTime expirationDate = new DateTime(2025, 11, 6, 23, 59, 59);
	}
}
