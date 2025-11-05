using System;
using System.Drawing;
using System.Windows.Forms;

namespace RedSkullShoot
{
	// Token: 0x02000002 RID: 2
	public partial class MainForm : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public MainForm()
		{
			this.InitializeComponent();
			this.InitializeUI();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020D8 File Offset: 0x000002D8
		private void InitializeUI()
		{
			Color.FromArgb(45, 45, 65);
			Color secondaryColor = Color.FromArgb(60, 60, 80);
			Color successColor = Color.FromArgb(76, 175, 80);
			Color dangerColor = Color.FromArgb(244, 67, 54);
			Color accentColor = Color.FromArgb(0, 120, 215);
			Color textColor = Color.White;
			Panel mainPanel = new Panel
			{
				Dock = DockStyle.Fill,
				BackColor = secondaryColor,
				Padding = new Padding(15),
				BorderStyle = BorderStyle.FixedSingle
			};
			Panel buttonPanel = new Panel
			{
				Location = new Point(10, 10),
				Size = new Size(base.Width - 40, 50),
				BackColor = Color.Transparent
			};
			this.InitializeButtons(buttonPanel, successColor, dangerColor, accentColor, textColor);
			Panel statusPanel = new Panel
			{
				Location = new Point(10, 70),
				Size = new Size(base.Width - 40, 120),
				BackColor = Color.Transparent
			};
			this.InitializeLabels(statusPanel);
			Label separator = new Label
			{
				Text = new string('─', 80),
				Location = new Point(10, 200),
				Size = new Size(450, 20),
				ForeColor = Color.Gray
			};
			Label lblVersion = new Label
			{
				Text = "v1.0 | ระดับพระกาฬ \ud83d\ude08 | สายฟ้าประจันตา ⚡",
				Location = new Point(20, 230),
				Size = new Size(440, 20),
				Font = new Font("Microsoft Sans Serif", 8f),
				ForeColor = Color.LightGray,
				TextAlign = ContentAlignment.MiddleRight
			};
			statusPanel.Controls.AddRange(new Control[]
			{
				this.lblStatus,
				this.lblToggleStatus,
				this.lblInfo
			});
			mainPanel.Controls.AddRange(new Control[]
			{
				buttonPanel,
				statusPanel,
				separator,
				lblVersion
			});
			base.Controls.Add(mainPanel);
			this.InitializeToolTips();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000022E8 File Offset: 0x000004E8
		private void InitializeButtons(Panel buttonPanel, Color successColor, Color dangerColor, Color accentColor, Color textColor)
		{
			this.btnStart = new Button
			{
				Text = "\ud83d\ude80 เริ่มทำงาน",
				Location = new Point(0, 0),
				Size = new Size(100, 40),
				BackColor = successColor,
				ForeColor = textColor,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
			};
			this.btnStart.FlatAppearance.BorderSize = 0;
			this.btnStop = new Button
			{
				Text = "⏹️ หยุดทำงาน",
				Location = new Point(110, 0),
				Size = new Size(100, 40),
				BackColor = Color.FromArgb(200, 0, 0),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Enabled = false,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
			};
			this.btnStop.FlatAppearance.BorderSize = 0;
			this.btnStop.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 0, 0);
			this.btnStop.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 0, 0);
			this.btnSettings = new Button
			{
				Text = "⚙️ การตั้งค่า",
				Location = new Point(220, 0),
				Size = new Size(100, 40),
				BackColor = accentColor,
				ForeColor = textColor,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
			};
			this.btnSettings.FlatAppearance.BorderSize = 0;
			this.btnMinimizeToTray = new Button
			{
				Text = "\ud83d\udccc ซ่อน",
				Location = new Point(330, 0),
				Size = new Size(70, 40),
				BackColor = Color.FromArgb(100, 100, 140),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Microsoft Sans Serif", 9f),
				Visible = true
			};
			this.btnMinimizeToTray.FlatAppearance.BorderSize = 0;
			this.btnMinimizeToTray.FlatAppearance.MouseOverBackColor = Color.FromArgb(120, 120, 160);
			this.btnMinimizeToTray.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 80, 120);
			this.btnExit = new Button
			{
				Text = "❌ ออก",
				Location = new Point(410, 0),
				Size = new Size(70, 40),
				BackColor = Color.Gray,
				ForeColor = textColor,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
			};
			this.btnExit.FlatAppearance.BorderSize = 0;
			buttonPanel.Controls.AddRange(new Control[]
			{
				this.btnStart,
				this.btnStop,
				this.btnSettings,
				this.btnMinimizeToTray,
				this.btnExit
			});
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000260C File Offset: 0x0000080C
		private void InitializeLabels(Panel statusPanel)
		{
			this.lblStatus = new Label
			{
				Text = "\ud83d\udcca สถานะ: หยุดทำงาน",
				Location = new Point(0, 0),
				Size = new Size(450, 25),
				Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold),
				ForeColor = Color.LightGreen
			};
			this.lblToggleStatus = new Label
			{
				Text = "\ud83d\udd18 สถานะ Toggle: ไม่ใช้งาน",
				Location = new Point(0, 30),
				Size = new Size(450, 25),
				Font = new Font("Microsoft Sans Serif", 9f),
				ForeColor = Color.LightGray,
				Name = "lblToggleStatus"
			};
			this.lblInfo = new Label
			{
				Text = "\ud83d\udca1 คลิก 'เริ่มทำงาน' เพื่อเริ่มตรวจจับพิกเซลสีแดง\n\ud83c\udfaf ตั้งค่าโหมดการทำงานใน 'การตั้งค่า' ก่อนเริ่มใช้งาน",
				Location = new Point(0, 60),
				Size = new Size(450, 40),
				Font = new Font("Microsoft Sans Serif", 9f),
				ForeColor = Color.LightBlue
			};
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002728 File Offset: 0x00000928
		private void InitializeToolTips()
		{
			ToolTip toolTip = new ToolTip();
			toolTip.AutomaticDelay = 500;
			toolTip.AutoPopDelay = 5000;
			toolTip.InitialDelay = 500;
			toolTip.ReshowDelay = 100;
			toolTip.BackColor = Color.FromArgb(60, 60, 80);
			toolTip.ForeColor = Color.White;
			toolTip.SetToolTip(this.btnStart, "\ud83d\ude80 เริ่มการตรวจจับพิกเซลสีแดงอัตโนมัติ");
			toolTip.SetToolTip(this.btnStop, "⏹️ หยุดการทำงานของโปรแกรม");
			toolTip.SetToolTip(this.btnSettings, "⚙️ เปิดการตั้งค่าขั้นสูง");
			toolTip.SetToolTip(this.btnMinimizeToTray, "\ud83d\udccc ซ่อนโปรแกรมไปที่ System Tray\nดับเบิลคลิกที่ไอคอนเพื่อแสดงอีกครั้ง");
			toolTip.SetToolTip(this.btnExit, "❌ ปิดโปรแกรม");
			toolTip.SetToolTip(this.lblStatus, "\ud83d\udcca แสดงสถานะปัจจุบันของโปรแกรม");
			toolTip.SetToolTip(this.lblToggleStatus, "\ud83d\udd18 แสดงสถานะโหมด Toggle");
			toolTip.SetToolTip(this.lblInfo, "\ud83d\udca1 ข้อมูลพื้นฐานเกี่ยวกับการใช้งานโปรแกรม");
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002806 File Offset: 0x00000A06
		public void SetEventHandlers(EventHandler startHandler, EventHandler stopHandler, EventHandler settingsHandler, EventHandler minimizeHandler, EventHandler exitHandler)
		{
			this.btnStart.Click += startHandler;
			this.btnStop.Click += stopHandler;
			this.btnSettings.Click += settingsHandler;
			this.btnMinimizeToTray.Click += minimizeHandler;
			this.btnExit.Click += exitHandler;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002846 File Offset: 0x00000A46
		public Button StartButton
		{
			get
			{
				return this.btnStart;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x0000284E File Offset: 0x00000A4E
		public Button StopButton
		{
			get
			{
				return this.btnStop;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002856 File Offset: 0x00000A56
		public Button SettingsButton
		{
			get
			{
				return this.btnSettings;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000285E File Offset: 0x00000A5E
		public Button MinimizeButton
		{
			get
			{
				return this.btnMinimizeToTray;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002866 File Offset: 0x00000A66
		public Button ExitButton
		{
			get
			{
				return this.btnExit;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000286E File Offset: 0x00000A6E
		public Label StatusLabel
		{
			get
			{
				return this.lblStatus;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002876 File Offset: 0x00000A76
		public Label ToggleStatusLabel
		{
			get
			{
				return this.lblToggleStatus;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000F RID: 15 RVA: 0x0000287E File Offset: 0x00000A7E
		public Label InfoLabel
		{
			get
			{
				return this.lblInfo;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002886 File Offset: 0x00000A86
		public void UpdateButtonStates(bool isRunning)
		{
			this.btnStart.Enabled = !isRunning;
			this.btnStop.Enabled = isRunning;
		}

		// Token: 0x04000001 RID: 1
		private Button btnStart;

		// Token: 0x04000002 RID: 2
		private Button btnStop;

		// Token: 0x04000003 RID: 3
		private Button btnSettings;

		// Token: 0x04000004 RID: 4
		private Button btnExit;

		// Token: 0x04000005 RID: 5
		private Button btnMinimizeToTray;

		// Token: 0x04000006 RID: 6
		private Label lblStatus;

		// Token: 0x04000007 RID: 7
		private Label lblToggleStatus;

		// Token: 0x04000008 RID: 8
		private Label lblInfo;
	}
}
