using System;
using System.Drawing;
using System.Management;

namespace RedPixelDetector
{
	// Token: 0x02000006 RID: 6
	public class LicenseManager
	{
		// Token: 0x06000039 RID: 57
		public static bool IsLicenseExpired()
		{
			return DateTime.Now > LicenseManager.expirationDate;
		}

		// Token: 0x0600003A RID: 58
		public static DateTime GetExpirationDate()
		{
			return LicenseManager.expirationDate;
		}

		// Token: 0x0600003B RID: 59
		public static string GetCountdownText()
		{
			TimeSpan timeSpan = LicenseManager.expirationDate - DateTime.Now;
			if (timeSpan.TotalDays >= 1.0)
			{
				return string.Format("เวลาการใช้งาน เหลือ {0} วัน {1} ชม. {2} นาที {3} วินาที", new object[]
				{
					timeSpan.Days,
					timeSpan.Hours,
					timeSpan.Minutes,
					timeSpan.Seconds
				});
			}
			if (timeSpan.TotalHours >= 1.0)
			{
				return string.Format("เวลาการใช้งาน เหลือ {0} ชม. {1} นาที {2} วินาที", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			if (timeSpan.TotalMinutes >= 1.0)
			{
				return string.Format("เวลาการใช้งาน เหลือ {0} นาที {1} วินาที", timeSpan.Minutes, timeSpan.Seconds);
			}
			if (timeSpan.TotalSeconds > 0.0)
			{
				return string.Format("เวลาการใช้งาน เหลือ {0} วินาที", timeSpan.Seconds);
			}
			return "❌ หมดอายุแล้ว";
		}

		// Token: 0x0600003C RID: 60
		public static Color GetCountdownColor()
		{
			TimeSpan timeSpan = LicenseManager.expirationDate - DateTime.Now;
			if (timeSpan.TotalDays <= 0.0)
			{
				return Color.Red;
			}
			if (timeSpan.TotalDays <= 7.0)
			{
				return Color.Orange;
			}
			return Color.Green;
		}

		// Token: 0x04000020 RID: 32
		private static readonly DateTime expirationDate = new DateTime(2026, 12, 31, 23, 59, 59);

		private static readonly string allowed_uuid = "37616BCC-29F2-11B2-A85C-EB15EAAF6326ModeHC";

		public static string GetMachineUUID()
		{
			try
			{
				ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");
				ManagementObjectCollection moc = mc.GetInstances();

				foreach (ManagementObject mo in moc)
				{
					return mo.Properties["UUID"].Value.ToString();
				}
				return "UNKNOWN";
			}
			catch
			{
				return "UNKNOWN";
			}
		}

		// ✅ Method เช็ค UUID
		public static bool IsUuidAllowed()
		{
			try
			{
				string uuid = GetMachineUuid() + "ModeHC";
				return uuid.Equals(allowed_uuid, StringComparison.OrdinalIgnoreCase);
			}
			catch
			{
				return false; // ถ้าอ่าน UUID ไม่ได้ ให้ถือว่าไม่ผ่าน
			}
		}

		// ✅ Helper: อ่าน UUID จริงของเครื่องจาก BIOS
		private static string GetMachineUuid()
		{
			string uuid = string.Empty;
			using (ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct"))
			using (ManagementObjectCollection moc = mc.GetInstances())
			{
				foreach (ManagementObject mo in moc)
				{
					uuid = mo["UUID"].ToString();
					break;
				}
			}
			return uuid;
		}

	}
}
