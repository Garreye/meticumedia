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
using Meticumedia.Classes;

namespace Meticumedia.Windows
{
    /// <summary>
    /// Interaction logic for EpisodeEditorWindow.xaml
    /// </summary>
    public partial class EpisodeEditorWindow : Window
    {
        public TvEpisode Results
        {
            get
            {
                if (viewModel.ResultsOk)
                    return viewModel.Episode;
                else
                    return null;
            }
        }

        private EpisodeEditorWindowViewModel viewModel;
        
        public EpisodeEditorWindow(TvEpisode episode)
        {
            InitializeComponent();
            viewModel = new EpisodeEditorWindowViewModel(episode);
            viewModel.ResultsSet += vm_ResultsSet;
            this.DataContext = viewModel;
        }

        void vm_ResultsSet(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
