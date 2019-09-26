using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolrLib.Domain.Request
{
    public class QueryRequest
    {
        public QueryRequest()
        {
            Criterias = new List<CriteriaInfo>();
            //DefaultSearchCriteria = new Dictionary<string, string>();
            ExtraData = new Dictionary<string, string>();
            OrderBy = new List<Sort>();
        }
        public List<CriteriaInfo> Criterias { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public bool Facet { get; set; }
        public int Start { get; set; }

        public int Rows { get; set; }
        public List<Sort> OrderBy { get; set; }

        public ICollection<string> Fields { get; set; }

        public string GroupByField { get; set; }
        public Dictionary<string, string> ExtraData { get; set; }
        public string BoostQuery { get; set; }
    }
    public class CriteriaInfo
    {
        public MatchType MatchType { get; set; } = MatchType.WildCard;
        public CriteriaType CriteriaType { get; set; } = CriteriaType.Filter;
        public double Boost { get; set; }
        public string SolrFieldName { get; set; }
        public string DefaultOperator { get; set; } = "AND";
        public bool Quoted { get; set; } = false;
        public IEnumerable<string> CriteriaValues { get; set; }
        public bool IsRange { get; set; }
    }

    public class Sort
    {
        public SortBy SortBy { get; set; }
        public string SortField { get; set; }
    }

    public enum CriteriaType
    {
        Filter,
        Query,
        Ordering // add at the end of Query to get proper ranking
    }

    public enum SortBy
    {
        None,
        Asc,
        Desc
    }

    public enum MatchType
    {
        WildCard,
        StartsWith,
        Exact
    }
}
