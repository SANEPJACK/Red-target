using System;

namespace RedPixelDetector
{
	// Token: 0x02000004 RID: 4
	public class ActionItem
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002200 File Offset: 0x00000400
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002208 File Offset: 0x00000408
		public ActionItem.ActionType Type { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002211 File Offset: 0x00000411
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002219 File Offset: 0x00000419
		public string Value { get; set; } = "";

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002222 File Offset: 0x00000422
		// (set) Token: 0x06000037 RID: 55 RVA: 0x0000222A File Offset: 0x0000042A
		public int DelayMs { get; set; }

		// Token: 0x02000005 RID: 5
		public enum ActionType
		{
			// Token: 0x0400001D RID: 29
			MouseClick,
			// Token: 0x0400001E RID: 30
			KeyPress,
			// Token: 0x0400001F RID: 31
			Delay
		}
	}
}
