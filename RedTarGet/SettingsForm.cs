using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RedPixelDetector
{
	// Token: 0x0200000E RID: 14
	public partial class SettingsForm : Form
	{
		// Token: 0x0600008F RID: 143
		public SettingsForm()
		{
			this.InitializeComponent();
			this.InitializeUI();
			this.currentActionSequence = new List<ActionItem>();
		}

		// Token: 0x06000091 RID: 145
		private void InitializeUI()
		{
			Panel mainPanel = new Panel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				BackColor = Color.FromArgb(60, 60, 80),
				Padding = new Padding(10)
			};
			this.InitializeControls(mainPanel);
			base.Controls.Add(mainPanel);
		}

		// Token: 0x06000092 RID: 146
		private void InitializeControls(Panel mainPanel)
		{
			int currentY = 20;
			Label lblResolution = new Label
			{
				Text = "���͡��Ҵ˹�Ҩ� :",
				Location = new Point(0, currentY),
				Size = new Size(320, 20),
				AutoSize = false,
				ForeColor = Color.White,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleCenter
			};
			currentY += lblResolution.Height + 10;
			this.cmbResolution = new ComboBox
			{
				Location = new Point(0, currentY),
				Size = new Size(250, 25),
				DropDownStyle = ComboBoxStyle.DropDownList,
				BackColor = Color.FromArgb(80, 80, 100),
				ForeColor = Color.White,
				Font = new Font("Microsoft Sans Serif", 9f),
				FlatStyle = FlatStyle.Flat,
				Anchor = AnchorStyles.Top
			};
			this.cmbResolution.DropDownWidth = this.cmbResolution.Width;
			System.Action centerResolutionControls = null;
			centerResolutionControls = delegate
			{
				int panelWidth = mainPanel.ClientSize.Width;
				lblResolution.Left = Math.Max((panelWidth - lblResolution.Width) / 2, 0);
				this.cmbResolution.Left = Math.Max((panelWidth - this.cmbResolution.Width) / 2, 0);
			};
			centerResolutionControls();
			mainPanel.Resize += delegate(object sender, System.EventArgs e)
			{
				centerResolutionControls();
			};
			mainPanel.Layout += delegate(object sender, LayoutEventArgs e)
			{
				centerResolutionControls();
			};
			currentY += this.cmbResolution.Height + 20;
			this.chkAutoMode = new CheckBox
			{
				Text = "\ud83e\udd16 โหมดอัตโนมัติ (ไม่ต้องกดปุ่ม)",
				Location = new Point(20, currentY),
				Size = new Size(300, 20),
				Checked = false,
				ForeColor = Color.White,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 30;
			this.chkToggleMode = new CheckBox
			{
				Text = "\ud83d\udd18 โหมด Toggle (กดปุ่มสลับเปิด/ปิด)",
				Location = new Point(20, currentY),
				Size = new Size(250, 20),
				Checked = false,
				ForeColor = Color.White,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 30;
			this.chkHoldToActivate = new CheckBox
			{
				Text = "\ud83c\udfae โหมดกดค้าง (กดปุ่มค้างเพื่อทำงาน)",
				Location = new Point(20, currentY),
				Size = new Size(250, 20),
				Checked = false,
				Enabled = true,
				ForeColor = Color.White,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 40;
			this.chkUseMouse = new CheckBox
			{
				Text = "\ud83d\uddb1️ ใช้เมาส์",
				Location = new Point(20, currentY),
				Size = new Size(100, 20),
				Checked = false,
				Enabled = true,
				ForeColor = Color.White,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 30;
			Label lblMouseButton = new Label
			{
				Text = "ปุ่มเมาส์:",
				Location = new Point(40, currentY),
				Size = new Size(100, 20),
				Enabled = true,
				ForeColor = Color.White
			};
			this.cmbMouseButton = new ComboBox
			{
				Location = new Point(180, currentY),
				Size = new Size(180, 20),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Enabled = true
			};
			currentY += 30;
			this.chkUseKeyboard = new CheckBox
			{
				Text = "⌨️ ใช้คีย์บอร์ด",
				Location = new Point(20, currentY),
				Size = new Size(100, 20),
				Checked = false,
				Enabled = true,
				ForeColor = Color.White,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 30;
			Label lblKey = new Label
			{
				Text = "ปุ่มคีย์บอร์ด:",
				Location = new Point(40, currentY),
				Size = new Size(100, 20),
				Enabled = true,
				ForeColor = Color.White
			};
			this.cmbKey = new ComboBox
			{
				Location = new Point(180, currentY),
				Size = new Size(180, 20),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Enabled = true
			};
			currentY += 40;
			Label lblFov = new Label
			{
				Text = "\ud83d\udccf ขนาด FOV:",
				Location = new Point(20, currentY),
				Size = new Size(150, 20),
				ForeColor = Color.White
			};
			this.numFov = new NumericUpDown
			{
				Location = new Point(180, currentY),
				Size = new Size(180, 20),
				Minimum = 0m,
				Maximum = 100m,
				Value = 20m
			};
			currentY += 30;
			Label lblScreenHeight = new Label
			{
				Text = "\ud83d\udcd0 ความสูงหน้าจอ:",
				Location = new Point(20, currentY),
				Size = new Size(150, 20),
				ForeColor = Color.White
			};
			this.numScreenHeight = new NumericUpDown
			{
				Location = new Point(180, currentY),
				Size = new Size(180, 20),
				Minimum = 0m,
				Maximum = 4000m,
				Value = 540m
			};
			currentY += 40;
			Label lblNewModes = new Label
			{
				Text = "\ud83c\udfaf โหมดการยิง:",
				Location = new Point(20, currentY),
				Size = new Size(200, 20),
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold),
				ForeColor = Color.LightCyan
			};
			currentY += 30;
			this.chkAutoShoot = new CheckBox
			{
				Text = "\ud83d\udd2b ยิงออโต้",
				Location = new Point(40, currentY),
				Size = new Size(150, 20),
				ForeColor = Color.LightGreen,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 30;
			this.chkSniperNoScope = new CheckBox
			{
				Text = "\ud83c\udfaf สไนโนสโคบ",
				Location = new Point(40, currentY),
				Size = new Size(150, 20),
				ForeColor = Color.LightCoral,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 30;
			this.chkSniperScope = new CheckBox
			{
				Text = "\ud83d\udd2d สไนสโคบ",
				Location = new Point(40, currentY),
				Size = new Size(150, 20),
				ForeColor = Color.LightBlue,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 30;
			this.chkManualMode = new CheckBox
			{
				Text = "\ud83d\udd04 โหมดแมนนวล",
				Location = new Point(40, currentY),
				Size = new Size(150, 20),
				ForeColor = Color.LightYellow,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 40;
			this.btnEditActions = new Button
			{
				Text = "\ud83c\udfac แก้ไขลำดับการทำงาน",
				Location = new Point(20, currentY),
				Size = new Size(200, 35),
				BackColor = Color.FromArgb(0, 120, 215),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Microsoft Sans Serif", 9f)
			};
			currentY += 50;
			Label lblColorSettings = new Label
			{
				Text = "\ud83c\udfa8 การตั้งค่าสี:",
				Location = new Point(20, currentY),
				Size = new Size(200, 20),
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold),
				ForeColor = Color.LightCyan
			};
			currentY += 30;
			Label lblRedThreshold = new Label
			{
				Text = "\ud83d\udd34 สีแดง (>=) :",
				Location = new Point(40, currentY),
				Size = new Size(120, 20),
				ForeColor = Color.LightCoral
			};
			this.numRedThreshold = new NumericUpDown
			{
				Location = new Point(180, currentY),
				Size = new Size(80, 20),
				Minimum = 0m,
				Maximum = 255m,
				Value = 200m
			};
			currentY += 30;
			Label lblGreenThreshold = new Label
			{
				Text = "\ud83d\udfe2 สีเขียว (<=) :",
				Location = new Point(40, currentY),
				Size = new Size(120, 20),
				ForeColor = Color.LightGreen
			};
			this.numGreenThreshold = new NumericUpDown
			{
				Location = new Point(180, currentY),
				Size = new Size(80, 20),
				Minimum = 0m,
				Maximum = 255m,
				Value = 50m
			};
			currentY += 30;
			Label lblBlueThreshold = new Label
			{
				Text = "\ud83d\udd35 สีน้ำเงิน (<=) :",
				Location = new Point(40, currentY),
				Size = new Size(120, 20),
				ForeColor = Color.LightBlue
			};
			this.numBlueThreshold = new NumericUpDown
			{
				Location = new Point(180, currentY),
				Size = new Size(80, 20),
				Minimum = 0m,
				Maximum = 255m,
				Value = 50m
			};
			currentY += 50;
			Button btnOk = new Button
			{
				Text = "✅ ตกลง",
				Location = new Point(150, currentY),
				Size = new Size(100, 35),
				BackColor = Color.FromArgb(76, 175, 80),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				DialogResult = DialogResult.OK,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
			};
			Button btnCancel = new Button
			{
				Text = "❌ ยกเลิก",
				Location = new Point(260, currentY),
				Size = new Size(100, 35),
				BackColor = Color.FromArgb(244, 67, 54),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				DialogResult = DialogResult.Cancel,
				Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold)
			};
			this.cmbResolution.Items.AddRange(new object[]
			{
				"\ud83c\udfae 800x600 (เกมเมอร์คลาสสิค)",
				"\ud83c\udfae 1024x768 (เกมเมอร์คลาสสิค)",
				"\ud83d\ude80 1280x720 (HD Ready)",
				"\ud83d\ude80 1280x768 (HD Wide)",
				"\ud83d\ude80 1280x1024 (SXGA)",
				"\ud83d\udd25 1360x720 (HD Wide)",
				"\ud83d\udd25 1360x768 (HD Wide)",
				"\ud83d\udd25 1360x1024 (HD Wide)",
				"\ud83d\udd25 1366x768 (มาตรฐานล่าสุด)",
				"\ud83d\udd25 1366x1024 (มาตรฐานล่าสุด)",
				"⚡ 1440x900 (WXGA+)",
				"⚡ 1440x1080 (ความคมชัดสูง)",
				"\ud83d\udc8e 1600x900 (ระดับโปร)",
				"\ud83d\udc8e 1680x1050 (WSXGA+)",
				"\ud83c\udf1f 1920x1080 (Full HD - ระดับเทพ)",
				"\ud83c\udf1f 1920x1200 (WUXGA)",
				"\ud83c\udfaf 2560x1080 (Ultrawide - กว้างไกล)",
				"\ud83d\ude80 2560x1440 (2K QHD - คมชัดสุด)",
				"\ud83d\udca5 3440x1440 (Ultrawide QHD - immersive)",
				"\ud83d\udd25 3840x1080 (Super Ultrawide - จอโค้ง)",
				"⚡ 3840x2160 (4K UHD - ระดับ cinema)",
				"\ud83c\udfae 5120x1440 (Super Ultrawide QHD - เกมมิ่งสุด)",
				"\ud83d\ude80 5120x2160 (5K Ultrawide - สุดยอด)",
				"\ud83d\udc8e 5120x2880 (5K - ความละเอียดสูงมาก)",
				"\ud83c\udf1f 7680x4320 (8K UHD - อนาคต)"
			});
			this.cmbMouseButton.Items.AddRange(new object[]
			{
				"คลิกซ้าย",
				"คลิกขวา",
				"คลิกกลาง",
				"ปุ่มข้าง 1",
				"ปุ่มข้าง 2"
			});
			this.cmbKey.Items.AddRange(new object[]
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
			this.cmbResolution.SelectedIndex = 14;
			this.cmbMouseButton.SelectedIndex = 1;
			this.cmbKey.SelectedIndex = this.cmbKey.Items.IndexOf("Alt");
			this.btnEditActions.Click += delegate(object s, EventArgs e)
			{
				this.OnEditActionsClicked(EventArgs.Empty);
			};

			mainPanel.Controls.AddRange(new Control[]
			{
				lblResolution,
				this.cmbResolution,
				this.chkAutoMode,
				this.chkToggleMode,
				this.chkHoldToActivate,
				this.chkUseMouse,
				lblMouseButton,
				this.cmbMouseButton,
				this.chkUseKeyboard,
				lblKey,
				this.cmbKey,
				lblFov,
				this.numFov,
				lblScreenHeight,
				this.numScreenHeight,
				lblNewModes,
				this.chkAutoShoot,
				this.chkSniperNoScope,
				this.chkSniperScope,
				this.chkManualMode,
				this.btnEditActions,
				lblColorSettings,
				lblRedThreshold,
				this.numRedThreshold,
				lblGreenThreshold,
				this.numGreenThreshold,
				lblBlueThreshold,
				this.numBlueThreshold,
				btnOk,
				btnCancel
			});
		}

		// Token: 0x06000093 RID: 147
		public void SetInitialValues(string resolution, bool autoMode, bool toggleMode, bool holdToActivate, bool useMouse, bool useKeyboard, MouseButtons mouseButton, byte key, Rectangle customRegion, int redThreshold, int greenThreshold, int blueThreshold, List<ActionItem> actionSequence)
		{
			if (!string.IsNullOrEmpty(resolution))
			{
				for (int i = 0; i < this.cmbResolution.Items.Count; i++)
				{
					if (this.cmbResolution.Items[i].ToString().Contains(resolution.Split(new char[]
					{
						' '
					})[0]))
					{
						this.cmbResolution.SelectedIndex = i;
						break;
					}
				}
			}
			this.chkAutoMode.Checked = autoMode;
			this.chkToggleMode.Checked = toggleMode;
			this.chkHoldToActivate.Checked = holdToActivate;
			this.chkUseMouse.Checked = useMouse;
			this.chkUseKeyboard.Checked = useKeyboard;
			if (mouseButton <= MouseButtons.Right)
			{
				if (mouseButton != MouseButtons.Left)
				{
					if (mouseButton == MouseButtons.Right)
					{
						this.cmbMouseButton.SelectedIndex = 1;
					}
				}
				else
				{
					this.cmbMouseButton.SelectedIndex = 0;
				}
			}
			else if (mouseButton != MouseButtons.Middle)
			{
				if (mouseButton != MouseButtons.XButton1)
				{
					if (mouseButton == MouseButtons.XButton2)
					{
						this.cmbMouseButton.SelectedIndex = 4;
					}
				}
				else
				{
					this.cmbMouseButton.SelectedIndex = 3;
				}
			}
			else
			{
				this.cmbMouseButton.SelectedIndex = 2;
			}
			string keyName = this.GetKeyName(key);
			int keyIndex = this.cmbKey.Items.IndexOf(keyName);
			if (keyIndex >= 0)
			{
				this.cmbKey.SelectedIndex = keyIndex;
			}
			this.numFov.Value = Math.Max(this.numFov.Minimum, Math.Min(customRegion.Width, this.numFov.Maximum));
			this.numScreenHeight.Value = Math.Max(this.numScreenHeight.Minimum, Math.Min(customRegion.Height, this.numScreenHeight.Maximum));
			this.numRedThreshold.Value = Math.Max(this.numRedThreshold.Minimum, Math.Min(redThreshold, this.numRedThreshold.Maximum));
			this.numGreenThreshold.Value = Math.Max(this.numGreenThreshold.Minimum, Math.Min(greenThreshold, this.numGreenThreshold.Maximum));
			this.numBlueThreshold.Value = Math.Max(this.numBlueThreshold.Minimum, Math.Min(blueThreshold, this.numBlueThreshold.Maximum));
			this.currentActionSequence = (actionSequence ?? new List<ActionItem>());
			this.SetShootingModeFromActionSequence(actionSequence);
		}

		// Token: 0x06000094 RID: 148
		public List<ActionItem> GetActionSequence()
		{
			return this.currentActionSequence;
		}

		// Token: 0x06000095 RID: 149
		private void SetShootingModeFromActionSequence(List<ActionItem> actionSequence)
		{
			if (actionSequence == null || actionSequence.Count == 0)
			{
				this.chkManualMode.Checked = true;
				return;
			}
			if (actionSequence.Count == 1 && actionSequence[0].Type == ActionItem.ActionType.MouseClick && actionSequence[0].Value == "Left")
			{
				this.chkAutoShoot.Checked = true;
				return;
			}
			if (actionSequence.Count == 4 && actionSequence[0].Type == ActionItem.ActionType.MouseClick && actionSequence[0].Value == "Left" && actionSequence[1].Type == ActionItem.ActionType.KeyPress && actionSequence[1].Value == "3" && actionSequence[2].Type == ActionItem.ActionType.KeyPress && actionSequence[2].Value == "1" && actionSequence[3].Type == ActionItem.ActionType.Delay)
			{
				this.chkSniperNoScope.Checked = true;
				return;
			}
			if (actionSequence.Count == 5 && actionSequence[0].Type == ActionItem.ActionType.MouseClick && actionSequence[0].Value == "Right" && actionSequence[1].Type == ActionItem.ActionType.MouseClick && actionSequence[1].Value == "Left" && actionSequence[2].Type == ActionItem.ActionType.KeyPress && actionSequence[2].Value == "3" && actionSequence[3].Type == ActionItem.ActionType.KeyPress && actionSequence[3].Value == "1" && actionSequence[4].Type == ActionItem.ActionType.Delay)
			{
				this.chkSniperScope.Checked = true;
				return;
			}
			this.chkManualMode.Checked = true;
		}

		// Token: 0x06000096 RID: 150
		private string GetKeyName(byte keyCode)
		{
			switch (keyCode)
			{
			case 8:
				return "Backspace";
			case 9:
				return "Tab";
			case 10:
			case 11:
			case 12:
			case 14:
			case 15:
			case 19:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 28:
			case 29:
			case 30:
			case 31:
				break;
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
			default:
				if (keyCode == 45)
				{
					return "Insert";
				}
				if (keyCode == 46)
				{
					return "Delete";
				}
				break;
			}
			return "Alt";
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000097 RID: 151
		public string SelectedResolution
		{
			get
			{
				object selectedItem = this.cmbResolution.SelectedItem;
				if (selectedItem == null)
				{
					return null;
				}
				return selectedItem.ToString();
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000098 RID: 152
		public bool AutoMode
		{
			get
			{
				return this.chkAutoMode.Checked;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000099 RID: 153
		public bool ToggleMode
		{
			get
			{
				return this.chkToggleMode.Checked;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600009A RID: 154
		public bool HoldToActivate
		{
			get
			{
				return this.chkHoldToActivate.Checked;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600009B RID: 155
		public bool UseMouse
		{
			get
			{
				return this.chkUseMouse.Checked;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600009C RID: 156
		public bool UseKeyboard
		{
			get
			{
				return this.chkUseKeyboard.Checked;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600009D RID: 157
		public string SelectedMouseButton
		{
			get
			{
				object selectedItem = this.cmbMouseButton.SelectedItem;
				if (selectedItem == null)
				{
					return null;
				}
				return selectedItem.ToString();
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600009E RID: 158
		public string SelectedKey
		{
			get
			{
				object selectedItem = this.cmbKey.SelectedItem;
				if (selectedItem == null)
				{
					return null;
				}
				return selectedItem.ToString();
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600009F RID: 159
		public int FovValue
		{
			get
			{
				return (int)this.numFov.Value;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000A0 RID: 160
		public int ScreenHeightValue
		{
			get
			{
				return (int)this.numScreenHeight.Value;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000A1 RID: 161
		public bool AutoShoot
		{
			get
			{
				return this.chkAutoShoot.Checked;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000A2 RID: 162
		public bool SniperNoScope
		{
			get
			{
				return this.chkSniperNoScope.Checked;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A3 RID: 163
		public bool SniperScope
		{
			get
			{
				return this.chkSniperScope.Checked;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A4 RID: 164
		public bool ManualMode
		{
			get
			{
				return this.chkManualMode.Checked;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000A5 RID: 165
		public int RedThreshold
		{
			get
			{
				return (int)this.numRedThreshold.Value;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000A6 RID: 166
		public int GreenThreshold
		{
			get
			{
				return (int)this.numGreenThreshold.Value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000A7 RID: 167
		public int BlueThreshold
		{
			get
			{
				return (int)this.numBlueThreshold.Value;
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060000A8 RID: 168
		// (remove) Token: 0x060000A9 RID: 169
		public event EventHandler EditActionsClicked;

		// Token: 0x060000AA RID: 170
		protected virtual void OnEditActionsClicked(EventArgs e)
		{
			EventHandler editActionsClicked = this.EditActionsClicked;
			if (editActionsClicked == null)
			{
				return;
			}
			editActionsClicked(this, e);
		}

		// Token: 0x040000D1 RID: 209
		private ComboBox cmbResolution;

		// Token: 0x040000D2 RID: 210
		private CheckBox chkAutoMode;

		// Token: 0x040000D3 RID: 211
		private CheckBox chkToggleMode;

		// Token: 0x040000D4 RID: 212
		private CheckBox chkHoldToActivate;

		// Token: 0x040000D5 RID: 213
		private CheckBox chkUseMouse;

		// Token: 0x040000D6 RID: 214
		private CheckBox chkUseKeyboard;

		// Token: 0x040000D7 RID: 215
		private ComboBox cmbMouseButton;

		// Token: 0x040000D8 RID: 216
		private ComboBox cmbKey;

		// Token: 0x040000D9 RID: 217
		private NumericUpDown numFov;

		// Token: 0x040000DA RID: 218
		private NumericUpDown numScreenHeight;

		// Token: 0x040000DB RID: 219
		private CheckBox chkAutoShoot;

		// Token: 0x040000DC RID: 220
		private CheckBox chkSniperNoScope;

		// Token: 0x040000DD RID: 221
		private CheckBox chkSniperScope;

		// Token: 0x040000DE RID: 222
		private CheckBox chkManualMode;

		// Token: 0x040000DF RID: 223
		private Button btnEditActions;

		// Token: 0x040000E0 RID: 224
		private NumericUpDown numRedThreshold;

		// Token: 0x040000E1 RID: 225
		private NumericUpDown numGreenThreshold;

		// Token: 0x040000E2 RID: 226
		private NumericUpDown numBlueThreshold;

		// Token: 0x040000E3 RID: 227
		private List<ActionItem> currentActionSequence;
	}
}
