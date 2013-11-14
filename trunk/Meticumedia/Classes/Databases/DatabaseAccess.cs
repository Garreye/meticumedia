using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Meticumedia
{
    public abstract class DatabaseAccess
    {
        #region Constants/Enums

        /// <summary>
        /// Database mirror types
        /// </summary>
        protected enum MirrorType { Xml = 1, Banner = 2, Zip = 4, Json = 8 }

        /// <summary>
        /// Type of mirror used for searching
        /// </summary>
        protected virtual MirrorType SearchMirrorType { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Type of mirror used for updating
        /// </summary>
        protected virtual MirrorType UpdateMirrorType { get { throw new NotImplementedException(); } }
        
        /// <summary>
        /// API Key for accessing database
        /// </summary>
        protected virtual string API_KEY { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Whether to limit rate of JSON requests or not
        /// </summary>
        protected virtual JsonRateLimit JSON_RATE_LIMITER { get { throw new NotImplementedException(); } }


        protected virtual string API_KEY_PARAM_NAME { get { return "api_key"; } }

        #endregion

        #region Variables

        /// <summary>
        /// Indicates whether mirors are valid.
        /// </summary>
        protected bool mirrorsValid = false;

        /// <summary>
        /// XML mirrors
        /// </summary>
        protected List<string> xmlMirrors;

        /// <summary>
        /// Zip mirrors
        /// </summary>
        protected List<string> zipMirrors;

        /// <summary>
        /// Zip mirrors
        /// </summary>
        protected List<string> jsonMirrors;

        #endregion

        #region Mirrors

        /// <summary>
        /// Updates local list of database mirrors.
        /// </summary>
        public virtual void UpdatesMirrors()
        {
            mirrorsValid = true;
        }

        /// <summary>
        /// Gets a database mirror.
        /// </summary>
        /// <returns>Whether mirror was found</returns>
        protected bool GetMirror(MirrorType type, out string mirror)
        {
            if (!mirrorsValid)
                UpdatesMirrors();

            if (mirrorsValid)
            {
                // Randomly select mirror
                switch (type)
                {
                    case MirrorType.Xml:
                        mirror = xmlMirrors[(new Random()).Next(xmlMirrors.Count)];
                        return true;
                    case MirrorType.Zip:
                        mirror = zipMirrors[(new Random()).Next(zipMirrors.Count)];
                        return true;
                    case MirrorType.Json:
                        mirror = jsonMirrors[(new Random()).Next(jsonMirrors.Count)];
                        return true;
                    default:
                        throw new Exception("Unknown mirror");
                }
            }

            mirror = string.Empty;
            return false;
        }

        #endregion

        #region Json Request

        protected class JsonRateLimit
        {
            /// <summary>
            /// Queue of times when database requests were made
            /// </summary>
            private Queue<DateTime> requestTimes = new Queue<DateTime>();
            
            public bool Enabled { get; set; }

            public int MaxRequests { get; set; }

            public int RequestLimitTime { get; set; }

            public JsonRateLimit(bool en, int maxRequests, int limitTime)
            {
                this.Enabled = en;
                this.MaxRequests = maxRequests;
                this.RequestLimitTime = limitTime;
            }

            public void DoWait()
            {
                if (!this.Enabled)
                    return;
                
                // Check rate of requests - maximum is 30 requests every 10 second
                DateTime oldest;
                lock (requestTimes)
                {
                    while (requestTimes.Count >= this.MaxRequests - 1)
                    {
                        oldest = requestTimes.Peek();
                        double requestAge = (DateTime.Now - oldest).TotalMilliseconds;
                        if (requestAge > this.RequestLimitTime)
                            requestTimes.Dequeue();
                        else
                            Thread.Sleep((RequestLimitTime + 1 - (int)requestAge));

                    }
                    requestTimes.Enqueue(DateTime.Now);
                }
            }
        }

        /// <summary>
        /// Performs HTTP JSON get from database.
        /// </summary>
        /// <param name="get">Get type string</param>
        /// <param name="parameters">Parameter for get</param>
        /// <returns>Get results as value of ResultNode with no name</returns>
        protected JsonNode GetJsonRequest(string mirror, List<HttpGetParameter> parameters, string page = "")
        {
            // Limit request rate
            JSON_RATE_LIMITER.DoWait();

            // Build URL
            string url = mirror + page + "?" + API_KEY_PARAM_NAME + "=" + this.API_KEY;
            if (parameters != null)
                foreach (HttpGetParameter param in parameters)
                    url += "&" + param.Name + "=" + param.Value;

            // Setup request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "application/json";
            HttpWebResponse response = null;

            // Execute the request - perform up to 5 times, has a tendency to fail randomly
            bool requestSucess = false;
            for (int i = 0; i < 5; i++)
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    requestSucess = true;
                    break;
                }
                catch 
                {
                    Thread.Sleep(2000);
                }

            // If no response return empty results
            if (!requestSucess)
                return new JsonNode(string.Empty, string.Empty);

            // Convert response into a string
            StreamReader stream = new StreamReader(response.GetResponseStream());
            string sLine = "";
            string contents = string.Empty;
            while (sLine != null)
            {
                sLine = stream.ReadLine();
                if (sLine != null)
                    contents += sLine;
            }

            // Return response string as value of ResultNode with no name - will be parsed for child when created
            return new JsonNode(string.Empty, contents);
        }

        #endregion

        #region Searching/Updating

        /// <summary>
        /// Gets database server's time
        /// </summary>
        /// <param name="time">retrieved server time</param>
        /// <returns>whether time was successfully retrieved</returns>
        public virtual bool GetServerTime(out string time)
        {
            time = string.Empty;
            return true;
        }

        /// <summary>
        /// Return lists of series Ids that need updating. 
        /// </summary>
        /// <param name="ids">List of series id that need to be updated locally</param>
        /// <returns></returns>
        public virtual bool GetDataToBeUpdated(out List<int> ids, out string time)
        {
            ids = new List<int>();
            time = string.Empty;
            return true;
        }

        /// <summary>
        /// Performs search for a show in database - with retrying
        /// </summary>
        /// <param name="searchString">The string to search for</param>
        /// <returns>Array of results from the search</returns>
        public List<Content> PerformSearch(string searchString, bool includeSummaries)
        {
            string mirror;
            if (!GetMirror(this.SearchMirrorType, out mirror))
                return null;

            // Try multiple times - databases requests tend to fail randomly
            for (int i = 0; i < 5; i++)
                try
                {
                    return DoSearch(mirror, searchString, includeSummaries);
                }
                catch(Exception ex)
                { 

                }
            return new List<Content>();
        }

        /// <summary>
        /// Performs search for content in database. Should be overriden!
        /// </summary>
        /// <param name="mirror">Mirror to use</param>
        /// <param name="searchString">Search string for show</param>
        /// <param name="includeSummaries">Whether to include summaries in search results (takes longer - set to false unless user is seeing them)</param>
        /// <returns>Results as list of content</returns>
        protected virtual List<Content> DoSearch(string mirror, string searchString, bool includeSummaries)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs update of content information from database. Should be overriden!
        /// </summary>
        /// <param name="show">Show instance to update</param>
        /// <returns>Updated show instance</returns>
        protected virtual bool DoUpdate(string mirror, Content content)
        {
            throw new NotImplementedException();
        }

        public virtual bool Update(Content content)
        {
            string mirror;
            if (!GetMirror(this.UpdateMirrorType, out mirror))
                return false;

            // Tries up to 5 times - database can fail randomly
            for (int j = 0; j < 5; j++)
            {
                try
                {
                    DoUpdate(mirror, content);
                    return true;

                }
                catch { }
            }

            return false;
        }

        #endregion
    }
}
