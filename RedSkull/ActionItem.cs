using System;

namespace RedSkullShoot
{
	// Token: 0x02000004 RID: 4
	public class ActionItem
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000029F0 File Offset: 0x00000BF0
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000029F8 File Offset: 0x00000BF8
		public ActionItem.ActionType Type { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002A01 File Offset: 0x00000C01
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002A09 File Offset: 0x00000C09
		public string Value { get; set; } = "";

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002A12 File Offset: 0x00000C12
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002A1A File Offset: 0x00000C1A
		public int DelayMs { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002A23 File Offset: 0x00000C23
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002A2B File Offset: 0x00000C2B
		public string ModeType { get; set; } = "Custom";

		// Token: 0x06000038 RID: 56 RVA: 0x00002A34 File Offset: 0x00000C34
		public ActionItem()
		{
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002A52 File Offset: 0x00000C52
		public ActionItem(ActionItem.ActionType type, string value, int delayMs)
		{
			this.Type = type;
			this.Value = value;
			this.DelayMs = delayMs;
		}

		// Token: 0x02000008 RID: 8
		public enum ActionType
		{
			// Token: 0x040000B6 RID: 182
			MouseClick,
			// Token: 0x040000B7 RID: 183
			KeyPress,
			// Token: 0x040000B8 RID: 184
			Delay
		}
	}
}
