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
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Meticumedia.Classes
{
    /// <summary>
    /// List of content with added properties. (Iheriting list to have sorting methods)
    /// </summary>
    public class ContentCollection : List<Content>, INotifyCollectionChanged
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

        /// <summary>
        /// Name of collection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag indicating collection loading is complete
        /// </summary>
        public bool LoadCompleted { get { return loaded; } }

        #endregion

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
        }

        public void OnCollectionChanged(NotifyCollectionChangedAction action, Content item)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item));
        }

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
            loaded = true;
        }

        /// <summary>
        /// Indication of whether loading is complete
        /// </summary>
        private bool loaded = false;

        /// <summary>
        /// Indicates that contents have been saved.
        /// </summary>
        public event EventHandler ContentSaved;

        /// <summary>
        /// Triggers ContentSaved event
        /// </summary>
        protected void OnContentSaved()
        {
            if (ContentSaved != null)
                ContentSaved(this, new EventArgs());
        }  

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentCollection(ContentType type, string name)
            : base()
        {
            this.ContentType = type;
            this.Name = name;
        }

        /// <summary>
        /// Override ToString to use Name property
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region Constants/Variable

        /// <summary>
        /// Root string from XML
        /// </summary>
        private string XML_ROOT { get { return this.ContentType.ToString() + "s"; } }

        #endregion

        #region New base methods with locking

        public new void Add(Content item)
        {
            lock (ContentLock)
            {
                base.Add(item);
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        public new void Remove(Content item)
        {
            lock (ContentLock)
            {
                base.Remove(item);
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        }

        public new void RemoveAt(int index)
        {
            Content item = this[index];
            lock (ContentLock)
            {
                base.RemoveAt(index);
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        }

        public new void Clear()
        {
            lock (ContentLock)
            {
                base.Clear();
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public new void Sort()
        {
            lock (ContentLock)
            {
                base.Sort();
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load collection from saved XML file
        /// </summary>
        public void Load()
        {
            XmlTextReader reader = null;
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                string path = Path.Combine(Organization.GetBasePath(false), XML_ROOT + ".xml");

                if (File.Exists(path))
                {
                    // Use dummy collection to load into so that loading doesn't hog use of object
                    ContentCollection loadContent = new ContentCollection(this.ContentType, "Loading Shows");
                    lock (XmlLock)
                    {
                        // Load XML
                        reader = new XmlTextReader(path);
                        xmlDoc.Load(reader);

                        // Extract data
                        XmlNodeList contentNodes = xmlDoc.DocumentElement.ChildNodes;
                        for (int i = 0; i < contentNodes.Count; i++)
                        {
                            // Update loading progress
                            OnLoadProgressChange((int)(((double)i / contentNodes.Count) * 100));

                            // All elements will be content items or last update time
                            if (contentNodes[i].Name == "LastUpdate")
                                loadContent.LastUpdate = contentNodes[i].InnerText;
                            else
                            {
                                // Load content from element based on type
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
                    }
                    // Update progress
                    OnLoadProgressChange(100);

                    // Lock collection and load content items from dummy collection
                    lock (ContentLock)
                    {
                        //Console.WriteLine(this.ToString() + " lock load");
                        this.LastUpdate = loadContent.LastUpdate;
                        this.Clear();
                        foreach (Content content in loadContent)
                            Add(content);
                    }
                    //Console.WriteLine(this.ToString() + " release load");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error loading " + this.ContentType + "s from saved data!");
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            // Start updating of TV episode in scan dirs.
            if (this.ContentType == ContentType.TvShow)
            {
                TvItemInScanDirHelper.DoUpdate(false);
                TvItemInScanDirHelper.StartUpdateTimer();
            }

            // Trigger load complete event
            OnLoadComplete();
        }

        /// <summary>
        /// Saves collection to XML file
        /// </summary>
        public void Save()
        {
            // Get path to xml file
            string path = Path.Combine(Organization.GetBasePath(true), XML_ROOT + ".xml");

            // Save data into temporary file, so that if application crashes in middle of saving XML is not corrupted!
            string tempPath = Path.Combine(Organization.GetBasePath(true), XML_ROOT + "_TEMP.xml");

            // Lock content and file
            lock (ContentLock)
            {
                //Console.WriteLine(this.ToString() + " lock save");
                lock (XmlLock)
                {
                    using (XmlWriter xw = XmlWriter.Create(tempPath))
                    {
                        xw.WriteStartElement(XML_ROOT);
                        xw.WriteElementString("LastUpdate", this.LastUpdate);

                        foreach (Content content in this)
                            content.Save(xw);
                        xw.WriteEndElement();
                    }

                    // Delete previous save data
                    if (File.Exists(path))
                        File.Delete(path);

                    // Move tempoarary save file to default
                    File.Move(tempPath, path);
                }
            }
            //Console.WriteLine(this.ToString() + " release save");

            // Trigger content saved event
            OnContentSaved();
        }

        /// <summary>
        /// Remove all content that no longer exist in a root folder
        /// </summary>
        /// <param name="rootFolder">Root folder to remove missing content from</param>
        public void RemoveMissing(ContentRootFolder rootFolder)
        {
            lock (this.ContentLock)
            {
                for (int i = this.Count - 1; i >= 0; i--)
                    if (!this[i].Found && this[i].RootFolder.StartsWith(rootFolder.FullPath))
                        RemoveAt(i);
            }
        }

        /// <summary>
        /// Get a lists of content that are set to be included in scanning (by IncludeInScan property)
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
