using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
// using WebSocketClientLib;

namespace RedPixelDetector
{
	// Token: 0x02000007 RID: 7
	internal class Program
	{
		// Token: 0x0600003F RID: 63
		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		// Token: 0x06000040 RID: 64
		[DllImport("user32.dll")]
		private static extern bool GetWindowRect(IntPtr hwnd, out Program.RECT lpRect);

		// Token: 0x06000041 RID: 65
		[DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		// Token: 0x06000042 RID: 66
		[DllImport("user32.dll")]
		private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

		// Token: 0x06000043 RID: 67
		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		// Token: 0x06000044 RID: 68
		[DllImport("user32.dll")]
		private static extern int GetKeyState(int nVirtKey);

		// Token: 0x06000045 RID: 69
		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		// Token: 0x06000046 RID: 70
		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		// Token: 0x06000047 RID: 71
		[STAThread]
		private static void Main(string[] args)
		{
			Program.LoadSettings();
			// try
			// {
			// 	new Thread(delegate()
			// 	{
			// 		try
			// 		{
			// 			new WebSocketClient("RedTarGet").Start();
			// 		}
			// 		catch (Exception ex2)
			// 		{
			// 			Console.WriteLine("WebSocket Error: " + ex2.Message);
			// 		}
			// 	})
			// 	{
			// 		IsBackground = true
			// 	}.Start();
			// }
			//catch (Exception ex)
			//{
			//	Console.WriteLine("Failed to start WebSocket: " + ex.Message);
			//}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Program.InitializeTrayIcon();
			Program.CreateMainForm();
			Program.UpdateStatusDisplay();
			Program.mainForm.FormClosing += delegate(object s, FormClosingEventArgs e)
			{
				if (Program.isMinimizedToTray)
				{
					e.Cancel = true;
					Program.HideToTray();
					return;
				}
				Program.SaveSettingsWithoutModes();
				Program.SetScrollLockLED(false);
				NotifyIcon notifyIcon = Program.trayIcon;
				if (notifyIcon == null)
				{
					return;
				}
				notifyIcon.Dispose();
			};
			Program.StartCountdownTimer();
			Program.Checkuuid();
			Application.Run(Program.mainForm);
			
		}

		// Token: 0x06000048 RID: 72
		private static void StartCountdownTimer()
		{
			Program.countdownTimer = new System.Windows.Forms.Timer();
			Program.countdownTimer.Interval = 1000;
			Program.countdownTimer.Tick += delegate(object s, EventArgs e)
			{
				Program.UpdateCountdown();
			};
			Program.countdownTimer.Start();
		}

		// Token: 0x06000049 RID: 73
		private static void UpdateCountdown()
		{
			if (Program.lblCountdown1 != null && Program.mainForm != null && !Program.mainForm.IsDisposed)
			{
				if (Program.mainForm.InvokeRequired)
				{
					Program.mainForm.Invoke(new Action(Program.UpdateCountdown));
					return;
				}
				Program.lblCountdown1.Text = LicenseManager.GetCountdownText();
				Program.lblCountdown1.ForeColor = LicenseManager.GetCountdownColor();
				if (LicenseManager.IsLicenseExpired())
				{
					Program.countdownTimer.Stop();
					Program.lblCountdown1.Text = "หมดอายุแล้ว";
					Program.lblCountdown1.ForeColor = Color.Red;
					MessageBox.Show("การใช้งาน หมดอายุแล้ว!\nโปรแกรมจะปิดตัวเอง", "แจ้งเตือน");
					Application.Exit();
				}
			}
		}

		//private static void Checkuuid()
		//{
		//	if (!LicenseManager.IsUuidAllowed())
		//	{
		//		MessageBox.Show("เครื่องนี้ไม่ได้รับอนุญาตให้ใช้งาน", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
		//		Program.lblCountdown.Text = "สถานะโปรแกรม : ไม่ได้รับอนุญาต";
		//		Program.lblCountdown.ForeColor = Color.Green;
		//		Application.Exit();
		//		Environment.Exit(0); // ปิดโปรแกรมทันที (เผื่อ Application.Exit ไม่ทำงานเร็วพอ)
  //          }
  //          else
  //          {
		//		Program.lblCountdown.Text = "สถานะโปรแกรม : ได้รับอนุญาต";
		//		Program.lblCountdown.ForeColor = Color.Green;
		//	}
		//}

		private static void Checkuuid()
		{
			string allowed_uuid = "37616BCC-29F2-11B2-A85C-EB15EAAF6326ModeHC";
			string current_uuid = LicenseManager.GetMachineUUID() + "ModeHC";

			string current_uuid1 = LicenseManager.GetMachineUUID();

			if (current_uuid != allowed_uuid)
			{
				DialogResult r = MessageBox.Show(
					"เครื่องนี้ไม่ได้รับอนุญาตให้ใช้งาน\n\nUUID เครื่องคุณคือ :\n\n" + current_uuid1 +
					"\n\n(กด OK เพื่อ Copy UUID)",
					"Access Denied",
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Error
				);

				Program.lblCountdown.Text = "สถานะโปรแกรม : ไม่ได้รับอนุญาต";
				Program.lblCountdown.ForeColor = Color.Green;

				if (r == DialogResult.OK)
				{
					Clipboard.SetText(current_uuid1);
					MessageBox.Show("คัดลอก UUID เรียบร้อยแล้ว!\nส่งให้ Admin เพื่อดำเนินการ", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				Application.Exit();
				Environment.Exit(0);
			}
			else
			{
				Program.lblCountdown.Text = "สถานะโปรแกรม : ได้รับอนุญาต";
				Program.lblCountdown.ForeColor = Color.Green;
			}
		}



		// Token: 0x0600004A RID: 74
		private static void LoadSettings()
		{
			try
			{
				if (File.Exists(Program.settingsFilePath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ProgramSettings));
					using (FileStream fs = new FileStream(Program.settingsFilePath, FileMode.Open))
					{
						ProgramSettings settings = (ProgramSettings)serializer.Deserialize(fs);
						Program.useMouse = settings.UseMouse;
						Program.useKeyboard = settings.UseKeyboard;
						Program.autoMode = settings.AutoMode;
						Program.holdToActivate = settings.HoldToActivate;
						Program.toggleMode = settings.ToggleMode;
						Program.isToggledOn = settings.IsToggledOn;
						Program.selectedMouseButton = settings.SelectedMouseButton;
						Program.selectedKey = settings.SelectedKey;
						if (settings.ActionSequence != null && settings.ActionSequence.Count > 0)
						{
							Program.actionSequence = settings.ActionSequence;
						}
						else
						{
							Program.actionSequence = new List<ActionItem>();
						}
						Program.currentResolution = (settings.CurrentResolution ?? "1920x1080");
						Program.currentShootingMode = (settings.CurrentShootingMode ?? "Manual");
						int width = Math.Max(0, Math.Min(settings.CustomRegion.Width, 100));
						int height = Math.Max(0, Math.Min(settings.CustomRegion.Height, 4000));
						Program.customRegion = new Rectangle(settings.CustomRegion.X, settings.CustomRegion.Y, width, height);
						Program.redThreshold = Math.Max(0, Math.Min(settings.RedThreshold, 255));
						Program.greenThreshold = Math.Max(0, Math.Min(settings.GreenThreshold, 255));
						Program.blueThreshold = Math.Max(0, Math.Min(settings.BlueThreshold, 255));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error loading settings: " + ex.Message);
				Program.currentResolution = "1920x1080";
			}
		}

		// Token: 0x0600004B RID: 75
		private static void SaveSettings()
		{
			try
			{
				ProgramSettings settings = new ProgramSettings
				{
					UseMouse = Program.useMouse,
					UseKeyboard = Program.useKeyboard,
					AutoMode = Program.autoMode,
					HoldToActivate = Program.holdToActivate,
					ToggleMode = Program.toggleMode,
					IsToggledOn = Program.isToggledOn,
					SelectedMouseButton = Program.selectedMouseButton,
					SelectedKey = Program.selectedKey,
					ActionSequence = Program.actionSequence,
					CurrentResolution = Program.currentResolution,
					CustomRegion = Program.customRegion,
					RedThreshold = Program.redThreshold,
					GreenThreshold = Program.greenThreshold,
					BlueThreshold = Program.blueThreshold,
					CurrentShootingMode = Program.currentShootingMode
				};
				XmlSerializer serializer = new XmlSerializer(typeof(ProgramSettings));
				using (FileStream fs = new FileStream(Program.settingsFilePath, FileMode.Create))
				{
					serializer.Serialize(fs, settings);
				}
				Console.WriteLine("Saved resolution: " + Program.currentResolution);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error saving settings: " + ex.Message);
			}
		}

		// Token: 0x0600004C RID: 76
		private static void SaveSettingsWithoutModes()
		{
			try
			{
				ProgramSettings settings = new ProgramSettings
				{
					SelectedMouseButton = Program.selectedMouseButton,
					SelectedKey = Program.selectedKey,
					ActionSequence = Program.actionSequence,
					CurrentResolution = Program.currentResolution,
					CustomRegion = Program.customRegion,
					RedThreshold = Program.redThreshold,
					GreenThreshold = Program.greenThreshold,
					BlueThreshold = Program.blueThreshold
				};
				XmlSerializer serializer = new XmlSerializer(typeof(ProgramSettings));
				using (FileStream fs = new FileStream(Program.settingsFilePath, FileMode.Create))
				{
					serializer.Serialize(fs, settings);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error saving settings: " + ex.Message);
			}
		}

		// Token: 0x0600004D RID: 77
		private static void InitializeTrayIcon()
		{
			Program.trayMenu = new ContextMenuStrip();
			Program.trayMenu.Items.Add("แสดงหน้าต่าง", null, delegate(object s, EventArgs e)
			{
				Program.ShowMainWindow();
			});
			Program.trayMenu.Items.Add("ซ่อนหน้าต่าง", null, delegate(object s, EventArgs e)
			{
				Program.HideToTray();
			});
			Program.trayMenu.Items.Add(new ToolStripSeparator());
			Program.trayMenu.Items.Add("ออกจากโปรแกรม", null, delegate(object s, EventArgs e)
			{
				Application.Exit();
			});
			Program.trayIcon = new NotifyIcon
			{
				Text = "กระโหลกแดง 💀 (Red Skull)",
				Icon = SystemIcons.Application,
				ContextMenuStrip = Program.trayMenu,
				Visible = false
			};
			Program.trayIcon.DoubleClick += delegate(object s, EventArgs e)
			{
				Program.ShowMainWindow();
			};
		}

		// Token: 0x0600004E RID: 78
		private static void HideToTray()
		{
			if (Program.mainForm != null && !Program.isMinimizedToTray)
			{
				Program.mainForm.Hide();
				Program.trayIcon.Visible = true;
				Program.isMinimizedToTray = true;
				Program.trayIcon.ShowBalloonTip(1000, "กระโหลกแดง 💀 (Red Skull)", "โปรแกรมถูกซ่อนไว้ใน System Tray", ToolTipIcon.Info);
			}
		}

		// Token: 0x0600004F RID: 79
		private static void ShowMainWindow()
		{
			if (Program.mainForm != null && Program.isMinimizedToTray)
			{
				Program.mainForm.Show();
				Program.mainForm.WindowState = FormWindowState.Normal;
				Program.mainForm.BringToFront();
				Program.trayIcon.Visible = false;
				Program.isMinimizedToTray = false;
			}
		}

		// Token: 0x06000050 RID: 80
		private static void CreateMainForm()
		{
			Color primaryColor = Color.FromArgb(237, 242, 248);
			Color surfaceColor = Color.White;
			Color successColor = Color.FromArgb(52, 199, 89);
			Color dangerColor = Color.FromArgb(231, 76, 60);
			Color accentColor = Color.FromArgb(79, 70, 229);
			Color neutralColor = Color.FromArgb(108, 117, 125);
			Color textColor = Color.FromArgb(34, 40, 49);

			Program.mainForm = new Form
			{
				Text = "กระโหลกแดง 💀 (Red Skull)",
				Size = new Size(560, 300),
				StartPosition = FormStartPosition.CenterScreen,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				MaximizeBox = false,
				MinimizeBox = false,
				BackColor = ColorTranslator.FromHtml("#4682B4"),
				ForeColor = Color.Red,
				Font = new Font("Segoe UI", 11, FontStyle.Bold) // ตัวหนา + สวยอ่านง่าย
			};

			// ==== Create Buttons ====
			Program.btnStart = new Button { Text = "เริ่มทำงาน", Dock = DockStyle.Fill, BackColor = successColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10f), Cursor = Cursors.Hand };
			Program.btnStop = new Button { Text = "หยุดทำงาน", Dock = DockStyle.Fill, BackColor = dangerColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10f), Cursor = Cursors.Hand, Enabled = false };
			Program.btnSettings = new Button { Text = "การตั้งค่า", Dock = DockStyle.Fill, BackColor = accentColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10f), Cursor = Cursors.Hand };
			Button btnMinimizeToTray = new Button { Text = "ย่อโปรแกรม", Dock = DockStyle.Fill, BackColor = Color.FromArgb(100, 100, 140), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f), Cursor = Cursors.Hand };
			Program.btnExit = new Button { Text = "ออก", Dock = DockStyle.Fill, BackColor = neutralColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10f), Cursor = Cursors.Hand };

			// ==== Layout ====
			TableLayoutPanel mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(28), ColumnCount = 1, BackColor = ColorTranslator.FromHtml("#4682B4") };
			mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // top buttons
			mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // status
			mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // bottom buttons

			// ==== Top Button Bar ====
			TableLayoutPanel topButtonsPanel = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 3 };
			topButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
			topButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
			topButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

			topButtonsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44f));

			topButtonsPanel.Controls.Add(Program.btnStart, 0, 0);
			topButtonsPanel.Controls.Add(Program.btnStop, 1, 0);
			topButtonsPanel.Controls.Add(Program.btnSettings, 2, 0);
			mainLayout.Controls.Add(topButtonsPanel);

			// ==== Status Card ====
			Panel statusCard = new Panel { Dock = DockStyle.Top, AutoSize = true, BackColor = Color.White, Padding = new Padding(20, 18, 20, 18), Margin = new Padding(0, 0, 0, 18), BorderStyle = BorderStyle.FixedSingle };
			Label statusHeading = new Label { Text = "สถานะระบบ", Dock = DockStyle.Top, Font = new Font("Segoe UI Semibold", 11f, FontStyle.Bold), ForeColor = accentColor, TextAlign = ContentAlignment.MiddleCenter };
			Panel statusDivider = new Panel { Dock = DockStyle.Top, Height = 2, BackColor = primaryColor, Margin = new Padding(0, 12, 0, 12) };

			Program.lblToggleStatus = new Label { Text = "สถานะ เปิด / ปิด : ไม่ใช้งาน", AutoSize = true, Font = new Font("Segoe UI Semibold", 10f), ForeColor = Color.Black, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top, Visible = false };
			Program.lblStatus = new Label { Text = "สถานะ: หยุดทำงาน", AutoSize = true, Font = new Font("Segoe UI", 10f), ForeColor = Color.Green, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top };
			Program.lblCountdown = new Label { Text = "กำลังโหลด...", AutoSize = true, Font = new Font("Segoe UI Semibold", 10f), ForeColor = Color.DarkGreen, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top };
			Program.lblCountdown1 = new Label { Text = "กำลังโหลด...", AutoSize = true, Font = new Font("Segoe UI Semibold", 10f), ForeColor = Color.DarkGreen, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Top };

			statusCard.Controls.Add(Program.lblCountdown1);
			statusCard.Controls.Add(Program.lblCountdown);
			statusCard.Controls.Add(Program.lblStatus);
			statusCard.Controls.Add(Program.lblToggleStatus);
			statusCard.Controls.Add(statusDivider);
			statusCard.Controls.Add(statusHeading);
			mainLayout.Controls.Add(statusCard);

			// ==== Bottom Buttons Panel ====
			TableLayoutPanel bottomButtonsPanel = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1 };
			bottomButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			bottomButtonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

			bottomButtonsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 33f));

			// ย่อโปรแกรม จะเปิด ต้อง ColumnCount = 2 ใน  Bottom Buttons Panel
			//bottomButtonsPanel.Controls.Add(btnMinimizeToTray, 0, 0);
			bottomButtonsPanel.Controls.Add(Program.btnExit, 1, 0);
			mainLayout.Controls.Add(bottomButtonsPanel);

			Program.mainForm.Controls.Add(mainLayout);

			// ==== Events & Tooltips ====
			//ToolTip toolTip = new ToolTip();
			//toolTip.SetToolTip(Program.btnStart, "เริ่มทำงาน");
			//toolTip.SetToolTip(Program.btnStop, "หยุดทำงาน");
			//toolTip.SetToolTip(Program.btnSettings, "ตั้งค่าโปรแกรม");
			//toolTip.SetToolTip(btnMinimizeToTray, "ย่อหน้าต่างไป Tray");
			//toolTip.SetToolTip(Program.btnExit, "ออกจากโปรแกรม");

			Program.btnStart.Click += (s, e) => Program.StartDetection();
			Program.btnStop.Click += (s, e) => Program.StopDetection();
			Program.btnSettings.Click += (s, e) => Program.ShowSettingsDialog();
			btnMinimizeToTray.Click += (s, e) =>
			{
				if (Program.isMinimizedToTray)
					Program.ShowMainWindow();
				else
					Program.HideToTray();
			};
			Program.btnExit.Click += (s, e) => Application.Exit();

			Program.btnStart.Enabled = !Program.isRunning;
			Program.btnStop.Enabled = Program.isRunning;
		}

		// Token: 0x06000051 RID: 81
		private static void StartDetection()
		{
			if (LicenseManager.IsLicenseExpired())
			{
				MessageBox.Show("การทำงาน หมดอายุแล้ว! ไม่สามารถเริ่มทำงานได้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!Program.autoMode && !Program.toggleMode && !Program.holdToActivate)
			{
				MessageBox.Show("กรุณาตั้งค่า ก่อนเริ่มทำงาน", "ระบบแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (!Program.autoMode && !Program.useMouse && !Program.useKeyboard)
			{
				MessageBox.Show("กรุณาเลือกโหมด เปิด / ปิด การทำงาน (เมาส์ หรือ คีย์บอร์ด) ใน การตั้งค่า", "ระบบแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (Program.actionSequence == null || Program.actionSequence.Count <= 0)
			{
				MessageBox.Show("กรุณาเลือกโหมดการยิงใน การตั้งค่า ก่อนเริ่มทำงาน", "ระบบแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			Program.isRunning = true;
			Program.btnStart.Enabled = false;
			Program.btnStop.Enabled = true;
			Program.UpdateStatusDisplay();
			Program.UpdateScrollLockLED();
			new Thread(new ThreadStart(Program.DetectionLoop))
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000052 RID: 82
		private static string GetCurrentModeDescription()
		{
			string modeDesc = "";

			if (Program.autoMode)
			{
				modeDesc += "โหมดอัตโนมัติ";
			}
			else if (Program.toggleMode)
			{
				string buttonName = "";
				if (Program.useMouse)
				{
					MouseButtons mouseButtons = Program.selectedMouseButton;
					if (mouseButtons <= MouseButtons.Right)
					{
						if (mouseButtons == MouseButtons.Left) buttonName = "คลิกซ้าย";
						else if (mouseButtons == MouseButtons.Right) buttonName = "คลิกขวา";
					}
					else
					{
						if (mouseButtons == MouseButtons.Middle) buttonName = "คลิกกลาง";
						else if (mouseButtons == MouseButtons.XButton1) buttonName = "ปุ่มข้าง 1";
						else if (mouseButtons == MouseButtons.XButton2) buttonName = "ปุ่มข้าง 2";
					}

					string label = "เปิด / ปิด เมาส์ (" + buttonName + ")";
					if (!Program.isToggledOn) label = "" + label + "";
					modeDesc += label;
				}
				else if (Program.useKeyboard)
				{
					string label = "เปิด / ปิด คีย์บอร์ด (" + Program.GetKeyName(Program.selectedKey) + ")";
					if (!Program.isToggledOn) label = "" + label + "";
					modeDesc += label;
				}

				modeDesc += (Program.isToggledOn ? " [เปิด]" : " [ปิด]");
			}
			else if (Program.useMouse)
			{
				string buttonName = "";
				MouseButtons mouseButtons = Program.selectedMouseButton;
				if (mouseButtons <= MouseButtons.Right)
				{
					if (mouseButtons == MouseButtons.Left) buttonName = "คลิกซ้าย";
					else if (mouseButtons == MouseButtons.Right) buttonName = "คลิกขวา";
				}
				else
				{
					if (mouseButtons == MouseButtons.Middle) buttonName = "คลิกกลาง";
					else if (mouseButtons == MouseButtons.XButton1) buttonName = "ปุ่มข้าง 1";
					else if (mouseButtons == MouseButtons.XButton2) buttonName = "ปุ่มข้าง 2";
				}

				modeDesc += "เมาส์ (" + buttonName + ")";
			}
			else if (Program.useKeyboard)
			{
				modeDesc += "คีย์บอร์ด (" + Program.GetKeyName(Program.selectedKey) + ")";
			}

			if (Program.holdToActivate)
			{
				modeDesc += " [กดค้าง]";
			}

			return modeDesc;
		}


		// Token: 0x06000053 RID: 83
		private static void StopDetection()
		{
			Program.isRunning = false;
			Program.btnStart.Enabled = true;
			Program.btnStop.Enabled = false;
			Program.UpdateStatusDisplay();
			Program.UpdateScrollLockLED();
		}

		// Token: 0x06000054 RID: 84
		private static void UpdateStatus(string text)
		{
			if (Program.mainForm.InvokeRequired)
			{
				Program.mainForm.Invoke(new Action<string>(Program.UpdateStatus), new object[]
				{
					text
				});
				return;
			}
			Program.lblStatus.Text = text;
		}

		// Token: 0x06000055 RID: 85
		private static void DetectionLoop()
		{
			while (Program.isRunning)
			{
				if (!Program.IsAnyModeSelected())
				{
					Thread.Sleep(100);
				}
				else
				{
					if (Program.toggleMode)
					{
						bool isKeyPressed = false;
						if (Program.useMouse)
						{
							MouseButtons mouseButtons = Program.selectedMouseButton;
							if (mouseButtons <= MouseButtons.Right)
							{
								if (mouseButtons != MouseButtons.Left)
								{
									if (mouseButtons == MouseButtons.Right)
									{
										isKeyPressed = (((int)Program.GetAsyncKeyState(2) & 32768) != 0);
									}
								}
								else
								{
									isKeyPressed = (((int)Program.GetAsyncKeyState(1) & 32768) != 0);
								}
							}
							else if (mouseButtons != MouseButtons.Middle)
							{
								if (mouseButtons != MouseButtons.XButton1)
								{
									if (mouseButtons == MouseButtons.XButton2)
									{
										isKeyPressed = (((int)Program.GetAsyncKeyState(6) & 32768) != 0);
									}
								}
								else
								{
									isKeyPressed = (((int)Program.GetAsyncKeyState(5) & 32768) != 0);
								}
							}
							else
							{
								isKeyPressed = (((int)Program.GetAsyncKeyState(4) & 32768) != 0);
							}
						}
						else if (Program.useKeyboard)
						{
							isKeyPressed = (((int)Program.GetAsyncKeyState((int)Program.selectedKey) & 32768) != 0);
						}
						if (isKeyPressed)
						{
							Thread.Sleep(200);
							Program.isToggledOn = !Program.isToggledOn;
							Program.UpdateStatusDisplay();
							Program.UpdateScrollLockLED();
						}

						if (Program.isToggledOn)
						{
							Program.MainFunction();
						}
					}
					else if (Program.holdToActivate)
					{
						bool isKeyPressed2 = false;
						if (Program.useMouse)
						{
							MouseButtons mouseButtons2 = Program.selectedMouseButton;
							if (mouseButtons2 <= MouseButtons.Right)
							{
								if (mouseButtons2 != MouseButtons.Left)
								{
									if (mouseButtons2 == MouseButtons.Right)
									{
										isKeyPressed2 = (((int)Program.GetAsyncKeyState(2) & 32768) != 0);
									}
								}
								else
								{
									isKeyPressed2 = (((int)Program.GetAsyncKeyState(1) & 32768) != 0);
								}
							}
							else if (mouseButtons2 != MouseButtons.Middle)
							{
								if (mouseButtons2 != MouseButtons.XButton1)
								{
									if (mouseButtons2 == MouseButtons.XButton2)
									{
										isKeyPressed2 = (((int)Program.GetAsyncKeyState(6) & 32768) != 0);
									}
								}
								else
								{
									isKeyPressed2 = (((int)Program.GetAsyncKeyState(5) & 32768) != 0);
								}
							}
							else
							{
								isKeyPressed2 = (((int)Program.GetAsyncKeyState(4) & 32768) != 0);
							}
						}
						else if (Program.useKeyboard)
						{
							isKeyPressed2 = (((int)Program.GetAsyncKeyState((int)Program.selectedKey) & 32768) != 0);
						}
						if (isKeyPressed2)
						{
							Program.MainFunction();
						}
					}
					else if (Program.autoMode)
					{
						Program.MainFunction();
					}
					else
					{
						Program.MainFunction();
					}
					Thread.Sleep(1);
				}
			}
		}

		// Token: 0x06000056 RID: 86
		private static void SetScrollLockLED(bool turnOn)
		{
			try
			{
				if ((Program.GetKeyState(145) & 1) != 0 != turnOn)
				{
					Program.keybd_event(145, 69, 1U, UIntPtr.Zero);
					Program.keybd_event(145, 69, 3U, UIntPtr.Zero);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000057 RID: 87
		private static void UpdateScrollLockLED()
		{
			if (Program.toggleMode && Program.isRunning)
			{
				Program.SetScrollLockLED(Program.isToggledOn);
				return;
			}
			Program.SetScrollLockLED(false);
		}

		// Token: 0x06000058 RID: 88
		private static bool IsAnyModeSelected()
		{
			bool flag = Program.autoMode || Program.toggleMode || Program.holdToActivate;
			bool hasActivationMethod = Program.autoMode || Program.useMouse || Program.useKeyboard;
			return flag && hasActivationMethod;
		}

		// Token: 0x06000059 RID: 89
		private static void ShowSettingsDialog()
		{
			Form form = new Form
			{
				Text = "ตั้งค่าการทำงาน",
				Size = new Size(500, 470),
				StartPosition = FormStartPosition.CenterScreen,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				MaximizeBox = false,
				MinimizeBox = false,
				//BackColor = Color.FromArgb(45, 45, 65),
				BackColor = ColorTranslator.FromHtml("#4682B4"),
				ForeColor = Color.White
			};
			Panel mainPanel = new Panel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				//BackColor = Color.FromArgb(60, 60, 80),
				BackColor = ColorTranslator.FromHtml("#4682B4"),
				Padding = new Padding(10)
			};
			int currentY = 20;
			Label lblResolution = new Label
			{
				Text = "เลือก ขนาดหน้าจอ :",
				AutoSize = false,
				Size = new Size(320, 20),
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleCenter
			};
			int labelTop = currentY;
			currentY = labelTop + lblResolution.Height + 10;
			ComboBox cmbResolution = new ComboBox
			{
				Size = new Size(250, 25),
				DropDownStyle = ComboBoxStyle.DropDownList,
				BackColor = Color.FromArgb(80, 80, 100),
				ForeColor = Color.White,
				Font = new Font("Microsoft Sans Serif", 9f),
				FlatStyle = FlatStyle.Flat,
				Anchor = AnchorStyles.Top
			};
			cmbResolution.DropDownWidth = cmbResolution.Width;
			Action centerModeControls = null;
			Action centerButtonRow = null;
			Action centerResolutionControls = null;
			centerResolutionControls = delegate
			{
				int panelWidth = mainPanel.ClientSize.Width;
				int labelLeft = Math.Max((panelWidth - lblResolution.Width) / 2, 20);
				lblResolution.Location = new Point(labelLeft, labelTop);
				int comboLeft = Math.Max((panelWidth - cmbResolution.Width) / 2, 20);
				cmbResolution.Location = new Point(comboLeft, lblResolution.Bottom + 10);
			};
			centerResolutionControls();
			mainPanel.Resize += delegate(object sender, EventArgs e)
			{
				centerResolutionControls();
				centerModeControls?.Invoke();
				centerButtonRow?.Invoke();
			};
			mainPanel.Layout += delegate(object sender, LayoutEventArgs e)
			{
				centerResolutionControls();
				centerModeControls?.Invoke();
				centerButtonRow?.Invoke();
			};
			currentY = cmbResolution.Bottom + 20;
			CheckBox chkAutoMode = new CheckBox
			{
				Text = "โหมดอัตโนมัติ",
				AutoSize = true,
				Checked = Program.autoMode,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Margin = new Padding(0, 0, 20, 0)
			};
			CheckBox chkToggleMode = new CheckBox
			{
				Text = "โหมดกดปุ่มสลับ เปิด/ปิด",
				AutoSize = true,
				Checked = Program.toggleMode,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Margin = new Padding(0)
			};
			int centerX = mainPanel.ClientSize.Width / 2;
			FlowLayoutPanel activationModeLayout = new FlowLayoutPanel
			{
				FlowDirection = FlowDirection.LeftToRight,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				WrapContents = false,
				Margin = new Padding(0),
				Padding = new Padding(0)
			};
			activationModeLayout.Controls.Add(chkAutoMode);
			activationModeLayout.Controls.Add(chkToggleMode);
			activationModeLayout.PerformLayout();
			Size activationSize = activationModeLayout.PreferredSize;
			activationModeLayout.Location = new Point(Math.Max(centerX - activationSize.Width / 2, 20), currentY);
			currentY = activationModeLayout.Bottom + 16;
			Label lblNewModes = new Label
			{
				Text = "เลือก โหมดการยิง:",
				AutoSize = false,
				Size = new Size(320, 20),
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleCenter
			};
			CheckBox chkAutoShoot = new CheckBox
			{
				Text = "ยิงอัตโนมัติ",
				AutoSize = true,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Margin = new Padding(0, 0, 20, 0)
			};
			CheckBox chkSniperNoScope = new CheckBox
			{
				Text = "ลูกซอง",
				AutoSize = true,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Margin = new Padding(0, 0, 20, 0)
			};
			CheckBox chkSniperScope = new CheckBox
			{
				Text = "สไนเปอร์",
				AutoSize = true,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Margin = new Padding(0, 0, 20, 0)
			};
			CheckBox chkManualMode = new CheckBox
			{
				Text = "ตั้งค่า ดีเลย์ กำหนดเอง",
				AutoSize = true,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Margin = new Padding(0)
			};
			FlowLayoutPanel shootingModeLayout = new FlowLayoutPanel
			{
				FlowDirection = FlowDirection.LeftToRight,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				WrapContents = false,
				Margin = new Padding(0),
				Padding = new Padding(0)
			};
			shootingModeLayout.Controls.Add(chkAutoShoot);
			shootingModeLayout.Controls.Add(chkSniperNoScope);
			shootingModeLayout.Controls.Add(chkSniperScope);
			shootingModeLayout.Controls.Add(chkManualMode);
			shootingModeLayout.PerformLayout();
			Size shootingSize = shootingModeLayout.PreferredSize;
			int settingsLeft = Math.Max(centerX - 150, 20);
			int inputRowHeight = 26;
			int valueLeft = settingsLeft + 160;
			
			CheckBox chkHoldToActivate = new CheckBox
			{
				Text = "โหมดกดค้าง",
				AutoSize = true,
				Checked = Program.holdToActivate,
				Enabled = !Program.autoMode,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Location = new Point(settingsLeft, currentY)
			};
			//currentY += inputRowHeight;
			currentY -= 70;
			CheckBox chkUseMouse = new CheckBox
			{
				Text = "ใช้เมาส์",
				AutoSize = true,
				Checked = Program.useMouse,
				Enabled = !Program.autoMode,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Location = new Point(settingsLeft, currentY)
			};
			ComboBox cmbMouseButton = new ComboBox
			{
				Location = new Point(valueLeft, currentY + 2),
				Size = new Size(180, 20),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Enabled = (Program.useMouse && !Program.autoMode)
			};
			currentY += inputRowHeight;
			CheckBox chkUseKeyboard = new CheckBox
			{
				Text = "ใช้คีย์บอร์ด",
				AutoSize = true,
				Checked = Program.useKeyboard,
				Enabled = !Program.autoMode,
				ForeColor = Color.Black,
				Font = new Font("Microsoft Sans Serif", 9f),
				Location = new Point(settingsLeft, currentY)
			};
			ComboBox cmbKey = new ComboBox
			{
				Location = new Point(valueLeft, currentY + 2),
				Size = new Size(180, 20),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Enabled = (Program.useKeyboard && !Program.autoMode)
			};
			currentY += inputRowHeight;
			Label lblFov = new Label
			{
				Text = "ขนาด FOV:",
				AutoSize = true,
				ForeColor = Color.Black,
				Location = new Point(settingsLeft, currentY)
			};
			NumericUpDown numFov = new NumericUpDown
			{
				Location = new Point(valueLeft, currentY + 2),
				Size = new Size(180, 20),
				Minimum = 0m,
				Maximum = 100m
			};
			currentY += inputRowHeight;
			Label lblScreenHeight = new Label
			{
				Text = "ความสูงหน้าจอ:",
				AutoSize = true,
				ForeColor = Color.Black,
				Location = new Point(settingsLeft, currentY)
			};
			NumericUpDown numScreenHeight = new NumericUpDown
			{
				Location = new Point(valueLeft, currentY + 2),
				Size = new Size(180, 20),
				Minimum = 0m,
				Maximum = 4000m
			};
			currentY += inputRowHeight + 8;
			// ----- วางโหมดการยิงก่อน -----
			lblNewModes.Location = new Point(Math.Max(centerX - lblNewModes.Width / 2, 20), currentY);
			currentY = lblNewModes.Bottom + 12;
			shootingModeLayout.PerformLayout();
			shootingSize = shootingModeLayout.PreferredSize;
			shootingModeLayout.Location = new Point(Math.Max(centerX - shootingSize.Width / 2, 20), currentY);
			//currentY = shootingModeLayout.Bottom + 24;

			currentY += 40;
			// ----- แล้วค่อยวางปุ่ม แก้ไขลำดับการทำงาน -----
			Button btnEditActions = new Button
			{
				Text = "แก้ไข ดีเลย์ การทำงาน",
				Width = mainPanel.ClientSize.Width - 40, // เว้นขอบด้านซ้าย/ขวา
				Height = 40,
				BackColor = Color.FromArgb(45, 45, 65),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			btnEditActions.Location = new Point(20, currentY);
			mainPanel.Resize += (s, e) =>
			{
				btnEditActions.Width = mainPanel.ClientSize.Width - 40;
			};

			currentY = btnEditActions.Bottom - 95;

			int activationTop = activationModeLayout.Location.Y;
			int modesLabelTop = lblNewModes.Location.Y;
			int shootingTop = shootingModeLayout.Location.Y;
			centerModeControls = delegate
			{
				int panelWidth = mainPanel.ClientSize.Width;
				int centerX2 = panelWidth / 2;
				activationModeLayout.PerformLayout();
				Size activationPreferred = activationModeLayout.PreferredSize;
				int activationLeft = Math.Max(centerX2 - activationPreferred.Width / 2, 20);
				activationModeLayout.Location = new Point(activationLeft, activationTop);
				int modesLabelLeft = Math.Max(centerX2 - lblNewModes.Width / 2, 20);
				lblNewModes.Location = new Point(modesLabelLeft, modesLabelTop);
				shootingModeLayout.PerformLayout();
				shootingSize = shootingModeLayout.PreferredSize;
				int shootingLeft = Math.Max(centerX2 - shootingSize.Width / 2, 20);
				shootingModeLayout.Location = new Point(shootingLeft, shootingTop);
			};
			centerModeControls();
			Label lblColorSettings = new Label
			{
				Text = "การตั้งค่าสี:",
				Location = new Point(settingsLeft, currentY),
				Size = new Size(200, 20),
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold),
				ForeColor = Color.Black
			};
			currentY = lblColorSettings.Bottom + 12;
			Label lblRedThreshold = new Label
			{
				Text = "สีแดง (>=) :",
				Location = new Point(settingsLeft, currentY),
				Size = new Size(120, 20),
				ForeColor = Color.Black
			};
			NumericUpDown numRedThreshold = new NumericUpDown
			{
				Location = new Point(valueLeft, currentY),
				Size = new Size(80, 20),
				Minimum = 0m,
				Maximum = 255m,
				Value = Program.redThreshold
			};
			currentY += inputRowHeight;
			Label lblGreenThreshold = new Label
			{
				Text = "สีเขียว (<=) :",
				Location = new Point(settingsLeft, currentY),
				Size = new Size(120, 20),
				ForeColor = Color.LightGreen
			};
			NumericUpDown numGreenThreshold = new NumericUpDown
			{
				Location = new Point(valueLeft, currentY),
				Size = new Size(80, 20),
				Minimum = 0m,
				Maximum = 255m,
				Value = Program.greenThreshold
			};
			currentY += inputRowHeight;
			Label lblBlueThreshold = new Label
			{
				Text = "สีน้ำเงิน (<=) :",
				Location = new Point(settingsLeft, currentY),
				Size = new Size(120, 20),
				ForeColor = Color.LightBlue
			};
			NumericUpDown numBlueThreshold = new NumericUpDown
			{
				Location = new Point(valueLeft, currentY),
				Size = new Size(80, 20),
				Minimum = 0m,
				Maximum = 255m,
				Value = Program.blueThreshold
			};
			currentY += 24;
			int buttonRowTop = currentY;
			int buttonSpacing = 20;
			Button btnOk = new Button
			{
				Text = "ตกลง",
				Height = 40,
				BackColor = Color.FromArgb(76, 175, 80),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				DialogResult = DialogResult.OK,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
			};

			Button btnCancel = new Button
			{
				Text = "ยกเลิก",
				Height = 40,
				BackColor = Color.FromArgb(244, 67, 54),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				DialogResult = DialogResult.Cancel,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
			};

			// ให้ปุ่มยืดเต็มหน้าจอแบบ 50/50 เมื่อ panel resize
			centerButtonRow = delegate
			{
				int spacing = 20; // ระยะห่างระหว่างปุ่ม
				int panelWidth = mainPanel.ClientSize.Width - 40;
				int buttonWidth = (panelWidth - spacing) / 2;

				btnOk.Size = new Size(buttonWidth, 40);
				btnCancel.Size = new Size(buttonWidth, 40);

				btnOk.Location = new Point(20, buttonRowTop);
				btnCancel.Location = new Point(20 + buttonWidth + spacing, buttonRowTop);

			};

			centerButtonRow();
			chkAutoShoot.CheckedChanged += delegate(object s, EventArgs e)
			{
				if (Program.isUpdatingCheckboxes)
				{
					return;
				}
				Program.isUpdatingCheckboxes = true;
				try
				{
					if (chkAutoShoot.Checked)
					{
						chkManualMode.Checked = false;
						chkSniperNoScope.Checked = false;
						chkSniperScope.Checked = false;
						btnEditActions.Enabled = false;
						Program.actionSequence = new List<ActionItem>
						{
							new ActionItem
							{
								Type = ActionItem.ActionType.MouseClick,
								Value = "Left",
								DelayMs = 10
							}
						};
					}
					else
					{
						btnEditActions.Enabled = chkManualMode.Checked;
					}
				}
				finally
				{
					Program.isUpdatingCheckboxes = false;
				}
			};
			chkSniperNoScope.CheckedChanged += delegate(object s, EventArgs e)
			{
				if (Program.isUpdatingCheckboxes)
				{
					return;
				}
				Program.isUpdatingCheckboxes = true;
				try
				{
					if (chkSniperNoScope.Checked)
					{
						chkManualMode.Checked = false;
						chkAutoShoot.Checked = false;
						chkSniperScope.Checked = false;
						btnEditActions.Enabled = false;
						Program.actionSequence = new List<ActionItem>
						{
							new ActionItem
							{
								Type = ActionItem.ActionType.MouseClick,
								Value = "Left",
								DelayMs = 10
							},
							new ActionItem
							{
								Type = ActionItem.ActionType.KeyPress,
								Value = "3",
								DelayMs = 0
							},
							new ActionItem
							{
								Type = ActionItem.ActionType.KeyPress,
								Value = "1",
								DelayMs = 0
							},
							new ActionItem
							{
								Type = ActionItem.ActionType.Delay,
								Value = "Wait",
								DelayMs = 550
							}
						};
					}
					else
					{
						btnEditActions.Enabled = chkManualMode.Checked;
					}
				}
				finally
				{
					Program.isUpdatingCheckboxes = false;
				}
			};
			chkSniperScope.CheckedChanged += delegate(object s, EventArgs e)
			{
				if (Program.isUpdatingCheckboxes)
				{
					return;
				}
				Program.isUpdatingCheckboxes = true;
				try
				{
					if (chkSniperScope.Checked)
					{
						chkManualMode.Checked = false;
						chkAutoShoot.Checked = false;
						chkSniperNoScope.Checked = false;
						btnEditActions.Enabled = false;
						Program.actionSequence = new List<ActionItem>
						{
							new ActionItem
							{
								Type = ActionItem.ActionType.MouseClick,
								Value = "Right",
								DelayMs = 20
							},
							new ActionItem
							{
								Type = ActionItem.ActionType.MouseClick,
								Value = "Left",
								DelayMs = 10
							},
							new ActionItem
							{
								Type = ActionItem.ActionType.KeyPress,
								Value = "3",
								DelayMs = 0
							},
							new ActionItem
							{
								Type = ActionItem.ActionType.KeyPress,
								Value = "1",
								DelayMs = 0
							},
							new ActionItem
							{
								Type = ActionItem.ActionType.Delay,
								Value = "Wait",
								DelayMs = 590
							}
						};
					}
					else
					{
						btnEditActions.Enabled = chkManualMode.Checked;
					}
				}
				finally
				{
					Program.isUpdatingCheckboxes = false;
				}
			};
			chkManualMode.CheckedChanged += delegate(object s, EventArgs e)
			{
				if (Program.isUpdatingCheckboxes)
				{
					return;
				}
				Program.isUpdatingCheckboxes = true;
				try
				{
					if (chkManualMode.Checked)
					{
						chkAutoShoot.Checked = false;
						chkSniperNoScope.Checked = false;
						chkSniperScope.Checked = false;
						btnEditActions.Enabled = true;
					}
					else
					{
						btnEditActions.Enabled = false;
					}
				}
				finally
				{
					Program.isUpdatingCheckboxes = false;
				}
			};
			btnEditActions.Enabled = chkManualMode.Checked;
			if (!chkAutoShoot.Checked && !chkSniperNoScope.Checked && !chkSniperScope.Checked && !chkManualMode.Checked)
			{
				chkManualMode.Checked = true;
				btnEditActions.Enabled = true;
			}
			//ToolTip toolTip = new ToolTip();
			//toolTip.AutomaticDelay = 500;
			//toolTip.AutoPopDelay = 5000;
			//toolTip.InitialDelay = 500;
			//toolTip.ReshowDelay = 100;
			//toolTip.BackColor = Color.FromArgb(60, 60, 80);
			//toolTip.ForeColor = Color.White;
			//toolTip.SetToolTip(lblResolution, "\ud83d\udda5️ ตั้งค่าความละเอียดหน้าจอเกมของคุณ\nต้องตรงกับเกมที่กำลังเล่นอยู่");
			//toolTip.SetToolTip(cmbResolution, "เลือกความละเอียดหน้าจอสำหรับการตรวจจับเป้า\nความละเอียดสูง = ความแม่นยำมากขึ้น");
			//toolTip.SetToolTip(chkAutoMode, "\ud83e\udd16 โหมดอัตโนมัติ - ทำงานโดยไม่ต้องกดปุ่ม\nเหมาะสำหรับการใช้งานต่อเนื่อง");
			//toolTip.SetToolTip(chkToggleMode, "\ud83d\udd18 โหมด Toggle - กดปุ่มเพื่อสลับเปิด/ปิด\nสะดวกสำหรับการใช้งานเป็นช่วงๆ");
			//toolTip.SetToolTip(chkHoldToActivate, "\ud83c\udfae โหมดกดค้าง - ทำงานเฉพาะเมื่อกดปุ่มค้าง\nให้ความควบคุมเต็มที่");
			//toolTip.SetToolTip(chkUseMouse, "\ud83d\uddb1️ ใช้ปุ่มเมาส์เป็นตัวกระตุ้นการทำงาน\nเช่น คลิกซ้าย/ขวา");
			////toolTip.SetToolTip(lblMouseButton, "เลือกปุ่มเมาส์ที่ต้องการใช้\nสำหรับกระตุ้นการทำงาน");
			//toolTip.SetToolTip(cmbMouseButton, "กำหนดปุ่มเมาส์ที่จะใช้ในการเริ่มตรวจจับ");
			//toolTip.SetToolTip(chkUseKeyboard, "⌨️ ใช้ปุ่มคีย์บอร์ดเป็นตัวกระตุ้นการทำงาน\nเช่น ปุ่ม Alt, Shift, Space");
			////toolTip.SetToolTip(lblKey, "เลือกปุ่มคีย์บอร์ดที่ต้องการใช้\nสำหรับกระตุ้นการทำงาน");
			//toolTip.SetToolTip(cmbKey, "กำหนดปุ่มคีย์บอร์ดที่จะใช้ในการเริ่มตรวจจับ");
			//toolTip.SetToolTip(lblFov, "\ud83d\udccf ตั้งค่าขนาด Field of View (FOV)\nพื้นที่ที่โปรแกรมจะสแกนหาพิกเซล\n(ปรับอัตโนมัติตามความละเอียดหน้าจอ)");
			//toolTip.SetToolTip(numFov, string.Format("ปรับขนาดพื้นที่สแกน\nค่าปัจจุบัน: {0}px\nค่ามาก = สแกนบริเวณกว้าง, ค่าน้อย = สแกนเฉพาะจุด", Program.customRegion.Width));
			//toolTip.SetToolTip(lblScreenHeight, "\ud83d\udcd0 ตั้งค่าความสูงของพื้นที่สแกน\nตามความละเอียดหน้าจอของคุณ");
			//toolTip.SetToolTip(numScreenHeight, "ปรับความสูงของพื้นที่ตรวจจับ\nให้เหมาะสมกับความละเอียดหน้าจอ");
			//toolTip.SetToolTip(btnEditActions, "\ud83c\udfac แก้ไข ดีเลย์ การทำงานเมื่อตรวจจับเป้าได้\nใช้งานได้เฉพาะโหมดแมนนวลเท่านั้น");
			//toolTip.SetToolTip(lblColorSettings, "\ud83c\udfa8 ตั้งค่าความไวในการตรวจจับสี\nปรับค่า RGB ตามต้องการ");
			//toolTip.SetToolTip(lblRedThreshold, "\ud83d\udd34 ค่าความแดงขั้นต่ำ\nค่าสูง = ตรวจจับเฉพาะสีแดงสด");
			//toolTip.SetToolTip(numRedThreshold, "ตั้งค่าความแดงขั้นต่ำที่ต้องการตรวจจับ\n0-255 (ค่ามาก = แดงมากขึ้น)");
			//toolTip.SetToolTip(lblGreenThreshold, "\ud83d\udfe2 ค่าความเขียวสูงสุด\nต่ำ = ละเอียดอ่อนต่อสีแดง");
			//toolTip.SetToolTip(numGreenThreshold, "ตั้งค่าความเขียวสูงสุดที่ยอมรับได้\n0-255 (ค่าน้อย = แดงบริสุทธิ์มากขึ้น)");
			//toolTip.SetToolTip(lblBlueThreshold, "\ud83d\udd35 ค่าความน้ำเงินสูงสุด\nต่ำ = ละเอียดอ่อนต่อสีแดง");
			//toolTip.SetToolTip(numBlueThreshold, "ตั้งค่าความน้ำเงินสูงสุดที่ยอมรับได้\n0-255 (ค่าน้อย = แดงบริสุทธิ์มากขึ้น)");
			//toolTip.SetToolTip(btnOk, "✅ บันทึกการตั้งค่าทั้งหมด\nและปิดหน้าต่างการตั้งค่า");
			//toolTip.SetToolTip(btnCancel, "❌ ยกเลิกการเปลี่ยนแปลง\nและการตั้งค่าจะไม่ถูกบันทึก");
			cmbResolution.Items.AddRange(new object[]
			{
				"800x600",
				"1024x768",
				"1280x720",
				"1280x768",
				"1280x1024",
				"1360x720",
				"1360x768",
				"1360x1024",
				"1366x768",
				"1366x1024",
				"1440x900",
				"1440x1080",
				"1600x900",
				"1680x1050 ",
				"1920x1080",
				"1920x1200",
				//"2560x1080",
				//"2560x1440",
				//"3440x1440",
				//"3840x1080",
				//"3840x2160"
			});
			cmbMouseButton.Items.AddRange(new object[]
			{
				"คลิกซ้าย",
				"คลิกขวา",
				"คลิกกลาง",
				"ปุ่มข้าง 1",
				"ปุ่มข้าง 2"
			});
			cmbKey.Items.AddRange(new object[]
			{
				"0",
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
				"A",
				"B",
				"C",
				"D",
				"E",
				"F",
				"G",
				"H",
				"I",
				"J",
				"K",
				"L",
				"M",
				"N",
				"O",
				"P",
				"Q",
				"R",
				"S",
				"T",
				"U",
				"V",
				"W",
				"X",
				"Y",
				"Z",
				"F1",
				"F2",
				"F3",
				"F4",
				"F5",
				"F6",
				"F7",
				"F8",
				"F9",
				"F10",
				"F11",
				"F12",
				"Alt",
				"Space",
				"Shift",
				"Ctrl",
				"Tab",
				"Caps Lock",
				"Escape",
				"Enter",
				"Backspace",
				"Insert",
				"Delete",
				"Home",
				"End",
				"Page Up",
				"Page Down",
				"Left Arrow",
				"Up Arrow",
				"Right Arrow",
				"Down Arrow",
				"Numpad 0",
				"Numpad 1",
				"Numpad 2",
				"Numpad 3",
				"Numpad 4",
				"Numpad 5",
				"Numpad 6",
				"Numpad 7",
				"Numpad 8",
				"Numpad 9",
				"Numpad *",
				"Numpad +",
				"Numpad -",
				"Numpad /",
				"Numpad ."
			});
			bool resolutionFound = false;
			if (!string.IsNullOrEmpty(Program.currentResolution))
			{
				for (int i = 0; i < cmbResolution.Items.Count; i++)
				{
					if (cmbResolution.Items[i].ToString() == Program.currentResolution)
					{
						cmbResolution.SelectedIndex = i;
						resolutionFound = true;
						break;
					}
				}
			}
			if (!resolutionFound && cmbResolution.Items.Count > 0)
			{
				for (int j = 0; j < cmbResolution.Items.Count; j++)
				{
					if (cmbResolution.Items[j].ToString().Contains("1920x1080"))
					{
						cmbResolution.SelectedIndex = j;
						Program.currentResolution = cmbResolution.Items[j].ToString();
						break;
					}
				}
			}
			MouseButtons mouseButtons = Program.selectedMouseButton;
			if (mouseButtons <= MouseButtons.Right)
			{
				if (mouseButtons != MouseButtons.Left)
				{
					if (mouseButtons == MouseButtons.Right)
					{
						cmbMouseButton.SelectedIndex = 1;
					}
				}
				else
				{
					cmbMouseButton.SelectedIndex = 0;
				}
			}
			else if (mouseButtons != MouseButtons.Middle)
			{
				if (mouseButtons != MouseButtons.XButton1)
				{
					if (mouseButtons == MouseButtons.XButton2)
					{
						cmbMouseButton.SelectedIndex = 4;
					}
				}
				else
				{
					cmbMouseButton.SelectedIndex = 3;
				}
			}
			else
			{
				cmbMouseButton.SelectedIndex = 2;
			}
			string currentKeyName = Program.GetKeyName(Program.selectedKey);
			int keyIndex = cmbKey.Items.IndexOf(currentKeyName);
			if (keyIndex >= 0)
			{
				cmbKey.SelectedIndex = keyIndex;
			}
			else if (cmbKey.Items.Count > 0)
			{
				cmbKey.SelectedIndex = 0;
			}
			numFov.Value = Math.Max(numFov.Minimum, Math.Min(Program.customRegion.Width, numFov.Maximum));
			numScreenHeight.Value = Math.Max(numScreenHeight.Minimum, Math.Min(Program.customRegion.Height, numScreenHeight.Maximum));
			if (Program.actionSequence != null && Program.actionSequence.Count > 0)
			{
				if (Program.actionSequence.Count == 1 && Program.actionSequence[0].Type == ActionItem.ActionType.MouseClick && Program.actionSequence[0].Value == "Left")
				{
					chkAutoShoot.Checked = true;
				}
				else if (Program.actionSequence.Count == 4 && Program.actionSequence[0].Type == ActionItem.ActionType.MouseClick && Program.actionSequence[0].Value == "Left" && Program.actionSequence[1].Type == ActionItem.ActionType.KeyPress && Program.actionSequence[1].Value == "3" && Program.actionSequence[2].Type == ActionItem.ActionType.KeyPress && Program.actionSequence[2].Value == "1" && Program.actionSequence[3].Type == ActionItem.ActionType.Delay)
				{
					chkSniperNoScope.Checked = true;
				}
				else if (Program.actionSequence.Count == 5 && Program.actionSequence[0].Type == ActionItem.ActionType.MouseClick && Program.actionSequence[0].Value == "Right" && Program.actionSequence[1].Type == ActionItem.ActionType.MouseClick && Program.actionSequence[1].Value == "Left" && Program.actionSequence[2].Type == ActionItem.ActionType.KeyPress && Program.actionSequence[2].Value == "3" && Program.actionSequence[3].Type == ActionItem.ActionType.KeyPress && Program.actionSequence[3].Value == "1" && Program.actionSequence[4].Type == ActionItem.ActionType.Delay)
				{
					chkSniperScope.Checked = true;
				}
				else
				{
					chkManualMode.Checked = true;
				}
			}
			else
			{
				chkManualMode.Checked = true;
			}
			if (!chkAutoShoot.Checked && !chkSniperNoScope.Checked && !chkSniperScope.Checked && !chkManualMode.Checked)
			{
				chkManualMode.Checked = true;
			}
			else
			{
				chkManualMode.Checked = true;
			}
			if (!chkAutoShoot.Checked && !chkSniperNoScope.Checked && !chkSniperScope.Checked && !chkManualMode.Checked)
			{
				chkManualMode.Checked = true;
			}
			EventHandler resolutionChangedHandler = null;
			resolutionChangedHandler = delegate(object s, EventArgs e)
			{
				object selectedItem3 = cmbResolution.SelectedItem;
				string selectedResolution = (selectedItem3 != null) ? selectedItem3.ToString() : null;
				if (!string.IsNullOrEmpty(selectedResolution))
				{
					try
					{
						string[] parts2 = selectedResolution.Split(new char[]
						{
							' '
						});
						if (parts2.Length >= 2)
						{
							Program.CalculateRegionFromResolution(parts2[1]);
							if (numFov != null && numScreenHeight != null)
							{
								numFov.Value = Math.Max(numFov.Minimum, Math.Min(Program.customRegion.Width, numFov.Maximum));
								numScreenHeight.Value = Math.Max(numScreenHeight.Minimum, Math.Min(Program.customRegion.Height, numScreenHeight.Maximum));
							}
						}
					}
					catch (Exception ex2)
					{
						Console.WriteLine("Error in resolution change: " + ex2.Message);
					}
				}
			};
			cmbResolution.SelectedIndexChanged += resolutionChangedHandler;
			chkAutoMode.CheckedChanged += delegate(object s, EventArgs e)
			{
				bool isAutoMode = chkAutoMode.Checked;
				chkHoldToActivate.Enabled = !isAutoMode;
				chkUseMouse.Enabled = !isAutoMode;
				chkUseKeyboard.Enabled = !isAutoMode;
				//lblMouseButton.Enabled = (chkUseMouse.Checked && !isAutoMode);
				cmbMouseButton.Enabled = (chkUseMouse.Checked && !isAutoMode);
				//lblKey.Enabled = (chkUseKeyboard.Checked && !isAutoMode);
				cmbKey.Enabled = (chkUseKeyboard.Checked && !isAutoMode);
				if (isAutoMode)
				{
					chkHoldToActivate.Checked = false;
				}
			};
			chkUseMouse.CheckedChanged += delegate(object s, EventArgs e)
			{
				//lblMouseButton.Enabled = (chkUseMouse.Checked && !chkAutoMode.Checked);
				cmbMouseButton.Enabled = (chkUseMouse.Checked && !chkAutoMode.Checked);
				if (chkUseMouse.Checked)
				{
					chkUseKeyboard.Checked = false;
				}
			};
			chkUseKeyboard.CheckedChanged += delegate(object s, EventArgs e)
			{
				//lblKey.Enabled = (chkUseKeyboard.Checked && !chkAutoMode.Checked);
				cmbKey.Enabled = (chkUseKeyboard.Checked && !chkAutoMode.Checked);
				if (chkUseKeyboard.Checked)
				{
					chkUseMouse.Checked = false;
				}
			};
			btnEditActions.Click += delegate(object s, EventArgs e)
			{
				Program.ShowActionSequenceDialog(form);
			};
			chkToggleMode.CheckedChanged += delegate(object s, EventArgs e)
			{
				if (chkToggleMode.Checked)
				{
					chkAutoMode.Checked = false;
					chkHoldToActivate.Checked = false;
					chkAutoMode.Enabled = false;
					chkHoldToActivate.Enabled = false;
					chkUseMouse.Enabled = true;
					chkUseKeyboard.Enabled = true;
					return;
				}
				chkAutoMode.Enabled = true;
				chkHoldToActivate.Enabled = !chkAutoMode.Checked;
			};
			chkAutoMode.CheckedChanged += delegate(object s, EventArgs e)
			{
				bool isAutoMode = chkAutoMode.Checked;
				if (isAutoMode)
				{
					chkToggleMode.Checked = false;
				}
				chkToggleMode.Enabled = !isAutoMode;
			};
			chkHoldToActivate.CheckedChanged += delegate(object s, EventArgs e)
			{
				if (chkHoldToActivate.Checked)
				{
					chkToggleMode.Checked = false;
				}
			};
			mainPanel.Controls.AddRange(new Control[]
			{
				lblResolution,
				cmbResolution,
				activationModeLayout,
				//chkHoldToActivate,
				chkUseMouse,
				cmbMouseButton,
				chkUseKeyboard,
				cmbKey,
				//lblFov,
				//numFov,
				//lblScreenHeight,
				//numScreenHeight,
				btnEditActions,
				lblNewModes,
				shootingModeLayout,
				//lblColorSettings,
				//lblRedThreshold,
				//numRedThreshold,
				//lblGreenThreshold,
				//numGreenThreshold,
				//lblBlueThreshold,
				//numBlueThreshold,
				btnOk,
				btnCancel
			});
			form.Controls.Add(mainPanel);
			if (form.ShowDialog() == DialogResult.OK)
			{
				Program.autoMode = chkAutoMode.Checked;
				Program.holdToActivate = chkHoldToActivate.Checked;
				Program.toggleMode = chkToggleMode.Checked;
				Program.useMouse = chkUseMouse.Checked;
				Program.useKeyboard = chkUseKeyboard.Checked;
				object selectedItem = cmbResolution.SelectedItem;
				Program.currentResolution = (((selectedItem != null) ? selectedItem.ToString() : null) ?? "1920x1080");
				Console.WriteLine("Selected resolution: " + Program.currentResolution);
				bool @checked = chkAutoShoot.Checked;
				bool isSniperNoScope = chkSniperNoScope.Checked;
				bool isSniperScope = chkSniperScope.Checked;
				bool isManualMode = chkManualMode.Checked;
				if (@checked)
				{
					Program.currentShootingMode = "AutoShoot";
				}
				else if (isSniperNoScope)
				{
					Program.currentShootingMode = "SniperNoScope";
				}
				else if (isSniperScope)
				{
					Program.currentShootingMode = "SniperScope";
				}
				else if (isManualMode)
				{
					Program.currentShootingMode = "Manual";
				}
				Program.redThreshold = (int)numRedThreshold.Value;
				Program.greenThreshold = (int)numGreenThreshold.Value;
				Program.blueThreshold = (int)numBlueThreshold.Value;
				if (!chkAutoShoot.Checked && !chkSniperNoScope.Checked && !chkSniperScope.Checked && !chkManualMode.Checked)
				{
					MessageBox.Show("กรุณาตั้งค่า โหมดการยิง", "ระบบแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
				string text = cmbMouseButton.SelectedItem.ToString();
				if (!(text == "คลิกซ้าย"))
				{
					if (!(text == "คลิกขวา"))
					{
						if (!(text == "คลิกกลาง"))
						{
							if (!(text == "ปุ่มข้าง 1"))
							{
								if (text == "ปุ่มข้าง 2")
								{
									Program.selectedMouseButton = MouseButtons.XButton2;
								}
							}
							else
							{
								Program.selectedMouseButton = MouseButtons.XButton1;
							}
						}
						else
						{
							Program.selectedMouseButton = MouseButtons.Middle;
						}
					}
					else
					{
						Program.selectedMouseButton = MouseButtons.Right;
					}
				}
				else
				{
					Program.selectedMouseButton = MouseButtons.Left;
				}
				text = cmbKey.SelectedItem.ToString();
				if (text != null)
				{
					switch (text.Length)
					{
					case 1:
						switch (text[0])
						{
						case '0':
							Program.selectedKey = 48;
							break;
						case '1':
							Program.selectedKey = 49;
							break;
						case '2':
							Program.selectedKey = 50;
							break;
						case '3':
							Program.selectedKey = 51;
							break;
						case '4':
							Program.selectedKey = 52;
							break;
						case '5':
							Program.selectedKey = 53;
							break;
						case '6':
							Program.selectedKey = 54;
							break;
						case '7':
							Program.selectedKey = 55;
							break;
						case '8':
							Program.selectedKey = 56;
							break;
						case '9':
							Program.selectedKey = 57;
							break;
						case 'A':
							Program.selectedKey = 65;
							break;
						case 'B':
							Program.selectedKey = 66;
							break;
						case 'C':
							Program.selectedKey = 67;
							break;
						case 'D':
							Program.selectedKey = 68;
							break;
						case 'E':
							Program.selectedKey = 69;
							break;
						case 'F':
							Program.selectedKey = 70;
							break;
						case 'G':
							Program.selectedKey = 71;
							break;
						case 'H':
							Program.selectedKey = 72;
							break;
						case 'I':
							Program.selectedKey = 73;
							break;
						case 'J':
							Program.selectedKey = 74;
							break;
						case 'K':
							Program.selectedKey = 75;
							break;
						case 'L':
							Program.selectedKey = 76;
							break;
						case 'M':
							Program.selectedKey = 77;
							break;
						case 'N':
							Program.selectedKey = 78;
							break;
						case 'O':
							Program.selectedKey = 79;
							break;
						case 'P':
							Program.selectedKey = 80;
							break;
						case 'Q':
							Program.selectedKey = 81;
							break;
						case 'R':
							Program.selectedKey = 82;
							break;
						case 'S':
							Program.selectedKey = 83;
							break;
						case 'T':
							Program.selectedKey = 84;
							break;
						case 'U':
							Program.selectedKey = 85;
							break;
						case 'V':
							Program.selectedKey = 86;
							break;
						case 'W':
							Program.selectedKey = 87;
							break;
						case 'X':
							Program.selectedKey = 88;
							break;
						case 'Y':
							Program.selectedKey = 89;
							break;
						case 'Z':
							Program.selectedKey = 90;
							break;
						}
						break;
					case 2:
						switch (text[1])
						{
						case '1':
							if (text == "F1")
							{
								Program.selectedKey = 112;
							}
							break;
						case '2':
							if (text == "F2")
							{
								Program.selectedKey = 113;
							}
							break;
						case '3':
							if (text == "F3")
							{
								Program.selectedKey = 114;
							}
							break;
						case '4':
							if (text == "F4")
							{
								Program.selectedKey = 115;
							}
							break;
						case '5':
							if (text == "F5")
							{
								Program.selectedKey = 116;
							}
							break;
						case '6':
							if (text == "F6")
							{
								Program.selectedKey = 117;
							}
							break;
						case '7':
							if (text == "F7")
							{
								Program.selectedKey = 118;
							}
							break;
						case '8':
							if (text == "F8")
							{
								Program.selectedKey = 119;
							}
							break;
						case '9':
							if (text == "F9")
							{
								Program.selectedKey = 120;
							}
							break;
						}
						break;
					case 3:
					{
						char c = text[2];
						if (c <= 'b')
						{
							switch (c)
							{
							case '0':
								if (text == "F10")
								{
									Program.selectedKey = 121;
								}
								break;
							case '1':
								if (text == "F11")
								{
									Program.selectedKey = 122;
								}
								break;
							case '2':
								if (text == "F12")
								{
									Program.selectedKey = 123;
								}
								break;
							default:
								if (c == 'b' && text == "Tab")
								{
									Program.selectedKey = 9;
								}
								break;
							}
						}
						else if (c != 'd')
						{
							if (c == 't' && text == "Alt")
							{
								Program.selectedKey = 18;
							}
						}
						else if (text == "End")
						{
							Program.selectedKey = 35;
						}
						break;
					}
					case 4:
					{
						char c2 = text[0];
						if (c2 != 'C')
						{
							if (c2 == 'H' && text == "Home")
							{
								Program.selectedKey = 36;
							}
						}
						else if (text == "Ctrl")
						{
							Program.selectedKey = 17;
						}
						break;
					}
					case 5:
					{
						char c3 = text[1];
						if (c3 != 'h')
						{
							if (c3 != 'n')
							{
								if (c3 == 'p' && text == "Space")
								{
									Program.selectedKey = 32;
								}
							}
							else if (text == "Enter")
							{
								Program.selectedKey = 13;
							}
						}
						else if (text == "Shift")
						{
							Program.selectedKey = 16;
						}
						break;
					}
					case 6:
					{
						char c4 = text[0];
						if (c4 != 'D')
						{
							if (c4 != 'E')
							{
								if (c4 == 'I' && text == "Insert")
								{
									Program.selectedKey = 45;
								}
							}
							else if (text == "Escape")
							{
								Program.selectedKey = 27;
							}
						}
						else if (text == "Delete")
						{
							Program.selectedKey = 46;
						}
						break;
					}
					case 7:
						if (text == "Page Up")
						{
							Program.selectedKey = 33;
						}
						break;
					case 8:
					{
						char c5 = text[7];
						switch (c5)
						{
						case '*':
							if (text == "Numpad *")
							{
								Program.selectedKey = 106;
							}
							break;
						case '+':
							if (text == "Numpad +")
							{
								Program.selectedKey = 107;
							}
							break;
						case ',':
							break;
						case '-':
							if (text == "Numpad -")
							{
								Program.selectedKey = 109;
							}
							break;
						case '.':
							if (text == "Numpad .")
							{
								Program.selectedKey = 110;
							}
							break;
						case '/':
							if (text == "Numpad /")
							{
								Program.selectedKey = 111;
							}
							break;
						case '0':
							if (text == "Numpad 0")
							{
								Program.selectedKey = 96;
							}
							break;
						case '1':
							if (text == "Numpad 1")
							{
								Program.selectedKey = 97;
							}
							break;
						case '2':
							if (text == "Numpad 2")
							{
								Program.selectedKey = 98;
							}
							break;
						case '3':
							if (text == "Numpad 3")
							{
								Program.selectedKey = 99;
							}
							break;
						case '4':
							if (text == "Numpad 4")
							{
								Program.selectedKey = 100;
							}
							break;
						case '5':
							if (text == "Numpad 5")
							{
								Program.selectedKey = 101;
							}
							break;
						case '6':
							if (text == "Numpad 6")
							{
								Program.selectedKey = 102;
							}
							break;
						case '7':
							if (text == "Numpad 7")
							{
								Program.selectedKey = 103;
							}
							break;
						case '8':
							if (text == "Numpad 8")
							{
								Program.selectedKey = 104;
							}
							break;
						case '9':
							if (text == "Numpad 9")
							{
								Program.selectedKey = 105;
							}
							break;
						default:
							if (c5 == 'w' && text == "Up Arrow")
							{
								Program.selectedKey = 38;
							}
							break;
						}
						break;
					}
					case 9:
					{
						char c6 = text[0];
						if (c6 != 'B')
						{
							if (c6 != 'C')
							{
								if (c6 == 'P' && text == "Page Down")
								{
									Program.selectedKey = 34;
								}
							}
							else if (text == "Caps Lock")
							{
								Program.selectedKey = 20;
							}
						}
						else if (text == "Backspace")
						{
							Program.selectedKey = 8;
						}
						break;
					}
					case 10:
					{
						char c7 = text[0];
						if (c7 != 'D')
						{
							if (c7 == 'L' && text == "Left Arrow")
							{
								Program.selectedKey = 37;
							}
						}
						else if (text == "Down Arrow")
						{
							Program.selectedKey = 40;
						}
						break;
					}
					case 11:
						if (text == "Right Arrow")
						{
							Program.selectedKey = 39;
						}
						break;
					}
				}
				object selectedItem2 = cmbResolution.SelectedItem;
				string selectedText = ((selectedItem2 != null) ? selectedItem2.ToString() : null) ?? "";
				if (!string.IsNullOrEmpty(selectedText))
				{
					try
					{
						string[] parts = selectedText.Split(new char[]
						{
							' '
						});
						if (parts.Length >= 2)
						{
							Program.CalculateRegionFromResolution(parts[1]);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error updating region: " + ex.Message);
					}
				}
				Program.customRegion = new Rectangle(Program.customRegion.X, Program.customRegion.Y, (int)numFov.Value, (int)numScreenHeight.Value);
				Program.SaveSettings();
				Program.UpdateStatusDisplay();
			}
			form.FormClosed += delegate(object s, FormClosedEventArgs e)
			{
				if (resolutionChangedHandler != null)
				{
					cmbResolution.SelectedIndexChanged -= resolutionChangedHandler;
				}
			};
		}

		// Token: 0x0600005A RID: 90
		private static void UpdateStatusDisplay()
		{
			Console.WriteLine("Current Resolution in memory: " + Program.currentResolution);

			string statusText = Program.isRunning ? "สถานะ: กำลังทำงาน - " : "สถานะ: หยุดทำงาน - ";

			Control[] foundControls = Program.mainForm.Controls.Find("lblToggleStatus", true);
			Label lblToggleStatusControl = (foundControls.Length != 0) ? (foundControls[0] as Label) : null;

			if (!Program.IsAnyModeSelected())
			{
				statusText += "ยังไม่ได้ตั้งค่า (กรุณาตั้งค่าใน การตั้งค่า)";
				if (lblToggleStatusControl != null)
				{
					lblToggleStatusControl.Text = "สถานะ เปิด / ปิด : ไม่ใช้งาน";
					lblToggleStatusControl.ForeColor = Color.Gray;
				}
			}
			else
			{
				statusText += Program.GetCurrentModeDescription();
				if (Program.toggleMode)
				{
					if (lblToggleStatusControl != null)
					{
						lblToggleStatusControl.Text = "สถานะ  เปิด / ปิด : " + (Program.isToggledOn ? "เปิด 🟢" : "ปิด 🔴");
						lblToggleStatusControl.ForeColor = Program.isToggledOn ? Color.LightGreen : Color.LightCoral;
					}
				}
				else if (lblToggleStatusControl != null)
				{
					lblToggleStatusControl.Text = "สถานะ  เปิด / ปิด : ไม่ใช้งาน";
					lblToggleStatusControl.ForeColor = Color.Gray;
				}
			}

			// อัปเดตข้อความใน lblStatus
			Program.UpdateStatus(statusText);
			Program.UpdateScrollLockLED();

			// ✅ ✅ ✅ ใส่ตรงนี้ ด้านล่างสุด เพื่อควบคุม "สี" ของ lblStatus
			if (!Program.isRunning)
			{
				Program.lblStatus.ForeColor = Color.Red;
			}
			else if (Program.toggleMode && !Program.isToggledOn)
			{
				Program.lblStatus.ForeColor = Color.Red;
			}
			else
			{
				Program.lblStatus.ForeColor = Color.FromArgb(16, 185, 129); // เขียว
			}
		}



		// Token: 0x0600005B RID: 91
		private static void SimulateKeyPress(byte keyCode)
		{
			Program.keybd_event(keyCode, 0, 0U, UIntPtr.Zero);
			Thread.Sleep(10);
			Program.keybd_event(keyCode, 0, 2U, UIntPtr.Zero);
		}

		// Token: 0x0600005C RID: 92
		private static string GetKeyName(byte keyCode)
		{
			switch (keyCode)
			{
			case 8:
				return "Backspace";
			case 9:
				return "Tab";
			case 13:
				return "Enter";
			case 16:
				return "Shift";
			case 17:
				return "Ctrl";
			case 18:
				return "Alt";
			case 20:
				return "Caps Lock";
			case 27:
				return "Escape";
			case 32:
				return "Space";
			case 33:
				return "Page Up";
			case 34:
				return "Page Down";
			case 35:
				return "End";
			case 36:
				return "Home";
			case 37:
				return "Left Arrow";
			case 38:
				return "Up Arrow";
			case 39:
				return "Right Arrow";
			case 40:
				return "Down Arrow";
			case 45:
				return "Insert";
			case 46:
				return "Delete";
			case 48:
				return "0";
			case 49:
				return "1";
			case 50:
				return "2";
			case 51:
				return "3";
			case 52:
				return "4";
			case 53:
				return "5";
			case 54:
				return "6";
			case 55:
				return "7";
			case 56:
				return "8";
			case 57:
				return "9";
			case 65:
				return "A";
			case 66:
				return "B";
			case 67:
				return "C";
			case 68:
				return "D";
			case 69:
				return "E";
			case 70:
				return "F";
			case 71:
				return "G";
			case 72:
				return "H";
			case 73:
				return "I";
			case 74:
				return "J";
			case 75:
				return "K";
			case 76:
				return "L";
			case 77:
				return "M";
			case 78:
				return "N";
			case 79:
				return "O";
			case 80:
				return "P";
			case 81:
				return "Q";
			case 82:
				return "R";
			case 83:
				return "S";
			case 84:
				return "T";
			case 85:
				return "U";
			case 86:
				return "V";
			case 87:
				return "W";
			case 88:
				return "X";
			case 89:
				return "Y";
			case 90:
				return "Z";
			case 96:
				return "Numpad 0";
			case 97:
				return "Numpad 1";
			case 98:
				return "Numpad 2";
			case 99:
				return "Numpad 3";
			case 100:
				return "Numpad 4";
			case 101:
				return "Numpad 5";
			case 102:
				return "Numpad 6";
			case 103:
				return "Numpad 7";
			case 104:
				return "Numpad 8";
			case 105:
				return "Numpad 9";
			case 106:
				return "Numpad *";
			case 107:
				return "Numpad +";
			case 109:
				return "Numpad -";
			case 110:
				return "Numpad .";
			case 111:
				return "Numpad /";
			case 112:
				return "F1";
			case 113:
				return "F2";
			case 114:
				return "F3";
			case 115:
				return "F4";
			case 116:
				return "F5";
			case 117:
				return "F6";
			case 118:
				return "F7";
			case 119:
				return "F8";
			case 120:
				return "F9";
			case 121:
				return "F10";
			case 122:
				return "F11";
			case 123:
				return "F12";
			}
			return "Unknown";
		}

		// Token: 0x0600005D RID: 93 แก้ไขลำดับการทำงาน
		private static void ShowActionSequenceDialog(Form parentForm)
		{
			Form form = new Form
			{
				Text = "แก้ไข ดีเลย์ การทำงาน",
				Size = new Size(540, 400),
				StartPosition = FormStartPosition.CenterScreen,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				BackColor = ColorTranslator.FromHtml("#4682B4"),
				ForeColor = Color.White
			};
			Panel panel = new Panel();
			panel.Dock = DockStyle.Fill;
			panel.BackColor = Color.FromArgb(60, 60, 80);
			panel.Padding = new Padding(10);
			ListView listView = new ListView
			{
				View = View.Details,
				Location = new Point(10, 10),
				Size = new Size(500, 250),
				FullRowSelect = true,
				BackColor = Color.FromArgb(80, 80, 100),
				ForeColor = Color.White
			};
			listView.Columns.Add("ประเภท", 100);
			listView.Columns.Add("ประเภท", 100);
			listView.Columns.Add("ดีเลย์ (ms)", 100);
			foreach (ActionItem action in Program.actionSequence)
			{
				ListViewItem item = new ListViewItem(action.Type.ToString());
				item.SubItems.Add(action.Value);
				item.SubItems.Add(action.DelayMs.ToString());
				item.Tag = action;
				listView.Items.Add(item);
			}
			Button btnAdd = new Button
			{
				Text = "เพิ่ม",
				Location = new Point(10, 270),
				Size = new Size(80, 30),
				BackColor = Color.FromArgb(0, 120, 215),
				ForeColor = Color.White
			};
			Button btnEdit = new Button
			{
				Text = "แก้ไข",
				Location = new Point(100, 270),
				Size = new Size(80, 30),
				BackColor = Color.FromArgb(0, 120, 215),
				ForeColor = Color.White
			};
			Button btnDelete = new Button
			{
				Text = "ลบ",
				Location = new Point(190, 270),
				Size = new Size(80, 30),
				BackColor = Color.Red
			};
			Button btnMoveUp = new Button
			{
				Text = "ย้ายขึ้น",
				Location = new Point(280, 270),
				Size = new Size(80, 30),
				BackColor = Color.Green
			};
			Button btnMoveDown = new Button
			{
				Text = "ย้ายลง",
				Location = new Point(370, 270),
				Size = new Size(80, 30),
				BackColor = Color.Green
			};
			Button btnOk = new Button
			{
				Text = "ตกลง",
				Location = new Point(150, 310),
				Size = new Size(80, 30),
				DialogResult = DialogResult.OK,
				BackColor = Color.FromArgb(76, 175, 80),
				ForeColor = Color.White
			};
			Button btnCancel = new Button
			{
				Text = "ยกเลิก",
				Location = new Point(240, 310),
				Size = new Size(80, 30),
				DialogResult = DialogResult.Cancel,
				BackColor = Color.FromArgb(244, 67, 54),
				ForeColor = Color.White
			};
			//ToolTip toolTip = new ToolTip();
			//toolTip.AutomaticDelay = 500;
			//toolTip.AutoPopDelay = 5000;
			//toolTip.InitialDelay = 500;
			//toolTip.ReshowDelay = 100;
			//toolTip.BackColor = Color.FromArgb(60, 60, 80);
			//toolTip.ForeColor = Color.White;
			//toolTip.SetToolTip(listView, "ลำดับการทำงานเมื่อตรวจจับเป้าได้\nเรียงจากบนลงล่าง");
			//toolTip.SetToolTip(btnAdd, "เพิ่มการกระทำใหม่\nเข้าไปในลำดับการทำงาน");
			//toolTip.SetToolTip(btnEdit, "แก้ไขการกระทำที่เลือก\nเปลี่ยนประเภท, ค่า, หรือความล่าช้า");
			//toolTip.SetToolTip(btnDelete, "ลบการกระทำที่เลือก\nออกจากลำดับการทำงาน");
			//toolTip.SetToolTip(btnMoveUp, "ย้ายการกระทำขึ้น\nเปลี่ยนลำดับการทำงาน");
			//toolTip.SetToolTip(btnMoveDown, "ย้ายการกระทำลง\nเปลี่ยนลำดับการทำงาน");
			//toolTip.SetToolTip(btnOk, "บันทึกลำดับการทำงาน\nและกลับไปยังการตั้งค่าหลัก");
			//toolTip.SetToolTip(btnCancel, "ยกเลิกการเปลี่ยนแปลง\nไม่บันทึกลำดับการทำงาน");
			btnAdd.Click += delegate(object s, EventArgs e)
			{
				Program.AddActionItem(form, listView);
			};
			btnEdit.Click += delegate(object s, EventArgs e)
			{
				Program.EditActionItem(form, listView);
			};
			btnDelete.Click += delegate(object s, EventArgs e)
			{
				if (listView.SelectedItems.Count > 0)
				{
					listView.Items.Remove(listView.SelectedItems[0]);
				}
			};
			btnMoveUp.Click += delegate(object s, EventArgs e)
			{
				if (listView.SelectedItems.Count == 0)
				{
					return;
				}
				int index = listView.SelectedItems[0].Index;
				int newIndex = index - 1;
				if (newIndex >= 0)
				{
					ListViewItem item3 = listView.Items[index];
					listView.Items.RemoveAt(index);
					listView.Items.Insert(newIndex, item3);
					item3.Selected = true;
				}
			};
			btnMoveDown.Click += delegate(object s, EventArgs e)
			{
				if (listView.SelectedItems.Count == 0)
				{
					return;
				}
				int index = listView.SelectedItems[0].Index;
				int newIndex = index + 1;
				if (newIndex < listView.Items.Count)
				{
					ListViewItem item3 = listView.Items[index];
					listView.Items.RemoveAt(index);
					listView.Items.Insert(newIndex, item3);
					item3.Selected = true;
				}
			};

			int btnWidth = 110;
			int btnHeight = 32;
			int bottomY = 300;

			btnEdit.Size = new Size(btnWidth, btnHeight);
			btnOk.Size = new Size(btnWidth, btnHeight);
			btnCancel.Size = new Size(btnWidth, btnHeight);

			btnEdit.Location = new Point(50, bottomY);
			btnOk.Location = new Point(200, bottomY);
			btnCancel.Location = new Point(350, bottomY);

			form.Controls.AddRange(new Control[]
			{
				listView,
				//btnAdd,
				btnEdit,
				//btnDelete,
				//btnMoveUp,
				//btnMoveDown,
				btnOk,
				btnCancel
			});
			if (form.ShowDialog(parentForm) == DialogResult.OK)
			{
				Program.actionSequence = new List<ActionItem>();
				foreach (object obj in listView.Items)
				{
					ListViewItem item2 = (ListViewItem)obj;
					Program.actionSequence.Add((ActionItem)item2.Tag);
				}
				Program.SaveSettings();
			}
		}

		// Token: 0x0600005E RID: 94 เพิ่มการกระทำ
		private static void AddActionItem(Form parent, ListView listView)
		{
			Form form = new Form
			{
				Text = "เพิ่มการกระทำ",
				Size = new Size(300, 200),
				StartPosition = FormStartPosition.CenterParent
			};
			Label lblType = new Label
			{
				Text = "ประเภท:",
				Location = new Point(10, 20),
				Size = new Size(50, 20)
			};
			ComboBox cmbType = new ComboBox
			{
				Location = new Point(70, 18),
				Size = new Size(200, 20),
				DropDownStyle = ComboBoxStyle.DropDownList
			};
			cmbType.Items.AddRange(new object[]
			{
				"MouseClick",
				"KeyPress",
				"Delay"
			});
			cmbType.SelectedIndex = 0;
			Label lblValue = new Label
			{
				Text = "ค่า:",
				Location = new Point(10, 50),
				Size = new Size(50, 20)
			};
			ComboBox cmbValue = new ComboBox
			{
				Location = new Point(70, 48),
				Size = new Size(200, 20),
				DropDownStyle = ComboBoxStyle.DropDownList
			};
			cmbValue.Items.AddRange(new object[]
			{
				"Left",
				"Right",
				"Middle",
				"Side Button 1",
				"Side Button 2"
			});
			cmbValue.SelectedIndex = 0;
			Label lblDelay = new Label
			{
				Text = "ดีเลย์ (ms):",
				Location = new Point(10, 80),
				Size = new Size(70, 20)
			};
			NumericUpDown numDelay = new NumericUpDown
			{
				Location = new Point(90, 78),
				Size = new Size(100, 20),
				Minimum = 0m,
				Maximum = 5000m,
				Value = 10m
			};
			Button btnOk = new Button
			{
				Text = "OK",
				Location = new Point(70, 120),
				Size = new Size(80, 30),
				DialogResult = DialogResult.OK
			};
			Button btnCancel = new Button
			{
				Text = "Cancel",
				Location = new Point(160, 120),
				Size = new Size(80, 30),
				DialogResult = DialogResult.Cancel
			};
			//ToolTip toolTip = new ToolTip();
			//toolTip.SetToolTip(lblType, "เลือกประเภทของการกระทำ\nMouseClick, KeyPress, หรือ Delay");
			//toolTip.SetToolTip(cmbType, "กำหนดประเภทของการกระทำที่จะเพิ่ม");
			//toolTip.SetToolTip(lblValue, "กำหนดค่าสำหรับการกระทำ\nขึ้นอยู่กับประเภทที่เลือก");
			//toolTip.SetToolTip(cmbValue, "เลือกค่าที่ต้องการใช้กับการกระทำ");
			//toolTip.SetToolTip(lblDelay, "ตั้งค่าความล่าช้าหลังจากกระทำ\nหน่วยมิลลิวินาที (ms)");
			//toolTip.SetToolTip(numDelay, "ระยะเวลารอหลังจากการกระทำ\n0-5000 มิลลิวินาที");
			cmbType.SelectedIndexChanged += delegate(object s, EventArgs e)
			{
				cmbValue.Items.Clear();
				string a = cmbType.SelectedItem.ToString();
				if (a == "MouseClick")
				{
					cmbValue.Items.AddRange(new object[]
					{
						"Left",
						"Right",
						"Middle",
						"Side Button 1",
						"Side Button 2"
					});
					cmbValue.SelectedIndex = 0;
					cmbValue.Enabled = true;
					lblDelay.Text = "ดีเลย์ (ms):";
					numDelay.Enabled = true;
					return;
				}
				if (a == "KeyPress")
				{
					cmbValue.Items.AddRange(new object[]
					{
						"0",
						"1",
						"2",
						"3",
						"4",
						"5",
						"6",
						"7",
						"8",
						"9",
						"A",
						"B",
						"C",
						"D",
						"E",
						"F",
						"G",
						"H",
						"I",
						"J",
						"K",
						"L",
						"M",
						"N",
						"O",
						"P",
						"Q",
						"R",
						"S",
						"T",
						"U",
						"V",
						"W",
						"X",
						"Y",
						"Z",
						"F1",
						"F2",
						"F3",
						"F4",
						"F5",
						"F6",
						"F7",
						"F8",
						"F9",
						"F10",
						"F11",
						"F12",
						"Alt",
						"Space",
						"Shift",
						"Ctrl",
						"Tab",
						"Caps Lock",
						"Escape",
						"Enter",
						"Backspace",
						"Insert",
						"Delete",
						"Home",
						"End",
						"Page Up",
						"Page Down",
						"Left Arrow",
						"Up Arrow",
						"Right Arrow",
						"Down Arrow",
						"Numpad 0",
						"Numpad 1",
						"Numpad 2",
						"Numpad 3",
						"Numpad 4",
						"Numpad 5",
						"Numpad 6",
						"Numpad 7",
						"Numpad 8",
						"Numpad 9",
						"Numpad *",
						"Numpad +",
						"Numpad -",
						"Numpad /",
						"Numpad ."
					});
					cmbValue.SelectedIndex = 0;
					cmbValue.Enabled = true;
					lblDelay.Text = "ดีเลย์ (ms):";
					numDelay.Enabled = true;
					return;
				}
				if (!(a == "Delay"))
				{
					return;
				}
				cmbValue.Items.Add("Wait");
				cmbValue.SelectedIndex = 0;
				cmbValue.Enabled = false;
				lblDelay.Text = "Duration (ms):";
				numDelay.Enabled = true;
			};
			form.Controls.AddRange(new Control[]
			{
				lblType,
				cmbType,
				lblValue,
				cmbValue,
				lblDelay,
				numDelay,
				btnOk,
				btnCancel
			});
			if (form.ShowDialog(parent) == DialogResult.OK)
			{
				ActionItem actionItem = new ActionItem();
				Type typeFromHandle = typeof(ActionItem.ActionType);
				object selectedItem = cmbType.SelectedItem;
				actionItem.Type = (ActionItem.ActionType)Enum.Parse(typeFromHandle, ((selectedItem != null) ? selectedItem.ToString() : null) ?? "Delay");
				object selectedItem2 = cmbValue.SelectedItem;
				actionItem.Value = (((selectedItem2 != null) ? selectedItem2.ToString() : null) ?? "");
				actionItem.DelayMs = (int)numDelay.Value;
				ActionItem action = actionItem;
				ListViewItem item = new ListViewItem(action.Type.ToString());
				item.SubItems.Add(action.Value);
				item.SubItems.Add(action.DelayMs.ToString());
				item.Tag = action;
				listView.Items.Add(item);
			}
		}

		// Token: 0x0600005F RID: 95 แก้ไขดีเลย์
		private static void EditActionItem(Form parent, ListView listView)
		{
			if (listView.SelectedItems.Count == 0)
			{
				return;
			}
			ListViewItem selectedItem = listView.SelectedItems[0];
			ActionItem action = (ActionItem)selectedItem.Tag;
			Form form = new Form
			{
				Text = "แก้ไข ดีเลย์",
				Size = new Size(300, 200),
				StartPosition = FormStartPosition.CenterParent,
				BackColor = ColorTranslator.FromHtml("#4682B4"),
				ShowIcon = false, // ✅ ซ่อนไอคอน
				FormBorderStyle = FormBorderStyle.FixedDialog, // ✅ ทำให้ไม่มีไอคอน
				MaximizeBox = false // ไม่ให้ขยายหน้าต่าง
			};
			Label lblType = new Label
			{
				Text = "ประเภท:",
				Location = new Point(10, 20),
				Size = new Size(50, 20)
			};
			ComboBox cmbType = new ComboBox
			{
				Location = new Point(70, 18),
				Size = new Size(200, 20),
				DropDownStyle = ComboBoxStyle.DropDownList
			};
			cmbType.Items.AddRange(new object[]
			{
				"MouseClick",
				"KeyPress",
				"Delay"
			});
			cmbType.SelectedItem = action.Type.ToString();
			Label lblValue = new Label
			{
				Text = "ประเภท:",
				Location = new Point(10, 50),
				Size = new Size(50, 20)
			};
			ComboBox cmbValue = new ComboBox
			{
				Location = new Point(70, 48),
				Size = new Size(200, 20),
				DropDownStyle = ComboBoxStyle.DropDownList
			};
			cmbType.SelectedIndexChanged += delegate(object s, EventArgs e)
			{
				cmbValue.Items.Clear();
				string a = cmbType.SelectedItem.ToString();
				if (a == "MouseClick")
				{
					cmbValue.Items.AddRange(new object[]
					{
						"Left",
						"Right",
						"Middle",
						"Side Button 1",
						"Side Button 2"
					});
					cmbValue.SelectedIndex = 0;
					cmbValue.Enabled = true;
					return;
				}
				if (a == "KeyPress")
				{
					cmbValue.Items.AddRange(new object[]
					{
						"0",
						"1",
						"2",
						"3",
						"4",
						"5",
						"6",
						"7",
						"8",
						"9",
						"A",
						"B",
						"C",
						"D",
						"E",
						"F",
						"G",
						"H",
						"I",
						"J",
						"K",
						"L",
						"M",
						"N",
						"O",
						"P",
						"Q",
						"R",
						"S",
						"T",
						"U",
						"V",
						"W",
						"X",
						"Y",
						"Z",
						"F1",
						"F2",
						"F3",
						"F4",
						"F5",
						"F6",
						"F7",
						"F8",
						"F9",
						"F10",
						"F11",
						"F12",
						"Alt",
						"Space",
						"Shift",
						"Ctrl",
						"Tab",
						"Caps Lock",
						"Escape",
						"Enter",
						"Backspace",
						"Insert",
						"Delete",
						"Home",
						"End",
						"Page Up",
						"Page Down",
						"Left Arrow",
						"Up Arrow",
						"Right Arrow",
						"Down Arrow",
						"Numpad 0",
						"Numpad 1",
						"Numpad 2",
						"Numpad 3",
						"Numpad 4",
						"Numpad 5",
						"Numpad 6",
						"Numpad 7",
						"Numpad 8",
						"Numpad 9",
						"Numpad *",
						"Numpad +",
						"Numpad -",
						"Numpad /",
						"Numpad ."
					});
					cmbValue.SelectedIndex = 0;
					cmbValue.Enabled = true;
					return;
				}
				if (!(a == "Delay"))
				{
					return;
				}
				cmbValue.Items.Add("Wait");
				cmbValue.SelectedIndex = 0;
				cmbValue.Enabled = false;
			};
			switch (action.Type)
			{
			case ActionItem.ActionType.MouseClick:
				cmbValue.Items.AddRange(new object[]
				{
					"Left",
					"Right",
					"Middle",
					"Side Button 1",
					"Side Button 2"
				});
				cmbValue.Enabled = true;
				break;
			case ActionItem.ActionType.KeyPress:
				cmbValue.Items.AddRange(new object[]
				{
					"1",
					"2",
					"3",
					"4",
					"5",
					"Alt",
					"Space",
					"Shift",
					"Ctrl"
				});
				cmbValue.Enabled = true;
				break;
			case ActionItem.ActionType.Delay:
				cmbValue.Items.Add("Wait");
				cmbValue.Enabled = false;
				break;
			}
			cmbValue.SelectedItem = action.Value;
			//Label lblDelay = new Label
			//{
			//	Text = "ดีเลย์ (ms):",
			//	Location = new Point(10, 80),
			//	Size = new Size(70, 20)
			//};
			//NumericUpDown numDelay = new NumericUpDown
			//{
			//	Location = new Point(90, 78),
			//	Size = new Size(100, 20),
			//	Minimum = 0m,
			//	Maximum = 5000m,
			//	Value = action.DelayMs
			//};

			int centerX = (form.ClientSize.Width / 2);

			Label lblDelay = new Label
			{
				Text = "ดีเลย์ (ms):",
				AutoSize = true
			};
			lblDelay.Location = new Point(centerX - (lblDelay.PreferredWidth + 100) / 2, 60);

			NumericUpDown numDelay = new NumericUpDown
			{
				Size = new Size(100, 20),
				Minimum = 0m,
				Maximum = 5000m,
				Value = action.DelayMs
			};
			numDelay.Location = new Point(centerX - numDelay.Width / 2, 80);

			Button btnOk = new Button
			{
				Text = "ตกลง",
				Location = new Point(70, 120),
				Size = new Size(80, 30),
				DialogResult = DialogResult.OK,
				BackColor = Color.Green
			};
			Button btnCancel = new Button
			{
				Text = "ยกเลิก",
				Location = new Point(160, 120),
				Size = new Size(80, 30),
				DialogResult = DialogResult.Cancel,
				BackColor = Color.Red
			};
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(lblType, "เลือกประเภทของการกระทำ\nMouseClick, KeyPress, หรือ Delay");
			toolTip.SetToolTip(cmbType, "กำหนดประเภทของการกระทำที่จะเพิ่ม");
			toolTip.SetToolTip(lblValue, "กำหนดค่าสำหรับการกระทำ\nขึ้นอยู่กับประเภทที่เลือก");
			toolTip.SetToolTip(cmbValue, "เลือกค่าที่ต้องการใช้กับการกระทำ");
			toolTip.SetToolTip(lblDelay, "ตั้งค่าความล่าช้าหลังจากกระทำ\nหน่วยมิลลิวินาที (ms)");
			toolTip.SetToolTip(numDelay, "ระยะเวลารอหลังจากการกระทำ\n0-5000 มิลลิวินาที");
			form.Controls.AddRange(new Control[]
			{
				//lblType,
				//cmbType,
				//lblValue,
				//cmbValue,
				lblDelay,
				numDelay,
				btnOk,
				btnCancel
			});

			form.Shown += (s, e2) =>
			{
				// center delay label
				lblDelay.Location = new Point(
					(form.ClientSize.Width - lblDelay.Width) / 2,
					60
				);

				// center numeric
				numDelay.Location = new Point(
					(form.ClientSize.Width - numDelay.Width) / 2,
					80
				);

				// center buttons
				int btnWidth = 90;
				btnOk.Size = new Size(btnWidth, 30);
				btnCancel.Size = new Size(btnWidth, 30);

				btnOk.Location = new Point(
					(form.ClientSize.Width / 2) - btnWidth - 5,
					120
				);

				btnCancel.Location = new Point(
					(form.ClientSize.Width / 2) + 5,
					120
				);
			};
			if (form.ShowDialog(parent) == DialogResult.OK)
			{
				ActionItem actionItem = action;
				Type typeFromHandle = typeof(ActionItem.ActionType);
				object selectedItem2 = cmbType.SelectedItem;
				actionItem.Type = (ActionItem.ActionType)Enum.Parse(typeFromHandle, ((selectedItem2 != null) ? selectedItem2.ToString() : null) ?? "Delay");
				ActionItem actionItem2 = action;
				object selectedItem3 = cmbValue.SelectedItem;
				actionItem2.Value = (((selectedItem3 != null) ? selectedItem3.ToString() : null) ?? "");
				action.DelayMs = (int)numDelay.Value;
				selectedItem.Text = action.Type.ToString();
				selectedItem.SubItems[1].Text = action.Value;
				selectedItem.SubItems[2].Text = action.DelayMs.ToString();
			}
		}

		// Token: 0x06000060 RID: 96
		private static void CalculateRegionFromResolution(string resolution)
		{
			try
			{
				string[] parts = resolution.Split(new char[]
				{
					'x'
				});
				if (parts.Length >= 2)
				{
					string widthStr = parts[0];
					string heightStr = parts[1].Split(new char[]
					{
						' '
					})[0].Split(new char[]
					{
						'('
					})[0];
					int screenWidth;
					int screenHeight;
					if (int.TryParse(widthStr, out screenWidth) && int.TryParse(heightStr, out screenHeight))
					{
						int regionX = (int)((double)screenWidth * 0.5);
						int regionY = (int)((double)screenHeight * 0.2);
						int regionWidth;
						if (screenWidth >= 3840)
						{
							regionWidth = 40;
						}
						else if (screenWidth >= 2560)
						{
							regionWidth = 30;
						}
						else if (screenWidth >= 1920)
						{
							regionWidth = 25;
						}
						else if (screenWidth >= 1366)
						{
							regionWidth = 20;
						}
						else
						{
							regionWidth = 15;
						}
						int regionHeight;
						if (screenHeight >= 2160)
						{
							regionHeight = (int)((double)screenHeight * 0.6);
						}
						else if (screenHeight >= 1440)
						{
							regionHeight = (int)((double)screenHeight * 0.55);
						}
						else
						{
							regionHeight = (int)((double)screenHeight * 0.5);
						}
						regionWidth = Math.Min(regionWidth, 100);
						regionHeight = Math.Min(regionHeight, 4000);
						Program.customRegion = new Rectangle(regionX, regionY, regionWidth, regionHeight);
						Console.WriteLine(string.Format("ตั้งค่า region: {0} สำหรับความละเอียด {1}x{2}", Program.customRegion, screenWidth, screenHeight));
					}
					else
					{
						Console.WriteLine("ไม่สามารถ parse resolution: " + resolution);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in CalculateRegionFromResolution: " + ex.Message);
			}
		}

		// Token: 0x06000061 RID: 97
		private static void MainFunction()
		{
			IntPtr hwnd = Program.FindGameWindow();
			if (hwnd == IntPtr.Zero)
			{
				return;
			}
			Rectangle windowRect = Program.GetWindowRect(hwnd);
			if (windowRect == Rectangle.Empty)
			{
				return;
			}
			using (Bitmap screenshot = Program.CaptureScreen(new Rectangle(windowRect.Left + Program.customRegion.Left, windowRect.Top + Program.customRegion.Top, Program.customRegion.Width, Program.customRegion.Height)))
			{
				if (screenshot != null && Program.DetectRedPixelBgr(screenshot))
				{
					foreach (ActionItem action in Program.actionSequence)
					{
						switch (action.Type)
						{
						case ActionItem.ActionType.MouseClick:
						{
							uint downFlag = 0U;
							uint upFlag = 0U;
							uint mouseData = 0U;
							string text = action.Value.ToLower();
							if (!(text == "left"))
							{
								if (!(text == "right"))
								{
									if (!(text == "middle"))
									{
										if (!(text == "side button 1"))
										{
											if (text == "side button 2")
											{
												downFlag = 128U;
												upFlag = 256U;
												mouseData = 2U;
											}
										}
										else
										{
											downFlag = 128U;
											upFlag = 256U;
											mouseData = 1U;
										}
									}
									else
									{
										downFlag = 32U;
										upFlag = 64U;
									}
								}
								else
								{
									downFlag = 8U;
									upFlag = 16U;
								}
							}
							else
							{
								downFlag = 2U;
								upFlag = 4U;
							}
							if (downFlag != 0U && upFlag != 0U)
							{
								Program.mouse_event(downFlag, 0, 0, mouseData, UIntPtr.Zero);
								Thread.Sleep(action.DelayMs);
								Program.mouse_event(upFlag, 0, 0, mouseData, UIntPtr.Zero);
							}
							break;
						}
						case ActionItem.ActionType.KeyPress:
						{
							byte keyCode = 0;
							string text2 = action.Value;
							if (text2 != null)
							{
								switch (text2.Length)
								{
								case 1:
									switch (text2[0])
									{
									case '0':
										keyCode = 48;
										break;
									case '1':
										keyCode = 49;
										break;
									case '2':
										keyCode = 50;
										break;
									case '3':
										keyCode = 51;
										break;
									case '4':
										keyCode = 52;
										break;
									case '5':
										keyCode = 53;
										break;
									case '6':
										keyCode = 54;
										break;
									case '7':
										keyCode = 55;
										break;
									case '8':
										keyCode = 56;
										break;
									case '9':
										keyCode = 57;
										break;
									case 'A':
										keyCode = 65;
										break;
									case 'B':
										keyCode = 66;
										break;
									case 'C':
										keyCode = 67;
										break;
									case 'D':
										keyCode = 68;
										break;
									case 'E':
										keyCode = 69;
										break;
									case 'F':
										keyCode = 70;
										break;
									case 'G':
										keyCode = 71;
										break;
									case 'H':
										keyCode = 72;
										break;
									case 'I':
										keyCode = 73;
										break;
									case 'J':
										keyCode = 74;
										break;
									case 'K':
										keyCode = 75;
										break;
									case 'L':
										keyCode = 76;
										break;
									case 'M':
										keyCode = 77;
										break;
									case 'N':
										keyCode = 78;
										break;
									case 'O':
										keyCode = 79;
										break;
									case 'P':
										keyCode = 80;
										break;
									case 'Q':
										keyCode = 81;
										break;
									case 'R':
										keyCode = 82;
										break;
									case 'S':
										keyCode = 83;
										break;
									case 'T':
										keyCode = 84;
										break;
									case 'U':
										keyCode = 85;
										break;
									case 'V':
										keyCode = 86;
										break;
									case 'W':
										keyCode = 87;
										break;
									case 'X':
										keyCode = 88;
										break;
									case 'Y':
										keyCode = 89;
										break;
									case 'Z':
										keyCode = 90;
										break;
									}
									break;
								case 2:
									switch (text2[1])
									{
									case '1':
										if (text2 == "F1")
										{
											keyCode = 112;
										}
										break;
									case '2':
										if (text2 == "F2")
										{
											keyCode = 113;
										}
										break;
									case '3':
										if (text2 == "F3")
										{
											keyCode = 114;
										}
										break;
									case '4':
										if (text2 == "F4")
										{
											keyCode = 115;
										}
										break;
									case '5':
										if (text2 == "F5")
										{
											keyCode = 116;
										}
										break;
									case '6':
										if (text2 == "F6")
										{
											keyCode = 117;
										}
										break;
									case '7':
										if (text2 == "F7")
										{
											keyCode = 118;
										}
										break;
									case '8':
										if (text2 == "F8")
										{
											keyCode = 119;
										}
										break;
									case '9':
										if (text2 == "F9")
										{
											keyCode = 120;
										}
										break;
									}
									break;
								case 3:
								{
									char c = text2[2];
									if (c <= 'b')
									{
										switch (c)
										{
										case '0':
											if (text2 == "F10")
											{
												keyCode = 121;
											}
											break;
										case '1':
											if (text2 == "F11")
											{
												keyCode = 122;
											}
											break;
										case '2':
											if (text2 == "F12")
											{
												keyCode = 123;
											}
											break;
										default:
											if (c == 'b' && text2 == "Tab")
											{
												keyCode = 9;
											}
											break;
										}
									}
									else if (c != 'd')
									{
										if (c == 't' && text2 == "Alt")
										{
											keyCode = 18;
										}
									}
									else if (text2 == "End")
									{
										keyCode = 35;
									}
									break;
								}
								case 4:
								{
									char c2 = text2[0];
									if (c2 != 'C')
									{
										if (c2 == 'H' && text2 == "Home")
										{
											keyCode = 36;
										}
									}
									else if (text2 == "Ctrl")
									{
										keyCode = 17;
									}
									break;
								}
								case 5:
								{
									char c3 = text2[1];
									if (c3 != 'h')
									{
										if (c3 != 'n')
										{
											if (c3 == 'p' && text2 == "Space")
											{
												keyCode = 32;
											}
										}
										else if (text2 == "Enter")
										{
											keyCode = 13;
										}
									}
									else if (text2 == "Shift")
									{
										keyCode = 16;
									}
									break;
								}
								case 6:
								{
									char c4 = text2[0];
									if (c4 != 'D')
									{
										if (c4 != 'E')
										{
											if (c4 == 'I' && text2 == "Insert")
											{
												keyCode = 45;
											}
										}
										else if (text2 == "Escape")
										{
											keyCode = 27;
										}
									}
									else if (text2 == "Delete")
									{
										keyCode = 46;
									}
									break;
								}
								case 7:
									if (text2 == "Page Up")
									{
										keyCode = 33;
									}
									break;
								case 8:
								{
									char c5 = text2[7];
									switch (c5)
									{
									case '*':
										if (text2 == "Numpad *")
										{
											keyCode = 106;
										}
										break;
									case '+':
										if (text2 == "Numpad +")
										{
											keyCode = 107;
										}
										break;
									case ',':
										break;
									case '-':
										if (text2 == "Numpad -")
										{
											keyCode = 109;
										}
										break;
									case '.':
										if (text2 == "Numpad .")
										{
											keyCode = 110;
										}
										break;
									case '/':
										if (text2 == "Numpad /")
										{
											keyCode = 111;
										}
										break;
									case '0':
										if (text2 == "Numpad 0")
										{
											keyCode = 96;
										}
										break;
									case '1':
										if (text2 == "Numpad 1")
										{
											keyCode = 97;
										}
										break;
									case '2':
										if (text2 == "Numpad 2")
										{
											keyCode = 98;
										}
										break;
									case '3':
										if (text2 == "Numpad 3")
										{
											keyCode = 99;
										}
										break;
									case '4':
										if (text2 == "Numpad 4")
										{
											keyCode = 100;
										}
										break;
									case '5':
										if (text2 == "Numpad 5")
										{
											keyCode = 101;
										}
										break;
									case '6':
										if (text2 == "Numpad 6")
										{
											keyCode = 102;
										}
										break;
									case '7':
										if (text2 == "Numpad 7")
										{
											keyCode = 103;
										}
										break;
									case '8':
										if (text2 == "Numpad 8")
										{
											keyCode = 104;
										}
										break;
									case '9':
										if (text2 == "Numpad 9")
										{
											keyCode = 105;
										}
										break;
									default:
										if (c5 == 'w' && text2 == "Up Arrow")
										{
											keyCode = 38;
										}
										break;
									}
									break;
								}
								case 9:
								{
									char c6 = text2[0];
									if (c6 != 'B')
									{
										if (c6 != 'C')
										{
											if (c6 == 'P' && text2 == "Page Down")
											{
												keyCode = 34;
											}
										}
										else if (text2 == "Caps Lock")
										{
											keyCode = 20;
										}
									}
									else if (text2 == "Backspace")
									{
										keyCode = 8;
									}
									break;
								}
								case 10:
								{
									char c7 = text2[0];
									if (c7 != 'D')
									{
										if (c7 == 'L' && text2 == "Left Arrow")
										{
											keyCode = 37;
										}
									}
									else if (text2 == "Down Arrow")
									{
										keyCode = 40;
									}
									break;
								}
								case 11:
									if (text2 == "Right Arrow")
									{
										keyCode = 39;
									}
									break;
								}
							}
							if (keyCode != 0)
							{
								Program.keybd_event(keyCode, 0, 0U, UIntPtr.Zero);
								Thread.Sleep(10);
								Program.keybd_event(keyCode, 0, 2U, UIntPtr.Zero);
							}
							break;
						}
						case ActionItem.ActionType.Delay:
							Thread.Sleep(action.DelayMs);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000062 RID: 98
		private static void cmbType_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		// Token: 0x06000063 RID: 99
		private static IntPtr FindGameWindow()
		{
			IntPtr hwnd = Program.FindWindow("PBApp", null);
			if (hwnd == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}
			return hwnd;
		}

		// Token: 0x06000064 RID: 100
		private static Rectangle GetWindowRect(IntPtr hwnd)
		{
			if (hwnd == IntPtr.Zero)
			{
				return Rectangle.Empty;
			}
			Program.RECT rect;
			Program.GetWindowRect(hwnd, out rect);
			return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
		}

		// Token: 0x06000065 RID: 101
		private static Bitmap CaptureScreen(Rectangle region)
		{
			Bitmap result;
			try
			{
				Bitmap bitmap = new Bitmap(region.Width, region.Height);
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.CopyFromScreen(region.Left, region.Top, 0, 0, bitmap.Size);
				}
				result = bitmap;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000066 RID: 102
		private static bool DetectRedPixelBgr(Bitmap image)
		{
			if (image == null)
			{
				return false;
			}
			bool result;
			try
			{
				for (int y = 0; y < image.Height; y++)
				{
					for (int x = 0; x < image.Width; x++)
					{
						Color pixel = image.GetPixel(x, y);
						if ((int)pixel.B <= Program.blueThreshold && (int)pixel.G <= Program.greenThreshold && (int)pixel.R >= Program.redThreshold)
						{
							return true;
						}
					}
				}
				result = false;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04000021 RID: 33
		private static bool isRunning = false;

		// Token: 0x04000022 RID: 34
		private static bool useMouse = false;

		// Token: 0x04000023 RID: 35
		private static bool useKeyboard = false;

		// Token: 0x04000024 RID: 36
		private static bool autoMode = false;

		// Token: 0x04000025 RID: 37
		private static bool holdToActivate = false;

		// Token: 0x04000026 RID: 38
		private static bool toggleMode = false;

		// Token: 0x04000027 RID: 39
		private static bool isToggledOn = false;

		// Token: 0x04000028 RID: 40
		private static bool isUpdatingCheckboxes = false;

		// Token: 0x04000029 RID: 41
		private static MouseButtons selectedMouseButton = MouseButtons.Right;

		// Token: 0x0400002A RID: 42
		private static byte selectedKey = 18;

		// Token: 0x0400002B RID: 43
		private static List<ActionItem> actionSequence = new List<ActionItem>();

		// Token: 0x0400002C RID: 44
		private static string currentShootingMode = "Manual";

		// Token: 0x0400002D RID: 45
		private static string currentResolution = "1920x1080 (Full HD)";

		// Token: 0x0400002E RID: 46
		private static Rectangle customRegion = new Rectangle(800, 0, 20, 460);

		// Token: 0x0400002F RID: 47
		private static Label lblCountdown;
		private static Label lblCountdown1;

		// Token: 0x04000030 RID: 48
		private static System.Windows.Forms.Timer countdownTimer;

		// Token: 0x04000031 RID: 49
		private static int redThreshold = 200;

		// Token: 0x04000032 RID: 50
		private static int greenThreshold = 50;

		// Token: 0x04000033 RID: 51
		private static int blueThreshold = 50;

		// Token: 0x04000034 RID: 52
		private static Label lblToggleStatus;

		// Token: 0x04000035 RID: 53
		private static Label lblInfo;

		// Token: 0x04000036 RID: 54
		private const byte VK_0 = 48;

		// Token: 0x04000037 RID: 55
		private const byte VK_1 = 49;

		// Token: 0x04000038 RID: 56
		private const byte VK_2 = 50;

		// Token: 0x04000039 RID: 57
		private const byte VK_3 = 51;

		// Token: 0x0400003A RID: 58
		private const byte VK_4 = 52;

		// Token: 0x0400003B RID: 59
		private const byte VK_5 = 53;

		// Token: 0x0400003C RID: 60
		private const byte VK_6 = 54;

		// Token: 0x0400003D RID: 61
		private const byte VK_7 = 55;

		// Token: 0x0400003E RID: 62
		private const byte VK_8 = 56;

		// Token: 0x0400003F RID: 63
		private const byte VK_9 = 57;

		// Token: 0x04000040 RID: 64
		private const byte VK_A = 65;

		// Token: 0x04000041 RID: 65
		private const byte VK_B = 66;

		// Token: 0x04000042 RID: 66
		private const byte VK_C = 67;

		// Token: 0x04000043 RID: 67
		private const byte VK_D = 68;

		// Token: 0x04000044 RID: 68
		private const byte VK_E = 69;

		// Token: 0x04000045 RID: 69
		private const byte VK_F = 70;

		// Token: 0x04000046 RID: 70
		private const byte VK_G = 71;

		// Token: 0x04000047 RID: 71
		private const byte VK_H = 72;

		// Token: 0x04000048 RID: 72
		private const byte VK_I = 73;

		// Token: 0x04000049 RID: 73
		private const byte VK_J = 74;

		// Token: 0x0400004A RID: 74
		private const byte VK_K = 75;

		// Token: 0x0400004B RID: 75
		private const byte VK_L = 76;

		// Token: 0x0400004C RID: 76
		private const byte VK_M = 77;

		// Token: 0x0400004D RID: 77
		private const byte VK_N = 78;

		// Token: 0x0400004E RID: 78
		private const byte VK_O = 79;

		// Token: 0x0400004F RID: 79
		private const byte VK_P = 80;

		// Token: 0x04000050 RID: 80
		private const byte VK_Q = 81;

		// Token: 0x04000051 RID: 81
		private const byte VK_R = 82;

		// Token: 0x04000052 RID: 82
		private const byte VK_S = 83;

		// Token: 0x04000053 RID: 83
		private const byte VK_T = 84;

		// Token: 0x04000054 RID: 84
		private const byte VK_U = 85;

		// Token: 0x04000055 RID: 85
		private const byte VK_V = 86;

		// Token: 0x04000056 RID: 86
		private const byte VK_W = 87;

		// Token: 0x04000057 RID: 87
		private const byte VK_X = 88;

		// Token: 0x04000058 RID: 88
		private const byte VK_Y = 89;

		// Token: 0x04000059 RID: 89
		private const byte VK_Z = 90;

		// Token: 0x0400005A RID: 90
		private const byte VK_F1 = 112;

		// Token: 0x0400005B RID: 91
		private const byte VK_F2 = 113;

		// Token: 0x0400005C RID: 92
		private const byte VK_F3 = 114;

		// Token: 0x0400005D RID: 93
		private const byte VK_F4 = 115;

		// Token: 0x0400005E RID: 94
		private const byte VK_F5 = 116;

		// Token: 0x0400005F RID: 95
		private const byte VK_F6 = 117;

		// Token: 0x04000060 RID: 96
		private const byte VK_F7 = 118;

		// Token: 0x04000061 RID: 97
		private const byte VK_F8 = 119;

		// Token: 0x04000062 RID: 98
		private const byte VK_F9 = 120;

		// Token: 0x04000063 RID: 99
		private const byte VK_F10 = 121;

		// Token: 0x04000064 RID: 100
		private const byte VK_F11 = 122;

		// Token: 0x04000065 RID: 101
		private const byte VK_F12 = 123;

		// Token: 0x04000066 RID: 102
		private const byte VK_ALT = 18;

		// Token: 0x04000067 RID: 103
		private const byte VK_SPACE = 32;

		// Token: 0x04000068 RID: 104
		private const byte VK_SHIFT = 16;

		// Token: 0x04000069 RID: 105
		private const byte VK_CTRL = 17;

		// Token: 0x0400006A RID: 106
		private const byte VK_TAB = 9;

		// Token: 0x0400006B RID: 107
		private const byte VK_CAPITAL = 20;

		// Token: 0x0400006C RID: 108
		private const byte VK_ESCAPE = 27;

		// Token: 0x0400006D RID: 109
		private const byte VK_ENTER = 13;

		// Token: 0x0400006E RID: 110
		private const byte VK_BACK = 8;

		// Token: 0x0400006F RID: 111
		private const byte VK_INSERT = 45;

		// Token: 0x04000070 RID: 112
		private const byte VK_DELETE = 46;

		// Token: 0x04000071 RID: 113
		private const byte VK_HOME = 36;

		// Token: 0x04000072 RID: 114
		private const byte VK_END = 35;

		// Token: 0x04000073 RID: 115
		private const byte VK_PRIOR = 33;

		// Token: 0x04000074 RID: 116
		private const byte VK_NEXT = 34;

		// Token: 0x04000075 RID: 117
		private const byte VK_LEFT = 37;

		// Token: 0x04000076 RID: 118
		private const byte VK_UP = 38;

		// Token: 0x04000077 RID: 119
		private const byte VK_RIGHT = 39;

		// Token: 0x04000078 RID: 120
		private const byte VK_DOWN = 40;

		// Token: 0x04000079 RID: 121
		private const byte VK_NUMPAD0 = 96;

		// Token: 0x0400007A RID: 122
		private const byte VK_NUMPAD1 = 97;

		// Token: 0x0400007B RID: 123
		private const byte VK_NUMPAD2 = 98;

		// Token: 0x0400007C RID: 124
		private const byte VK_NUMPAD3 = 99;

		// Token: 0x0400007D RID: 125
		private const byte VK_NUMPAD4 = 100;

		// Token: 0x0400007E RID: 126
		private const byte VK_NUMPAD5 = 101;

		// Token: 0x0400007F RID: 127
		private const byte VK_NUMPAD6 = 102;

		// Token: 0x04000080 RID: 128
		private const byte VK_NUMPAD7 = 103;

		// Token: 0x04000081 RID: 129
		private const byte VK_NUMPAD8 = 104;

		// Token: 0x04000082 RID: 130
		private const byte VK_NUMPAD9 = 105;

		// Token: 0x04000083 RID: 131
		private const byte VK_MULTIPLY = 106;

		// Token: 0x04000084 RID: 132
		private const byte VK_ADD = 107;

		// Token: 0x04000085 RID: 133
		private const byte VK_SUBTRACT = 109;

		// Token: 0x04000086 RID: 134
		private const byte VK_DIVIDE = 111;

		// Token: 0x04000087 RID: 135
		private const byte VK_DECIMAL = 110;

		// Token: 0x04000088 RID: 136
		private const int SW_HIDE = 0;

		// Token: 0x04000089 RID: 137
		private const int SW_SHOW = 5;

		// Token: 0x0400008A RID: 138
		private const int SW_RESTORE = 9;

		// Token: 0x0400008B RID: 139
		private const uint SWP_NOSIZE = 1U;

		// Token: 0x0400008C RID: 140
		private const uint SWP_NOZORDER = 4U;

		// Token: 0x0400008D RID: 141
		private const uint SWP_SHOWWINDOW = 64U;

		// Token: 0x0400008E RID: 142
		private static NotifyIcon trayIcon;

		// Token: 0x0400008F RID: 143
		private static ContextMenuStrip trayMenu;

		// Token: 0x04000090 RID: 144
		private static bool isMinimizedToTray = false;

		// Token: 0x04000091 RID: 145
		private const byte VK_SCROLL = 145;

		// Token: 0x04000092 RID: 146
		private const uint MOUSEEVENTF_LEFTDOWN = 2U;

		// Token: 0x04000093 RID: 147
		private const uint MOUSEEVENTF_LEFTUP = 4U;

		// Token: 0x04000094 RID: 148
		private const uint MOUSEEVENTF_RIGHTDOWN = 8U;

		// Token: 0x04000095 RID: 149
		private const uint MOUSEEVENTF_RIGHTUP = 16U;

		// Token: 0x04000096 RID: 150
		private const uint MOUSEEVENTF_MIDDLEDOWN = 32U;

		// Token: 0x04000097 RID: 151
		private const uint MOUSEEVENTF_MIDDLEUP = 64U;

		// Token: 0x04000098 RID: 152
		private const uint MOUSEEVENTF_XDOWN = 128U;

		// Token: 0x04000099 RID: 153
		private const uint MOUSEEVENTF_XUP = 256U;

		// Token: 0x0400009A RID: 154
		private const uint XBUTTON1 = 1U;

		// Token: 0x0400009B RID: 155
		private const uint XBUTTON2 = 2U;

		// Token: 0x0400009C RID: 156
		private const uint KEYEVENTF_KEYDOWN = 0U;

		// Token: 0x0400009D RID: 157
		private const uint KEYEVENTF_KEYUP = 2U;

		// Token: 0x0400009E RID: 158
		private static Form mainForm;

		// Token: 0x0400009F RID: 159
		private static Button btnStart;

		// Token: 0x040000A0 RID: 160
		private static Button btnStop;

		// Token: 0x040000A1 RID: 161
		private static Button btnSettings;

		// Token: 0x040000A2 RID: 162
		private static Button btnExit;

		// Token: 0x040000A3 RID: 163
		private static Label lblStatus;

		// Token: 0x040000A4 RID: 164
		private static readonly string settingsFilePath = Path.Combine(Path.GetTempPath(), "Settings.xml");

		// Token: 0x02000008 RID: 8
		public struct RECT
		{
			// Token: 0x040000A5 RID: 165
			public int Left;

			// Token: 0x040000A6 RID: 166
			public int Top;

			// Token: 0x040000A7 RID: 167
			public int Right;

			// Token: 0x040000A8 RID: 168
			public int Bottom;
		}
	}
}


