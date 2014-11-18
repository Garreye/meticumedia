using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class ProgressViewModel : ViewModel
    {

        #region Properties

        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                OnPropertyChanged(this, "Progress");
            }
        }
        private int progress = 0;

        public string ProgressMessage
        {
            get
            {
                return scanMessage;
            }
            set
            {
                scanMessage = value;
                OnPropertyChanged(this, "ProgressMessage");
            }
        }
        private string scanMessage = string.Empty;

        public Visibility ProgressVisibility
        {
            get
            {
                return progressVisibility;
            }
            set
            {
                progressVisibility = value;
                OnPropertyChanged(this, "ProgressVisibility");
            }
        }
        private Visibility progressVisibility = Visibility.Visible;

        #endregion

        #region Constructor

        protected ProgressViewModel()
        {
        }

        #endregion

        #region Methods

        protected void UpdateProgressSafe(int progress, string msg, bool visible = true)
        {
            try
            {
                if (App.Current == null)
                    return;

                if (App.Current.Dispatcher.CheckAccess())
                    UpdateProgress(progress, msg, visible);
                else
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        UpdateProgress(progress, msg, visible);
                    });
            }
            catch { }
        }

        private object progLock = new object();

        private void UpdateProgress(int progress, string msg, bool visible)
        {
            lock (progLock)
            {
                this.Progress = Math.Min(progress, 100);
                this.ProgressMessage = msg + " (" + progress + "%)";
                this.ProgressVisibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

    }
}
