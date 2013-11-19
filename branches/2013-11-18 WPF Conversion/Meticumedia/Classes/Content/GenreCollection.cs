// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Collection of genres
    /// </summary>
    public class GenreCollection : List<string>, INotifyCollectionChanged
    {
        #region Type

        /// <summary>
        /// Type of collections
        /// </summary>
        public enum CollectionType 
        { 
            Global = 1, // Collection is a global list of available genres
            Movie = 2,  // Collection is for a single movie
            Tv = 4 // Collection is for a single TV show
        }

        /// <summary>
        /// Type of content genre collection is related to
        /// </summary>
        private CollectionType genreType;

        #endregion

        #region Properties

        /// <summary>
        /// Object used for locking collection
        /// </summary>
        public object AccessLock { get; set; }

        public string GenresString
        {
            get
            {
                string genresString = string.Empty;
                foreach (string genre in this)
                    genresString += genre + ", ";
                if (genresString.Length > 0)
                    genresString = genresString.Remove(genresString.Length - 2);
                return genresString;
            }
        }

        #endregion

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Triggers CollectionChanged event
        /// </summary>
        /// <param name="items"></param>
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, string item)
        {
            if (CollectionChanged != null)
                CollectionChanged(null, new NotifyCollectionChangedEventArgs(action, item));
        }

        protected void OnCollectionReset()
        {
            if (CollectionChanged != null)
                CollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with type of content genre is related to
        /// </summary>
        /// <param name="type">Content type genre is related to</param>
        public GenreCollection(CollectionType type)
        {
            this.genreType = type;
            this.AccessLock = new object();
        }

        /// <summary>
        /// Constructor with type of content genre is related to
        /// </summary>
        /// <param name="type">Content type genre is related to</param>
        public GenreCollection(ContentType type)
        {
            this.genreType = type == ContentType.Movie ? CollectionType.Movie : CollectionType.Tv;
            this.AccessLock = new object();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item">item to add to list</param>
        public new void Add(string item)
        {
            lock (this.AccessLock)
            {
                // Check for empty
                if (string.IsNullOrWhiteSpace(item))
                    return;

                // Add to list
                base.Add(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);

                // Keep global list of genres for content types
                if ((this.genreType & CollectionType.Global) == 0)
                {
                    if ((this.genreType & CollectionType.Tv) > 0)
                    {
                        if (!Organization.AllTvGenres.Contains(item))
                            Organization.AllTvGenres.Add(item);
                    }
                    if ((this.genreType & CollectionType.Movie) > 0)
                    {
                        if (!Organization.AllMovieGenres.Contains(item))
                            Organization.AllMovieGenres.Add(item);
                    }

                }
            }
        }

        /// <summary>
        /// Remove item with locking
        /// </summary>
        /// <param name="item">Item to remove</param>
        public new void Remove(string item)
        {
            lock (AccessLock)
                base.Remove(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        }

        /// <summary>
        /// RemoveAt with locking
        /// </summary>
        /// <param name="index">Index of item to remove</param>
        public new void RemoveAt(int index)
        {
            string item = string.Empty;
            lock (AccessLock)
            {
                item = this[index];
                base.RemoveAt(index);
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        }

        /// <summary>
        /// Sorting with locking
        /// </summary>
        public new void Sort()
        {
            lock (AccessLock)
                base.Sort();
            OnCollectionReset();
        }

        /// <summary>
        /// Clear with locking
        /// </summary>
        public new void Clear()
        {
            lock (AccessLock)
                base.Clear();
            OnCollectionReset();
        }

        #endregion

        
    }
}
