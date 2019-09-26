using SolrLib.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolrLib.Operations
{
    public interface ISolrCommands<T> where T : SolrCore
    {
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        void Delete(string query);
        void DeleteAll();
        void CommitTransaction();
        void Optimize();
    }
}
