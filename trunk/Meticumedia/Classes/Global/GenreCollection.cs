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
            Global, // Collection is a global list of available genres
            Movie,  // Collection is for a single movie
            Tv  // Collection is for a single TV show
        }

        /// <summary>
        /// Type of content genre collection is related to
        /// </summary>
        private CollectionType genreType;

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
        }
        
        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item">item to add to list</param>
        public new void Add(string item)
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
                case CollectionType.Tv:
                    if (!Organization.AllTvGenres.Contains(item))
                        Organization.AllTvGenres.Add(item);
                    break;
                case CollectionType.Movie:
                    if (!Organization.AllMovieGenres.Contains(item))
                        Organization.AllMovieGenres.Add(item);
                    break;
                case CollectionType.Global:
                    OnGenresUpdated();
                    break;
            }
        }
    }
}
