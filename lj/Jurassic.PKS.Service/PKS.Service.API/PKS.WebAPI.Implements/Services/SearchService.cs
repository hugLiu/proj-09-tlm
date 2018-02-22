using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Principal;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nest;
using PKS.Core;
using PKS.Models;
using PKS.Utils;
//using PKS.Utils;
using PKS.WebAPI.ES;
using PKS.WebAPI.Models;
using SearchRequest = PKS.WebAPI.Models.SearchRequest;

namespace PKS.WebAPI.Services
{
    /// <summary>搜索服务实现</summary>
    public class SearchService : ISearchService, ISingletonAppService
    {
        private ESAccess<Metadata> _esAccess = null;
        private SearchEntity<Metadata> _search = new SearchEntity<Metadata>();
        /// <summary>构造函数</summary>
        public SearchService(IElasticConfig elasticConfig, IMongoConfig mongoConfig, IMongoCollection<MetadataDefinition> accessor)
        {
            MdAccessor = accessor;
            _esAccess = new ESAccess<Metadata>();
        }

        /// <summary>访问器</summary>
        private IMongoCollection<MetadataDefinition> MdAccessor { get; }
        /// <summary>按短语搜索</summary>
        public virtual SearchResult<Metadata> Search(SearchRequest request)
        {
            return Task.Run(() => SearchAsync(request)).Result;
        }
        /// <summary>按短语搜索</summary>
        public virtual async Task<SearchResult<Metadata>> SearchAsync(SearchRequest request)
        {
            var _provider = new SearchProvider<Metadata>();
            var filterQuery = _provider.BuildFilterQuery(request.Filter);
            // var fulltextQuery = _provider.BuildFullTextQuery(request.Sentence, request.Ranks);
            var fulltextQuery = _provider.BuildCustomScoreQuery(request.Sentence, request.Boost);
            var query = _provider.CombineMustQuery(filterQuery, fulltextQuery);
            var sort = _provider.BuildSort(request.Sort);
            var aggs = _provider.BuildAggs(request.Group);
            var fields = _provider.BuildFields(request.Fields);

            var from = request.From;
            var size = request.Size;

            var searchresponse = await _esAccess.PagingQueryAsync(query, sort, fields, aggs, from, size);
            Extensions<Metadata>.ToMetadataCollection(searchresponse);
            //var response = searchresponse.ToMetadataCollection<Metadata>();
            var response = Extensions<Metadata>.ToMetadataCollection(searchresponse);
            if (searchresponse.Aggregations.Count <= 0)
                return await Task.FromResult(response);

            var groups = new Dictionary<string, Dictionary<string, long?>>();
            foreach (var agg in searchresponse.Aggregations)
            {
                var aggregates = agg.Value.As<BucketAggregate>().Items;
                Dictionary<string, long?> dic = new Dictionary<string, long?>();
                foreach (var aggregate in aggregates)
                {
                    var keyedBucket = aggregate as Nest.KeyedBucket<object>;
                    if (keyedBucket != null)
                        dic.Add(keyedBucket.Key.ToString(), keyedBucket.DocCount);
                }
                groups.Add(agg.Key, dic);
            }

            response.Groups = groups;
            return await Task.FromResult(response);
        }


        /// <summary>
        /// 搜索加强版
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual SearchResult<Metadata> SearchPlus(SearchRequest request)
        {
            return Task.Run(() => SearchPlusAsync(request)).Result;
        }
        /// <summary>
        /// 搜索加强版
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<Metadata>> SearchPlusAsync(SearchRequest request)
        {
            var searchresponse = await _search.SearchPlusAsync(request);
            return searchresponse;

            #region 原代码
            /*
            var _provider = new SearchProvider<Metadata>();

            //或关系过滤集合
            List<QueryContainer> shouldQueryContainerList = null;
            //与关系过滤集合
            List<QueryContainer> mustQueryContainerList = null;
            //费关系过滤集合
            List<QueryContainer> mustnotQueryContainerList = null;
            //过滤条件构建
            if (request.Filter != null || request.Filter.Count > 0)
            {
                //循环获取Filter
                foreach (var itemExp in request.Filter)
                {
                    //判断是否包含$or $and $not关键字
                    switch (itemExp.Key)
                    {
                        case "_or"://$or运算符使用 ES Should
                            {
                                shouldQueryContainerList = new List<QueryContainer>();
                                _provider.BuildFilterQuery(shouldQueryContainerList, itemExp.Value);
                                break;
                            }
                        case "_and"://$or运算符使用 ES Must
                            {
                                mustQueryContainerList = new List<QueryContainer>();
                                _provider.BuildFilterQuery(mustQueryContainerList, itemExp.Value);
                                break;
                            }
                        case "_not"://$or运算符使用 ES MustNot
                            {
                                mustnotQueryContainerList = new List<QueryContainer>();
                                _provider.BuildFilterQuery(mustnotQueryContainerList, itemExp.Value);
                                break;
                            }
                        default://默认与关系 不包含运算符
                            {
                                if (mustQueryContainerList == null)
                                    mustQueryContainerList = new List<QueryContainer>();
                                mustQueryContainerList.Add(new TermsQuery { Field = _provider.KeyWord(itemExp.Key), Terms = itemExp.Value });
                                break;
                            }
                    }
                }

            }
            //多条件集合
            var filterQuery = new BoolQuery { Should = shouldQueryContainerList, Must = mustQueryContainerList, MustNot = mustnotQueryContainerList };

            var fulltextQuery = _provider.BuildCustomScoreQuery(request.Sentence, request.Boost);
            var query = _provider.CombineMustQuery(filterQuery, fulltextQuery);
            var sort = _provider.BuildSort(request.Sort);
            var aggs = _provider.BuildAggs(request.Group);
            var fields = _provider.BuildFields(request.Fields);

            var from = request.From;
            var size = request.Size;

            var searchresponse = await _esAccess.PagingQueryAsync(query, sort, fields, aggs, from, size);

            var response = Extensions<Metadata>.ToMetadataCollection(searchresponse);
            if (searchresponse.Aggregations.Count <= 0)
                return await Task.FromResult(response);

            var groups = new Dictionary<string, Dictionary<string, long?>>();
            foreach (var agg in searchresponse.Aggregations)
            {
                var aggregates = agg.Value.As<BucketAggregate>().Items;
                Dictionary<string, long?> dic = new Dictionary<string, long?>();
                foreach (var aggregate in aggregates)
                {
                    var keyedBucket = aggregate as Nest.KeyedBucket<object>;
                    if (keyedBucket != null)
                        dic.Add(keyedBucket.Key.ToString(), keyedBucket.DocCount);
                }
                groups.Add(agg.Key, dic);
            }

            response.Groups = groups;
            return await Task.FromResult(response);
            */
            #endregion
        }

        /// <summary>按ES语法搜索</summary>
        public virtual string ESSearch(string request)
        {
            return Task.Run(() => ESSearchAsync(request)).Result;
        }
        /// <summary>按ES语法搜索</summary>
        public virtual async Task<string> ESSearchAsync(string request)
        {
            if (string.IsNullOrWhiteSpace(request))
            {
                return string.Empty;
            }
            return await _esAccess.GetDocumentsByRawQueryAsyn(request);
        }
        /// <summary>按完全匹配条件搜索</summary>
        public virtual MatchResult<Metadata> Match(MatchRequest request)
        {
            return Task.Run(() => MatchAsync(request)).Result;
        }

        /// <summary>按完全匹配条件搜索</summary>
        public virtual async Task<MatchResult<Metadata>> MatchAsync(MatchRequest request)
        {
            var _provider = new SearchProvider<Metadata>();
            var filterQuery = _provider.BuildFilterQuery(request.Filter);
            var sort = _provider.BuildSort(request.Sort);
            var fields = _provider.BuildFields(request.Fields);

            var from = 0;
            var size = request.Top;
            var searchresponse = await _esAccess.PagingQueryAsync(filterQuery, sort, fields, null, from, size);
            var response = Extensions<Metadata>.ToMetadataCollection(searchresponse);

            return await Task.FromResult(response);
        }
        /// <summary>按多个完全匹配条件搜索</summary>
        public virtual MatchResult<Metadata>[] MatchMany(MatchRequest[] request)
        {
            return Task.Run(() => MatchManyAsync(request)).Result;
        }

        /// <summary>按多个完全匹配条件搜索</summary>
        public virtual async Task<MatchResult<Metadata>[]> MatchManyAsync(MatchRequest[] requests)
        {
            var _provider = new SearchProvider<Metadata>();
            MultiSearchDescriptor multiSearchDescriptor = new MultiSearchDescriptor();
            for (int i = 0; i < requests.Length; i++)
            {
                var request = requests[i];
                var filterQuery = _provider.BuildFilterQuery(request.Filter);
                var sort = _provider.BuildSort(request.Sort);

                var fields = _provider.BuildFields(request.Fields);

                var from = 0;
                var size = request.Top;

                var searchDescriptor = _esAccess.BuildSearchDescriptor(filterQuery, sort, fields, null, from, size);
                multiSearchDescriptor.Search<Metadata>("search" + i, s => searchDescriptor);
            }

            List<MatchResult<Metadata>> result = new List<MatchResult<Metadata>>();
            var searchResponse = await _esAccess.MultiSearch(multiSearchDescriptor);
            foreach (var responseItem in searchResponse.AllResponses)
            {
                var metadata = responseItem as ISearchResponse<Metadata>;
                if (metadata != null)
                {
                    result.Add(Extensions<Metadata>.ToMetadataCollection(metadata));
                };

            }
            return await Task.FromResult(result.ToArray());

        }
        /// <summary>根据iiid搜索</summary>
        public virtual Metadata GetMetadata(SearchMetadataRequest request)
        {
            return Task.Run(() => GetMetadataAsync(request)).Result;
        }

        /// <summary>根据iiid搜索</summary>
        public virtual async Task<Metadata> GetMetadataAsync(SearchMetadataRequest request)
        {
            var searchresponse = await _search.GetEsdataAsync(request);
            var response = searchresponse.Documents.FirstOrDefault();
            return await Task.FromResult(response);
        }
 
        /// <summary>根据iiid数组搜索</summary>
        public virtual MetadataCollection<Metadata> GetMetadatas(SearchMetadatasRequest request)
        {
            return Task.Run(() => GetMetadatasAsync(request)).Result;
        }

        /// <summary>根据iiid数组搜索</summary>
        public virtual async Task<MetadataCollection<Metadata>> GetMetadatasAsync(SearchMetadatasRequest request)
        {
            var searchresponse = await _search.GetEsdatasAsync(request);
            var response = new MetadataCollection<Metadata>(searchresponse.Documents);
            return await Task.FromResult(response);
        }
        /// <summary>根据聚合条件获取统计信息</summary>
        public virtual SearchStatisticsResult Statistics(SearchStatisticsRequest request)
        {
            return Task.Run(() => StatisticsAsync(request)).Result;
        }
        /// <summary>根据聚合条件获取统计信息</summary>
        public virtual async Task<SearchStatisticsResult> StatisticsAsync(SearchStatisticsRequest request)
        {
            SearchStatisticsResult result = new SearchStatisticsResult();
            result.Groups = await _search.StatisticsAsync(request);
            return result;
        }


        /// <summary>查询元数据定义信息</summary>
        public MetadataDefinition[] GetMetadataDefinitions()
        {
            return Task.Run(() => GetMetadataDefinitionsAsync()).Result;
        }
        /// <summary>查询元数据定义信息</summary>
        public async Task<MetadataDefinition[]> GetMetadataDefinitionsAsync()
        {
            var values = MetadataDefinitionCollection.Instance;
            IEnumerable<MetadataDefinition> values2 = null;
            if (values == null)
            {
                values2 = await this.MdAccessor.AsQueryable().ToListAsync();
            }
            else
            {
                values2 = values.Values;
            }
            return values2.OrderBy(e => e.GroupOrder).ThenBy(e => e.ItemOrder).ToArray();
        }

        public object ESSearchEx(object query)
        {
            return Task.Run(() => ESSearchExAsync(query)).Result;
        }
        public async Task<object> ESSearchExAsync(object query)
        {
            return await _esAccess.QueryAsync(query);
        }
    }
}