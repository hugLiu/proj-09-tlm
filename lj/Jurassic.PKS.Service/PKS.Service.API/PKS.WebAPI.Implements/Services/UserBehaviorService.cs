using AutoMapper;
using PKS.Core;
using PKS.TLM.Infrastructure.Core;
using PKS.Utils;
using PKS.Web;
using PKS.WebAPI.Models.UserBehavior;
using PKS.WebAPI.Models.UserBehavior.REST;
using System;
using Nest;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKS.WebAPI.ES;
using PKS.Models;
using PKS.WebAPI.Models;
using MongoDB.Driver;
using SearchRequest = PKS.WebAPI.Models.SearchRequest;

namespace PKS.WebAPI.Services
{
    /// <summary>
    /// 用户行为日志分析服务
    /// </summary>
    public class UserBehaviorService : ServiceBase, IUserBehaviorService, IPerRequestAppService
    {
        private string apiurl;
        private Uri apinode;
        private string service;
        private SearchEntity<UserBehavior> _search = new SearchEntity<UserBehavior>();
       // private ESAccess<UserBehavior> _esAccess = null;

        public UserBehaviorService()
        {
            var configSection = ConfigurationManager.GetSection("pks.userbehavior").As<NameValueCollection>();
            apiurl = configSection["APIUrl"];
            apinode = new Uri(apiurl);
            service = configSection["Service"];
        }
        /// <summary>构造函数</summary>
        public UserBehaviorService(IElasticConfig elasticConfig, IMongoConfig mongoConfig, IMongoCollection<MetadataDefinition> accessor)
        {
            MdAccessor = accessor;
            //_esAccess = new ESAccess<UserBehavior>();
        }
        /// <summary>访问器</summary>
        private IMongoCollection<MetadataDefinition> MdAccessor { get; }
        /// <summary>服务方法 - HotTopics</summary>
        public string MethodUrlHotTopics
        {
            get { return string.Format("{0}/{1}/anaHotTopics", apiurl, service); }
        }

        /// <summary>服务方法 - YourLikes</summary>
        public string MethodUrlYourLikes
        {
            get { return string.Format("{0}/{1}/anaYourLikes", apiurl, service); }
        }

        /// <summary>生成JSON串</summary>
        public override string ToString()
        {
            return JsonUtil.ToJson(this);
        }

        #region 同步方法

        /// <summary>根据iiid搜索</summary>
        public virtual UserBehavior GetUserBehavior(SearchMetadataRequest request)
        {
            return Task.Run(() => GetUserBehaviorAsync(request)).Result;
        }
        /// <summary>
        /// 分析热点条目
        /// </summary>
        /// <param name="request">栏目板块参数</param>
        /// <returns>返回热点信息条目</returns>
        public virtual AnaHotTopicsResult AnaHotTopics(AnaHotTopicsRequest request)
        {
            return Task.Run(() => AnaHotTopicsAsync(request)).Result;
        }

        /// <summary>
        /// 分析猜你喜欢
        /// </summary>
        /// <param name="request">用户信息</param>
        /// <returns></returns>
        public virtual AnaYourLikesResult AnaYourLikes(AnaYourLikesRequest request)
        {
            return Task.Run(() => AnaYourLikesAsync(request)).Result;
        }
        #endregion 同步方法

        #region 异步方法

        /// <summary>根据iiid搜索</summary>
        public virtual async Task<UserBehavior> GetUserBehaviorAsync(SearchMetadataRequest request)
        {            
            var searchresponse = await _search.GetEsdataAsync(request);
            var response = searchresponse.Documents.FirstOrDefault();
            return await Task.FromResult(response);
        }

        /// <summary>根据iiid数组搜索</summary>
        public virtual MetadataCollection<UserBehavior> GetUserBehaviors(SearchMetadatasRequest request)
        {
            return Task.Run(() => GetUserBehaviorsAsync(request)).Result;
        }

        /// <summary>根据iiid数组搜索</summary>
        public virtual async Task<MetadataCollection<UserBehavior>> GetUserBehaviorsAsync(SearchMetadatasRequest request)
        {
            var searchresponse = await _search.GetEsdatasAsync(request);
            var response = new MetadataCollection<UserBehavior>(searchresponse.Documents);
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
        /// <summary>
        /// 搜索加强版
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<List<TopicResult>> BlockTopicWrapperAsync(TopicRequest request)
        {
            var searchresponse = await _search.BlockTopicWrapperAsync(request);
            return searchresponse;
        }
    
        /// <summary>
        /// 分析热点条目
        /// </summary>
        /// <param name="request">栏目板块参数</param>
        /// <returns>返回热点信息条目</returns>
        public virtual async Task<AnaHotTopicsResult> AnaHotTopicsAsync(AnaHotTopicsRequest request)
        {
            var param = new Dictionary<string, object>();
            if (request.Analysis != null)
                foreach (var kv in request.Analysis)
                    if (kv.Value != null && !string.IsNullOrEmpty(kv.Value.ToString()))
                        param.Add(kv.Key, kv.Value);

            if (request.ResultLimit != 0)
                param.Add("count", request.ResultLimit);
            var httpcontent = JsonUtil.ToObject( new HttpClientHelper().Get(MethodUrlHotTopics, param) );
            var rest = httpcontent.MapTo<anaHotTopicsResult>();
            var result = rest.MapTo<AnaHotTopicsResult>();
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 分析猜你喜欢
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<AnaYourLikesResult> AnaYourLikesAsync(AnaYourLikesRequest request)
        {
            var param = new Dictionary<string, object>();
            if (request.Analysis != null)
                foreach (var kv in request.Analysis)
                    if (kv.Value != null && !string.IsNullOrEmpty(kv.Value.ToString()))
                        param.Add(kv.Key, kv.Value);

            if (request.ResultLimit != 0)
                param.Add("count", request.ResultLimit);
            var httpcontent = JsonUtil.ToObject(new HttpClientHelper().Get(MethodUrlYourLikes, param));
            var rest = httpcontent.MapTo<anaYourLikesResult>();
            var result = rest.MapTo<AnaYourLikesResult>();
            return await Task.FromResult(result);
        }
        #endregion 异步方法
    }
}
