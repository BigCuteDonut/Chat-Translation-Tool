using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TranslateTool;
using System.Windows.Controls.Primitives;

namespace ChatTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindowLogic Logic;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var windowInteropHelper = new WindowInteropHelper(this);
            var settingsWindow = new SettingsWindow();

            Logic = new MainWindowLogic();
            Logic.Load(windowInteropHelper.Handle, this, settingsWindow, MainGrid, OutputDock, InputDock, ClickThroughButton, AutoShowButton, AutoScrollButton, SettingsButton, CloseButton, AutoShowButtonBackground, AutoScrollButtonBackground, SettingsButtonBackground, CloseButtonBackground, MoveButton, MinimiseButton, MinimiseButtonBackground, Output, Input);
            settingsWindow.Verify();
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void Style_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
