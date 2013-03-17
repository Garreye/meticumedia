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
using System.Windows.Forms;

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

        public string Name { get; set; }

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
            loaded = true;
        }

        private bool loaded = false;

        /// <summary>
        /// Static event that fires when content is added to the collection
        /// </summary>
        public event EventHandler ContentAdded;

        /// <summary>
        /// Triggers ContentAdded event
        /// </summary>
        public void OnContentAdded()
        {
            loaded = true;
            if (!loaded)
                return;

            if (ContentAdded != null)
                ContentAdded(this, new EventArgs());
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
                Console.WriteLine(this.ToString() + " lock add");
                base.Add(item);
            }
            OnContentAdded();
            Console.WriteLine(this.ToString() + " release add");
        }

        public new void Remove(Content item)
        {
            lock (ContentLock)
            {
                Console.WriteLine(this.ToString() + " lock remove");
                base.Remove(item);
            }
            Console.WriteLine(this.ToString() + " release remove");
        }

        public new void RemoveAt(int index)
        {
            lock (ContentLock)
            {
                Console.WriteLine(this.ToString() + " lock removeAt");
                base.RemoveAt(index);
            }
            Console.WriteLine(this.ToString() + " release removeAt");
        }

        public new void Sort()
        {
            lock (ContentLock)
            {
                Console.WriteLine(this.ToString() + " lock sort");
                base.Sort();
            }
            Console.WriteLine(this.ToString() + " release sort");
        }

        public new void Clear()
        {
            lock (ContentLock)
            {
                Console.WriteLine(this.ToString() + " lock clear");
                base.Clear();
            }
            Console.WriteLine(this.ToString() + " release clear");
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
                        lock (XmlLock)
                        {

                            // Load XML
                            reader = new XmlTextReader(path);
                            xmlDoc.Load(reader);

                            // Load show data
                            ContentCollection loadContent = new ContentCollection(this.ContentType, "Loading Shows");
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

                            OnLoadProgressChange(100);
                            lock (ContentLock)
                            {
                                Console.WriteLine(this.ToString() + " lock load");
                                this.LastUpdate = loadContent.LastUpdate;
                                this.Clear();
                                foreach (Content content in loadContent)
                                    base.Add(content);
                            }
                            Console.WriteLine(this.ToString() + " release load");
                        }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }

            // Start updating of TV episode in scan dirs.
                if (this.ContentType == ContentType.TvShow)
                {
                    TvItemInScanDirHelper.DoUpdate();
                    TvItemInScanDirHelper.StartUpdateTimer();
                }

            OnLoadComplete();
        }

        /// <summary>
        /// Saves collection to XML file
        /// </summary>
        public void Save()
        {
            // Get path to xml file
            string path = Path.Combine(Organization.GetBasePath(true), XML_ROOT + ".xml");
            string tempPath = Path.Combine(Organization.GetBasePath(true), XML_ROOT + "_TEMP.xml");

            // Lock content and file
            lock (ContentLock)
            {
                Console.WriteLine(this.ToString() + " lock save");
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
                    if (File.Exists(path))
                        File.Delete(path);
                    File.Move(tempPath, path);
                }
            }
            Console.WriteLine(this.ToString() + " release save");
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
                Console.WriteLine(this.ToString() + " lock removeMissing");
                for (int i = this.Count - 1; i >= 0; i--)
                    if (!this[i].Found && this[i].RootFolder.StartsWith(rootFolder.FullPath))
                        base.RemoveAt(i);
            }
            Console.WriteLine(this.ToString() + " release removeMissing");
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
