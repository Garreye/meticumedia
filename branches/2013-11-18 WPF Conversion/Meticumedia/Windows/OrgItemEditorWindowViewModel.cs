using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticumedia.Classes;
using Meticumedia.Controls;

namespace Meticumedia.Windows
{
    public class OrgItemEditorWindowViewModel : ViewModel
    {
        #region Properties

        public OrgItem Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
                OnPropertyChanged(this, "Item");
            }
        }
        private OrgItem item;

        public ContentEditorControlViewModel MovieViewModel
        {
            get
            {
                if (movieViewModel == null)
                {
                    Movie movie = this.Item.Movie;
                    if (movie == null)
                        movie = new Movie();

                    movieViewModel = new ContentEditorControlViewModel(movie);
                }
                return movieViewModel;
            }

        }
        private ContentEditorControlViewModel movieViewModel;

        public ObservableCollection<TvShow> Shows { get; set; }

        #endregion

        #region Commands



        #endregion

        #region Constructor

        public OrgItemEditorWindowViewModel(OrgItem item)
        {

        }

        #endregion

        #region Methods



        #endregion
    }
}
