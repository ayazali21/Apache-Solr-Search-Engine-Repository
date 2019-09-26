using CommonServiceLocator;
using SolrLib.Domain;
using SolrLib.SolrUtility;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolrLib.Operations
{
    public abstract class BaseOperation<T> where T : SolrCore
    {
        protected readonly ISolrOperations<T> _solrInstance;

        public BaseOperation(string solrServerUrl, string coreName)
        {
            SolrConnectionManager.InitConnection<T>(solrServerUrl, coreName);
            _solrInstance = ServiceLocator.Current.GetInstance<ISolrOperations<T>>();
        }
    }
}
