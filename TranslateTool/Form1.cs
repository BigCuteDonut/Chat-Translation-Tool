/*
    Chat Translation Tool
    Copyright (C) 2020 Johanna Sierak

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TranslateTool.Properties;

namespace TranslateTool
{
    public partial class Form1 : Form
    {
        public const string VersionNumber = "1.0.7";
        public static Form1 Form;
        public static Language Language;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetScrollPos(IntPtr hWnd, Orientation nBar);
        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, Orientation nBar, int nPos, bool bRedraw);
        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public bool Moving = false;
        public bool ResizeMove = false;
        public bool ResizingX = false;
        public bool ResizingY = false;
        public double TargetOpacity = 1;
        public int LastPosX;
        public int LastPosY;
        public int LastSizeX;
        public int LastSizeY;
        public int LastX;
        public int LastY;
        public int Wait;
        public double TimeScale = 0.2;
        public double ToolTipTargetOpacity = 0;
        public bool ToolTipShow = false;
        public bool AutoScrollEnabled = true;
        public bool AutoShowEnabled = false;
        public bool ClickThroughEnabled = false;
        new public bool Closed = false;
        public int OldStyle;
        public Dictionary<Control, string> ToolTipInfo = new Dictionary<Control, string>();
        public ToolTipForm ToolTip;
        public ResizeForm ExtendedResizeForm;
        public SettingsForm SettingsForm;
        public bool SettingsShown = false;
        public bool AutoHideEnabled = true;
        public string[] InputHistory = new string[256];
        public byte HistoryIndex = 0;
        public byte HistoryCurrent = 0;
        public bool TranslatorLoaded = false;

        public Form1()
        {
            InitializeComponent();
            Form = this;
            AcceptButton = EnterButton;
        }

        public void ToolTipHover(object sender, EventArgs e)
        {
            HandleToolTip((Control)sender);
        }

        public void ToolTipLeave(object sender, EventArgs e)
        {
            CancelToolTip();
        }

        public void SetToolTip(Control control, string text)
        {
            ToolTipInfo[control] = text;
            control.MouseHover += ToolTipHover;
            control.MouseLeave += ToolTipLeave;
        }

        public void HandleToolTip(Control control)
        {
            if (ToolTipInfo.TryGetValue(control, out string text))
            {

                var textSize = TextRenderer.MeasureText(text, Output.Font, new Size(260, 500), TextFormatFlags.WordBreak);

                ToolTip.Size = textSize;
                ToolTip.Location = new Point(Cursor.Position.X, Cursor.Position.Y + (Cursor.Size.Height / 2));
                ToolTipTargetOpacity = 1;
                ToolTip.ShowText(text);
            }
        }

        public void CancelToolTip()
        {
            ToolTipTargetOpacity = 0;
        }

        private void ApplySettings()
        {
            Location = Settings.Default.WindowLocation;
            Size = Settings.Default.WindowSize;
            ResizeForm.Form.Location = new Point(Location.X - 4, Location.Y - 4);
            ResizeForm.Form.Size = new Size(Size.Width + 8, Size.Height + 8);
            AutoHideEnabled = !Settings.Default.PreventWindowTransparency;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ExtendedResizeForm = ResizeForm.Form;
            ResizeForm.Form.Start();
            ToolTip = new ToolTipForm();
            ToolTip.Show(this);
            ToolTip.MaximumSize = new Size(500, 500);
            SettingsForm = new SettingsForm();
            ApplySettings();
            AutoScaleMode = AutoScaleMode.None;
            RegisterHotKey(Handle, 1, 2, (int)Keys.T);
            ApplyTranslation(new Language(VersionNumber));
            Opacity = 1;
            Wait = 500;
        }

        private void RevertInput()
        {
            var targetIndex = (byte)(HistoryIndex - 1);

            if (InputHistory[targetIndex] != null)
            {
                Input.Text = InputHistory[targetIndex];
                HistoryIndex--;
            }
        }

        private void NextInput()
        {
            var targetIndex = (byte)(HistoryIndex + 1);

            if (InputHistory[targetIndex] != null)
            {
                Input.Text = InputHistory[targetIndex];
                HistoryIndex++;
                Input.SelectionLength = 0;
                Input.SelectionStart = Input.TextLength;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1)
            {
                ToggleClickThrough();
            }
            base.WndProc(ref m);
        }

        private void ToggleClickThrough()
        {
            if (!ClickThroughEnabled)
            {
                OldStyle = GetWindowLong(Handle, -20);

                ClickThroughEnabled = true;
                SetWindowLong(Handle, -20, OldStyle | 0x80000 | 0x20);
                ClickThroughButton.BackgroundImage = Resources.ClickThrough;
            }
            else
            {
                SetWindowLong(Handle, -20, OldStyle);
                ClickThroughEnabled = false;
                ClickThroughButton.BackgroundImage = Resources.ClickThroughDisabled;
            }
        }

        private void ToggleAutoShow()
        {
            if (!AutoShowEnabled)
            {
                AutoShowEnabled = true;
                AutoShowButton.BackgroundImage = Resources.AutoShow;
            }
            else
            {
                AutoShowEnabled = false;
                AutoShowButton.BackgroundImage = Resources.AutoShowDisabled;
            }
        }

        private void ToggleAutoScroll()
        {
            if (!AutoScrollEnabled)
            {
                AutoScrollEnabled = true;
                AutoScrollButton.BackgroundImage = Resources.AutoScroll;
            }
            else
            {
                AutoScrollEnabled = false;
                AutoScrollButton.BackgroundImage = Resources.AutoScrollDisabled;
            }
        }

        public void WriteLine(string text)
        {
            Action<string> action;
            if (AutoScrollEnabled)
            {
                action = Output.AppendText;
            }
            else
            {
                action = (value) =>
                {
                    var position = GetScrollPos(Output.Handle, Orientation.Vertical);
                    Output.Text += value;
                    SendMessage(Output.Handle, 0x00B6, 0, position);
                };
            }
            Invoke(action, text + "\r\n");

            if (AutoShowEnabled)
            {
                TargetOpacity = 1;
                Wait = 400;
            }
        }

        public void ApplyTranslation(Language language)
        {
            Action action = () =>
            {
                Language = language;
                WriteLine(language.Text[8]);
                WriteLine(language.Text[11]);
                SetToolTip(ClickThroughButton, language.Text[12]);
                SetToolTip(AutoScrollButton, language.Text[13]);
                SetToolTip(AutoShowButton, language.Text[14]);
                SetToolTip(CloseButton, language.Text[15]);
                SettingsForm.AutoHideCheck.Text = language.Text[17];

                if (language.IsJapanese)
                {
                    SettingsForm.InfoCheck.Enabled = false;
                    SettingsForm.InfoCheck.Checked = false;
                    Translate.DisableAdditionalInputInfo();
                }
                else
                {
                    SettingsForm.InfoCheck.Enabled = true;
                    SettingsForm.InfoCheck.Checked = Settings.Default.InputInfoEnabled;
                    if (SettingsForm.InfoCheck.Checked)
                    {
                        Translate.EnableAdditionalInputInfo();
                    }
                }
                SettingsForm.InfoCheck.Text = language.Text[16];
            };
            Invoke(action);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!ResizingX && !ResizingY)
            {
                Moving = true;
                LastPosX = Location.X;
                LastPosY = Location.Y;
                LastX = Cursor.Position.X;
                LastY = Cursor.Position.Y;
            }
        }

        private void BeginResizing(Label selectedLabel)
        {
            if (selectedLabel.Name.Contains("N") || selectedLabel.Name.Contains("W"))
            {
                ResizeMove = true;
            }
            LastPosX = Location.X;
            LastPosY = Location.Y;
            LastSizeX = Size.Width;
            LastSizeY = Size.Height;
            LastX = Cursor.Position.X;
            LastY = Cursor.Position.Y;
        }

        private void EndResizeOrMove()
        {
            Moving = false;
            ResizingX = false;
            ResizingY = false;
            ResizeMove = false;
            ResizeForm.Form.Location = new Point(Location.X - 4, Location.Y - 4);
            ResizeForm.Form.Size = new Size(Size.Width + 8, Size.Height + 8);
            Settings.Default.WindowLocation = Location;
            Settings.Default.WindowSize = Size;
            Settings.Default.Save();
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            EndResizeOrMove();
        }

        public void ResizeX_MouseDown(object sender, MouseEventArgs e)
        {
            ResizingX = true;
            BeginResizing(sender as Label);
        }

        public void ResizeX_MouseUp(object sender, MouseEventArgs e)
        {
            EndResizeOrMove();
        }

        public void ResizeY_MouseDown(object sender, MouseEventArgs e)
        {
            ResizingY = true;
            BeginResizing(sender as Label);
        }

        public void ResizeY_MouseUp(object sender, MouseEventArgs e)
        {
            EndResizeOrMove();
        }

        public void ResizeXY_MouseDown(object sender, MouseEventArgs e)
        {
            ResizingX = true;
            ResizingY = true;
            BeginResizing(sender as Label);
        }

        public void ResizeXY_MouseUp(object sender, MouseEventArgs e)
        {
            EndResizeOrMove();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var bounds = DesktopBounds;
            bounds.Offset(-4, 26);
            bounds.Width += 8;
            bounds.Height += 8;

            if (bounds.Contains(Cursor.Position))
            {
                TargetOpacity = 1;
                if (Opacity == 1)
                {
                    Wait = 50;
                }
            }
            else
            {
                if (Opacity == 1 && Wait > 0)
                {
                    Wait--;
                }
                else if (Wait <= 0)
                {
                    TargetOpacity = 0;
                }
            }
            if (Moving || ResizingX || ResizingY)
            {
                if (!MouseButtons.HasFlag(MouseButtons.Left))
                {
                    EndResizeOrMove();
                }
                if (Moving)
                {
                    Location = new Point(LastPosX + (Cursor.Position.X - LastX), LastPosY + (Cursor.Position.Y - LastY));
                }
                else if (ResizingX || ResizingY)
                {
                    var newWidth = Width;
                    var newHeight = Height;

                    if (ResizeMove)
                    {
                        var newX = Location.X;
                        var newY = Location.Y;

                        if (ResizingX)
                        {
                            newX = LastPosX + (Cursor.Position.X - LastX);
                            newWidth = LastSizeX - ((int)(Cursor.Position.X) - LastX);
                        }
                        if (ResizingY)
                        {
                            newY = LastPosY + (Cursor.Position.Y - LastY);
                            newHeight = LastSizeY - ((int)(Cursor.Position.Y) - LastY);
                        }
                        Location = new Point(newX, newY);
                    }
                    else
                    {

                        if (ResizingX)
                        {
                            newWidth = LastSizeX + ((int)(Cursor.Position.X) - LastX);
                        }
                        if (ResizingY)
                        {
                            newHeight = LastSizeY + ((int)(Cursor.Position.Y) - LastY);
                        }

                    }

                    Size = new Size(newWidth, newHeight);
                }
                TargetOpacity = 1;
            }
            if (AutoHideEnabled)
            {
                if (TargetOpacity != Opacity)
                {
                    if (Opacity < TargetOpacity)
                    {
                        Opacity += 0.06;
                    }
                    else
                    {
                        if (Wait > 0)
                        {
                            Wait--;
                        }
                        else
                        {
                            Opacity -= 0.02;
                        }
                    }
                    if (Opacity > 1)
                    {
                        Opacity = 1;
                    }
                    else if (Opacity < 0)
                    {
                        Opacity = 0;
                    }
                    if (Opacity > 0)
                    {
                        ResizeForm.Form.Opacity = 1;
                    }
                    else if (ToolTip.Opacity == 0)
                    {
                        ResizeForm.Form.Opacity = 0;
                    }
                }
            }
            else
            {
                if (Opacity != 1)
                {
                    Opacity = 1;
                    ResizeForm.Form.Opacity = 1;
                }
            }
            if (ToolTipTargetOpacity != ToolTip.Opacity)
            {
                if (ToolTip.Opacity < ToolTipTargetOpacity)
                {
                    ToolTip.Opacity += 0.25 * TimeScale;
                }
                else
                {
                    ToolTip.Opacity -= 0.40 * TimeScale;
                }
                if (ToolTip.Opacity > 1)
                {
                    ToolTip.Opacity = 1;
                }
                else if (ToolTip.Opacity < 0)
                {
                    ToolTip.Opacity = 0;
                }
            }
        }

        public void SetInputValue(string text)
        {
            Action<string> action = (input) =>
            {
                Input.Text = input;
            };

            Invoke(action, text);
        }

        private void TranslateButton_Click(object sender, EventArgs e)
        {
            if (Input.Text != "")
            {
                InputHistory[HistoryCurrent++] = Input.Text;
                HistoryIndex = HistoryCurrent;
            }
            Translate.Input = Input.Text;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Translate.StopBrowsers();
        }

        private void AutoScrollHighlight()
        {
            AutoScrollButton.BackColor = Color.FromArgb(36, 68, 100);
        }

        private void DisableAutoScrollHighlight()
        {
            AutoScrollButton.BackColor = Color.FromArgb(19, 41, 63);
        }

        private void AutoShowHighlight()
        {
            AutoShowButton.BackColor = Color.FromArgb(36, 68, 100);
        }

        private void DisableAutoShowHighlight()
        {
            AutoShowButton.BackColor = Color.FromArgb(19, 41, 63);
        }

        private void CloseHighlight()
        {
            CloseButton.BackColor = Color.FromArgb(36, 68, 100);
        }

        private void DisableCloseHighlight()
        {
            CloseButton.BackColor = Color.FromArgb(19, 41, 63);
        }

        private void AutoScrollButton_MouseEnter(object sender, EventArgs e)
        {
            AutoScrollHighlight();
        }

        private void AutoScrollButton_MouseLeave(object sender, EventArgs e)
        {
            DisableAutoScrollHighlight();
        }

        private void AutoScrollButton_Click(object sender, MouseEventArgs e)
        {
            ToggleAutoScroll();
            AutoScrollHighlight();
        }

        private void AutoShowButton_Click(object sender, MouseEventArgs e)
        {
            ToggleAutoShow();
            AutoShowHighlight();
        }

        private void AutoShowButton_MouseEnter(object sender, EventArgs e)
        {
            AutoShowHighlight();
        }

        private void AutoShowButton_MouseLeave(object sender, EventArgs e)
        {
            DisableAutoShowHighlight();
        }

        private void CloseButton_Click(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            CloseHighlight();
        }

        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            DisableCloseHighlight();
        }

        private void SettingsForm_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void SettingsButton_MouseEnter(object sender, EventArgs e)
        {
            SettingsHighlight();
        }

        private void SettingsButton_MouseLeave(object sender, EventArgs e)
        {
            DisableSettingsHighlight();
        }

        private void SettingsHighlight()
        {
            SettingsButton.BackColor = Color.FromArgb(36, 68, 100);
        }

        private void DisableSettingsHighlight()
        {
            SettingsButton.BackColor = Color.FromArgb(19, 41, 63);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Closed = true;
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsShown = true;
            SettingsForm.Show(this);
            SettingsForm.Location = new Point(Location.X + 48, Location.Y + 48);
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (Input.SelectionStart == 0)
                {
                    RevertInput();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (Input.SelectionStart == Input.TextLength)
                {
                    NextInput();
                }
            }
            else
            {
                HistoryIndex = HistoryCurrent;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!TranslatorLoaded)
            {
                Invoke(new Action(() => { Translate.Start(); }));

            }
        }
    }
}
