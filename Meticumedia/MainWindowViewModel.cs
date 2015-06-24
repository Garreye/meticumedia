using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Windows;
using Meticumedia.Controls;
using Meticumedia.WPF;
using System.Reflection;
using Xceed.Wpf.Toolkit;
using Meticumedia.Classes;

namespace Meticumedia
{
    public class MainWindowViewModel : ViewModel
    {
        #region Properties

        public ContentCollectionControlViewModel TvShowViewModel
        {
            get
            {
                return tvShowViewModel;
            }
            set
            {
                tvShowViewModel = value;
                OnPropertyChanged(this, "TvShowViewModel");
            }
        }
        private ContentCollectionControlViewModel tvShowViewModel = new ContentCollectionControlViewModel(Classes.ContentType.TvShow);

        public ScheduleControlViewModel ScheduleViewModel
        {
            get
            {
                return scheduleViewModel;
            }
            set
            {
                scheduleViewModel = value;
                OnPropertyChanged(this, "ScheduleViewModel");
            }
        }
        private ScheduleControlViewModel scheduleViewModel = new ScheduleControlViewModel();

        public ContentCollectionControlViewModel MoviesViewModel
        {
            get
            {
                return moviesViewModel;
            }
            set
            {
                moviesViewModel = value;
                OnPropertyChanged(this, "MoviesViewModel");
            }
        }
        private ContentCollectionControlViewModel moviesViewModel = new ContentCollectionControlViewModel(Classes.ContentType.Movie);

        public ScanControlViewModel ScanViewModel
        {
            get
            {
                return scanViewModel;
            }
            set
            {
                scanViewModel = value;
                OnPropertyChanged(this, "ScanViewModel");
            }
        }
        private ScanControlViewModel scanViewModel = new ScanControlViewModel();

        public QueueControlViewModel QueueViewModel
        {
            get
            {
                return queueViewModel;
            }
            set
            {
                queueViewModel = value;
                OnPropertyChanged(this, "QueueViewModel");
            }
        }
        private QueueControlViewModel queueViewModel = new QueueControlViewModel();

        public LogControlViewModel LogViewModel
        {
            get
            {
                return logViewModel;
            }
            set
            {
                logViewModel = value;
                OnPropertyChanged(this, "LogViewModel");
            }
        }
        private LogControlViewModel logViewModel = new LogControlViewModel();

        #endregion

        #region Commands

        private ICommand openSettingsCommand;
        public ICommand OpenSettingsCommand
        {
            get
            {
                if (openSettingsCommand == null)
                {
                    openSettingsCommand = new RelayCommand(
                        param => this.OpenSettings()
                    );
                }
                return openSettingsCommand;
            }
        }

        private ICommand openAboutCommand;
        public ICommand OpenAboutCommand
        {
            get
            {
                if (openAboutCommand == null)
                {
                    openAboutCommand = new RelayCommand(
                        param => this.OpenAbout()
                    );
                }
                return openAboutCommand;
            }
        }

        private ICommand donateCommand;
        public ICommand DonateCommand
        {
            get
            {
                if (donateCommand == null)
                {
                    donateCommand = new RelayCommand(
                        param => this.Donate()
                    );
                }
                return donateCommand;
            }
        }

        private ICommand refreshMoviesCommand;
        public ICommand RefreshMoviesCommand
        {
            get
            {
                if (refreshMoviesCommand == null)
                {
                    refreshMoviesCommand = new RelayCommand(
                        param => this.RefeshMovies()
                    );
                }
                return refreshMoviesCommand;
            }
        }

        private ICommand refreshTvShowsCommand;
        public ICommand RefreshTvShowsCommand
        {
            get
            {
                if (refreshTvShowsCommand == null)
                {
                    refreshTvShowsCommand = new RelayCommand(
                        param => this.RefreshTvShows()
                    );
                }
                return refreshTvShowsCommand;
            }
        }

        #endregion

        #region Constructor

        #endregion

        #region Methods

        public void OpenSettings()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void OpenAbout()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            MessageBox.Show("meticumedia v" + version + " (alpha)\nCopyright © 2015", "About", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void Donate()
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=NE42NQGGL8Q9C&lc=CA&item_name=meticumedia&currency_code=CAD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
        }

        private void RefeshMovies()
        {
            Organization.UpdateRootFolders(ContentType.Movie);
        }

        private void RefreshTvShows()
        {
            Organization.UpdateRootFolders(ContentType.TvShow);
        }

        #endregion
    }
}
