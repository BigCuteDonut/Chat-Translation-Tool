using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using TranslateTool;
using System.Windows.Interop;
using WinKey = System.Windows.Forms.Keys;
using Microsoft.Win32;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ChatTranslator
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private List<HotkeyInputHandler> hotkeyInputHandlers = new List<HotkeyInputHandler>();
        private bool windowLoaded = false;
        public static bool windowConfigured = false;
        public static TranslateTool.Keyboard Keyboard;
        public Dictionary<UIElement, ClickInfo> MouseClickConfigurationInfo = new Dictionary<UIElement, ClickInfo>();

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private RoutedEventHandler CreateBinding<T>(Setting<T> setting, T value)
            where T : unmanaged
        {
            return (sender, eventArgs) => { setting.Value = value; };
        }

        private void BindCheckBoxSetting(CheckBox checkBox, Setting<bool> setting)
        {
            checkBox.IsChecked = setting;
            checkBox.Checked += CreateBinding(setting, true);
            checkBox.Unchecked += CreateBinding(setting, false);
        }

        private void BindRadioSetting<T>(RadioButton[] buttons, T[] values, Setting<T> setting)
            where T : unmanaged
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Checked += CreateBinding(setting, values[i]);
            }
        }

        private void BindHotkeySelect(Button button, string hotkeyMessageID, Setting<Hotkey> setting)
        {
            var handler = new HotkeyInputHandler(button, hotkeyMessageID, setting);

            hotkeyInputHandlers.Add(handler);
            button.Click += handler.CreateHotkeySelectBinding();
        }

        public void ApplyTranslation(Language language)
        {
            PreventTransparencyCheck.Content = language["AutoHideCheckbox"];
            PartialTransparencyCheck.Content = language["PartialTransparencyCheckbox"];
            ColourChatMessagesCheck.Content = language["ColourChatMessagesCheckbox"];
            ClickthroughKeyEnabledCheck.Content = language["ClickthroughKeyEnabledCheckbox"];
            OCRKeyEnabledCheck.Content = language["OCRKeyEnabledCheckbox"];
            LanguageSelectEnglish.Content = language["English"];
            LanguageSelectJapanese.Content = language["Japanese"];
            ShowAdditionalInfoCheck.Content = language["AdditionalInfoCheckbox"];
            LanguageSelectLabel.Content = language["LanguageSelect"];
            GeneralTab.Header = language["GeneralTab"];
            OCRTab.Header = language["OCRTab"];
            LanguageTab.Header = language["LanguageTab"];
            OCRNoticeLabel.Content = language["OCRNotice"];

            if (language.Current != UserLanguage.English)
            {
                ShowAdditionalInfoCheck.IsEnabled = false;
                OCRKeyEnabledCheck.IsEnabled = false;

                if (Settings.AdditionalInfo)
                {
                    Settings.AdditionalInfo.Value = false;
                }
                if (Settings.OCRKeyEnabled)
                {
                    Settings.OCRKeyEnabled.Value = false;
                }
            }
            else
            {
                ShowAdditionalInfoCheck.IsEnabled = true;
                OCRKeyEnabledCheck.IsEnabled = true;
            }
        }

        public void Verify()
        {
            if (windowLoaded && !windowConfigured)
            {
                WindowLoaded(null, null);
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (MainWindowLogic.Language != null && !windowConfigured)
            {
                SettingsWindow.Keyboard = new TranslateTool.Keyboard(HwndSource.FromVisual(this) as HwndSource);
                ApplyTranslation(MainWindowLogic.Language);
                CloseButton.Background = ImageResources.Close;
                ConfigureMouseClickEvent(CloseButton, CloseButton_Clicked);
                CloseButtonBackground.Opacity = 0;
                BindCheckBoxSetting(ShowAdditionalInfoCheck, Settings.AdditionalInfo);
                BindCheckBoxSetting(PreventTransparencyCheck, Settings.AutoHide);
                BindCheckBoxSetting(PartialTransparencyCheck, Settings.PartialTransparency);
                BindCheckBoxSetting(ColourChatMessagesCheck, Settings.ColourChat);
                BindCheckBoxSetting(ClickthroughKeyEnabledCheck, Settings.ClickthroughKeyEnabled);
                BindCheckBoxSetting(OCRKeyEnabledCheck, Settings.OCRKeyEnabled);
                LanguageSelectEnglish.Checked += (s, eventArgs) => { Settings.ChangeLanguage(UserLanguage.English); };
                LanguageSelectJapanese.Checked += (s, eventArgs) => { Settings.ChangeLanguage(UserLanguage.Japanese); };
                BindHotkeySelect(ClickthroughHotkeyInput, "ClickthroughHotkeyInput", Settings.ClickthroughKey);
                BindHotkeySelect(OCRHotkeyInput, "OCRHotkeyInput", Settings.OCRKey);
                OCRDirectoryInput.Content = (string)Settings.OCRDirectory.Value;
                OCRDirectoryInput.Click += (s, eventArgs) =>
                {
                    var dialog = new CommonOpenFileDialog();
                    dialog.IsFolderPicker = true;
                    var shareXPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ShareX"));

                    if (Directory.Exists(shareXPath))
                    {
                        dialog.InitialDirectory = shareXPath;
                    }

                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        Settings.OCRDirectory.Value = dialog.FileName;
                    }
                    OCRDirectoryInput.Content = (string)Settings.OCRDirectory.Value;
                };

                if (!Settings.ClickthroughKeyEnabled)
                {
                    ClickthroughHotkeyInput.IsEnabled = false;
                }
                if (!Settings.OCRKeyEnabled)
                {
                    OCRHotkeyInput.IsEnabled = false;
                }
                Settings.OCRKeyEnabled.OnChanged += OCRKeyEnabledSettingChanged;
                Settings.ClickthroughKeyEnabled.OnChanged += ClickthroughKeyEnabledSettingChanged;
                Settings.OnLanguageChanged += LanguageSettingChanged;
                windowConfigured = true;
            }
            windowLoaded = true;
        }

        private void ClickthroughKeyEnabledSettingChanged(object sender, SettingChangedEventArgs<bool> e)
        {
            ClickthroughHotkeyInput.IsEnabled = e.NewValue;
        }

        private void OCRKeyEnabledSettingChanged(object sender, SettingChangedEventArgs<bool> e)
        {
            OCRHotkeyInput.IsEnabled = e.NewValue;
        }

        public void LanguageSettingChanged(object sender, SettingChangedEventArgs<UserLanguage> e)
        {
            if (e.NewValue != e.OldValue)
            {
                ApplyTranslation(MainWindowLogic.Language);
            }
        }

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseButtonBackground.Opacity = 1;
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseButtonBackground.Opacity = 0;
        }

        private void CloseButton_Clicked(object sender, MouseEventArgs e)
        {
            Hide();
        }

        private void CloseButton_Loaded(object sender, RoutedEventArgs e)
        {

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
    }
}
