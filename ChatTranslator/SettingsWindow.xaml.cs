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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CloseButton.Background = ImageResources.Close;
            ConfigureMouseClickEvent(CloseButton, CloseButton_Clicked);
            CloseButtonBackground.Opacity = 0;
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
