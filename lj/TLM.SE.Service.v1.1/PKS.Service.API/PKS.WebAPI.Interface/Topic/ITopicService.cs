using PKS.Models;
using PKS.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Services
{
    /// <summary>
    /// 信息标题服务接口
    /// </summary>
    public interface ITopicService
    {
        #region 同步方法
        /// <summary>
        /// 按短语搜索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchResult<Metadata> Search(SearchRequest request);

        /// <summary>
        /// 获取头条信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<TopicResult> GetHeadlines(TopicRequest request);

        /// <summary>
        /// 获取热点信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<TopicResult> GetHotTopics(TopicRequest request);

        /// <summary>
        /// 获取个人最近浏览条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<TopicResult> GetRecency(TopicRequest request);

        /// <summary>
        /// 获取猜你喜欢推荐条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<TopicResult> GetYourLikes(TopicRequest request);

        /// <summary>
        /// 获取关联推荐信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        TopicStandResult GetRelTopics(TopicRelationRequest request);

        /// <summary>
        /// 获取信息板块条目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<TopicResult> GetBlockTopics(TopicRequest request);

        /// <summary>
        /// 获取信息板块条目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<TopicResult> BlockTopicWrapper(TopicRequest request);

        /// <summary>
        /// 设置信息条目头条标记
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        string[] SetHeadlineFlag(List<string> request);
        #endregion 同步方法

        #region 异步方法
        /// <summary>
        /// 按短语搜索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<SearchResult<Metadata>> SearchAsync(SearchRequest request);

        /// <summary>
        /// 获取头条信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<TopicResult>> GetHeadlinesAsync(TopicRequest request);

        /// <summary>
        /// 获取热点信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<TopicResult>> GetHotTopicsAsync(TopicRequest request);

        /// <summary>
        /// 获取个人最近浏览条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<TopicResult>> GetRecencyAsync(TopicRequest request);

        /// <summary>
        /// 获取猜你喜欢推荐条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<TopicResult>> GetYourLikesAsync(TopicRequest request);

        /// <summary>
        /// 获取关联推荐信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<TopicStandResult> GetRelTopicsAsync(TopicRelationRequest request);

        /// <summary>
        /// 获取信息板块条目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<TopicResult>> GetBlockTopicsAsync(TopicRequest request);

        /// <summary>
        /// 获取信息板块条目列表（包装方法）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<TopicResult>> BlockTopicWrapperAsync(TopicRequest request);

        /// <summary>
        /// 设置信息条目头条标记
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<string[]> SetHeadlineFlagAsync(List<string> request);
        #endregion 异步方法
    }
}
