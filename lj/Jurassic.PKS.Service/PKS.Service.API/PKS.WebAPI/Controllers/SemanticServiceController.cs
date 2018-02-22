using Jurassic.PKS.Service.Semantics;
using Jurassic.PKS.WebAPI.Semantics;
using PKS.WebAPI.Models;
using PKS.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PKS.WebAPI.Controllers
{
    /// <summary>语义服务控制器</summary>
    public class SemanticServiceController : PKSApiController
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SemanticServiceController(ISemanticService service)
        {
            ServiceImpl = service;
        }

        /// <summary>
        /// 服务实例
        /// </summary>
        private ISemanticService ServiceImpl { get; }

        /// <summary>
        /// 获得服务信息
        /// </summary>
        /// <returns></returns>
        protected override ServiceInfo GetServiceInfo()
        {
            return new ServiceInfo
            {
                Description = "语义服务主要用于叙词分词、获得同义词，翻译词，概念类的树层次结构等"
            };
        }

        #region 异步方法
        /// <summary>根据叙词名称获得指定概念类的叙词信息</summary>
        [HttpPost]
        public async Task<List<TermInfo>> GetTermInfo(GetTermInfoRequest request)
        {
            return await this.ServiceImpl.GetTermInfo(request.Term, request.Cc);
        }

        /// <summary>
        /// 无用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<string>> GetTranslationById([FromUri]GetTranslationByIDRequest request)
        {
            return await this.ServiceImpl.GetTranslationById(request.Id, request.LangCode, request.OnlyMain);
        }

        /// <summary>根据叙词id返回对应的子树的层级结构，并限定返回叙词的概念类</summary>
        /// 无用
        [HttpGet]
        public async Task<TreeResult> Hierarchy([FromUri]HierarchyRequest request)
        {
            if (request == null)
            {
                return null;
            }
            return await this.ServiceImpl.Hierarchy(request.Id, request.Cc, request.DeepLevel);
        }

        /// <summary>获取所有的概念类</summary>
        [HttpGet]
        public async Task<List<ConceptClass>> GetCC()
        {
            return await this.ServiceImpl.GetCC();
        }

        /// <summary>获取所有语义关系类型</summary>
        [HttpGet]
        public async Task<List<SemanticsType>> GetSemanticsType()
        {
            return await this.ServiceImpl.GetSemanticsType();
        }

        /// <summary>获取语义关系</summary>
        [HttpGet]
        public async Task<List<TermInfo>> Semantics([FromUri]SemanticsRequest request)
        {
            return await this.ServiceImpl.Semantics(request.Term, request.SR, request.Direction);
        }

        /// <summary>获取语义关系</summary>
        [HttpGet]
        public async Task<List<TermInfo>> Semantics(string term)
        {
            return await this.ServiceImpl.Semantics(term);
        }

        /// <summary>获取语义关系</summary>
        [HttpPost]
        public async Task<List<TermInfo>> Semantics(List<string> terms)
        {
            return await this.ServiceImpl.Semantics(terms);
        }

        /// <summary>获得指定类型词库</summary>
        [HttpPost]
        public async Task<List<string>> GetDictionary(GetDictionaryRequest request)
        {
            List<WordResult> result;
            if (request == null) result = await this.ServiceImpl.GetDictionary(null);
            else result = await this.ServiceImpl.GetDictionary(request.Cc);
            return result.Select(t => t.Term).ToList();
        }
        #endregion 异步方法
    }
}
