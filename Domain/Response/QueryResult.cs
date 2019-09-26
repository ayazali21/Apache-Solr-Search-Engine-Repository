using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolrLib.Domain.Response
{
    public class QueryResult<T>
    {
        public int CurrentPage { get; set; }
        
        public int PageSize { get; set; }
       
        public List<T> Results { get; set; }
       
        public int NumberOfFilteredResults { get; set; }
       
        public int NumberOfTotalResults { get; set; }
    }
}
