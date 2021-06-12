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
using System.Drawing.Imaging;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Interop;
using TranslateTool.Properties;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media.TextFormatting;

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
        public int precision = 16;
        public event EventHandler<TickEventArgs> Tick;

        public Timer(Window window, int precision, Action<object, TickEventArgs> action)
        {
            this.precision = precision;
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
                window.Dispatcher.Invoke(() => { Tick.Invoke(this, new TickEventArgs((double)stopwatch.ElapsedTicks / 10000000)); });
                stopwatch.Reset();
                stopwatch.Start();
                Thread.Sleep(precision);
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
    public class DelayHandler
    {
        private static List<DelayHandler> activeInstances = new List<DelayHandler>();

        public static void Stop()
        {
            foreach (var instance in activeInstances)
            {
                instance.timer.Stop();
            }
        }

        private MainWindowLogic window;
        private System.Windows.Controls.Image image;
        private InlineUIContainer imageContainer;
        private Paragraph paragraph;
        private Timer timer;
        private int imageIndex;
        private double frameDelay;
        System.Windows.Media.Color textColor;

        public bool Finished { get; private set; } = false;

        public DelayHandler(MainWindowLogic window, System.Windows.Media.Color color)
        {
            window.Invoke(() =>
            {
                this.window = window;
                textColor = color;
                image = new System.Windows.Controls.Image();
                imageContainer = new InlineUIContainer();
                paragraph = window.Output.Document.Blocks.LastBlock as Paragraph;
                image.Source = ImageResources.Loading[0].ImageSource;
                image.MaxHeight = 22;
                image.MaxWidth = 22;
                imageContainer.Child = image;
                paragraph.Inlines.Add(imageContainer);
                frameDelay = 0.1;
                imageIndex = 1;
                window.Output.AppendText("\r\n");

                var timer = new Timer(window.MainWindow, 50, (e, args) =>
                {
                    frameDelay -= args.SecondsPassed;

                    if (frameDelay <= 0)
                    {
                        frameDelay = 0.1;
                        image.Source = ImageResources.Loading[imageIndex++].ImageSource;
                        if (imageIndex > 7)
                        {
                            imageIndex = 0;
                        }
                    }
                });
                this.timer = timer;
                timer.Start();
                if(MainWindowLogic.AutoScrollEnabled)
                {
                    window.Output.ScrollToEnd();
                }
            });
        }

        public DelayHandler(MainWindowLogic window) : this(window, MainWindowLogic.TextColor)
        {

        }

        public void Resolve(string text)
        {
            window.Invoke(() =>
            {
                //paragraph.Inlines.Remove(imageContainer);
                //imageContainer.Child = null;
                var textRange = new TextRange(imageContainer.ContentStart, imageContainer.ContentEnd);
                window.UpdateTextSpan(text, textRange, textColor);
            });
            timer.Stop();
            Finished = true;
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
        public static ImageBrush Minimise = new ImageBrush(BitmapToImageSource(Resources.Minimise));
        public static ImageBrush[] Loading = new ImageBrush[8];

        static ImageResources()
        {
            Loading[0] = new ImageBrush(BitmapToImageSource(Resources.loading1));
            Loading[1] = new ImageBrush(BitmapToImageSource(Resources.loading2));
            Loading[2] = new ImageBrush(BitmapToImageSource(Resources.loading3));
            Loading[3] = new ImageBrush(BitmapToImageSource(Resources.loading4));
            Loading[4] = new ImageBrush(BitmapToImageSource(Resources.loading5));
            Loading[5] = new ImageBrush(BitmapToImageSource(Resources.loading6));
            Loading[6] = new ImageBrush(BitmapToImageSource(Resources.loading7));
            Loading[7] = new ImageBrush(BitmapToImageSource(Resources.loading8));
        }

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
    public struct Vector2
    {
        public int X;
        public int Y;

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public partial class MainWindowLogic
    {
        public const int DEFAULTWIDTH = 355;
        public const int DEFAULTHEIGHT = 400;
        public const string VersionNumber = "12.6.21";
        public const double WaitDelay = 1.5;
        public const double FadeTime = 0.5;
        public const double ShineTime = 3;
        public const double ShowDelay = 0.2;
        public static Window Form;
        public static Language Language;
        public static System.Windows.Media.Color BackgroundColor = System.Windows.Media.Color.FromRgb(19, 41, 63);
        public static System.Windows.Media.Color TextBackgroundColor = System.Windows.Media.Color.FromRgb(12, 29, 44);
        public static System.Windows.Media.Color TextColor = System.Windows.Media.Color.FromRgb(255, 255, 255);

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
        private Timer timer;
        private double dpiX;
        private double dpiY;
        private double partialOpacity = 1;
        public bool Moving = false;
        public double TargetOpacity = 1;
        public double ForceTargetOpacity = 0;
        public double LastPosX;
        public double LastPosY;
        public double LastX;
        public double LastY;
        public double Wait;
        public double ForceWait = 1;
        public double TimeScale = 0.2;
        public bool ToolTipShow = false;
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
        public Canvas MinimiseButton;
        public Canvas AutoScrollButtonBackground;
        public Canvas AutoShowButtonBackground;
        public Canvas CloseButtonBackground;
        public Canvas MinimiseButtonBackground;
        public Canvas SettingsButtonBackground;
        public RichTextBox Output;
        public TextBox Input;
        public bool SettingsShown = false;
        public string[] InputHistory = new string[256];
        public byte HistoryIndex = 0;
        public byte HistoryCurrent = 0;
        public bool TranslatorLoaded = false;
        public bool WasColouredLine = false;
        public static bool AutoScrollEnabled = true;
        public double ShineOpacityModifierValue = 0;

        public MainWindowLogic()
        {
        }

        public double ShineOpacityModifier
        {
            get
            {
                return ShineOpacityModifierValue;
            }
            set
            {
                ShineOpacityModifierValue = value;
                MainWindow.Resources["ShineOpacity"] = InterfaceOpacity * ShineOpacityModifierValue;
            }
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
                var alphaOutputBackground = (byte)(95 + Math.Round(value * 160));

                MainGrid.Background =
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaWindow, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
                OutputDock.Background =
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaOutputBackground, TextBackgroundColor.R, TextBackgroundColor.G, TextBackgroundColor.B));
                MainWindow.Resources["ScrollOpacity"] = 0.2 + (value * 0.8);
                MainWindow.Resources["ShineOpacity"] = value * ShineOpacityModifierValue;
                InputDock.Opacity = value;
                ClickThroughButton.Opacity = value;
                SettingsButton.Opacity = value;
                AutoShowButton.Opacity = value;
                CloseButton.Opacity = value;
                AutoScrollButton.Opacity = value;
                MinimiseButton.Opacity = value;
                partialOpacity = value;
            }
        }


        public double InterfaceOpacity
        {
            get
            {
                if (Settings.PartialTransparency)
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
                if (Settings.PartialTransparency)
                {
                    PartialOpacity = value;
                }
                else
                {
                    MainWindow.Opacity = value;
                }
            }
        }

        public void Invoke(Action action)
        {
            MainWindow.Dispatcher.Invoke(action);
        }

        public void ApplySettings()
        {
            MainWindow.Left = Settings.WindowPosition.Value.X;
            MainWindow.Top = Settings.WindowPosition.Value.Y;
            MainWindow.Width = Settings.WindowSize.Value.X;
            MainWindow.Height = Settings.WindowSize.Value.Y;

            if (Settings.ColourChat)
            {
                Translate.EnableColourChat();
            }
            else
            {
                Translate.DisableColourChat();
            }
            if (Settings.ClickthroughKeyEnabled)
            {
                Settings.ClickthroughKey.Value.Register(MainWindowHandle,1);
            }
            if (Settings.OCRKeyEnabled)
            {
                Settings.OCRKey.Value.Register(MainWindowHandle,2);
            }
            ((ScaleTransform)MainWindow.Resources["ShineScale"]).ScaleX = MainWindow.Width / DEFAULTWIDTH;
            ((ScaleTransform)MainWindow.Resources["ShineScale"]).ScaleY = MainWindow.Height / DEFAULTHEIGHT;
        }
        public void EnhanceImageClarity(string input, string output)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(input);
            System.Drawing.Image adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);
            var brightness = 1.2f;
            var contrast = 1f; 
            var gamma = 2f; 
            var adjustedBrightness = brightness - 1.0f;
            float[][] ptsArray ={new float[] {contrast, 0, 0, 0, 0}, new float[] {0, contrast, 0, 0, 0}, new float[] {0, 0, contrast, 0, 0}, new float[] {0, 0, 0, 1.0f, 0}, new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(originalImage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height)
                , 0, 0, originalImage.Width, originalImage.Height,
                GraphicsUnit.Pixel, imageAttributes);
            adjustedImage.Save(output);
        }
        public void TranslateRecentScreenshot()
        {
            var directory = new DirectoryInfo(Settings.OCRDirectory.Value);
            var mostRecentTime = new DateTime(0);
            FileInfo latestFile = null;

            foreach (var file in directory.GetFiles())
            {
                if (file.CreationTime > mostRecentTime)
                {
                    mostRecentTime = file.CreationTime;
                    latestFile = file;
                }
            }
            if (latestFile != null)
            {
                string result;
                var process = new Process();

                EnhanceImageClarity(latestFile.FullName, "Tesseract-OCR\\input");
                process.StartInfo.WorkingDirectory = "Tesseract-OCR";
                process.StartInfo.FileName = "Tesseract-OCR\\tesseract.exe";
                process.StartInfo.Arguments = "input output -l jpn";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                Console.WriteLine(process.StandardOutput.ReadToEnd());
                result = File.ReadAllText("Tesseract-OCR\\output.txt").Trim();
                if (result.Length > 0)
                {
                    Translate.TranslateChat("OCR", result, TextColor);
                }
                else
                {
                    WriteLine("Image recognition failed.");
                }
                process.Dispose();
                File.Delete("Tesseract-OCR\\output.txt");
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

        public void Load(IntPtr mainWindowHandle, Window mainWindow, Window settingsWindow, Grid mainGrid, DockPanel outputDock, DockPanel inputDock, Canvas clickThroughButton, Canvas autoShowButton, Canvas autoScrollButton, Canvas settingsButton, Canvas closeButton, Canvas autoShowButtonBackground, Canvas autoScrollButtonBackground, Canvas settingsButtonBackground, Canvas closeButtonBackground, Canvas moveButton, Canvas minimiseButton, Canvas minimiseButtonBackground, RichTextBox output, TextBox input)
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
            var h = HwndSource.FromVisual(mainWindow) as HwndSource;

            MainWindowHandle = mainWindowHandle;
            BackgroundColor = ((SolidColorBrush)mainWindow.Resources["BaseColour"]).Color;
            TextBackgroundColor = ((SolidColorBrush)mainWindow.Resources["SubsectionColour"]).Color;
            TextColor = ((SolidColorBrush)mainWindow.Resources["TextColour"]).Color;
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
            MinimiseButton = minimiseButton;
            MoveButton = moveButton;
            AutoShowButtonBackground = autoShowButtonBackground;
            AutoScrollButtonBackground = autoScrollButtonBackground;
            SettingsButtonBackground = settingsButtonBackground;
            CloseButtonBackground = closeButtonBackground;
            MinimiseButtonBackground = minimiseButtonBackground;
            Output = output;
            Input = input;
            ClickThroughButton.Background = ImageResources.ClickThroughDisabled;
            AutoShowButton.Background = ImageResources.AutoShowDisabled;
            AutoScrollButton.Background = ImageResources.AutoScroll;
            CloseButton.Background = ImageResources.Close;
            SettingsButton.Background = ImageResources.Settings;
            MinimiseButton.Background = ImageResources.Minimise;
            DisableAutoShowHighlight();
            DisableAutoScrollHighlight();
            DisableSettingsHighlight();
            DisableCloseHighlight();
            DisableMinimiseHighlight();
            ApplySettings();
            ApplyTranslation(Settings.Language);
            MainWindow.Closing += WindowClosing;
            MainWindow.Closed += WindowClosed;
            MainWindow.SizeChanged += MainWindow_SizeChanged;
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
            MinimiseButton.MouseEnter += MinimiseButton_MouseEnter;
            MinimiseButton.MouseLeave += MinimiseButton_MouseLeave;
            MoveButton.MouseDown += MoveStart;
            Settings.AdditionalInfo.OnChanged += AdditionalInfoSettingChanged;
            Settings.PartialTransparency.OnChanged += PartialOpacitySettingChanged;
            Settings.ColourChat.OnChanged += ColourChatMessagesSettingChanged;
            Settings.ClickthroughKeyEnabled.OnChanged += EnableClickthroughKeySettingChanged;
            Settings.ClickthroughKey.OnChanged += ClickthroughKeySettingChanged;
            Settings.OCRKey.OnChanged += OCRKeySettingChanged;
            Settings.OCRKeyEnabled.OnChanged += EnableOCRKeySettingChanged;
            Settings.OnLanguageChanged += LanguageSettingChanged;
            Settings.AutoShow.OnChanged += AutoShowSettingChanged;
            ConfigureMouseClickEvent(AutoShowButton, AutoShowButton_Click);
            ConfigureMouseClickEvent(AutoScrollButton, AutoScrollButton_Click);
            ConfigureMouseClickEvent(SettingsButton, SettingsButton_Click);
            ConfigureMouseClickEvent(CloseButton, CloseButton_Click);
            ConfigureMouseClickEvent(MinimiseButton, MinimiseButton_Click);
            Wait = 4;
            //MainWindow.Hide();
            Translate.Start(this);
            timer = new Timer(MainWindow, 16, OnTick);
            timer.Start();
            Settings.FirstTimeStartup.Value = false;
        }

        private void LanguageSettingChanged(object sender, SettingChangedEventArgs<UserLanguage> e)
        {
            if (e.NewValue != e.OldValue)
            {
                var language = new Language(VersionNumber,e.NewValue);

                Translate.Language = language;
                ApplyTranslation(language);
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Settings.WindowSize.Value = new Vector2((int)MainWindow.Width, (int)MainWindow.Height);
            ((ScaleTransform)MainWindow.Resources["ShineScale"]).ScaleX = MainWindow.Width / DEFAULTWIDTH;
            ((ScaleTransform)MainWindow.Resources["ShineScale"]).ScaleY = MainWindow.Height / DEFAULTHEIGHT;
        }

        private void AdditionalInfoSettingChanged(object sender, SettingChangedEventArgs<bool> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue)
                {
                    Translate.EnableAdditionalInputInfo();
                }
                else
                {
                    Translate.DisableAdditionalInputInfo();
                }
            }
        }

        private void PartialOpacitySettingChanged(object sender, SettingChangedEventArgs<bool> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue == false)
                {
                    PartialOpacity = 1;
                }
            }
        }

        private void ColourChatMessagesSettingChanged(object sender, SettingChangedEventArgs<bool> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue)
                {
                    Translate.EnableAdditionalInputInfo();
                }
                else
                {
                    Translate.DisableAdditionalInputInfo();
                }
            }
        }

        private void EnableClickthroughKeySettingChanged(object sender, SettingChangedEventArgs<bool> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue)
                {
                    Settings.ClickthroughKey.Value.Register(MainWindowHandle,1);
                }
                else
                {
                    if(ClickThroughEnabled)
                    {
                        ToggleClickThrough(MainWindowHandle);
                    }
                    Settings.ClickthroughKey.Value.Unregister(MainWindowHandle,1);
                }
            }
        }

        private void ClickthroughKeySettingChanged(object sender, SettingChangedEventArgs<Hotkey> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (Settings.ClickthroughKeyEnabled)
                {
                    e.OldValue.Unregister(MainWindowHandle, 1);
                    e.NewValue.Register(MainWindowHandle, 1);
                }
            }
        }

        private void EnableOCRKeySettingChanged(object sender, SettingChangedEventArgs<bool> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue)
                {
                    Settings.OCRKey.Value.Register(MainWindowHandle,2);
                }
                else
                {
                    Settings.OCRKey.Value.Unregister(MainWindowHandle, 2);
                }
            }
        }

        private void OCRKeySettingChanged(object sender, SettingChangedEventArgs<Hotkey> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (Settings.OCRKeyEnabled)
                {
                    e.OldValue.Unregister(MainWindowHandle, 1);
                    e.NewValue.Register(MainWindowHandle, 1);
                }
            }
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
            if (message == 0x0312)
            {
                if (wParam.ToInt32() == 1)
                {
                    ToggleClickThrough(windowHandle);
                }
                else if (wParam.ToInt32() == 2)
                {
                    TranslateRecentScreenshot();
                }
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
            Settings.AutoShow.Value = !Settings.AutoShow;
        }

        public void AutoShowSettingChanged(object sender, SettingChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                AutoShowButton.Background = ImageResources.AutoShow;
            }
            else
            {
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

        public void UpdateTextSpan(string text, TextRange textRange, System.Windows.Media.Color color)
        {
            WasColouredLine = true;
            MainWindow.Dispatcher.Invoke(() =>
            {
                textRange.Text = $"{text}";
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

                if (AutoScrollEnabled)
                {
                    Output.ScrollToEnd();
                }

            });

            if (Settings.AutoShow)
            {
                if (Settings.PartialTransparency)
                {
                    ForceTargetOpacity = 1;
                    ForceWait += 1.1 + (text.Length * 0.09);
                }
                else
                {
                    TargetOpacity = 1;
                    Wait = 1.1 + (text.Length * 0.09);
                }
            }

        }

        public void WriteLine(string text, System.Windows.Media.Color color)
        {
            WasColouredLine = true;
            MainWindow.Dispatcher.Invoke(() =>
            {
                var textRange = new TextRange(Output.Document.Blocks.LastBlock.ContentEnd, Output.Document.Blocks.LastBlock.ContentEnd);

                textRange.Text = $"{text}\r\n";
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

                if (AutoScrollEnabled)
                {
                    Output.ScrollToEnd();
                }

            });

            if (Settings.AutoShow)
            {
                if (Settings.PartialTransparency)
                {
                    ForceTargetOpacity = 1;
                    ForceWait += 1.1 + (text.Length * 0.09);
                }
                else
                {
                    TargetOpacity = 1;
                    Wait = 1.1 + (text.Length * 0.09);
                }
            }
        }

        public void WriteLine(string text)
        {
            WriteLine(text, TextColor);
            WasColouredLine = false;
        }

        public void ApplyTranslation(Language language)
        {
            Action action = () =>
            {
                Language = language;
                WriteLine(language["Help"]);
                WriteLine(language["Introduction"]);
                ClickThroughButton.ToolTip = language["ClickthoughButtonToolTip"];
                AutoScrollButton.ToolTip = language["AutoScrollButtonToolTip"];
                AutoShowButton.ToolTip = language["AutoShowButtonToolTip"];
                CloseButton.ToolTip = language["CloseButtonToolTip"];
                MainWindow.Title = language["WindowTitle"];

                if (language.Current != UserLanguage.English)
                {
                    Translate.DisableAdditionalInputInfo();
                    Input.SpellCheck.IsEnabled = false;
                }
                else
                {
                    Input.SpellCheck.IsEnabled = true;
                    if (Settings.AdditionalInfo)
                    {
                        Translate.EnableAdditionalInputInfo();
                    }
                }
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
            Settings.WindowPosition.Value = new Vector2((int)MainWindow.Left, (int)MainWindow.Top);
        }

        private void OnTick(object sender, TickEventArgs e)
        {
            var bounds = new Rect(new System.Windows.Point(MainWindow.Left, MainWindow.Top), new System.Windows.Point(MainWindow.RestoreBounds.Right, MainWindow.RestoreBounds.Bottom));
            var cursorPosition = new System.Windows.Point(System.Windows.Forms.Cursor.Position.X * dpiX, System.Windows.Forms.Cursor.Position.Y * dpiY);

            bounds.Offset(-6, -6);
            bounds.Width += 12;
            bounds.Height += 12;

            if (Translate.IsNGS && ShineOpacityModifier < 1)
            {
                ShineOpacityModifier = e.Add(ShineOpacityModifier, ShineTime);
            }
            else if (!Translate.IsNGS && ShineOpacityModifier > 0)
            {
                ShineOpacityModifier = e.Subtract(ShineOpacityModifier, ShineTime);
            }
            if (ShineOpacityModifier > 1)
            {
                ShineOpacityModifier = 1;
            }
            else if (ShineOpacityModifier < 0)
            {
                ShineOpacityModifier = 0;
            }
            if (bounds.Contains(cursorPosition))
            {
                TargetOpacity = 1;
                ForceTargetOpacity = 1;

                if (InterfaceOpacity == 1)
                {
                    Wait = 1;
                    ForceWait = 1.5;
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
                if (Settings.AutoShow.Value && Settings.PartialTransparency.Value)
                {
                    if (MainWindow.Opacity == 1 && ForceWait > 0)
                    {
                        ForceWait = e.Subtract(ForceWait, WaitDelay);
                    }
                    else if (ForceWait <= 0)
                    {
                        ForceTargetOpacity = 0;
                    }
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
                ForceTargetOpacity = 1;
            }
            if (Settings.AutoHide)
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
                if (Settings.AutoShow.Value && Settings.PartialTransparency.Value)
                {
                    if (MainWindow.Opacity < ForceTargetOpacity)
                    {
                        MainWindow.Opacity = e.Add(MainWindow.Opacity, ShowDelay);
                    }
                    else
                    {
                        if (ForceWait > 0)
                        {
                            ForceWait = e.Subtract(ForceWait, WaitDelay);
                        }
                        else
                        {
                            MainWindow.Opacity = e.Subtract(MainWindow.Opacity, FadeTime);
                        }
                    }
                    if (MainWindow.Opacity > 1)
                    {
                        MainWindow.Opacity = 1;
                    }
                    else if (MainWindow.Opacity < 0)
                    {
                        MainWindow.Opacity = 0;
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
            DelayHandler.Stop();
            SettingsWindow.Close();
            timer.Stop();
            Translate.Stop();
            CefSharp.Cef.Shutdown();
            SettingHandler.Close();
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

        private void MinimiseHighlight()
        {
            MinimiseButtonBackground.Opacity = 1;
        }

        private void DisableMinimiseHighlight()
        {
            MinimiseButtonBackground.Opacity = 0;
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

        private void MinimiseButton_Click(object sender, MouseEventArgs e)
        {
            MainWindow.WindowState = WindowState.Minimized;
        }

        private void MinimiseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            MinimiseHighlight();
        }

        private void MinimiseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DisableMinimiseHighlight();
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

        private void Input_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void Input_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift) && !System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift))
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
