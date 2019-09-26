using SolrLib.Domain;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolrLib.Operations
{
    public class SolrCommands<T> : BaseOperation<T>, ISolrCommands<T> where T : SolrCore
    {
        public SolrCommands(string solrServerUrl, string coreName) : base(solrServerUrl, coreName)
        {
        }

        public void Add(T entity)
        {
            base._solrInstance.Add(entity);
            base._solrInstance.Commit();
        }

        public void Add(IEnumerable<T> entities)
        {
            base._solrInstance.AddRange(entities);
            base._solrInstance.Commit();
        }

        public void Delete(T entity)
        {
            base._solrInstance.Delete(entity);
            base._solrInstance.Commit();
        }

        public void Delete(IEnumerable<T> entities)
        {
            base._solrInstance.Delete(entities);
            base._solrInstance.Commit();
        }

        public void Delete(string query)
        {
            base._solrInstance.Delete(base._solrInstance.Query(query));
            base._solrInstance.Commit();
        }

        public void DeleteAll()
        {
            base._solrInstance.Delete(SolrQuery.All);
            base._solrInstance.Commit();
        }

        public void CommitTransaction()
        {
            base._solrInstance.Commit();
        }
        public void Optimize()
        {
            base._solrInstance.Optimize();
        }
    }
}
