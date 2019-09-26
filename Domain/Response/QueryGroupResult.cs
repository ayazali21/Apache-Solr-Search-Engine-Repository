using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolrLib.Domain.Response
{
    public class Group<T>
    {
        public List<T> Documents { get; set; }

        public string valueField { get; set; }

        public string value { get; set; }

        public long Count { get; set; }

    }


    public class QueryGroupResult<T>
    {
        public List<Group<T>> Groups { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int FilteredResults { get; set; }

        public int TotalResults { get; set; }

        public List<T> SelectedResult { get; set; }
    }
}
