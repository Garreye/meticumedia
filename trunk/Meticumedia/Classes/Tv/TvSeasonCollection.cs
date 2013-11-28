// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Meticumedia
{

    /// <summary>
    /// Collection of TvSeason objects. Custom object used so that
    /// seasons can be accessed by the season number. (List cannot be used because
    /// seasons may or may not be consecutive, and may or may not start at 0.)
    /// </summary>
    public class TvSeasonCollection : ICollection<TvSeason>, ICollection
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TvSeasonCollection()
        {
            innerList = new List<TvSeason>();
        }

         #endregion

        #region Accessor

        /// <summary>
        /// Accesor for TvSeason collection.
        /// </summary>
        /// <param name="season">The number of the season to be accessed</param>
        /// <returns>The TvSeason object related to the season</returns>
        public virtual TvSeason this[int season]
        {
            get
            {
                int index = -1;
                for (int i = 0; i < innerList.Count; i++)
                    if (innerList[i].Number == season)
                    {
                        index = i;
                        break;
                    }
                if (index >= 0)
                    return innerList[index];
                else
                    return null;
            }
            set
            {
                int index = -1;
                for(int i=0;i<innerList.Count;i++)
                    if (innerList[i].Number == season)
                    {
                        index = i;
                        break;
                    }
                innerList[index] = value;
            }
        }

        #endregion

        #region Variable

        /// <summary>
        /// The internal list of TvSeasons.
        /// </summary>
        private List<TvSeason> innerList;

        #endregion

        #region Properties

        /// <summary>
        /// Number of elements in the collection
        /// </summary>
        public virtual int Count
        {
            get
            {
                return innerList.Count;
            }
        }

        /// <summary>
        /// Gets whether collection is synchronized. Not supported, always false.
        /// </summary>
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Synchronization root.
        /// </summary>
        public virtual object SyncRoot { get; set; }

        /// <summary>
        /// Gets the highest season number.
        /// </summary>
        public virtual int HighestSeason
        {
            get
            {
                int highest = -1;
                foreach (TvSeason season in innerList)
                    if (season.Number > highest)
                        highest = season.Number;
                return highest;
            }
        }

        /// <summary>
        /// Gets the lowest season number.
        /// </summary>
        public virtual int LowestSeason
        {
            get
            {
                int lowest = int.MaxValue;
                foreach (TvSeason season in innerList)
                    if (season.Number < lowest)
                        lowest = season.Number;
                if (lowest == int.MaxValue)
                    return -1;
                return lowest;
            }
        }

        /// <summary>
        /// Get whether the collection is read-only, always false.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a season to the collection.
        /// </summary>
        /// <param name="season"></param>
        public virtual void Add(TvSeason season)
        {
            innerList.Add(season);
        }

        /// <summary>
        /// Remove a season from the collection
        /// </summary>
        /// <param name="season">The season to remove</param>
        /// <returns>Whether the season was removed</returns>
        public virtual bool Remove(TvSeason season) 
        {
            for (int i = 0; i < innerList.Count; i++)
            {
                if (innerList[i].Number == season.Number)
                {
                    innerList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets whether collection contains a specific season number.
        /// </summary>
        /// <param name="season"></param>
        /// <returns></returns>
        public bool Contains(int season)
        {
            foreach (TvSeason obj in innerList)
                if (obj.Number == season)
                    return true;
            return false;
        }

        /// <summary>
        /// Gets whether a season is contained in the collection.
        /// </summary>
        /// <param name="season"></param>
        /// <returns></returns>
        public bool Contains(TvSeason season)
        {
            foreach (TvSeason obj in innerList)
                if (obj == season)
                    return true;
            return false;
        }
 
        /// <summary>
        /// Copy objects from this collection into a TvSeason array.
        /// TODO: not implemented yet!
        /// </summary>
        /// <param name="array">Array to copy to into</param>
        /// <param name="index"></param>
        public virtual void CopyTo(TvSeason[] array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copy objects from this collection into a an Array.
        /// TODO: not implemented yet!
        /// </summary>
        /// <param name="array">Array to copy to into</param>
        /// <param name="index"></param>
        public virtual void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear the collection of all it's elements
        /// </summary>
        public virtual void Clear()
        {
            innerList.Clear();
        }

        /// <summary>
        /// Returns custom generic enumerator for this collection
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<TvSeason> GetEnumerator()
        {
            return new TvSeasonEnumerator(this);
        }

        /// <summary>
        /// Returns custom generic enumerator for this collection
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TvSeasonEnumerator(this);
        }

        #endregion
    }

    /// <summary>
    /// Enumerator for TvSeasonCollection.
    /// </summary>
    #region Enumerator

    public class TvSeasonEnumerator : IEnumerator<TvSeason>
    {
        /// <summary>
        /// Enumerated collection
        /// </summary>
        protected TvSeasonCollection _collection;

        /// <summary>
        /// Current index
        /// </summary>
        protected int _seasonIndex;

        /// <summary>
        /// Current enumerated object in the collection
        /// </summary>
        protected TvSeason _current;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TvSeasonEnumerator()
        {
        }

        /// <summary>
        /// Constructor which takes the collection which this enumerator will enumerate
        /// </summary>
        /// <param name="collection"></param>
        public TvSeasonEnumerator(TvSeasonCollection collection)
        {
            _collection = collection;
            _seasonIndex = -1;
            _current = default(TvSeason);
        }

        /// <summary>
        /// Current Enumerated object in the inner collection
        /// </summary>
        public virtual TvSeason Current
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Explicit non-generic interface implementation for IEnumerator
        /// (extended and required by IEnumerator<T>)
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Disposes enumerator
        /// </summary>
        public virtual void Dispose()
        {
            _collection = null;
            _current = default(TvSeason);
            _seasonIndex = -1;
        }

        /// <summary>
        ///  Move to next element in the inner collection
        /// </summary>
        /// <returns></returns>
        public virtual bool MoveNext()
        {
            while (++_seasonIndex <= _collection.HighestSeason)
            {
                if (_collection.Contains(_seasonIndex))
                {
                    _current = _collection[_seasonIndex];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reset the enumerator
        /// </summary>
        public virtual void Reset()
        {
            _current = default(TvSeason); //reset current object
            _seasonIndex = -1;
        }
    }

    #endregion
}

