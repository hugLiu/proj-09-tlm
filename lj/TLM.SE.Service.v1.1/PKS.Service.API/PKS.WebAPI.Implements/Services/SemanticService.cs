using AutoMapper;
using Jurassic.PKS.Service.Semantics;
using PKS.Core;
using PKS.TLM.DbServices;
using PKS.TLM.DbServices.Semantics;
using PKS.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Services
{
    public class SemanticService : ServiceBase, ISemanticService, IPerRequestAppService
    {
        private readonly SemanticsSQLProvider _sqlProvider;
        /// <summary>
        /// 构造函数
        /// </summary>
        public SemanticService()
        {
            _sqlProvider = new SemanticsSQLProvider();
        }

        #region 同步方法
        #endregion 同步方法

        #region 异步方法
        /// <summary>
        /// 根据叙词名称获得指定概念类的叙词信息
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        public async Task<List<TermInfo>> GetTermInfo(List<string> terms, string cc)
        {
            if (terms == null || terms.Count == 0) throw new ArgumentNullException(nameof(terms));
            if (cc == null) throw new ArgumentNullException(nameof(cc));
            var result = new List<TermInfo>();
            foreach (var term in terms)
            {
                var dalResult = await _sqlProvider.GetTermInfo(term, cc);
                result.AddRange(dalResult.Select(t => t.MapTo<TermInfo>()).ToList());
            }
            return result;
        }

        /// <summary>
        /// 无用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="langcode"></param>
        /// <param name="onlymain"></param>
        /// <returns></returns>
        public async Task<List<string>> GetTranslationById(string id, string langcode, bool onlymain)
        {
            var guid = Guid.Empty;
            if (!id.IsNullOrEmpty())
                guid = id.ToGuid();
            return await _sqlProvider.GetTranslationById(guid, langcode, onlymain);
        }

        /// <summary>
        /// 无用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cc"></param>
        /// <param name="deeplevel"></param>
        /// <returns></returns>
        public async Task<TreeResult> Hierarchy(string id, string cc, int deeplevel)
        {
            var guid = Guid.Empty;
            //如果id为空，返回该概念类的整棵树
            if (!id.IsNullOrEmpty())
                guid = id.ToGuid();
            if (cc == null) throw new ArgumentNullException(nameof(cc));
            //修改，使支持扩展
            var relation = "R_" + cc + "_XF_" + cc;
            return await _sqlProvider.GetWholeTree(guid, null, cc, relation, deeplevel);
        }

        /// <summary>
        /// 获取所有的概念类
        /// </summary>
        /// <returns></returns>
        public async Task<List<ConceptClass>> GetCC()
        {
            var result = await _sqlProvider.GetCC();
            return result.Select(t => t.MapTo<ConceptClass>()).ToList();
        }

        /// <summary>
        /// 获取所有语义关系类型
        /// </summary>
        /// <returns></returns>
        public async Task<List<SemanticsType>> GetSemanticsType()
        {
            var result = await _sqlProvider.GetSemanticsType();
            return result.Select(t => t.MapTo<SemanticsType>()).ToList();
        }

        /// <summary>
        /// 获取语义关系
        /// </summary>
        /// <param name="term"></param>
        /// <param name="sr"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<List<TermInfo>> Semantics(string term, string sr, string direction)
        {
            term = await _sqlProvider.Formal(term);
            sr = sr.ToString(CultureInfo.InvariantCulture);
            List<SD_CCTerm> result;
            switch (direction)
            {
                case "forward": result = await _sqlProvider.GetSemantics(term, sr); break;
                case "backward": result = await _sqlProvider.GetReverseSemantics(term, sr); break;
                default: throw new ArgumentException(@"direction");
            }
            return result.Select(t => t.MapTo<TermInfo>()).ToList();
        }

        /// <summary>
        /// 获取语义关系
        /// </summary>
        /// <param name="term"></param>
        /// <param name="sr"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<List<TermInfo>> Semantics(string term) {
            term = await _sqlProvider.Formal(term);
            List<SD_CCTerm> result = await _sqlProvider.GetSemantics(term);
            return result.Select(t => t.MapTo<TermInfo>()).ToList();
        }

        /// <summary>
        /// 获取语义关系
        /// </summary>
        /// <param name="term"></param>
        /// <param name="sr"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<List<TermInfo>> Semantics(List<string> terms)
        {
            if (terms == null) throw new ArgumentException(@"参数不能为空");

            List<string> vterms = new List<string>();
            foreach (string t in terms)
            {
                var f = await _sqlProvider.Formal(t);
                vterms.Add(f);
            }
            List<SD_CCTerm> result = await _sqlProvider.GetSemantics(vterms);
            return result.Select(t => t.MapTo<TermInfo>()).ToList();
        }

        /// <summary>
        /// 获得指定类型词库
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public async Task<List<WordResult>> GetDictionary(List<string> cc)
        {
            if (cc == null || cc.Count == 0)
            {
                return await _sqlProvider.GetWholeDict();
            }
            return await _sqlProvider.GetDictByCc(cc);
        }
        #endregion 异步方法
    }
}
