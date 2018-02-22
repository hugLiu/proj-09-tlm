using PKS.Models;
using PKS.WebAPI.Models;
using PKS.WebAPI.Models.UserBehavior;
using PKS.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Services
{
    public interface IUserBehaviorService
    {
        ///// <summary>
        ///// 添加
        ///// </summary>
        ///// <param name="requestModel"></param>
        //void Add(UserBehavior requestModel);
        ///// <summary>
        ///// 查询
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //string Search(string query);

        //Task<string> SearchAsync(string request);


        #region 同步方法
        /// <summary>
        /// 分析热点条目
        /// </summary>
        /// <param name="request">栏目板块参数</param>
        /// <returns>返回热点信息条目</returns>
        AnaHotTopicsResult AnaHotTopics(AnaHotTopicsRequest request);

        /// <summary>
        /// 分析猜你喜欢
        /// </summary>
        /// <param name="request">用户信息</param>
        /// <returns></returns>
        AnaYourLikesResult AnaYourLikes(AnaYourLikesRequest request);
        #endregion 同步方法

        #region 异步方法

        /// <summary>根据iiid搜索</summary>
        Task<UserBehavior> GetUserBehaviorAsync(SearchMetadataRequest request);
        /// <summary>
        /// 根据iiids组搜索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<MetadataCollection<UserBehavior>> GetUserBehaviorsAsync(SearchMetadatasRequest request);
        /// <summary>
        /// 搜索加强版
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<TopicResult>> BlockTopicWrapperAsync(TopicRequest request);

        /// <summary>根据聚合条件获取统计信息</summary>
        Task<SearchStatisticsResult> StatisticsAsync(SearchStatisticsRequest request);
        /// <summary>
        /// 分析热点条目
        /// </summary>
        /// <param name="request">栏目板块参数</param>
        /// <returns>返回热点信息条目</returns>
        Task<AnaHotTopicsResult> AnaHotTopicsAsync(AnaHotTopicsRequest request);

        /// <summary>
        /// 分析猜你喜欢
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AnaYourLikesResult> AnaYourLikesAsync(AnaYourLikesRequest request);
        #endregion 异步方法
    }
}
