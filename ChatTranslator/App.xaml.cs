using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ChatTranslator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ResourceDictionary AppResources;
        public static SettingsWindow SettingsWindow;
        public App()
        {
            AppResources = Resources;
            SettingsWindow = new SettingsWindow();
            SettingsWindow.Hide();
        }
    }
}
