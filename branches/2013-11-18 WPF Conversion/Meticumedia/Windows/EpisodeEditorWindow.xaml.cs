﻿using System;
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
using Meticumedia.Classes;

namespace Meticumedia.Windows
{
    /// <summary>
    /// Interaction logic for EpisodeEditorWindow.xaml
    /// </summary>
    public partial class EpisodeEditorWindow : Window
    {
        public EpisodeEditorWindow(TvEpisode episode)
        {
            InitializeComponent();
            EpisodeEditorWindowViewModel vm = new EpisodeEditorWindowViewModel(episode);
            this.DataContext = vm;
        }
    }
}
