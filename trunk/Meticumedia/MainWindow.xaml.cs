using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Meticumedia.Windows;
using Meticumedia.Classes;
using System.Diagnostics;

namespace Meticumedia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Init word helper
            WordHelper.Initialize();
            
            // Load organization and settings from XML
            Settings.Load();
            Organization.Load(true);
        }

        private void menuSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("meticumedia v0.9.3 (alpha)\nCopyright © 2013", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=NE42NQGGL8Q9C&lc=CA&item_name=meticumedia&currency_code=CAD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
        }
    }
}
