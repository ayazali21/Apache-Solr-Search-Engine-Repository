using SolrLib.Domain;
using SolrLib.Domain.Request;
using SolrLib.Domain.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolrLib.Operations
{
    public interface ISolrQueries<T> where T : SolrCore
    {
        QueryResult<T> Get(QueryRequest request);
        QueryGroupResult<T> GetGroup(QueryRequest request);
        QueryResult<T> GetRaw(string query);
        PingResult Ping();
    }
}
