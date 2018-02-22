using PKS.Core;
using PKS.Models;
using PKS.Utils;
using PKS.WebAPI.Models;
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
    public class TopicServiceController : PKSApiController
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TopicServiceController(ITopicService service)
        {
            ServiceImpl = service;
        }

        /// <summary>
        /// 服务实例
        /// </summary>
        private ITopicService ServiceImpl { get; }

        /// <summary>
        /// 获得服务信息
        /// </summary>
        /// <returns></returns>
        protected override ServiceInfo GetServiceInfo()
        {
            return new ServiceInfo
            {
                Description = "信息条目数据服务。用于信息共享平台各板块版面获取信息条目。"
            };
        }

        #region 异步方法
        /// <summary>按短语搜索</summary>
        [HttpPost]
        public async Task<SearchResult<Metadata>> Search(SearchRequest request)
        {
            request.Sentence = HttpUtility.UrlDecode(request.Sentence);
            return await ServiceImpl.SearchAsync(request);
        }

        /// <summary>
        /// 获取头条信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<TopicResult>> GetHeadlines(TopicRequest request)
        {
            var result = this.ServiceImpl.GetHeadlinesAsync(request);
            return await result;
        }

        /// <summary>
        /// 获取热点信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<TopicResult>> GetHotTopics(TopicRequest request)
        {
            var result = this.ServiceImpl.GetHotTopicsAsync(request);
            return await result;
        }

        /// <summary>
        /// 获取个人最近浏览条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<TopicResult>> GetRecency(TopicRequest request)
        {
            var result = this.ServiceImpl.GetRecencyAsync(request);
            return await result;
        }

        /// <summary>
        /// 获取猜你喜欢推荐条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<TopicResult>> GetYourLikes(TopicRequest request)
        {
            var result = this.ServiceImpl.GetYourLikesAsync(request);
            return await result;
        }

        /// <summary>
        /// 获取关联推荐信息条目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TopicStandResult> GetRelTopics(TopicRelationRequest request)
        {
            var result = this.ServiceImpl.GetRelTopicsAsync(request);
            return await result;
        }

        /// <summary>
        /// 获取信息板块条目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<TopicResult>> GetBlockTopics(TopicRequest request)
        {
            var result = this.ServiceImpl.GetBlockTopicsAsync(request);
            return await result;
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

        /// <summary>
        /// 获取信息板块条目列表（包装方法）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<TopicResult>> BlockTopicWrapper(TopicRequest request)
        {
            var result = this.ServiceImpl.BlockTopicWrapperAsync(request);
            return await result;
        }

        /// <summary>
        /// 设置信息条目头条标记
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string[]> SetHeadlineFlag(List<string> request) {
            var result = this.ServiceImpl.SetHeadlineFlagAsync(request);
            return await result;
        }
        #endregion 异步方法

    }
}
