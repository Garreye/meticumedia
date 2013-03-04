// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    /// <summary>
    /// Collection of genres
    /// </summary>
    public class GenreCollection : List<string>
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

        public object AccessLock { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event indicating that a new item has been added to list (for global type only)
        /// </summary>
        public event EventHandler GenresUpdated;

        /// <summary>
        /// Triggers ItemsToQueue event
        /// </summary>
        /// <param name="items"></param>
        protected void OnGenresUpdated()
        {
            if (GenresUpdated != null)
                GenresUpdated(null, new EventArgs());
        }

        #endregion

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
                Sort();

                // Add to global list for content type
                switch (this.genreType)
                {
                    case CollectionType.Global:
                        OnGenresUpdated();
                        break;
                    case CollectionType.Tv:
                        if (!Organization.AllTvGenres.Contains(item))
                            Organization.AllTvGenres.Add(item);
                        break;
                    case CollectionType.Movie:
                        if (!Organization.AllMovieGenres.Contains(item))
                            Organization.AllMovieGenres.Add(item);
                        break;
                }
            }
        }

        public new void Remove(string item)
        {
            lock (AccessLock)
                base.Remove(item);
        }

        public new void RemoveAt(int index)
        {
            lock (AccessLock)
                base.RemoveAt(index);
        }

        public new void Sort()
        {
            lock (AccessLock)
                base.Sort();
        }

        public new void Clear()
        {
            lock (AccessLock)
                base.Clear();
        }
    }
}
