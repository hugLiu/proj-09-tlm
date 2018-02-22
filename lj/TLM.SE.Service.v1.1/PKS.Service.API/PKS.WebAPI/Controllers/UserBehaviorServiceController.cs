using PKS.Core;
using PKS.Models;
using PKS.Utils;
using PKS.WebAPI.Models;
using PKS.WebAPI.Models.UserBehavior;
using PKS.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PKS.WebAPI.Controllers
{
    /// <summary>
    /// 信息标题服务
    /// </summary>
    public class UserBehaviorServiceController : PKSApiController
    {
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserBehaviorServiceController(IUserBehaviorService service)
        {
            ServiceImpl = service;
        }

        /// <summary>
        /// 服务实例
        /// </summary>
        private IUserBehaviorService ServiceImpl { get; }

        /// <summary>
        /// 获得服务信息
        /// </summary>
        /// <returns></returns>
        protected override ServiceInfo GetServiceInfo()
        {
            return new ServiceInfo
            {
                Description = "用户行为分析服务。用于信息共享平台各板块版面获取信息条目。"
            };
        }

        #region 异步方法
        /// <summary>根据iiid搜索</summary>
        [HttpPost]
        public async Task<UserBehavior> GetUserBehavior(SearchMetadataRequest request)
        {
            return await ServiceImpl.GetUserBehaviorAsync(request);
        }
        /// <summary>
        /// 根据iiids数组搜索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MetadataCollection<UserBehavior>> GetUserBehaviors(SearchMetadatasRequest request)
        {
            return await ServiceImpl.GetUserBehaviorsAsync(request);
        }
        /// <summary>
        /// 根据聚合条件获取统计信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<SearchStatisticsResult> Statistics(SearchStatisticsRequest request)
        {
            return await ServiceImpl.StatisticsAsync(request);
        }

        /// <summary>
        /// 获取信息板块条目列表（包装方法）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<TopicResult>> GetTopPic(TopicRequest request)
        {
            var result = this.ServiceImpl.BlockTopicWrapperAsync(request);
            return await result;
        }
        /// <summary>按短语搜索</summary>
        [HttpPost]
        public async Task<AnaHotTopicsResult> AnaHotTopics(AnaHotTopicsRequest request)
        {
            var result = this.ServiceImpl.AnaHotTopicsAsync(request);
            return await result;
        }

        /// <summary>按短语搜索</summary>
        [HttpPost]
        public async Task<AnaYourLikesResult> AnaYourLikes(AnaYourLikesRequest request)
        {
            var result = this.ServiceImpl.AnaYourLikesAsync(request);
            return await result;
        }
        #endregion 异步方法

    }
}
