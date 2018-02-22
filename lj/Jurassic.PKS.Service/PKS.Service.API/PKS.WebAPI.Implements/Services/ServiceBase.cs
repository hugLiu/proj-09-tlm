using Jurassic.PKS.Service.Semantics;
using Newtonsoft.Json.Linq;
using Ninject;
using PKS.Core;
using PKS.Models;
using PKS.TLM.Infrastructure;
using PKS.WebAPI.ES.Model.EsRawResult;
using PKS.WebAPI.Models;
using PKS.WebAPI.Models.UserBehavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Services
{
    /// <summary>
    /// 数据服务基类
    /// </summary>
    public class ServiceBase: IPerRequestAppService
    {
        /// <summary>
        /// 搜索服务
        /// </summary>
        [Inject]
        public ISearchService SearchService { get; set; }
        [Inject]
        public ISemanticService SemanticService { get; set; }
        [Inject]
        public IIndexerService IndexerService { get; set; }
        [Inject]
        public IUserBehaviorService UserBehaviorService { get; set; }

        #region 搜索服务
        /// <summary>
        /// 获得短语搜索结果
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual SearchResult<Metadata> GetSearchResult(SearchRequest query)
        {
            return SearchService.Search(query);
        }

        /// <summary>
        /// 获得加强版搜索
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual SearchResult<Metadata> SearchPlusResult(SearchRequest query)
        {
            return SearchService.SearchPlus(query);
        }

        /// <summary>
        /// 获得ES语法搜索结果
        /// </summary>
        /// <param name="esQuery"></param>
        /// <returns></returns>
        protected virtual ESRoot GetESSearchResult(string esQuery)
        {
            return SearchService.ESSearch(esQuery).To<ESRoot>();
        }
        /// <summary>
        /// 构建TopicRelationRequest
        /// </summary>
        /// <param name="relQuery"></param>
        /// <returns></returns>
        protected virtual TopicRelationRequest BuildRelRequest(JObject relQuery)
        {
            TopicRelationRequest request = new TopicRelationRequest();
            JToken jtoken;

            jtoken = relQuery.SelectToken("masterdata");
            if (jtoken != null)
                request.MasterData = (JObject)jtoken;

            jtoken = relQuery.SelectToken("masterfields");
            if (jtoken != null)
                request.MasterFields = jtoken.ToObject<List<string>>();

            jtoken = relQuery.SelectToken("relationfields");
            if (jtoken != null)
                request.RelationFields = jtoken.ToObject<List<string>>();

            jtoken = relQuery.SelectToken("relationsymbol");
            if (jtoken != null)
                request.RelationSymbol = jtoken.ToObject<string>();

            jtoken = relQuery.SelectToken("intelligent");
            if (jtoken != null)
                request.Intelligent = jtoken.ToObject<bool>();

            jtoken = relQuery.SelectToken("query");
            if (jtoken != null)
                request.Query = (JObject)jtoken;
            return request;
        }
        /// <summary>
        /// 构建Search请求
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual SearchRequest BuildSearchRequest(JObject query)
        {
            SearchRequest request = new SearchRequest();
            JToken jtoken;

            // string Sentence
            jtoken = query.SelectToken("sentence");
            if (jtoken != null)
                request.Sentence = jtoken.ToObject<string>();

            // int From
            jtoken = query.SelectToken("from");
            if (jtoken != null)
                request.From = jtoken.ToObject<int>();

            // int Size
            jtoken = query.SelectToken("size");
            if (jtoken != null)
                request.Size = jtoken.ToObject<int>();

            // Dictionary<string, object[]> Filter
            jtoken = query.SelectToken("filter");
            if (jtoken != null)
                request.Filter = jtoken.ToObject<Dictionary<string, object[]>>();

            // SearchSourceFilter Fields
            jtoken = query.SelectToken("fields");
            if (jtoken != null)
                request.Fields = jtoken.ToObject<SearchSourceFilter>();

            // Dictionary<string, int> Boost
            jtoken = query.SelectToken("boost");
            if (jtoken != null)
                request.Boost = jtoken.ToObject<Dictionary<string, int>>();

            // List<PKSKeyValuePair<string, object>> Sort
            // 排序特殊处理
            jtoken = query.SelectToken("sort");
            if (jtoken != null)
            {
                var sortvals = jtoken.ToObject<List<Dictionary<string, object>>>();
                List<PKSKeyValuePair<string, object>> sort = new List<PKSKeyValuePair<string, object>>();

                foreach (var sortv in sortvals)
                    foreach (var v in sortv)
                        sort.Add(new PKSKeyValuePair<string, object> { Key = v.Key, Value = (v.Value.ToString().ToLower() == "desc") ? 1 : 0 });
                request.Sort = sort;
            }

            // SearchGroupRules Group
            jtoken = query.SelectToken("group");
            if (jtoken != null)
            {
                SearchGroupRules groups = new SearchGroupRules();
                groups.Top = int.MaxValue / 2;
                groups.Fields = jtoken.ToObject<List<string>>();
                request.Group = groups;
            }
            return request;
        }


        /// <summary>
        /// 追加用户过滤
        /// </summary>
        /// <param name="request"></param>
        /// <param name=""></param>
        protected virtual void AppendFilterByUser(SearchRequest request, UserInfoRequest userinfo)
        {
            JObject t = JObject.FromObject(userinfo);
            IEnumerable<JProperty> PropertyList = t.Properties();
            foreach (JProperty item in PropertyList)
            {
                if (item.Value == null) continue;

                List<object> values = new List<object>();
                if (item.Value.GetType() == typeof(JArray))
                {
                    var vv = item.Values();
                    foreach(var v in vv)
                    {
                        if (string.IsNullOrEmpty(v.ToString())) continue;
                        values.Add(v);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(item.Value.ToString())) continue;
                    values.Add(item.Value);
                }

                if (values.Count == 0) continue;
                var name = string.Format("u{0}", item.Name);
                request.Filter.Add(name, values.ToArray());
            }
        }

        ///// <summary>
        ///// 附加ES Query
        ///// </summary>
        ///// <param name="request"></param>
        //protected virtual void AppendQueryByMatch<T>(JObject query, T append, string keyprefix = "")
        //{
        //    JObject querynode = (JObject)query.SelectToken("query");
        //    JObject content;
        //    List <Dictionary<string, object>> newquery = new List<Dictionary<string, object>>();

        //    //获取不到query.query 自动添加query节点
        //    if (querynode == null)
        //        query.Add(new JProperty("query", newquery));

        //    JObject t = JObject.FromObject(append);
        //    IEnumerable<JProperty> PropertyList = t.Properties();
        //    foreach (JProperty item in PropertyList)
        //    {
        //        content = new JObject { new JProperty(string.Format("{0}{1}", keyprefix, item.Name), item.Value) };
        //        var matchnode = new JObject { new JProperty("match", content) };
        //        querynode.Add(matchnode);
        //    }
        //}

        //protected virtual void AppendFilterByMatch<T>(JObject query, T append, string keyprefix = "")
        //{
        //    JObject querynode = (JObject)query.SelectToken("query");
        //    JObject content;
        //    List<Dictionary<string, object>> newquery = new List<Dictionary<string, object>>();

        //    //获取不到query.query 自动添加query节点
        //    if (querynode == null)
        //        query.Add(new JProperty("query", newquery));

        //    JObject t = JObject.FromObject(append);
        //    IEnumerable<JProperty> PropertyList = t.Properties();
        //    foreach (JProperty item in PropertyList)
        //    {
        //        content = new JObject { new JProperty(string.Format("{0}{1}", keyprefix, item.Name), item.Value) };
        //        var matchnode = new JObject { new JProperty("match", content) };
        //        querynode.Add(matchnode);
        //    }
        //}

        #endregion 搜索服务

        #region 语义服务
        /// <summary>
        /// 获取所有语义关系类型
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<List<SemanticsType>> GetSemanticsType()
        {
            return await SemanticService.GetSemanticsType();
        }

        /// <summary>
        /// 获取语义关系
        /// </summary>
        /// <param name="term"></param>
        /// <param name="sr"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected async Task<List<TermInfo>> Semantics(string term, string sr, string direction)
        {
            return await SemanticService.Semantics(term, sr, direction);
        }

        /// <summary>
        /// 获取语义关系
        /// </summary>
        /// <param name="term"></param>
        /// <param name="sr"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected async Task<List<TermInfo>> Semantics(string term)
        {
            return await SemanticService.Semantics(term);
        }

        /// <summary>
        /// 获取语义关系
        /// </summary>
        /// <param name="term"></param>
        /// <param name="sr"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected async Task<List<TermInfo>> Semantics(List<string> terms)
        {
            return await SemanticService.Semantics(terms);
        }
        #endregion 语义服务

        #region 索引服务
        /// <summary>保存</summary>
        protected async Task<string[]> IndexSave(IndexSaveRequest request)
        {
            return await IndexerService.SaveAsync(request);
        }
        #endregion 索引服务

        #region 用户行为日志分析服务
        /// <summary>
        /// 分析热点条目
        /// </summary>
        /// <param name="request">栏目板块参数</param>
        /// <returns>返回热点信息条目</returns>
        protected async Task<AnaHotTopicsResult> AnaHotTopics(AnaHotTopicsRequest request)
        {
            return await UserBehaviorService.AnaHotTopicsAsync(request);
        }

        /// <summary>
        /// 分析猜你喜欢
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected async Task<AnaYourLikesResult> AnaYourLikes(AnaYourLikesRequest request)
        {
            return await UserBehaviorService.AnaYourLikesAsync(request);
        }
        #endregion 用户行为日志分析服务
    }
}
