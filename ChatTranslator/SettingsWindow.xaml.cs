using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TranslateTool;

namespace ChatTranslator
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
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

        public void ApplyTranslation(Language language)
        {
            PreventTransparencyCheck.Content = language.Text["AutoHideCheckbox"];
            PartialTransparencyCheck.Content = language.Text["PartialTransparencyCheckbox"];
            ColourChatMessagesCheck.Content = language.Text["ColourChatMessagesCheckbox"];
            ClickthroughKeyEnabledCheck.Content = language.Text["ClickthroughKeyEnabledCheckbox"];
            LanguageSelectEnglish.Content = language.Text["English"];
            LanguageSelectJapanese.Content = language.Text["Japanese"];
            ShowAdditionalInfoCheck.Content = language.Text["AdditionalInfoCheckbox"];
            LanguageSelectLabel.Content = language.Text["LanguageSelect"];

            if (language.Current != UserLanguage.English)
            {
                ShowAdditionalInfoCheck.IsEnabled = false;
            }
            else
            {
                ShowAdditionalInfoCheck.IsEnabled = true;
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CloseButton.Background = ImageResources.Close;
            ConfigureMouseClickEvent(CloseButton, CloseButton_Clicked);
            CloseButtonBackground.Opacity = 0;
            BindCheckBoxSetting(ShowAdditionalInfoCheck, Settings.AdditionalInfo);
            BindCheckBoxSetting(PreventTransparencyCheck, Settings.AutoHide);
            BindCheckBoxSetting(PartialTransparencyCheck, Settings.PartialTransparency);
            BindCheckBoxSetting(ColourChatMessagesCheck, Settings.ColourChat);
            BindCheckBoxSetting(ClickthroughKeyEnabledCheck, Settings.ClickthroughKeyEnabled);
            BindCheckBoxSetting(OCRKeyEnabledCheck, Settings.OCRKeyEnabled);
            BindRadioSetting(new RadioButton[] { LanguageSelectEnglish, LanguageSelectJapanese }, new UserLanguage[] { UserLanguage.English, UserLanguage.Japanese }, Settings.Language);
            Settings.Language.OnChanged += LanguageSettingChanged;
        }

        private void LanguageSettingChanged(object sender, SettingChangedEventArgs<UserLanguage> e)
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
