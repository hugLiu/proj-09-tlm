using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Jurassic.PKS.Service;
using PKS.Core;
using PKS.Models;
using PKS.Utils;
using PKS.WebAPI.Models;
using PKS.WebAPI.Services;

namespace PKS.WebAPI.Controllers
{
    /// <summary>搜索服务控制器</summary>
    public class SearchServiceController : PKSApiController
    {
        /// <summary>构造函数</summary>
        public SearchServiceController(ISearchService service)
        {
            ServiceImpl = service;
        }

        /// <summary>服务实例</summary>
        private ISearchService ServiceImpl { get; }

        /// <summary>获得服务信息</summary>
        protected override ServiceInfo GetServiceInfo()
        {
            return new ServiceInfo
            {
                Description = "搜索服务用于搜索索引库提供索引数据"
            };
        }
        /// <summary>按短语搜索</summary>
        [HttpPost]
        public async Task<SearchResult<Metadata>> Search(SearchRequest request)
        {
            request.Sentence = HttpUtility.UrlDecode(request.Sentence);
            return await ServiceImpl.SearchAsync(request);
        }

        /// <summary>按ES语法搜索</summary>
        [HttpPost]
        public async Task<object> ESSearch()
        {
            var query = await this.Request.Content.ReadAsStringAsync();
            if (query.IsNullOrEmpty())
            {
                ExceptionCodes.MissingParameterValue.ThrowUserFriendly("缺少请求参数", "请求内容应是ES搜索条件");
            }
            var searchResult = await ServiceImpl.ESSearchAsync(query);

            HttpResponseMessage result = new HttpResponseMessage
            {
                Content = new StringContent(searchResult, Encoding.GetEncoding("UTF-8"), "application/json")
            };
            return result;
        }

        /// <summary>按完全匹配条件搜索</summary>
        [HttpPost]
        public async Task<MatchResult<Metadata>> Match(MatchRequest request)
        {
            return await ServiceImpl.MatchAsync(request);
        }
        /// <summary>按多个完全匹配条件搜索</summary>
        [HttpPost]
        public async Task<MatchResult<Metadata>[]> MatchMany(MatchRequest[] requests)
        {
            return await ServiceImpl.MatchManyAsync(requests);
        }
        /// <summary>根据iiid搜索</summary>
        [HttpPost]
        public async Task<Metadata> GetMetadata(SearchMetadataRequest request)
        {
            return await ServiceImpl.GetMetadataAsync(request);
        }
        /// <summary>根据iiid数组搜索</summary>
        [HttpPost]
        public async Task<MetadataCollection<Metadata>> GetMetadatas(SearchMetadatasRequest request)
        {
            return await ServiceImpl.GetMetadatasAsync(request);
        }
        /// <summary>根据聚合条件获取统计信息</summary>
        [HttpPost]
        public async Task<SearchStatisticsResult> Statistics(SearchStatisticsRequest request)
        {
            return await ServiceImpl.StatisticsAsync(request);
        }


        /// <summary>查询元数据定义信息</summary>
        [HttpGet]
        public async Task<MetadataDefinition[]> GetMetadataDefinitions()
        {
            return await ServiceImpl.GetMetadataDefinitionsAsync();
        }

        /// <summary>
        /// 只返回source
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> ESSearchEx(object query)
        {
            return await ServiceImpl.ESSearchExAsync(query);
        }
    }
}
