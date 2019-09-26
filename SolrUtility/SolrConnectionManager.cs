using SolrNet;
using SolrNet.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolrLib.SolrUtility
{
    /// <summary>
    /// Manages a state whether a SolrConnection for a particuler set of Server Url and Core Name is been initialized or not.
    /// Initializes the connection if not already initialized.
    /// </summary>
    public static class SolrConnectionManager
    {
        private static readonly List<string> InitializedCoreUrls;


        static SolrConnectionManager()
        {
            InitializedCoreUrls = new List<string>();
        }

        /// <summary>
        /// Initializes the SolrConnection if not already initialized for the given Server Url and Core Name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="solrServerUrl"></param>
        /// <param name="coreName"></param>
        public static void InitConnection<T>(string solrServerUrl, string coreName)
        {
            var coreUrl = solrServerUrl + coreName;
            if (InitializedCoreUrls.Any(x => x.Equals(coreUrl, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            //https://localhost/solr/product
            //https://localhost/solr/product_translated
            var connection = new SolrConnection(solrServerUrl + coreName);
            var loggingConnection = new LoggingConnection(connection);
            Startup.Init<T>(loggingConnection);
            var obj = new object();
            lock (obj)
            {
                InitializedCoreUrls.Add(coreUrl);
            }
            
        }
    }
}
