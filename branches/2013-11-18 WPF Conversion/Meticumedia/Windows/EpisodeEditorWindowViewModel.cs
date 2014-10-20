using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticumedia.Classes;
using Meticumedia.WPF;

namespace Meticumedia.Windows
{
    public class EpisodeEditorWindowViewModel : ViewModel
    {
        #region Properties

        public TvEpisode Episode
        {
            get
            {
                return episode;
            }
            set
            {
                episode = value;
                OnPropertyChanged(this, "Episode");
            }
        }
        private TvEpisode episode;

        #endregion

        #region Constructor

        public EpisodeEditorWindowViewModel(TvEpisode episode)
        {
            this.Episode = new TvEpisode(episode);
        }

        #endregion

        #region Methods
        


        #endregion
    }
}
