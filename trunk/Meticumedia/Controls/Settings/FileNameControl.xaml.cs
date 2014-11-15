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
using Meticumedia.Classes;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for TvFileNameControl.xaml
    /// </summary>
    public partial class FileNameControl : UserControl
    {
        public FileNameControlViewModel ViewModel
        {
            get
            {
                return this.DataContext as FileNameControlViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public FileNameControl()
        {
            InitializeComponent();
        }

        //private ContentType contentType;

        //#region Save/Load

        //public void LoadSettings(ContentType type)
        //{
        //    this.contentType = type;
        //    switch (type)
        //    {
        //        case ContentType.Movie:
        //            this.ViewModel = new FileNameControlViewModel(Settings.MovieFileFormat, type);
        //            break;
        //        case ContentType.TvShow:
        //            this.ViewModel = new FileNameControlViewModel(Settings.TvFileFormat, type);
        //            break;
        //    }
        //}

        //public void SaveSettings()
        //{
        //    switch (contentType)
        //    {
        //        case ContentType.Movie:
        //            Settings.MovieFileFormat = this.ViewModel.FileNameFormat;
        //            break;
        //        case ContentType.TvShow:
        //            Settings.TvFileFormat = this.ViewModel.FileNameFormat;
        //            break;
        //    }
        //}

        //#endregion
    }
}
