// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.ComponentModel;

namespace Meticumedia
{
    /// <summary>
    /// List of content with added properties.
    /// </summary>
    public class ContentCollection : List<Content>
    {
        #region Properties

        /// <summary>
        /// Database time when collection was updated.
        /// </summary>
        public string LastUpdate { get; set; }

        /// <summary>
        /// Type of content stored in collection
        /// </summary>
        public ContentType ContentType { get; set; }

        /// <summary>
        /// Lock for accessing content
        /// </summary>
        public object ContentLock = new object();

        /// <summary>
        /// Lock for accessing content xml file
        /// </summary>
        public object XmlLock = new object();

        #endregion

        #region Events

        /// <summary>
        /// Static event that fires when show loading progress changes
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> LoadProgressChange;

        /// <summary>
        /// Triggers TvShowLoadProgressChange event
        /// </summary>
        public void OnLoadProgressChange(int percent)
        {
            if (LoadProgressChange != null)
                LoadProgressChange(this, new ProgressChangedEventArgs(percent, null));
        }

        /// <summary>
        /// Static event that fires when show loading completes
        /// </summary>
        public event EventHandler LoadComplete;

        /// <summary>
        /// Triggers TvShowLoadComplete event
        /// </summary>
        public void OnLoadComplete()
        {
            if (LoadComplete != null)
                LoadComplete(this, new EventArgs());
        }

        /// <summary>
        /// Indicates that contents have changed.
        /// </summary>
        public event EventHandler ContentSaved;

        /// <summary>
        /// Triggers ShowsChanged event
        /// </summary>
        protected void OnContentSaved()
        {
            //UpdateShow(false);
            if (ContentSaved != null)
                ContentSaved(this, new EventArgs());
        }  

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentCollection(ContentType type)
            : base()
        {
            this.ContentType = type;
        }

        #endregion

        #region Constants/Variable

        private string XML_ROOT { get { return this.ContentType.ToString() + "s"; } }

        #endregion

        #region New base methods with locking

        public new void Add(Content item)
        {
            lock (ContentLock)
                base.Add(item);
        }

        public new void Remove(Content item)
        {
            lock (ContentLock)
                base.Remove(item);
        }

        public new void RemoveAt(int index)
        {
            lock (ContentLock)
                base.RemoveAt(index);
        }

        public new void Sort()
        {
            lock (ContentLock)
                base.Sort();
        }

        public new void Clear()
        {
            lock (ContentLock)
                base.Clear();
        }

        #endregion

        #region Methods

        public void Load()
        {
            try
            {
                string path = Path.Combine(Organization.GetBasePath(false), XML_ROOT + ".xml");
                if (File.Exists(path))
                    lock (XmlLock)
                    {

                        // Load XML
                        XmlTextReader reader = new XmlTextReader(path);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(reader);

                        // Load show data
                        ContentCollection loadContent = new ContentCollection(this.ContentType);
                        XmlNodeList contentNodes = xmlDoc.DocumentElement.ChildNodes;
                        for (int i = 0; i < contentNodes.Count; i++)
                        {
                            OnLoadProgressChange((int)(((double)i / contentNodes.Count) * 100));

                            if (contentNodes[i].Name == "LastUpdate")
                                loadContent.LastUpdate = contentNodes[i].InnerText;
                            else
                            {
                                switch (this.ContentType)
                                {
                                    case ContentType.TvShow:
                                        TvShow show = new TvShow();
                                        if (show.Load(contentNodes[i]))
                                        {
                                            loadContent.Add(show);
                                            show.UpdateMissing();
                                        }
                                        break;
                                    case ContentType.Movie:
                                        Movie movie = new Movie();
                                        if (movie.Load(contentNodes[i]))
                                            loadContent.Add(movie);
                                        break;
                                    default:
                                        throw new Exception("Unknown content type");
                                }
                            }
                        }

                        reader.Close();
                        OnLoadProgressChange(100);
                        lock (ContentLock)
                        {
                            this.LastUpdate = loadContent.LastUpdate;
                            this.Clear();
                            foreach (Content content in loadContent)
                                this.Add(content);
                        }
                    }
            }
            catch { }

            // Start updating of TV episode in scan dirs.
            if (this.ContentType == ContentType.TvShow)
                ScanHelper.UpdateScanDirTvItems();

            OnLoadComplete();
        }

        public void Save()
        {
            // Get path to xml file
            string path = Path.Combine(Organization.GetBasePath(true), XML_ROOT + ".xml");

            // Lock content and file
            lock (ContentLock)
                lock (XmlLock)
                    using (XmlWriter xw = XmlWriter.Create(path))
                    {
                        xw.WriteStartElement(XML_ROOT);
                        xw.WriteElementString("LastUpdate", this.LastUpdate);

                        foreach (Content content in this)
                            content.Save(xw);
                        xw.WriteEndElement();
                    }
            OnContentSaved();
        }

        /// <summary>
        /// Remove all content that no longer exist in a root folder
        /// </summary>
        /// <param name="rootFolder">Root folder to remove missing content from</param>
        public void RemoveMissing(ContentRootFolder rootFolder)
        {
            lock (this.ContentLock)
                for (int i = this.Count - 1; i >= 0; i--)
                    if (!this[i].Found && this[i].RootFolder.StartsWith(rootFolder.FullPath))
                        this.RemoveAt(i);
        }

        /// <summary>
        /// Get a lists of shows that have the include in scan property enabled.
        /// </summary>
        /// <returns>List of show that are included in scanning</returns>
        public List<Content> GetScannableContent(bool updateMissing)
        {
            List<Content> includeShow = new List<Content>();
            for (int i = 0; i < this.Count; i++)
                if (this[i].IncludeInScan && !string.IsNullOrEmpty(this[i].Name))
                {
                    if (updateMissing)
                        this[i].UpdateMissing();
                    includeShow.Add(this[i]);
                }
            return includeShow;
        }

        #endregion

    }
}
