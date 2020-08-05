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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using TranslateTool.Properties;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using System.Linq;

namespace TranslateTool
{
    public class ClickInfo
    {
        public bool Clicked = false;
        public Action<object, MouseEventArgs> Action;

        public ClickInfo(Action<object, MouseEventArgs> action)
        {
            Action = action;
        }
    }
    public class TickEventArgs : EventArgs
    {
        public double SecondsPassed;

        public TickEventArgs(double secondsPassed)
        {
            SecondsPassed = secondsPassed;
        }

        public double Add(double value, double totalTime)
        {
            return value + (SecondsPassed / totalTime);
        }

        public double Subtract(double value, double totalTime)
        {
            return value - (SecondsPassed / totalTime);
        }
    }
    public class Timer
    {
        private Thread thread;
        private bool enabled = false;
        private Window window;
        private Stopwatch stopwatch;
        public event EventHandler<TickEventArgs> Tick;

        public Timer(Window window, Action<object, TickEventArgs> action)
        {
            this.window = window;
            stopwatch = new Stopwatch();
            Tick = new EventHandler<TickEventArgs>(action);
            thread = new Thread(new ThreadStart(Loop));
        }

        private void Loop()
        {
            while (enabled)
            {
                stopwatch.Stop();
                window.Dispatcher.Invoke(() => { Tick.Invoke(this, new TickEventArgs((double)stopwatch.ElapsedTicks / 10000 / 1000)); });
                stopwatch.Reset();
                stopwatch.Start();
                Thread.Sleep(2);
            }
        }

        public void Start()
        {
            enabled = true;
            thread.Start();
        }

        public void Stop()
        {
            enabled = false;
        }
    }
    public static class ImageResources
    {
        public static ImageBrush ClickThrough = new ImageBrush(BitmapToImageSource(Resources.ClickThrough));
        public static ImageBrush AutoShow = new ImageBrush(BitmapToImageSource(Resources.AutoShow));
        public static ImageBrush AutoScroll = new ImageBrush(BitmapToImageSource(Resources.AutoScroll));
        public static ImageBrush Close = new ImageBrush(BitmapToImageSource(Resources.Close));
        public static ImageBrush Settings = new ImageBrush(BitmapToImageSource(Resources.Settings));
        public static ImageBrush ClickThroughDisabled = new ImageBrush(BitmapToImageSource(Resources.ClickThroughDisabled));
        public static ImageBrush AutoShowDisabled = new ImageBrush(BitmapToImageSource(Resources.AutoShowDisabled));
        public static ImageBrush AutoScrollDisabled = new ImageBrush(BitmapToImageSource(Resources.AutoScrollDisabled));

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
    public partial class MainWindowLogic
    {
        public const string VersionNumber = "1.1.0";
        public const double WaitDelay = 1.5;
        public const double FadeTime = 0.5;
        public const double ShowDelay = 0.2;
        public static Window Form;
        public static Language Language;
        public static System.Windows.Media.Color BackgroundColor = System.Windows.Media.Color.FromRgb(19, 41, 63);
        public static System.Windows.Media.Color TextBackgroundColor = System.Windows.Media.Color.FromRgb(12, 29, 44);

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
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);[DllImport("user32.dll")]
        static extern IntPtr SetCapture(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();

        private bool leftMouseButtonPressed = false;
        private bool partialOpacityEnabled = false;
        private Timer timer;
        private double dpiX;
        private double dpiY;
        private double partialOpacity = 1;
        public bool Moving = false;
        public double TargetOpacity = 1;
        public double LastPosX;
        public double LastPosY;
        public double LastX;
        public double LastY;
        public double Wait;
        public double TimeScale = 0.2;
        public double ToolTipTargetOpacity = 0;
        public bool ToolTipShow = false;
        public bool AutoScrollEnabled = true;
        public bool AutoShowEnabled = false;
        public bool ClickThroughEnabled = false;
        public bool Closed = false;
        public int OldStyle;
        public Dictionary<Canvas, string> ToolTipInfo = new Dictionary<Canvas, string>();
        public Dictionary<UIElement, ClickInfo> MouseClickConfigurationInfo = new Dictionary<UIElement, ClickInfo>();
        public Window ToolTip;
        public IntPtr MainWindowHandle;
        public Window MainWindow;
        public Window SettingsWindow;
        public Grid MainGrid;
        public DockPanel OutputDock;
        public DockPanel InputDock;
        public Canvas ClickThroughButton;
        public Canvas AutoScrollButton;
        public Canvas AutoShowButton;
        public Canvas CloseButton;
        public Canvas SettingsButton;
        public Canvas MoveButton;
        public Canvas AutoScrollButtonBackground;
        public Canvas AutoShowButtonBackground;
        public Canvas CloseButtonBackground;
        public Canvas SettingsButtonBackground;
        public RichTextBox Output;
        public TextBox Input;
        public CheckBox PreventTransparencyCheck;
        public CheckBox ShowAdditionalInfoCheck;
        public CheckBox PartialOpacityCheck;
        public CheckBox ColourChatMessagesCheck;
        public bool SettingsShown = false;
        public bool AutoHideEnabled = true;
        public string[] InputHistory = new string[256];
        public byte HistoryIndex = 0;
        public byte HistoryCurrent = 0;
        public bool TranslatorLoaded = false;

        public MainWindowLogic()
        {
        }

        public double PartialOpacity
        {
            get
            {
                return partialOpacity;
            }
            set
            {
                var alphaWindow = (byte)(Math.Round(value * 255));
                var alphaOutputBackground = (byte)(75 + Math.Round(value * 180));

                MainWindow.Background =
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaWindow, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
                OutputDock.Background =
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaOutputBackground, TextBackgroundColor.R, TextBackgroundColor.G, TextBackgroundColor.B));
                MainWindow.Resources["ScrollOpacity"] = 0.2 + (value * 0.8);
                InputDock.Opacity = value;
                ClickThroughButton.Opacity = value;
                SettingsButton.Opacity = value;
                AutoShowButton.Opacity = value;
                CloseButton.Opacity = value;
                AutoScrollButton.Opacity = value;
                partialOpacity = value;
            }
        }

        public double InterfaceOpacity
        {
            get
            {
                if (partialOpacityEnabled)
                {
                    return partialOpacity;
                }
                else
                {
                    return MainWindow.Opacity;
                }
            }
            set
            {
                if (partialOpacityEnabled)
                {
                    PartialOpacity = value;
                }
                else
                {
                    MainWindow.Opacity = value;
                }
            }
        }

        public void ToolTipEnter(object sender, MouseEventArgs e)
        {
            HandleToolTip((Canvas)sender);
        }

        public void ToolTipLeave(object sender, MouseEventArgs e)
        {
            CancelToolTip();
        }

        public void SetToolTip(Canvas control, string text)
        {
            ToolTipInfo[control] = text;
            control.MouseEnter += ToolTipEnter;
            control.MouseLeave += ToolTipLeave;
        }

        public void HandleToolTip(Canvas control)
        {
            if (ToolTipInfo.TryGetValue(control, out string text))
            {

                var textSize = System.Windows.Forms.TextRenderer.MeasureText(text, new System.Drawing.Font(ToolTip.FontFamily.Source, (float)ToolTip.FontSize, System.Drawing.FontStyle.Regular), new System.Drawing.Size(260, 500), System.Windows.Forms.TextFormatFlags.WordBreak);

                ToolTip.Width = textSize.Width;
                ToolTip.Height = textSize.Height;
                ToolTip.Left = System.Windows.Forms.Cursor.Position.X;
                ToolTip.Top = System.Windows.Forms.Cursor.Position.Y + (System.Windows.Forms.Cursor.Current.Size.Height / 2);
                ToolTipTargetOpacity = 1;
            }
        }

        public void Invoke(Action action)
        {
            MainWindow.Dispatcher.Invoke(action);
        }

        public void CancelToolTip()
        {
            ToolTipTargetOpacity = 0;
        }

        public void ApplySettings()
        {
            MainWindow.Left = Settings.Default.WindowLocation.X;
            MainWindow.Top = Settings.Default.WindowLocation.Y;
            MainWindow.Width = Settings.Default.WindowSize.Width;
            MainWindow.Height = Settings.Default.WindowSize.Height;
            AutoHideEnabled = !Settings.Default.PreventWindowTransparency;
            PreventTransparencyCheck.IsChecked = Settings.Default.PreventWindowTransparency;
            partialOpacityEnabled = Settings.Default.PartialOpacity;
            PartialOpacityCheck.IsChecked = Settings.Default.PartialOpacity;

            if(Settings.Default.ColourChatMessages)
            {
                ColourChatMessagesCheck.IsChecked = true;
            }
            else
            {
                ColourChatMessagesCheck.IsChecked = false;
            }
        }

        public void ConfigureMouseClickEvent(UIElement element, Action<object, MouseEventArgs> action)
        {
            MouseClickConfigurationInfo[element] = new ClickInfo(action);
            element.MouseDown += new MouseButtonEventHandler((sender, e) =>
            {
                MouseClickConfigurationInfo[element].Clicked = true;
            });
            element.MouseUp += new MouseButtonEventHandler((sender, e) =>
            {
                var info = MouseClickConfigurationInfo[element];

                if (info.Clicked)
                {
                    info.Clicked = false;
                    info.Action(sender, e);
                }
            });
            element.MouseLeave += new MouseEventHandler((sender, e) =>
            {
                MouseClickConfigurationInfo[element].Clicked = false;
            });

        }

        public void Load(IntPtr mainWindowHandle, Window mainWindow, Window settingsWindow, Grid mainGrid, DockPanel outputDock, DockPanel inputDock, Canvas clickThroughButton, Canvas autoShowButton, Canvas autoScrollButton, Canvas settingsButton, Canvas closeButton, Canvas autoShowButtonBackground, Canvas autoScrollButtonBackground, Canvas settingsButtonBackground, Canvas closeButtonBackground, Canvas moveButton, RichTextBox output, TextBox input, CheckBox preventTransparencyCheck, CheckBox showAdditionalInfoCheck, CheckBox partialOpacityCheck, CheckBox colourChatMessagesCheck)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
            var h = HwndSource.FromVisual(mainWindow) as HwndSource;

            h.AddHook(WndProc);
            dpiX = (double)96 / (int)dpiXProperty.GetValue(null, null);
            dpiY = (double)96 / (int)dpiYProperty.GetValue(null, null);
            SettingsWindow = settingsWindow;
            MainWindow = mainWindow;
            MainGrid = mainGrid;
            OutputDock = outputDock;
            InputDock = inputDock;
            ClickThroughButton = clickThroughButton;
            AutoShowButton = autoShowButton;
            AutoScrollButton = autoScrollButton;
            SettingsButton = settingsButton;
            CloseButton = closeButton;
            MoveButton = moveButton;
            AutoShowButtonBackground = autoShowButtonBackground;
            AutoScrollButtonBackground = autoScrollButtonBackground;
            SettingsButtonBackground = settingsButtonBackground;
            CloseButtonBackground = closeButtonBackground;
            Output = output;
            Input = input;
            PreventTransparencyCheck = preventTransparencyCheck;
            ShowAdditionalInfoCheck = showAdditionalInfoCheck;
            PartialOpacityCheck = partialOpacityCheck;
            ColourChatMessagesCheck = colourChatMessagesCheck;
            MainWindow.Closing += WindowClosing;
            MainWindow.Closed += WindowClosed;
            Input.KeyDown += Input_KeyDown;
            Input.PreviewKeyDown += Input_PreviewKeyDown;
            SettingsWindow.Deactivated += SettingsWindow_Deactivate;
            AutoShowButton.MouseEnter += AutoShowButton_MouseEnter;
            AutoShowButton.MouseLeave += AutoShowButton_MouseLeave;
            AutoScrollButton.MouseEnter += AutoScrollButton_MouseEnter;
            AutoScrollButton.MouseLeave += AutoScrollButton_MouseLeave;
            SettingsButton.MouseEnter += SettingsButton_MouseEnter;
            SettingsButton.MouseLeave += SettingsButton_MouseLeave;
            CloseButton.MouseEnter += CloseButton_MouseEnter;
            CloseButton.MouseLeave += CloseButton_MouseLeave;
            MoveButton.MouseDown += MoveStart;
            PreventTransparencyCheck.Checked += PreventTransparencyCheck_Checked;
            PreventTransparencyCheck.Unchecked += PreventTransparencyCheck_UnChecked;
            ShowAdditionalInfoCheck.Checked += ShowAdditionalInfoCheck_Checked;
            ShowAdditionalInfoCheck.Unchecked += ShowAdditionalInfoCheck_UnChecked;
            PartialOpacityCheck.Checked += PartialOpacityCheck_Checked;
            PartialOpacityCheck.Unchecked += PartialOpacityCheck_UnChecked;
            ColourChatMessagesCheck.Checked += ColourChatMessagesCheck_Checked;
            ColourChatMessagesCheck.Unchecked += ColourChatMessagesCheck_UnChecked;
            ConfigureMouseClickEvent(AutoShowButton, AutoShowButton_Click);
            ConfigureMouseClickEvent(AutoScrollButton, AutoScrollButton_Click);
            ConfigureMouseClickEvent(SettingsButton, SettingsButton_Click);
            ConfigureMouseClickEvent(CloseButton, CloseButton_Click);
            DisableAutoShowHighlight();
            DisableAutoScrollHighlight();
            DisableSettingsHighlight();
            DisableCloseHighlight();
            ClickThroughButton.Background = ImageResources.ClickThroughDisabled;
            AutoShowButton.Background = ImageResources.AutoShowDisabled;
            AutoScrollButton.Background = ImageResources.AutoScroll;
            CloseButton.Background = ImageResources.Close;
            SettingsButton.Background = ImageResources.Settings;
            ApplySettings();
            RegisterHotKey(mainWindowHandle, 1, 2, (int)System.Windows.Forms.Keys.T);
            ApplyTranslation(new Language(VersionNumber));
            Wait = 4;
            Translate.Start(this);
            timer = new Timer(MainWindow, OnTick);
            timer.Start();
        }

        private void PreventTransparencyCheck_Checked(object sender, RoutedEventArgs e)
        {
            AutoHideEnabled = false;
            Settings.Default.PreventWindowTransparency = false;
        }

        private void PreventTransparencyCheck_UnChecked(object sender, RoutedEventArgs e)
        {
            AutoHideEnabled = true;
            Settings.Default.PreventWindowTransparency = true;
        }

        private void ShowAdditionalInfoCheck_Checked(object sender, RoutedEventArgs e)
        {
            Translate.EnableAdditionalInputInfo();
            Settings.Default.InputInfoEnabled = true;
        }

        private void ShowAdditionalInfoCheck_UnChecked(object sender, RoutedEventArgs e)
        {
            Translate.DisableAdditionalInputInfo();
            Settings.Default.InputInfoEnabled = false;
        }

        private void PartialOpacityCheck_Checked(object sender, RoutedEventArgs e)
        {
            partialOpacityEnabled = true;
            Settings.Default.PartialOpacity = true;
        }

        private void PartialOpacityCheck_UnChecked(object sender, RoutedEventArgs e)
        {
            PartialOpacity = 1;
            partialOpacityEnabled = false;
            Settings.Default.PartialOpacity = false;
        }

        private void ColourChatMessagesCheck_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ColourChatMessages = true;
            Translate.EnableColourChat();
        }

        private void ColourChatMessagesCheck_UnChecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ColourChatMessages = false;
            Translate.DisableColourChat();
        }

        public void RevertInput()
        {
            var targetIndex = (byte)(HistoryIndex - 1);

            if (InputHistory[targetIndex] != null)
            {
                Input.Clear();
                Input.AppendText(InputHistory[targetIndex]);
                HistoryIndex--;
            }
        }

        public void NextInput()
        {
            var targetIndex = (byte)(HistoryIndex + 1);

            if (InputHistory[targetIndex] != null)
            {
                Input.Clear();
                Input.AppendText(InputHistory[targetIndex]);
                HistoryIndex++;
                Input.Select(Input.Text.Length, 0);
            }
        }

        public IntPtr WndProc(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (message == 0x0312 && wParam.ToInt32() == 1)
            {
                ToggleClickThrough(windowHandle);
            }
            else if (message == 0x201)
            {
                SetCapture(windowHandle);
                leftMouseButtonPressed = true;
            }
            else if (message == 0x202)
            {
                ReleaseCapture();
                leftMouseButtonPressed = false;
            }
            return IntPtr.Zero;
        }

        public void ToggleClickThrough(IntPtr windowHandle)
        {
            if (!ClickThroughEnabled)
            {
                OldStyle = GetWindowLong(windowHandle, -20);

                ClickThroughEnabled = true;
                SetWindowLong(windowHandle, -20, OldStyle | 0x80000 | 0x20);
                ClickThroughButton.Background = ImageResources.ClickThrough;
            }
            else
            {
                SetWindowLong(windowHandle, -20, OldStyle);
                ClickThroughEnabled = false;
                ClickThroughButton.Background = ImageResources.ClickThroughDisabled;
            }
        }

        public void ToggleAutoShow()
        {
            if (!AutoShowEnabled)
            {
                AutoShowEnabled = true;
                AutoShowButton.Background = ImageResources.AutoShow;
            }
            else
            {
                AutoShowEnabled = false;
                AutoShowButton.Background = ImageResources.AutoShowDisabled;
            }
        }

        public void ToggleAutoScroll()
        {
            if (!AutoScrollEnabled)
            {
                AutoScrollEnabled = true;
                AutoScrollButton.Background = ImageResources.AutoScroll;
            }
            else
            {
                AutoScrollEnabled = false;
                AutoScrollButton.Background = ImageResources.AutoScrollDisabled;
            }
        }

        public void WriteLine(string text, System.Windows.Media.Color color)
        {
            MainWindow.Dispatcher.Invoke(() =>
            {
                var textRange = new TextRange(Output.Document.ContentEnd, Output.Document.ContentEnd);

                textRange.Text = text;
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
                Output.AppendText("\r\n");

                if (AutoScrollEnabled)
                {
                    Output.ScrollToEnd();
                }

            });
        }

        public void WriteLine(string text)
        {
            Action<string> action;
            if (AutoScrollEnabled)
            {
                action = (value) =>
                {
                    Output.AppendText(value);
                    Output.ScrollToEnd();
                };
            }
            else
            {
                action = Output.AppendText;
            }
            MainWindow.Dispatcher.Invoke(action, text + "\r\n");

            if (AutoShowEnabled)
            {
                TargetOpacity = 1;
                Wait = 4;
            }
        }

        public void ApplyTranslation(Language language)
        {
            Action action = () =>
            {
                Language = language;
                WriteLine(language.Text[8]);
                WriteLine(language.Text[11]);
                ClickThroughButton.ToolTip = language.Text[12];
                AutoScrollButton.ToolTip = language.Text[13];
                AutoShowButton.ToolTip = language.Text[14];
                CloseButton.ToolTip = language.Text[15];
                PreventTransparencyCheck.Content = language.Text[17];
                PartialOpacityCheck.Content = language.Text[19];
                ColourChatMessagesCheck.Content = language.Text[21];

                if (language.IsJapanese)
                {
                    ShowAdditionalInfoCheck.IsEnabled = false;
                    ShowAdditionalInfoCheck.IsChecked = false;
                    Translate.DisableAdditionalInputInfo();
                }
                else
                {
                    ShowAdditionalInfoCheck.IsEnabled = true;
                    ShowAdditionalInfoCheck.IsChecked = Settings.Default.InputInfoEnabled;
                    if (ShowAdditionalInfoCheck.IsChecked ?? false)
                    {
                        Translate.EnableAdditionalInputInfo();
                    }
                }
                ShowAdditionalInfoCheck.Content = language.Text[16];
            };
            MainWindow.Dispatcher.Invoke(action);
        }

        public void MoveStart(object sender, EventArgs e)
        {
            Moving = true;
            LastPosX = MainWindow.Left;
            LastPosY = MainWindow.Top;
            LastX = System.Windows.Forms.Cursor.Position.X * dpiX;
            LastY = System.Windows.Forms.Cursor.Position.Y * dpiY;
        }

        public void MoveButtonMouseUp(object sender, EventArgs e)
        {
            MoveEnd();
        }

        public void MoveEnd()
        {
            Moving = false;
            Settings.Default.WindowLocation = new System.Drawing.Point((int)MainWindow.Left, (int)MainWindow.Top);
            Settings.Default.Save();
        }

        private void OnTick(object sender, TickEventArgs e)
        {
            var bounds = new Rect(new System.Windows.Point(MainWindow.Left, MainWindow.Top), new System.Windows.Point(MainWindow.RestoreBounds.Right, MainWindow.RestoreBounds.Bottom));
            var cursorPosition = new System.Windows.Point(System.Windows.Forms.Cursor.Position.X * dpiX, System.Windows.Forms.Cursor.Position.Y * dpiY);

            bounds.Offset(-6, -6);
            bounds.Width += 12;
            bounds.Height += 12;

            if (bounds.Contains(cursorPosition))
            {
                TargetOpacity = 1;

                if (InterfaceOpacity == 1)
                {
                    Wait = 1;
                }
            }
            else
            {
                if (InterfaceOpacity == 1 && Wait > 0)
                {
                    Wait = e.Subtract(Wait, WaitDelay);
                }
                else if (Wait <= 0)
                {
                    TargetOpacity = 0;
                }
            }
            if (Moving)
            {
                if (!leftMouseButtonPressed)
                {
                    MoveEnd();
                }
                else if (Moving)
                {
                    var newPos = new System.Windows.Point(LastPosX + (cursorPosition.X - LastX), LastPosY + (cursorPosition.Y - LastY));

                    MainWindow.Left = newPos.X;
                    MainWindow.Top = newPos.Y;
                }
                TargetOpacity = 1;
            }
            if (AutoHideEnabled)
            {
                if (TargetOpacity != InterfaceOpacity)
                {
                    if (InterfaceOpacity < TargetOpacity)
                    {
                        InterfaceOpacity = e.Add(InterfaceOpacity, ShowDelay);
                    }
                    else
                    {
                        if (Wait > 0)
                        {
                            Wait = e.Subtract(Wait, WaitDelay);
                        }
                        else
                        {
                            InterfaceOpacity = e.Subtract(InterfaceOpacity, FadeTime);
                        }
                    }
                    if (InterfaceOpacity > 1)
                    {
                        InterfaceOpacity = 1;
                    }
                    else if (InterfaceOpacity < 0)
                    {
                        InterfaceOpacity = 0;
                    }
                }
            }
            else
            {
                if (InterfaceOpacity != 1)
                {
                    InterfaceOpacity = 1;
                }
            }
        }

        public void SetInputValue(string text)
        {
            Action<string> action = (input) =>
            {
                Input.Text = input;
            };

            MainWindow.Dispatcher.Invoke(action, text);
        }

        private void TranslateInput()
        {
            if (Input.Text != "")
            {
                InputHistory[HistoryCurrent++] = Input.Text;
                HistoryIndex = HistoryCurrent;
            }
            Translate.Input = Input.Text;
        }

        private void WindowClosing(object sender, EventArgs e)
        {
            SettingsWindow.Close();
            timer.Stop();
            Translate.StopBrowsers();
        }

        private void AutoScrollHighlight()
        {
            AutoScrollButtonBackground.Opacity = 1;
        }

        private void DisableAutoScrollHighlight()
        {
            AutoScrollButtonBackground.Opacity = 0;
        }

        private void AutoShowHighlight()
        {
            AutoShowButtonBackground.Opacity = 1;
        }

        private void DisableAutoShowHighlight()
        {
            AutoShowButtonBackground.Opacity = 0;
        }

        private void CloseHighlight()
        {
            CloseButtonBackground.Opacity = 1;
        }

        private void DisableCloseHighlight()
        {
            CloseButtonBackground.Opacity = 0;
        }

        private void SettingsHighlight()
        {
            SettingsButtonBackground.Opacity = 1;
        }

        private void DisableSettingsHighlight()
        {
            SettingsButtonBackground.Opacity = 0;
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

        private void AutoShowButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AutoShowHighlight();
        }

        private void AutoShowButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DisableAutoShowHighlight();
        }

        private void SettingsButton_MouseEnter(object sender, MouseEventArgs e)
        {
            SettingsHighlight();
        }

        private void SettingsButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DisableSettingsHighlight();
        }

        private void CloseButton_Click(object sender, MouseEventArgs e)
        {
            MainWindow.Close();
        }

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseHighlight();
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DisableCloseHighlight();
        }

        private void SettingsWindow_Deactivate(object sender, EventArgs e)
        {
            SettingsWindow.Hide();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Closed = true;
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsShown = true;
            SettingsWindow.Left = MainWindow.Left + 48;
            SettingsWindow.Top = MainWindow.Top + 48;
            SettingsWindow.Show();
        }

        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (Input.SelectionStart == 0)
                {
                    RevertInput();
                }
            }
            else if (e.Key == Key.Down)
            {
                if (Input.SelectionStart == Input.Text.Length)
                {
                    NextInput();
                }
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                TranslateInput();
            }
            else
            {
                HistoryIndex = HistoryCurrent;
            }
        }
    }
}
