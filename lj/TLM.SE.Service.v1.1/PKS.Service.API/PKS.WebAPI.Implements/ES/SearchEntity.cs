using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.PKS.Service;
using Nest;
using PKS.Models;
using PKS.WebAPI.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using PKS.Utils;
using PKS.WebAPI.ES;
using SearchRequest = PKS.WebAPI.Models.SearchRequest;
using PKS.WebAPI.Services;

namespace PKS.WebAPI.ES
{
    /// <summary>
    /// ES搜索方法
    /// </summary>
    /// <typeparam name="T">泛型结构</typeparam>
    // 说明：支持不同type查询  薛炜 2017-11-15
    public class SearchEntity<T> : ServiceBase where T : class 
    {
        private static ESAccess<T> _esAccess = new ESAccess<T>();
        private static SearchProvider<T> provider = new SearchProvider<T>();
        #region 构造

        #endregion
        #region 搜索方法 薛炜 2017-11-15
        public virtual async Task<ISearchResponse<T>> GetEsdataAsync(SearchMetadataRequest request)
        {
           // var provider = new SearchProvider<T>();
            var termQuery = new TermQuery { Field = "iiid.keyword", Value = request.IIId };
            var query = new BoolQuery() { Must = new List<QueryContainer> { termQuery } };

            var fields = provider.BuildFields(request.Fields);

            var from = 0;
            var size = 1;

            var searchresponse = await _esAccess.PagingQueryAsync(query, null, fields, null, from, size);
            //var response = searchresponse.Documents.FirstOrDefault();
            return searchresponse;
        }
        /// <summary>
        /// 根据iiid数组搜索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<ISearchResponse<T>> GetEsdatasAsync(SearchMetadatasRequest request)
        {
            // var provider = new SearchProvider<T>();
            var termQuery = new TermsQuery { Field = "iiid.keyword", Terms = request.IIIds };
            var query = new BoolQuery() { Must = new List<QueryContainer> { termQuery } };
            var fields = provider.BuildFields(request.Fields);
            var from = 0;
            var size = request.IIIds.Count;
            var searchresponse = await _esAccess.PagingQueryAsync(query, null, fields, null, from, size);
            return searchresponse;
        }


        /// <summary>根据聚合条件获取统计信息</summary>
        public virtual async Task<Dictionary<string, Dictionary<string, long?>>> StatisticsAsync(SearchStatisticsRequest request)
        {

           // var provider = new SearchProvider<Metadata>();
            SearchGroupRules groups = new SearchGroupRules();
            groups.Top = int.MaxValue / 2;
            groups.Fields = request.Groups;
            var aggs = provider.BuildAggs(groups);

            var searchresponse = await _esAccess.PagingQueryAsync(null, null, null, aggs, 0, 0);

            SearchStatisticsResult result = new SearchStatisticsResult();

            var groupResult = new Dictionary<string, Dictionary<string, long?>>();
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
                groupResult.Add(agg.Key, dic);
            }

            return  groupResult;
            //return result;
        }

        /// <summary>
        /// 搜索加强版
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual SearchResult<T> SearchPlus(SearchRequest request)
        {
            return Task.Run(() => SearchPlusAsync(request)).Result;
        }
        /// <summary>
        /// 搜索加强版
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<T>> SearchPlusAsync(SearchRequest request)
        {
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
                                provider.BuildFilterQuery(shouldQueryContainerList, itemExp.Value);
                                break;
                            }
                        case "_and"://$or运算符使用 ES Must
                            {
                                mustQueryContainerList = new List<QueryContainer>();
                                provider.BuildFilterQuery(mustQueryContainerList, itemExp.Value);
                                break;
                            }
                        case "_not"://$or运算符使用 ES MustNot
                            {
                                mustnotQueryContainerList = new List<QueryContainer>();
                                provider.BuildFilterQuery(mustnotQueryContainerList, itemExp.Value);
                                break;
                            }
                        default://默认与关系 不包含运算符
                            {
                                if (mustQueryContainerList == null)
                                    mustQueryContainerList = new List<QueryContainer>();
                                mustQueryContainerList.Add(new TermsQuery { Field = provider.KeyWord(itemExp.Key), Terms = itemExp.Value });
                                break;
                            }
                    }
                }

            }
            //多条件集合
            var filterQuery = new BoolQuery { Should = shouldQueryContainerList, Must = mustQueryContainerList, MustNot = mustnotQueryContainerList };

            var fulltextQuery = provider.BuildCustomScoreQuery(request.Sentence, request.Boost);
            var query = provider.CombineMustQuery(filterQuery, fulltextQuery);
            var sort = provider.BuildSort(request.Sort);
            var aggs = provider.BuildAggs(request.Group);
            var fields = provider.BuildFields(request.Fields);

            var from = request.From;
            var size = request.Size;

            var searchresponse = await _esAccess.PagingQueryAsync(query, sort, fields, aggs, from, size);

            var response = Extensions<T>.ToMetadataCollection(searchresponse);
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


        #endregion

        #region 业务
        /// <summary>
        /// 获取信息板块条目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<List<TopicResult>> BlockTopicWrapperAsync(TopicRequest request)
        {
            List<TopicResult> listtopicresult = new List<TopicResult>();
            TopicResult topicresult;
            List<TabResult> listtabresult = new List<TabResult>();
            TabResult tabresult;
            SearchRequest searchrequest;
            SearchResult<T> searchresult;

            //获取搜索请求
            foreach (BlockRequest br in request.Blocks)
            {
                //直接存回栏目信息
                topicresult = new TopicResult { BlockId = br.BlockId };
                listtabresult = new List<TabResult>();
                #region 分栏处理
                foreach (TabGroupRequest tgr in br.TabGroups)
                {
                    //分栏数据
                    tabresult = new TabResult();
                    //增加默认搜索选项的配置 薛炜 2017-11-3
                    JToken rjtoken;
                    foreach (var job in request.configure)
                    {
                        JToken jtoken;
                        jtoken = tgr.Query.SelectToken(job.Key);
                        if (jtoken == null)
                            tgr.Query.Add(job.Key, job.Value);
                    }
                    //参数处理
                    searchrequest = base.BuildSearchRequest(tgr.Query);

                    //追加用户信息过滤
                    base.AppendFilterByUser(searchrequest, request.UserInfo);

                    //根据搜索类别附加过滤
                    if (tgr.SearchType == TopicSearchType.None)//普通请求不需要处理
                    { }
                    if (tgr.SearchType == TopicSearchType.GetHotTopics)//追加 热点信息条目
                    { }
                    if (tgr.SearchType == TopicSearchType.GetRecency)//追加 个人最近浏览条目
                    { }
                    if (tgr.SearchType == TopicSearchType.GetYourLikes)//追加 猜你喜欢推荐条目
                    { }
                    if (tgr.SearchType == TopicSearchType.GetRelTopics)//获取关联推荐条目
                    {
                        #region 获取关联推荐条目 薛炜 2017-11-08
                        TopicRelationRequest relRequest = new TopicRelationRequest();
                        rjtoken = request.configure.SelectToken("reltopics");
                        relRequest = BuildRelRequest((JObject)rjtoken);
                        //获得当前索引相关词
                        List<string> relterms = new List<string>();
                        //获取主数据
                        SearchRequest requestmasterdata = new SearchRequest();
                        requestmasterdata.Filter = relRequest.MasterData.ToObject<Dictionary<string, object[]>>();
                        //搜索主数据
                        SearchResult<Metadata> searchmaster = base.SearchPlusResult(requestmasterdata);
                        var mastermeta = searchmaster.Metadatas;
                        //关系附加条件
                        Dictionary<string, object[]> innerfilter = new Dictionary<string, object[]>();

                        //获取并添加有关系的字段值
                        foreach (var meta in mastermeta)
                        {
                            //主数据相关
                            foreach (string k in relRequest.MasterFields)
                            {
                                //查找原来参数集合是否已存在，存在则添加到原数组中
                                if (innerfilter.ContainsKey(k))
                                {
                                    //追加值
                                    var fi = searchrequest.Filter[k].ToList();
                                    fi.Add(meta.GetValue(k));
                                    innerfilter[k] = fi.ToArray();
                                }
                                else
                                    innerfilter.Add(k, new object[] { meta.GetValue(k) });
                            }

                            //其他相关
                            foreach (string k in relRequest.RelationFields)
                            {
                                var v = meta.GetValue(k);
                                if (!relterms.Contains(v))
                                    relterms.Add(v.ToString());

                                //查找原来参数集合是否已存在，存在则添加到原数组中
                                if (innerfilter.ContainsKey(k))
                                {
                                    //追加值
                                    var fi = innerfilter[k].ToList();
                                    fi.Add(v);
                                    innerfilter[k] = fi.ToArray();
                                }
                                else
                                    innerfilter.Add(k, new object[] { v });
                            }
                        }

                        if (relRequest.Intelligent)//如果开启语义支持
                        {
                            //获取语义分词
                            var terms = await base.Semantics(relterms);
                            //遍历分词
                            foreach (var t in terms)
                            {
                                var k = t.CCCode.ToLower();//小写
                                                           //查找原来参数集合是否已存在，存在则添加到原数组中
                                if (innerfilter.ContainsKey(k))
                                {
                                    //追加值
                                    var fi = innerfilter[k].ToList();
                                    fi.Add(t.Term);
                                    innerfilter[k] = fi.ToArray();
                                }
                                else
                                    innerfilter.Add(k, new object[] { t.Term });
                            }
                        }

                        //关联关系条件附加
                        if (string.IsNullOrEmpty(relRequest.RelationSymbol))
                            relRequest.RelationSymbol = "_or";
                        if (innerfilter.Count > 0)
                        {
                            //判断是否有 _or 节点，如果有 删除，建立推荐搜索的关联过滤条件
                            if (searchrequest.Filter.ContainsKey(relRequest.RelationSymbol))
                            {
                                searchrequest.Filter.Remove(relRequest.RelationSymbol);
                            }
                            searchrequest.Filter.Add(relRequest.RelationSymbol, new object[] { new { filter = innerfilter } });
                        }
                        #endregion
                    }
                    if (tgr.SearchType == TopicSearchType.GetHeadlines)//获取头条条目
                    {
                        #region 获取头条 薛炜 2017-11-01
                        searchrequest.Filter.Add("headline", new object[] { true });
                        #endregion
                    }

                    //搜索
                    searchresult = SearchPlus(searchrequest);
                    #region 同一TabId返回的数据加入到 同一Result 下 薛炜 2017-12-17
                    bool idNewTab = true;
                    if (topicresult.Tabs != null)
                    {
                        foreach (TabResult tr in topicresult.Tabs)
                        {
                            if (tr.TabId == tgr.TabId)
                            {
                                MetadataCollection<T> mc = tr.Result as MetadataCollection<T>;
                                tr.Total = tr.Total + searchresult.Total;
                                var mc1 = searchresult.Metadatas;
                                mc.AddRange(mc1);
                                tr.Result = mc;
                                idNewTab = false;
                                continue;
                            }
                        }
                    }
                    if (idNewTab)
                    {
                        tabresult.TabId = tgr.TabId;
                        tabresult.TabName = tgr.TabName;
                        tabresult.Total = searchresult.Total;
                        tabresult.Result = searchresult.Metadatas;
                        tabresult.SearchType = tgr.SearchType;
                        tabresult.Group = searchresult.Groups;
                        listtabresult.Add(tabresult);
                    }
                    #endregion
                    //存回分栏数据
                    topicresult.Tabs = listtabresult;
                }
                #endregion
                //存回数据
                listtopicresult.Add(topicresult);
            }

            //返回结果
            return await Task.FromResult(listtopicresult);
        }
        #endregion

    }
}
