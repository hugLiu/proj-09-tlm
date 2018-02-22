using System.Security.Principal;
using System.Threading.Tasks;
using Jurassic.PKS.Service;
using Nest;
using PKS.Models;
using PKS.WebAPI.Models;
using SearchRequest = PKS.WebAPI.Models.SearchRequest;

namespace PKS.WebAPI.Services
{
    /// <summary>搜索服务包装器接口</summary>
    public interface ISearchServiceWrapper : ISearchService, IApiServiceWrapper
    {
    }

    /// <summary>搜索服务接口</summary>
    public interface ISearchService
    {
        /// <summary>按短语搜索</summary>
        SearchResult<Metadata> Search(SearchRequest request);

        /// <summary>按短语搜索</summary>
        Task<SearchResult<Metadata>> SearchAsync(SearchRequest request);


        /// <summary>按短语搜索</summary>
        SearchResult<Metadata> SearchPlus(SearchRequest request);

        /// <summary>按短语搜索</summary>
        Task<SearchResult<Metadata>> SearchPlusAsync(SearchRequest request);


        /// <summary>按ES语法搜索</summary>
        string ESSearch(string request);

        /// <summary>按ES语法搜索</summary>
        Task<string> ESSearchAsync(string request);
        /// <summary>按完全匹配条件搜索</summary>
        MatchResult<Metadata> Match(MatchRequest request);

        /// <summary>按完全匹配条件搜索</summary>
        Task<MatchResult<Metadata>> MatchAsync(MatchRequest request);
        /// <summary>按多个完全匹配条件搜索</summary>
        MatchResult<Metadata>[] MatchMany(MatchRequest[] request);

        /// <summary>按多个完全匹配条件搜索</summary>
        Task<MatchResult<Metadata>[]> MatchManyAsync(MatchRequest[] request);
        /// <summary>根据iiid搜索</summary>
        Metadata GetMetadata(SearchMetadataRequest request);

        /// <summary>根据iiid搜索</summary>
        Task<Metadata> GetMetadataAsync(SearchMetadataRequest request);
        /// <summary>根据iiid数组搜索</summary>
        MetadataCollection<Metadata> GetMetadatas(SearchMetadatasRequest request);

        /// <summary>根据iiid数组搜索</summary>
        Task<MetadataCollection<Metadata>> GetMetadatasAsync(SearchMetadatasRequest request);
        /// <summary>根据聚合条件获取统计信息</summary>
        SearchStatisticsResult Statistics(SearchStatisticsRequest request);

        /// <summary>根据聚合条件获取统计信息</summary>
        Task<SearchStatisticsResult> StatisticsAsync(SearchStatisticsRequest request);
        /// <summary>查询元数据定义信息</summary>

        object ESSearchEx(object query);
        Task<object> ESSearchExAsync(object query);
    }
}