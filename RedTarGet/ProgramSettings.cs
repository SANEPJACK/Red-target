using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace RedPixelDetector
{
	// Token: 0x02000003 RID: 3
	[Serializable]
	public class ProgramSettings
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002101 File Offset: 0x00000301
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002109 File Offset: 0x00000309
		public MouseButtons SelectedMouseButton { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002112 File Offset: 0x00000312
		// (set) Token: 0x06000016 RID: 22 RVA: 0x0000211A File Offset: 0x0000031A
		public byte SelectedKey { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002123 File Offset: 0x00000323
		// (set) Token: 0x06000018 RID: 24 RVA: 0x0000212B File Offset: 0x0000032B
		public List<ActionItem> ActionSequence { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002134 File Offset: 0x00000334
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000213C File Offset: 0x0000033C
		public string CurrentResolution { get; set; } = "1920x1080 (Full HD)";

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002145 File Offset: 0x00000345
		// (set) Token: 0x0600001C RID: 28 RVA: 0x0000214D File Offset: 0x0000034D
		public Rectangle CustomRegion { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002156 File Offset: 0x00000356
		// (set) Token: 0x0600001E RID: 30 RVA: 0x0000215E File Offset: 0x0000035E
		public int RedThreshold { get; set; } = 255;

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002167 File Offset: 0x00000367
		// (set) Token: 0x06000020 RID: 32 RVA: 0x0000216F File Offset: 0x0000036F
		public int GreenThreshold { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002178 File Offset: 0x00000378
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002180 File Offset: 0x00000380
		public int BlueThreshold { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002189 File Offset: 0x00000389
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002191 File Offset: 0x00000391
		[XmlIgnore]
		public bool UseMouse { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000025 RID: 37 RVA: 0x0000219A File Offset: 0x0000039A
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000021A2 File Offset: 0x000003A2
		[XmlIgnore]
		public bool UseKeyboard { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000021AB File Offset: 0x000003AB
		// (set) Token: 0x06000028 RID: 40 RVA: 0x000021B3 File Offset: 0x000003B3
		[XmlIgnore]
		public bool AutoMode { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000029 RID: 41 RVA: 0x000021BC File Offset: 0x000003BC
		// (set) Token: 0x0600002A RID: 42 RVA: 0x000021C4 File Offset: 0x000003C4
		[XmlIgnore]
		public bool HoldToActivate { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000021CD File Offset: 0x000003CD
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000021D5 File Offset: 0x000003D5
		[XmlIgnore]
		public bool ToggleMode { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000021DE File Offset: 0x000003DE
		// (set) Token: 0x0600002E RID: 46 RVA: 0x000021E6 File Offset: 0x000003E6
		[XmlIgnore]
		public bool IsToggledOn { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000021EF File Offset: 0x000003EF
		// (set) Token: 0x06000030 RID: 48 RVA: 0x000021F7 File Offset: 0x000003F7
		[XmlIgnore]
		public string CurrentShootingMode { get; set; }

		// Token: 0x06000031 RID: 49 RVA: 0x00002E00 File Offset: 0x00001000
		public ProgramSettings()
		{
			this.ActionSequence = new List<ActionItem>();
			this.CustomRegion = new Rectangle(960, 0, 20, 540);
		}
	}
}
