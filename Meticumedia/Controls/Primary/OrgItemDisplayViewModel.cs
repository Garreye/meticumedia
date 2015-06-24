using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Meticumedia.Classes;

namespace Meticumedia.Controls
{
    public abstract class OrgItemDisplayViewModel : OrgItemQueueableViewModel
    {        
        #region Properties

        #region Items

        public ObservableCollection<OrgItem> OrgItems
        {
            get;
            private set;
        }

        public IList SelectedOrgItems
        {
            get
            {
                return selectedOrgItems;
            }
            set
            {
                selectedOrgItems = value;
                SelectedOrgItemsChange();
            }
        }
        private IList selectedOrgItems;

        public ICollectionView OrgItemsCollection { get; set; }

        #endregion

        #region Action Filter

        public OrgAction ActionFilter
        {
            get
            {
                return actionFilter;
            }
            set
            {
                actionFilter = value;
                OnPropertyChanged(this, "ActionFilter");
                OnPropertyChanged(this, "NoneActionFilter");
                OnPropertyChanged(this, "MoveCopyActionFilter");
                OnPropertyChanged(this, "RenameActionFilter");
                OnPropertyChanged(this, "DeleteActionFilter");
                OnPropertyChanged(this, "QueuedActionFilter");
                OnPropertyChanged(this, "TbdActionFilter");
                RefreshResultsSafe(false);
            }
        }
        private OrgAction actionFilter = OrgAction.All ^ OrgAction.Queued;

        public bool NoneActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.None) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.None;
                else
                    this.ActionFilter = actionFilter & ~OrgAction.None;
            }
        }

        public bool MoveCopyActionFilter
        {
            get
            {
                return (int)(actionFilter & (OrgAction.Move | OrgAction.Copy)) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | (OrgAction.Move | OrgAction.Copy);
                else
                    this.ActionFilter = actionFilter & ~(OrgAction.Move | OrgAction.Copy);
            }
        }

        public bool RenameActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.Rename) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.Rename;
                else
                    this.ActionFilter = actionFilter & ~OrgAction.Rename;
            }
        }

        public bool DeleteActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.Delete) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.Delete;
                else
                    this.ActionFilter = actionFilter & ~OrgAction.Delete;
            }
        }

        public bool QueuedActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.Queued) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.Queued;
                else
                    this.ActionFilter = actionFilter & ~OrgAction.Queued;
            }
        }

        public bool TbdActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.TBD) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.TBD | OrgAction.Processing;
                else
                    this.ActionFilter = actionFilter & ~(OrgAction.TBD | OrgAction.Processing);
            }
        }

        #endregion

        #region Category Filter

        public bool TvVideoCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.TvVideo) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.TvVideo;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.TvVideo;
            }
        }

        public bool MovieVideoCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.MovieVideo) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.MovieVideo;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.MovieVideo;
            }
        }

        public bool CustomCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Custom) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Custom;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Custom;
            }
        }

        public bool TrashCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Trash) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Trash;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Trash;
            }
        }

        public bool UnknownCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Unknown) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Unknown;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Unknown;
            }
        }

        public bool FolderCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Folder) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Folder;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Folder;
            }
        }

        public bool IgnoredCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Ignored) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Ignored;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Ignored;
            }
        }

        public bool AutoMoveCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.AutoMove) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.AutoMove;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.AutoMove;
            }
        }

        public FileCategory CategoryFilter
        {
            get
            {
                return categoryFilter;
            }
            set
            {
                categoryFilter = value;
                OnPropertyChanged(this, "CategoryFilter");
                OnPropertyChanged(this, "TvVideoCategoryFilter");
                OnPropertyChanged(this, "MovieVideoCategoryFilter");
                OnPropertyChanged(this, "CustomCategoryFilter");
                OnPropertyChanged(this, "TrashCategoryFilter");
                OnPropertyChanged(this, "UnknownCategoryFilter");
                OnPropertyChanged(this, "FolderCategoryFilter");
                OnPropertyChanged(this, "IgnoredCategoryFilter");
                RefreshResultsSafe(false);
            }
        }
        private FileCategory categoryFilter = FileCategory.All ^ FileCategory.Ignored;

        #endregion        

        #endregion

        #region Constructor

        public OrgItemDisplayViewModel()
        {            
            this.OrgItems = new ObservableCollection<OrgItem>();
            this.OrgItemsCollection = CollectionViewSource.GetDefaultView(OrgItems);
            this.OrgItemsCollection.Filter = new Predicate<object>(FilterItem);
            
            // Set properties to trigger live updating
            ICollectionViewLiveShaping liveCollection = this.OrgItemsCollection as ICollectionViewLiveShaping;
            liveCollection.LiveFilteringProperties.Add("Category");
            liveCollection.LiveFilteringProperties.Add("Action");
            liveCollection.IsLiveFiltering = true;

            liveCollection.IsLiveSorting = true;
            liveCollection.LiveSortingProperties.Add("ActionTime");
            liveCollection.LiveSortingProperties.Add("Action");
            liveCollection.LiveSortingProperties.Add("Category");
            liveCollection.LiveSortingProperties.Add("QueueStatus");
            liveCollection.LiveSortingProperties.Add("DestinationPath");
            liveCollection.LiveSortingProperties.Add("SourcePath");
            liveCollection.LiveSortingProperties.Add("Status");
        }

        #endregion

        protected virtual void SelectedOrgItemsChange()
        {
        }

        private DateTime lastRefresh = DateTime.Now;

        protected bool FilterItem(object obj)
        {
            OrgItem item = obj as OrgItem;

            // Action filter
            if ((int)(item.Action & this.ActionFilter) == 0)
                return false;

            // Category filter
            if ((int)(item.Category & this.CategoryFilter) == 0)
                return false;

            return true;
        }

        protected void RefreshResultsSafe(bool limitRate)
        {            
            if (limitRate && (DateTime.Now - lastRefresh).TotalSeconds < 5)
                return;

            lastRefresh = DateTime.Now;

            try
            {
                if (App.Current.Dispatcher.CheckAccess())
                {
                    this.OrgItemsCollection.Refresh();
                }
                else
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        this.OrgItemsCollection.Refresh();
                    });
            }
            catch { }
        }
    }
}
