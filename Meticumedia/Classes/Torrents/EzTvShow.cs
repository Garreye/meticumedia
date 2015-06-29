using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public class EzTvShow : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        private int id = -1;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        private string name = "";

        public List<EzTvEpisode> Episodes { get; set; }

        public EzTvShow(string name, int id)
        {
            this.Id = id;
            this.Name = name;
            this.Episodes = new List<EzTvEpisode>();
        }

        public EzTvShow(EzTvShow clone)
        {
            this.Id = clone.Id;
            this.Name = clone.Name;
            this.Episodes = new List<EzTvEpisode>();
            foreach (EzTvEpisode ep in clone.Episodes)
                this.Episodes.Add(new EzTvEpisode(ep));
        }

        public void UpdateEpisodes()
        {
            EzTvAccess.GetEpisodes(this);
        }

        public override string ToString()
        {
            return this.Name + " - " + this.Id;
        }
    }
}
