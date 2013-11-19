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
using System.Windows.Shapes;
using Meticumedia.Classes;

namespace Meticumedia.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            cntrlScanFolder.LoadSettings();
            cntrlMovieFolder.LoadSettings(ContentType.Movie);
            cntrlTvFolder.LoadSettings(ContentType.TvShow);
            cntrlTvFileName.LoadSettings(ContentType.TvShow);
            cntrlMovieFileName.LoadSettings(ContentType.Movie);
        }

        #region Event Handlers

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            cntrlScanFolder.SaveSettings();
            cntrlMovieFolder.SaveSettings();
            cntrlTvFolder.SaveSettings();
            cntrlTvFileName.SaveSettings();
            cntrlMovieFileName.SaveSettings();
            Settings.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
