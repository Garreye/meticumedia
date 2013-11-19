﻿using System;
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
using System.Collections.ObjectModel;
using Meticumedia.Classes;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for ContentControl.xaml
    /// </summary>
    public partial class ContentControl : UserControl
    {
        public ContentControl()
        {
            InitializeComponent();
            lbShows.ItemsSource = Organization.Shows;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Organization.Shows.Add(new TvShow("Test Show " + Organization.Shows.Count, 102 + Organization.Shows.Count, 2013, "C:/Content/Show " + Organization.Shows.Count, "C:/content"));
            Organization.Shows[Organization.Shows.Count - 1].Genres.Add("new genre");
            Organization.Shows[Organization.Shows.Count - 1].Overview = "Blah Blah Blasjdlk fjaldnfkj adhflajdfla jdlfjad;lfjal kd;jfla;";
        }

        private void btnMod_Click(object sender, RoutedEventArgs e)
        {
            Organization.Shows[Organization.Shows.Count - 1].Name = "Mod Show " + (Organization.Shows.Count - 1);
            TvShow show = ((TvShow)Organization.Shows[Organization.Shows.Count - 1]);
            show.Episodes.Add(new TvEpisode("Name", show.Name, show.Episodes.Count+1, show.Episodes.Count + 1, "2013/01/01", "Episdoe overview blah blaj"));
        }
    }
}
