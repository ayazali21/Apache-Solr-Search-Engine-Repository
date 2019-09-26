using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrLib.Domain;
using SolrLib.Domain.Request;
using SolrLib.Domain.Response;
using SolrLib.Extensions;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace SolrLib.Operations
{
    public class SolrQueries<T> : BaseOperation<T>, ISolrQueries<T> where T : SolrCore
    {
        public SolrQueries(string solrServerUrl, string coreName) : base(solrServerUrl, coreName)
        {
        }

        public QueryResult<T> Get(QueryRequest request)
        {
            SetRequestDefaultValues(request);

            var solrProds = Execute(request);
            var searchResponse = new QueryResult<T>
            {
                NumberOfTotalResults = solrProds.NumFound,
                Results = solrProds,
                CurrentPage = request.Start + 1,
                PageSize = request.Rows
            };
            return searchResponse;
        }

        public QueryGroupResult<T> GetGroup(QueryRequest request)
        {
            SetRequestDefaultValues(request);

            var solrProds = Execute(request);

            var searchResponse = new QueryGroupResult<T>
            {
                Groups = solrProds.Grouping[request.GroupByField].Groups.Select(x => new Domain.Response.Group<T>()
                {
                    Count = x.NumFound,
                    value = x.GroupValue,
                    Documents = x.Documents.ToList(),
                    valueField = request.GroupByField
                }).ToList(),
                TotalResults = solrProds.Grouping[request.GroupByField].Matches,
                CurrentPage = request.Start + 1,
                PageSize = request.Rows,
                FilteredResults = solrProds.Grouping[request.GroupByField].Ngroups.HasValue ? solrProds.Grouping[request.GroupByField].Ngroups.Value : 0,
            };
            return searchResponse;
        }

        public QueryResult<T> GetRaw(string query)
        {
            var queryResults = new SolrQueryResults<T>();
            query = query.RemoveSpecialCharacters();
            if (!string.IsNullOrWhiteSpace(query))
            {
                queryResults = base._solrInstance.Query(query.RemoveSpecialCharacters());
            }

            var solrProds = queryResults;
            var searchResponse = new QueryResult<T>
            {
                NumberOfTotalResults = solrProds.NumFound,
                Results = solrProds,
                CurrentPage = 0,
                PageSize = 0
            };
            return searchResponse;
        }

        public PingResult Ping()
        {
            var pingResponse = _solrInstance.Ping();
            return new PingResult
            {
                QTime = pingResponse.QTime,
                Status = pingResponse.Status,
                Params = pingResponse.Params
            };
        }

        #region Private Functions

        private static void SetRequestDefaultValues(QueryRequest request)
        {
            if (request.Start < 0)
            {
                request.Start = 0;
            }
            if (request.Rows < 1)
            {
                request.Rows = 50;
            }
        }

        private SolrQueryResults<T> Execute(QueryRequest request)
        {
            var filters = this.BuildFilters(request.Criterias);
            var baseQuery = this.BuildQuery(request.Criterias, request.BoostQuery);
            ICollection<SortOrder> orderCollection = new List<SortOrder>();
            if (request.OrderBy.Any())
            {
                foreach (var item in request.OrderBy)
                {
                    Order direction;
                    Enum.TryParse(item.SortBy.ToString(), true, out direction);
                    orderCollection.Add(new SortOrder(item.SortField, direction));
                }
            }
            var queryOptions = new QueryOptions
            {
                Rows = request.Rows,
                Facet = new FacetParameters(),
                StartOrCursor = new StartOrCursor.Start(request.Start),
                OrderBy = orderCollection,
                FilterQueries = filters,
                ExtraParams = request.ExtraData
            };
            if (!string.IsNullOrEmpty(request.GroupByField))
            {
                queryOptions.Grouping = new GroupingParameters()
                {
                    Fields = new string[] { request.GroupByField },
                    Format = GroupingFormat.Grouped,
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Ngroups = true,
                    OrderBy = orderCollection,
                };
            }

            // If specific fields are supplied then map only those fields
            if (request.Fields != null && request.Fields.Count > 0)
            {
                queryOptions.Fields = request.Fields;
            }

            var queryResults = _solrInstance.Query(baseQuery, queryOptions);

            //if (!string.IsNullOrWhiteSpace(request.GroupByField))
            //{
            //    var numberGroupResults = queryResults.Grouping[request.GroupByField];
            //    var result = queryResults.Grouping[request.GroupByField].Groups;
            //    var result2 = queryResults.Grouping[request.GroupByField].Groups.FirstOrDefault();

            //}

            return queryResults;
        }

        private ICollection<ISolrQuery> BuildFilters(IEnumerable<CriteriaInfo> filters)
        {
            var filterInfos = filters as CriteriaInfo[] ?? filters.ToArray();
            if (!filterInfos.Any())
            {
                return null;
            }

            var solrQueryFilters = new List<ISolrQuery>();

            foreach (var filter in filterInfos.Where(c => c.CriteriaType == CriteriaType.Filter))
            {
                AddFilterQuery(solrQueryFilters, filter.CriteriaValues, filter.SolrFieldName, filter.DefaultOperator, filter.Quoted, filter.IsRange);
            }

            return solrQueryFilters;
        }

        private void AddFilterQuery(List<ISolrQuery> filters, IEnumerable<string> filterValues, string solrFieldName, string defaultOperator = "AND", bool quoted = false, bool isRange = false)
        {
            if (!filterValues.Any())
            {
                return;
            }
            var list = new List<SolrQueryByField>();
            foreach (var filterValue in filterValues)
            {
                var normalizedFilterValue = isRange ? filterValue : filterValue.RemoveSpecialCharacters();
                if (!string.IsNullOrWhiteSpace(normalizedFilterValue))
                {
                    var queryByField = new SolrQueryByField(solrFieldName, normalizedFilterValue);
                    queryByField.Quoted = quoted;// || filterValues.ToList().Any(c => c.Contains(" "));
                    list.Add(queryByField);
                }
            }
            if (list.Any())
            {
                filters.Add(new SolrMultipleCriteriaQuery(list, defaultOperator));
            }


        }

        private ISolrQuery BuildQuery(List<CriteriaInfo> filterInfos, string boostQuery)
        {
            var query = string.Empty;

            foreach (var filter in filterInfos.Where(c => c.CriteriaType == CriteriaType.Query))
            {
                var subQuery = "";
                foreach (var paramx in filter.CriteriaValues)
                {
                    var param = paramx.RemoveSpecialCharacters();
                    if (string.IsNullOrWhiteSpace(param))
                    {
                        continue;
                    }
                    if (!string.IsNullOrWhiteSpace(subQuery))
                    {
                        subQuery += " AND ";
                    }
                    switch (filter.MatchType)
                    {
                        case MatchType.WildCard:
                            subQuery += $"{filter.SolrFieldName}:(*{param}*)";
                            break;
                        case MatchType.StartsWith:
                            subQuery += $"{filter.SolrFieldName}:({param}*)^{filter.Boost}";
                            break;
                        case MatchType.Exact:
                            subQuery += $"{filter.SolrFieldName}:({param})^{filter.Boost}";
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(subQuery))
                {
                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        query = "(" + query + ")";
                        query +=$" {filter.DefaultOperator} ";
                    }
                    query += "(" + subQuery + ")";
                }
            }

            var orderingQuery = string.Empty;

            foreach (var filter in filterInfos.Where(c => c.CriteriaType == CriteriaType.Ordering))
            {
                var subQuery = "";
                foreach (var paramx in filter.CriteriaValues)
                {
                    var param = paramx.RemoveSpecialCharacters();
                    if (string.IsNullOrWhiteSpace(param))
                    {
                        continue;
                    }
                    switch (filter.MatchType)
                    {
                        case MatchType.Exact:
                            subQuery += $"{filter.SolrFieldName}:({param})^{filter.Boost}";
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(subQuery))
                {
                    if (!string.IsNullOrWhiteSpace(orderingQuery))
                    {
                        orderingQuery += " OR ";
                    }
                    orderingQuery += subQuery;
                }
            }
            if (!string.IsNullOrWhiteSpace(orderingQuery))
            {
                query = $"({query}) AND ({orderingQuery})";
            }
            //foreach (var searchParameterse in param)
            //{
            //    if (!string.IsNullOrWhiteSpace(query))
            //    {
            //        query += " OR ";
            //    }
            //    query += $"{searchParameterse.Key}:(*{searchParameterse.Value}*) ";
            //}
            if (string.IsNullOrWhiteSpace(boostQuery))
            {
                boostQuery = string.Empty;
            }
            if (!string.IsNullOrEmpty(query))
                return new SolrQuery($"{boostQuery}{query}");
            return new SolrQuery($"{boostQuery}*:*");
        }
        #endregion
    }
}
