﻿using Microsoft.Win32;
using QuickLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace quick_picture_viewer
{
	partial class SettingsForm : QlibFixedForm
	{
		private struct Language
		{
			public string Code;
			public string[] Authors;
			public bool Beta;
		}

		private Language[] languages = {
			new Language
			{
				Code = "en",
				Authors = new string[] { "Beelink" },
				Beta = false
			},
			new Language
			{
				Code = "ru",
				Authors = new string[] { "Beelink" },
				Beta = false
			},
			new Language
			{
				Code = "es",
				Authors = new string[] { "IpsumRy" },
				Beta = true
			}
		};

		private bool settingsStarted = false;
		private MainForm owner;

		public SettingsForm(bool darkMode)
		{
			if (darkMode)
			{
				HandleCreated += new EventHandler(ThemeManager.formHandleCreated);
			}

			InitializeComponent();
			SetDraggableControls(new List<Control>() { titlePanel, titleLabel });

			int theme = Properties.Settings.Default.Theme;
			if (theme == 0)
			{
				systemThemeRadio.Checked = true;
			}
			else if (theme == 1)
			{
				lightThemeRadio.Checked = true;
			}
			else
			{
				darkThemeRadio.Checked = true;
			}

			fullscrCursorCheckBox.Checked = Properties.Settings.Default.ShowCursorInFullscreen;
			escToExitCheckBox.Checked = Properties.Settings.Default.EscToExit;

			int mouseWheelAction = Properties.Settings.Default.MouseWheelScrollAction;
			if (mouseWheelAction == 0)
			{
				mouseWheelActionRadio1.Checked = true;
			}
			else if (mouseWheelAction == 1)
			{
				mouseWheelActionRadio2.Checked = true;
			}
			else if (mouseWheelAction == 2)
			{
				mouseWheelActionRadio3.Checked = true;
			}

			updatesCheckBox.Checked = Properties.Settings.Default.CheckForUpdates;
			startupPasteCheckBox.Checked = Properties.Settings.Default.StartupPaste;
			startupMaximizeCheckBox.Checked = Properties.Settings.Default.StartupMaximize;
			startupBoundsCheckBox.Checked = Properties.Settings.Default.StartupRestoreBounds;

			favExtTextBox.Text = Properties.Settings.Default.FavoriteExternalApp;

			openWithCheckBox.Checked = GetOpenWithState();
			const string browseWithKey1 = "HKEY_CLASSES_ROOT\\Directory\\Background\\shell\\QuickPictureViewer";
			const string browseWithKey2 = "HKEY_CLASSES_ROOT\\Directory\\shell\\QuickPictureViewer";
			string browseWithValue1 = (string)Registry.GetValue(browseWithKey1, string.Empty, string.Empty);
			string browseWithValue2 = (string)Registry.GetValue(browseWithKey2, string.Empty, string.Empty);
			if (browseWithValue1.Length > 0 && browseWithValue2.Length > 0)
			{
				browseWithCheckBox.Checked = true;
			}

			slideshowTimeNumeric.Value = Properties.Settings.Default.SlideshowTime;
			slideshowCounterCheckBox.Checked = Properties.Settings.Default.SlideshowCounter;

			for (int i = 0; i < languages.Length; i++)
			{
				if (Properties.Settings.Default.Language == languages[i].Code)
				{
					langComboBox.SelectedIndex = i;
					break;
				}
			}

			themeRestart.LinkColor = ThemeManager.AccentColor;
			localizationRestart.LinkColor = ThemeManager.AccentColor;

			SetDarkMode(darkMode);
		}

		private void InitLanguage()
		{
			Text = owner.resMan.GetString("settings");
			langPage.Text = owner.resMan.GetString("localization");
			startupPage.Text = owner.resMan.GetString("startup");
			restartLabel1.Text = "* " + owner.resMan.GetString("restart-required");
			restartLabel2.Text = "* " + owner.resMan.GetString("restart-required");
			systemThemeRadio.Text = owner.resMan.GetString("use-system-setting");
			lightThemeRadio.Text = owner.resMan.GetString("light");
			darkThemeRadio.Text = owner.resMan.GetString("dark");
			themePage.Text = owner.resMan.GetString("theme");
			startupLabel.Text = owner.resMan.GetString("app-startup-actions") + ":";
			startupPasteCheckBox.Text = owner.resMan.GetString("paste-from-clipboard");
			startupMaximizeCheckBox.Text = owner.resMan.GetString("maximize-window");
			startupBoundsCheckBox.Text = owner.resMan.GetString("restore-last-window-bounds");
			updatesCheckBox.Text = owner.resMan.GetString("check-for-app-updates");
			favExtLabel.Text = owner.resMan.GetString("fav-external-app") + ":";
			browseBtn.Text = " " + owner.resMan.GetString("browse");
			slideshowPage.Text = owner.resMan.GetString("slideshow");
			slideshowTimeLabel.Text = owner.resMan.GetString("switching-time") + ":";
			mousePage.Text = owner.resMan.GetString("input");
			slideshowSecondsLabel.Text = owner.resMan.GetString("seconds");
			slideshowCounterCheckBox.Text = owner.resMan.GetString("show-slideshow-counter");
			fullscrCursorCheckBox.Text = owner.resMan.GetString("fullscreen-cursor");
			langLabel.Text = owner.resMan.GetString("ui-lang") + ":";
			infoTooltip.SetToolTip(closeBtn, owner.resMan.GetString("close") + " | Alt+F4");
			translatedByLabel.Text = owner.resMan.GetString("translated-by") + ": ";
			escToExitCheckBox.Text = string.Format(owner.resMan.GetString("esc-to-exit"), "Esc");
			mouseWheelActionLabel.Text = owner.resMan.GetString("mouse-wheel-action") + ":";
			mouseWheelActionRadio1.Text = owner.resMan.GetString("scroll-up-down");
			mouseWheelActionRadio2.Text = owner.resMan.GetString("zoom-in-out");
			mouseWheelActionRadio3.Text = owner.resMan.GetString("next-prev-image");
			themeRestart.Text = owner.resMan.GetString("restart");
			localizationRestart.Text = owner.resMan.GetString("restart");
			contextMenuLabel.Text = owner.resMan.GetString("context-menu") + ":";
		}

		private void SetDarkMode(bool dark)
		{
			if (dark)
			{
				settingsTabs.BackTabColor = ThemeManager.DarkBackColor;
				settingsTabs.HeaderColor = ThemeManager.DarkSecondColor;
				settingsTabs.TextColor = Color.White;
				settingsTabs.HorizontalLineColor = Color.Transparent;

				browseBtn.Image = Properties.Resources.white_open;
				browseBtn.BackColor = ThemeManager.DarkSecondColor;
				browseBtn.ForeColor = Color.White;
			}

			DarkMode = dark;
			updatesCheckBox.DarkMode = dark;
			fullscrCursorCheckBox.DarkMode = dark;
			darkThemeRadio.SetDarkMode(dark);
			lightThemeRadio.SetDarkMode(dark);
			systemThemeRadio.SetDarkMode(dark);
			closeBtn.SetDarkMode(dark);
			slideshowTimeNumeric.DarkMode = dark;
			slideshowCounterCheckBox.DarkMode = dark;
			langComboBox.SetDarkMode(dark);
			favExtTextBox.DarkMode = dark;
			escToExitCheckBox.DarkMode = dark;
			mouseWheelActionRadio1.SetDarkMode(dark);
			mouseWheelActionRadio2.SetDarkMode(dark);
			mouseWheelActionRadio3.SetDarkMode(dark);
			startupMaximizeCheckBox.DarkMode = dark;
			startupPasteCheckBox.DarkMode = dark;
			startupBoundsCheckBox.DarkMode = dark;
			openWithCheckBox.DarkMode = dark;
			browseWithCheckBox.DarkMode = dark;
		}

		private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				Close();
			}
		}

		private void updatesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.CheckForUpdates = updatesCheckBox.Checked;
			Properties.Settings.Default.Save();
		}

		private void systemThemeRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (systemThemeRadio.Checked)
			{
				Properties.Settings.Default.Theme = 0;
				Properties.Settings.Default.Save();
			}
		}

		private void lightThemeRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (lightThemeRadio.Checked)
			{
				Properties.Settings.Default.Theme = 1;
				Properties.Settings.Default.Save();
			}
		}

		private void darkThemeRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (darkThemeRadio.Checked)
			{
				Properties.Settings.Default.Theme = 2;
				Properties.Settings.Default.Save();
			}
		}

		private void fullscrCursorCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.ShowCursorInFullscreen = fullscrCursorCheckBox.Checked;
			Properties.Settings.Default.Save();
		}

		private void closeBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void favExtTextBox_TextChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.FavoriteExternalApp = favExtTextBox.Text;
			Properties.Settings.Default.Save();
		}

		private void browseBtn_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				favExtTextBox.Text = openFileDialog1.FileName;
			}
			openFileDialog1.Dispose();
		}

		private void slideshowCounterCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.SlideshowCounter = slideshowCounterCheckBox.Checked;
			Properties.Settings.Default.Save();
		}

		private void slideshowTimeNumeric_ValueChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.SlideshowTime = (int)slideshowTimeNumeric.Value;
			Properties.Settings.Default.Save();
		}

		private void langComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.Language = languages[langComboBox.SelectedIndex].Code;
			Properties.Settings.Default.Save();

			if (owner != null)
			{
				if (languages[langComboBox.SelectedIndex].Beta && settingsStarted)
				{
					MessageBox.Show(
						owner.resMan.GetString("beta-lang-warning"),
						owner.resMan.GetString("warning") + " - " + langComboBox.Items[langComboBox.SelectedIndex].ToString(),
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);
				}

				translateAuthorsPanel.Controls.Clear();
				translateAuthorsPanel.Left = translatedByLabel.Location.X + translatedByLabel.Width;

				int curX = 0;
				for (int i = 0; i < languages[langComboBox.SelectedIndex].Authors.Length; i++)
				{
					LinkLabel ll = new LinkLabel();
					ll.AutoSize = true;
					ll.Text = languages[langComboBox.SelectedIndex].Authors[i];
					ll.LinkColor = ThemeManager.AccentColor;
					ll.Margin = new Padding(0, 0, 0, 0);
					ll.Click += Ll_Click;
					translateAuthorsPanel.Controls.Add(ll);
					ll.Left = curX;
					curX += ll.Width;
				}
			}
		}

		private void Ll_Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/" + (sender as LinkLabel).Text);
		}

		private void SettingsForm_Load(object sender, EventArgs e)
		{
			owner = Owner as MainForm;
			InitLanguage();
			langComboBox_SelectedIndexChanged(null, null);
			settingsStarted = true;
		}

		private void escToExitCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.EscToExit = escToExitCheckBox.Checked;
			Properties.Settings.Default.Save();
		}

		private void mouseWheelActionRadio1_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.MouseWheelScrollAction = 0;
			Properties.Settings.Default.Save();
		}

		private void mouseWheelActionRadio2_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.MouseWheelScrollAction = 1;
			Properties.Settings.Default.Save();
		}

		private void mouseWheelActionRadio3_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.MouseWheelScrollAction = 2;
			Properties.Settings.Default.Save();
		}

		private void restartLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			owner.RestartApp();
		}

		private void startupPasteCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.StartupPaste = startupPasteCheckBox.Checked;
			Properties.Settings.Default.Save();
		}

		private void startupMaximizeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.StartupMaximize = startupMaximizeCheckBox.Checked;
			Properties.Settings.Default.Save();
		}

		private void startupBoundsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.StartupRestoreBounds = startupBoundsCheckBox.Checked;
			Properties.Settings.Default.Save();
		}

		private void openWithCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (settingsStarted)
			{
				try
				{
					if (openWithCheckBox.Checked)
					{
						Registry.SetValue("HKEY_CLASSES_ROOT\\*\\shell\\QuickPictureViewer", "", "Open with QuickPictureViewer");
						Registry.SetValue("HKEY_CLASSES_ROOT\\*\\shell\\QuickPictureViewer", "Icon", string.Format("\"{0}picture.ico\"", AppDomain.CurrentDomain.BaseDirectory));
						Registry.SetValue("HKEY_CLASSES_ROOT\\*\\shell\\QuickPictureViewer\\command", "", string.Format("\"{0}quick-picture-viewer.exe\" \"%V\"", AppDomain.CurrentDomain.BaseDirectory));
					}
					else
					{
						RegistryKey RegKey = Registry.ClassesRoot.OpenSubKey("*\\shell\\QuickPictureViewer", true);
						RegKey.DeleteSubKeyTree("");
					}
				}
				catch
				{
					MessageBox.Show("To change context menu options you need to run app with Administrator", owner.resMan.GetString("error"));
				}
			}
		}

		private void browseWithCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (settingsStarted)
			{
				try
				{
					if (browseWithCheckBox.Checked)
					{
						Registry.SetValue("HKEY_CLASSES_ROOT\\Directory\\Background\\shell\\QuickPictureViewer", "", "Browse folder with QuickPictureViewer");
						Registry.SetValue("HKEY_CLASSES_ROOT\\Directory\\Background\\shell\\QuickPictureViewer", "Icon", string.Format("\"{0}picture.ico\"", AppDomain.CurrentDomain.BaseDirectory));
						Registry.SetValue("HKEY_CLASSES_ROOT\\Directory\\Background\\shell\\QuickPictureViewer\\command", "", string.Format("\"{0}quick-picture-viewer.exe\" \"%V\"", AppDomain.CurrentDomain.BaseDirectory));

						Registry.SetValue("HKEY_CLASSES_ROOT\\Directory\\shell\\QuickPictureViewer", "", "Browse folder with QuickPictureViewer");
						Registry.SetValue("HKEY_CLASSES_ROOT\\Directory\\shell\\QuickPictureViewer", "Icon", string.Format("\"{0}picture.ico\"", AppDomain.CurrentDomain.BaseDirectory));
						Registry.SetValue("HKEY_CLASSES_ROOT\\Directory\\shell\\QuickPictureViewer\\command", "", string.Format("\"{0}quick-picture-viewer.exe\" \"%V\"", AppDomain.CurrentDomain.BaseDirectory));
					}
					else
					{
						RegistryKey RegKey = Registry.ClassesRoot.OpenSubKey("Directory\\Background\\shell\\QuickPictureViewer", true);
						RegKey.DeleteSubKeyTree("");
						RegistryKey RegKey2 = Registry.ClassesRoot.OpenSubKey("Directory\\shell\\QuickPictureViewer", true);
						RegKey2.DeleteSubKeyTree("");
					}
				}
				catch
				{
					MessageBox.Show("To change context menu options you need to run app with Administrator", owner.resMan.GetString("error"));
				}
			}
		}

		private bool GetOpenWithState()
		{
			string openWithValue = (string)Registry.GetValue("HKEY_CLASSES_ROOT\\*\\shell\\QuickPictureViewer", string.Empty, string.Empty);
			return openWithValue.Length > 0;
		}
	}
}
