using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace RedSkullShoot
{
	// Token: 0x02000003 RID: 3
	[Serializable]
	public class ProgramSettings
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000028A3 File Offset: 0x00000AA3
		// (set) Token: 0x06000012 RID: 18 RVA: 0x000028AB File Offset: 0x00000AAB
		public MouseButtons SelectedMouseButton { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000028B4 File Offset: 0x00000AB4
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000028BC File Offset: 0x00000ABC
		public byte SelectedKey { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000028C5 File Offset: 0x00000AC5
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000028CD File Offset: 0x00000ACD
		public List<ActionItem> ActionSequence { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000028D6 File Offset: 0x00000AD6
		// (set) Token: 0x06000018 RID: 24 RVA: 0x000028DE File Offset: 0x00000ADE
		public string CurrentResolution { get; set; } = "1920x1080 (Full HD)";

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000019 RID: 25 RVA: 0x000028E7 File Offset: 0x00000AE7
		// (set) Token: 0x0600001A RID: 26 RVA: 0x000028EF File Offset: 0x00000AEF
		public Rectangle CustomRegion { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001B RID: 27 RVA: 0x000028F8 File Offset: 0x00000AF8
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002900 File Offset: 0x00000B00
		public int RedThreshold { get; set; } = 255;

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002909 File Offset: 0x00000B09
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002911 File Offset: 0x00000B11
		public int GreenThreshold { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600001F RID: 31 RVA: 0x0000291A File Offset: 0x00000B1A
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002922 File Offset: 0x00000B22
		public int BlueThreshold { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000021 RID: 33 RVA: 0x0000292B File Offset: 0x00000B2B
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002933 File Offset: 0x00000B33
		public bool SniperZoomEnabled { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000023 RID: 35 RVA: 0x0000293C File Offset: 0x00000B3C
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002944 File Offset: 0x00000B44
		public MouseButtons SniperZoomButton { get; set; } = MouseButtons.XButton1;

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000021 RID: 33 RVA: 0x0000292B File Offset: 0x00000B2B
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002933 File Offset: 0x00000B33
		[XmlIgnore]
		public bool UseMouse { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000023 RID: 35 RVA: 0x0000293C File Offset: 0x00000B3C
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002944 File Offset: 0x00000B44
		[XmlIgnore]
		public bool UseKeyboard { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000025 RID: 37 RVA: 0x0000294D File Offset: 0x00000B4D
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002955 File Offset: 0x00000B55
		[XmlIgnore]
		public bool AutoMode { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000027 RID: 39 RVA: 0x0000295E File Offset: 0x00000B5E
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002966 File Offset: 0x00000B66
		[XmlIgnore]
		public bool HoldToActivate { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000029 RID: 41 RVA: 0x0000296F File Offset: 0x00000B6F
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002977 File Offset: 0x00000B77
		[XmlIgnore]
		public bool ToggleMode { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002980 File Offset: 0x00000B80
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002988 File Offset: 0x00000B88
		[XmlIgnore]
		public bool IsToggledOn { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002991 File Offset: 0x00000B91
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002999 File Offset: 0x00000B99
		[XmlIgnore]
		public string CurrentShootingMode { get; set; }

		// Token: 0x0600002F RID: 47 RVA: 0x000029A4 File Offset: 0x00000BA4
		public ProgramSettings()
		{
			this.ActionSequence = new List<ActionItem>();
			this.CustomRegion = new Rectangle(960, 0, 20, 540);
		}
	}
}
