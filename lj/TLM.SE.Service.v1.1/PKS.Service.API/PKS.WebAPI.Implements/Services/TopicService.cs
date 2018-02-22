using PKS.Core;
using PKS.Models;
using PKS.Utils;
using PKS.WebAPI.ES;
using PKS.WebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PKS.WebAPI.Services
{
    /// <summary>
    /// 信息条目服务实现
    /// </summary>
    public class TopicService : ServiceBase, ITopicService, IPerRequestAppService
    {
        #region 私有方法
        #endregion 私有方法
        private SearchEntity<UserBehavior> _search = new SearchEntity<UserBehavior>();
        #region 同步方法
        /// <summary>
        /// 按短语搜索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual SearchResult<Metadata> Search(SearchRequest request)
        {
            return Task.Run(() => SearchAsync(request)).Result;
        }

        /// <summary>
        /// 获取头条信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<TopicResult> GetHeadlines(TopicRequest request)
        {
            return Task.Run(() => GetHeadlinesAsync(request)).Result;
        }

        /// <summary>
        /// 获取热点信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<TopicResult> GetHotTopics(TopicRequest request)
        {
            return Task.Run(() => GetHotTopicsAsync(request)).Result;
        }

        /// <summary>
        /// 获取个人最近浏览条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<TopicResult> GetRecency(TopicRequest request)
        {
            return Task.Run(() => GetRecencyAsync(request)).Result;
        }

        /// <summary>
        /// 获取猜你喜欢推荐条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<TopicResult> GetYourLikes(TopicRequest request)
        {
            return Task.Run(() => GetYourLikesAsync(request)).Result;
        }

        /// <summary>
        /// 获取关联推荐信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual TopicStandResult GetRelTopics(TopicRelationRequest request)
        {
            return Task.Run(() => GetRelTopicsAsync(request)).Result;
        }

        /// <summary>
        /// 获取信息板块条目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<TopicResult> GetBlockTopics(TopicRequest request)
        {
            return Task.Run(() => GetBlockTopicsAsync(request)).Result;
        }

        /// <summary>
        /// 获取信息板块条目列表（包装方法）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<TopicResult> BlockTopicWrapper(TopicRequest request)
        {
            return Task.Run(() => BlockTopicWrapperAsync(request)).Result;
        }

        /// <summary>
        /// 设置信息条目头条标记
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual string[] SetHeadlineFlag(List<string> request)
        {
            return Task.Run(() => SetHeadlineFlagAsync(request)).Result;
        }
        #endregion 同步方法

        #region 异步方法
        /// <summary>
        /// 按短语搜索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<Metadata>> SearchAsync(SearchRequest request)
        {
            var result = base.GetSearchResult(request);
            return await Task.FromResult(result);
        }
        /// <summary>
        /// 获取头条信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<List<TopicResult>> GetHeadlinesAsync(TopicRequest request)
        {
            List<TopicResult> listtopicresult = new List<TopicResult>();
            TopicResult topicresult;
            List<TabResult> listtabresult = new List<TabResult>();
            TabResult tabresult;
            SearchRequest searchrequest;
            SearchResult<Metadata> searchresult;

            //获取搜索请求
            foreach (BlockRequest br in request.Blocks)
            {
                //直接存回栏目信息
                topicresult = new TopicResult { BlockId = br.BlockId };
                listtabresult = new List<TabResult>();

                foreach (TabGroupRequest tgr in br.TabGroups)
                {
                    //分栏数据
                    tabresult = new TabResult();

                    //参数处理
                    searchrequest = base.BuildSearchRequest(tgr.Query);

                    //追加用户信息过滤
                    base.AppendFilterByUser(searchrequest, request.UserInfo);

                    //追加头条过滤
                    searchrequest.Filter.Add("headline.nokey", new object[] { true });

                    //搜索
                    searchresult = base.GetSearchResult(searchrequest);

                    tabresult.TabId = tgr.TabId;
                    tabresult.TabName = tgr.TabName;
                    tabresult.Total = searchresult.Total;
                    tabresult.Result = searchresult.Metadatas;
                    listtabresult.Add(tabresult);

                    //存回分栏数据
                    topicresult.Tabs = listtabresult;
                }

                //存回数据
                listtopicresult.Add(topicresult);
            }

            //返回结果
            return await Task.FromResult(listtopicresult);
        }

        /// <summary>
        /// 获取热点信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<List<TopicResult>> GetHotTopicsAsync(TopicRequest request)
        {
            List<TopicResult> listtopicresult = new List<TopicResult>();
            TopicResult topicresult;
            List<TabResult> listtabresult = new List<TabResult>();
            TabResult tabresult;
            SearchRequest searchrequest;
            SearchResult<Metadata> searchresult;

            //获取搜索请求
            foreach (BlockRequest br in request.Blocks)
            {
                //直接存回栏目信息
                topicresult = new TopicResult { BlockId = br.BlockId };
                listtabresult = new List<TabResult>();

                foreach (TabGroupRequest tgr in br.TabGroups)
                {
                    //分栏数据
                    tabresult = new TabResult();

                    //参数处理
                    searchrequest = base.BuildSearchRequest(tgr.Query);

                    //追加用户信息过滤
                    base.AppendFilterByUser(searchrequest, request.UserInfo);

                    //追加头条过滤
                    //searchrequest.Filter.Add("headline", new object[] { true });

                    //搜索
                    searchresult = base.GetSearchResult(searchrequest);

                    tabresult.TabId = tgr.TabId;
                    tabresult.TabName = tgr.TabName;
                    tabresult.Total = searchresult.Total;
                    tabresult.Result = searchresult.Metadatas;
                    listtabresult.Add(tabresult);

                    //存回分栏数据
                    topicresult.Tabs = listtabresult;
                }

                //存回数据
                listtopicresult.Add(topicresult);
            }

            //返回结果
            return await Task.FromResult(listtopicresult);
        }

        /// <summary>
        /// 获取个人最近浏览条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<List<TopicResult>> GetRecencyAsync(TopicRequest request)
        {
            List<TopicResult> listtopicresult = new List<TopicResult>();
            TopicResult topicresult;
            List<TabResult> listtabresult = new List<TabResult>();
            TabResult tabresult;
            SearchRequest searchrequest;
            SearchResult<Metadata> searchresult;

            //获取搜索请求
            foreach (BlockRequest br in request.Blocks)
            {
                //直接存回栏目信息
                topicresult = new TopicResult { BlockId = br.BlockId };
                listtabresult = new List<TabResult>();

                foreach (TabGroupRequest tgr in br.TabGroups)
                {
                    //分栏数据
                    tabresult = new TabResult();

                    //参数处理
                    searchrequest = base.BuildSearchRequest(tgr.Query);

                    //追加用户信息过滤
                    base.AppendFilterByUser(searchrequest, request.UserInfo);

                    //追加头条过滤
                    //searchrequest.Filter.Add("headline", new object[] { true });

                    //搜索
                    searchresult = base.GetSearchResult(searchrequest);

                    tabresult.TabId = tgr.TabId;
                    tabresult.TabName = tgr.TabName;
                    tabresult.Total = searchresult.Total;
                    tabresult.Result = searchresult.Metadatas;
                    listtabresult.Add(tabresult);

                    //存回分栏数据
                    topicresult.Tabs = listtabresult;
                }

                //存回数据
                listtopicresult.Add(topicresult);
            }

            //返回结果
            return await Task.FromResult(listtopicresult);
        }

        /// <summary>
        /// 获取猜你喜欢推荐条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<List<TopicResult>> GetYourLikesAsync(TopicRequest request)
        {
            List<TopicResult> listtopicresult = new List<TopicResult>();
            TopicResult topicresult;
            List<TabResult> listtabresult = new List<TabResult>();
            TabResult tabresult;
            SearchRequest searchrequest;
            SearchResult<Metadata> searchresult;

            //获取搜索请求
            foreach (BlockRequest br in request.Blocks)
            {
                //直接存回栏目信息
                topicresult = new TopicResult { BlockId = br.BlockId };
                listtabresult = new List<TabResult>();

                foreach (TabGroupRequest tgr in br.TabGroups)
                {
                    //分栏数据
                    tabresult = new TabResult();

                    //参数处理
                    searchrequest = base.BuildSearchRequest(tgr.Query);

                    //追加用户信息过滤
                    base.AppendFilterByUser(searchrequest, request.UserInfo);

                    //追加头条过滤
                    //searchrequest.Filter.Add("headline", new object[] { true });

                    //搜索
                    searchresult = base.GetSearchResult(searchrequest);

                    tabresult.TabId = tgr.TabId;
                    tabresult.TabName = tgr.TabName;
                    tabresult.Total = searchresult.Total;
                    tabresult.Result = searchresult.Metadatas;
                    listtabresult.Add(tabresult);

                    //存回分栏数据
                    topicresult.Tabs = listtabresult;
                }

                //存回数据
                listtopicresult.Add(topicresult);
            }

            //返回结果
            return await Task.FromResult(listtopicresult);
        }

        /// <summary>
        /// 获取关联推荐信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<TopicStandResult> GetRelTopicsAsync(TopicRelationRequest request)
        {
            //结果
            TopicStandResult result = new TopicStandResult();
            //获得当前索引相关词
            List<string> relterms = new List<string>();

            //获取主数据
            SearchRequest requestmasterdata = new SearchRequest();
            requestmasterdata.Filter = request.MasterData.ToObject<Dictionary<string, object[]>>();
            //搜索主数据
            SearchResult<Metadata> searchmaster = base.SearchPlusResult(requestmasterdata);
            var mastermeta = searchmaster.Metadatas;

            if (mastermeta.Count == 0) return await Task.FromResult(result);

            //其他Query参数处理
            var searchrequest = base.BuildSearchRequest(request.Query);

            //追加用户信息过滤
            base.AppendFilterByUser(searchrequest, request.UserInfo);

            //关系附加条件
            Dictionary<string, object[]> innerfilter = new Dictionary<string, object[]>();

            //获取并添加有关系的字段值
            foreach (var meta in mastermeta)
            {
                //主数据相关
                foreach (string k in request.MasterFields)
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
                foreach (string k in request.RelationFields)
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

            if (request.Intelligent)//如果开启语义支持
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
            if (string.IsNullOrEmpty(request.RelationSymbol))
                request.RelationSymbol = "_or";
            if (innerfilter.Count > 0)
                searchrequest.Filter.Add(request.RelationSymbol, new object[] { new { filter = innerfilter } });

            //搜索
            var searchresult = base.SearchPlusResult(searchrequest);

            result.Total = searchresult.Total;
            result.Result = searchresult.Metadatas;

            //返回结果
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 获取信息板块条目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<List<TopicResult>> GetBlockTopicsAsync(TopicRequest request)
        {
            List<TopicResult> listtopicresult = new List<TopicResult>();
            TopicResult topicresult;
            List<TabResult> listtabresult = new List<TabResult>();
            TabResult tabresult;
            SearchRequest searchrequest;
            SearchResult<Metadata> searchresult;

            //获取搜索请求
            foreach (BlockRequest br in request.Blocks)
            {
                //直接存回栏目信息
                topicresult = new TopicResult { BlockId = br.BlockId };
                listtabresult = new List<TabResult>();

                foreach (TabGroupRequest tgr in br.TabGroups)
                {
                    //分栏数据
                    tabresult = new TabResult();

                    //参数处理
                    searchrequest = base.BuildSearchRequest(tgr.Query);

                    //追加用户信息过滤
                    base.AppendFilterByUser(searchrequest, request.UserInfo);

                    //搜索
                    searchresult = base.SearchPlusResult(searchrequest);

                    tabresult.TabId = tgr.TabId;
                    tabresult.TabName = tgr.TabName;
                    tabresult.Total = searchresult.Total;
                    tabresult.Result = searchresult.Metadatas;
                    listtabresult.Add(tabresult);

                    //存回分栏数据
                    topicresult.Tabs = listtabresult;
                }

                //存回数据
                listtopicresult.Add(topicresult);
            }

            //返回结果
            return await Task.FromResult(listtopicresult);
        }

        /// <summary>
        /// 获取信息板块条目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<List<TopicResult>> BlockTopicWrapperAsync(TopicRequest request)
        {
            var searchresponse = await _search.BlockTopicWrapperAsync(request);
            return searchresponse;
            #region 原代码
            /*
            List<TopicResult> listtopicresult = new List<TopicResult>();
            TopicResult topicresult;
            List<TabResult> listtabresult = new List<TabResult>();
            TabResult tabresult;
            SearchRequest searchrequest;
            SearchResult<Metadata> searchresult;
            
            //获取搜索请求
            foreach (BlockRequest br in request.Blocks)
            {
                //直接存回栏目信息
                topicresult = new TopicResult { BlockId = br.BlockId };
                listtabresult = new List<TabResult>();

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
                    searchresult = base.SearchPlusResult(searchrequest);

                    tabresult.TabId = tgr.TabId;
                    tabresult.TabName = tgr.TabName;
                    tabresult.Total = searchresult.Total;
                    tabresult.Result = searchresult.Metadatas;
                    tabresult.SearchType = tgr.SearchType;
                    tabresult.Group = searchresult.Groups;
                    listtabresult.Add(tabresult);

                    //存回分栏数据
                    topicresult.Tabs = listtabresult;
                }

                //存回数据
                listtopicresult.Add(topicresult);
            }

            //返回结果
            return await Task.FromResult(listtopicresult);
            */
            #endregion
        }

        /// <summary>
        /// 设置信息条目头条标记
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<string[]> SetHeadlineFlagAsync(List<string> request)
        {
            MetadataCollection<Metadata> metadatas = new MetadataCollection<Metadata>();
            Metadata metadata;

            foreach (var r in request)
            {
                metadata = new Metadata { IIId = r };
                metadata.Add("headline", true);
                metadatas.Add(metadata);
            }
            var indexsaverequest = new IndexSaveRequest { Metadatas = metadatas };
            return await base.IndexSave(indexsaverequest);
        }
        #endregion 异步方法
    }
}
