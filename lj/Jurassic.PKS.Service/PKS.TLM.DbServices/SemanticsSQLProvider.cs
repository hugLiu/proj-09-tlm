using Jurassic.PKS.Service.Semantics;
using PKS.TLM.DbServices.LINQ;
using PKS.TLM.DbServices.Semantics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PKS.TLM.DbServices
{
    public class SemanticsSQLProvider
    {
        private SemanticsDBContext _dbContext;

        public SemanticsSQLProvider()
        {
            _dbContext = new SemanticsDBContext();
            _dbContext.Schema = "SEMANTICS";
        }

        #region 获得指定概念类的叙词信息
        /// <summary>
        /// 获得指定概念类的 叙词信息
        /// </summary>
        /// <param name="term">正式叙词</param>
        /// <param name="cc">概念类（可为空）</param>
        /// <returns>给定叙词（满足指定概念类）的详细信息</returns>
        public async Task<List<SD_CCTerm>> GetTermInfo(string term, string cc)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));

            //如果cc为空则返回该叙词的所有概念类信息
            return await _dbContext.SD_CCTerm.Where(w =>
                (w.Term.Equals(term) || w.PathTerm.Equals(term))
                && (w.CCCode.Equals(cc) || cc.Equals("") || cc.Equals(null)))
                .Include("SD_ConceptClass").ToListAsync();
        }
        #endregion

        #region 获得指定叙词ID的翻译词列表
        /// <summary>
        /// 根据叙词ID获取其翻译词
        /// </summary>
        /// <param name="id">叙词ID</param>
        /// <param name="langCode">语言类型</param>
        /// <param name="onlyMain">是否只包含主词</param>
        /// <returns>翻译叙词结果列表</returns>
        public async Task<List<string>> GetTranslationById(Guid id, string langCode, bool onlyMain)
        {
            var query = _dbContext.SD_TermTranslation.Where(tt => tt.TermClassID.Equals(id));

            if (!string.IsNullOrEmpty(langCode))
            {
                query = query.Where(tt => tt.LangCode.Equals(langCode));
            }
                
            if (onlyMain)
            {
                query = query.Where(w => w.IsMain == 1);
            }
            return await query.OrderBy(o => o.OrderIndex).Select(s => s.Translation).Distinct().ToListAsync();
        }

        /// <summary>
        /// 根据叙词获取其翻译词
        /// </summary>
        /// <param name="term">叙词</param>
        /// <param name="langCode">语言类型</param>
        /// <param name="onlyMain">是否只包含主词</param>
        /// <returns>翻译叙词结果列表</returns>
        public async Task<List<string>> GetTranslationByName(string term, string langCode, bool onlyMain)
        {
            var query = from cc in _dbContext.SD_CCTerm
                        join tt in _dbContext.SD_TermTranslation
                        on cc.TermClassID equals tt.TermClassID
                        where cc.Term.Equals(term)
                        select tt;

            if (!string.IsNullOrEmpty(langCode))
            {
                query = query.Where(tt => tt.LangCode.Equals(langCode));
            }

            if (onlyMain)
            {
                query = query.Where(w => w.IsMain == 1);
            }

            return await  query.Select(t => t.Translation).Distinct().ToListAsync();
        }

        #endregion

        #region 获得所有的概念类
        public async Task<List<SD_ConceptClass>> GetCC()
        {
            return await _dbContext.SD_ConceptClass.ToListAsync();
        }
        #endregion

        #region 获得所有语义关系类型
        public async Task<List<SD_SemanticsType>> GetSemanticsType()
        {
            return await _dbContext.SD_SemanticsType.ToListAsync();
        }
        #endregion

        #region 获得概念树
        public async Task<TreeResult> GetWholeTree(Guid termClassId, string term, string cc, string sr, int deepLevel)
        {
            //var ds = await Descendant(termClassId, term, cc, sr, deepLevel);
            var results = new TreeResult();
            //if (ds.Tables.Count == 0) return results;
            //var dt = ds.Tables[0].DefaultView.ToTable(true, "TermClassId", "Term", "PathTerm", "PId", "Source", "OrderIndex");
            ////DataView dv = new DataView(dt);                         //虚拟视图
            ////dt = dv.ToTable(true, "TermClassId","Term","PId");      //参数为true 表示去除重复 ，通过该方法对合并的数据集去重
            //foreach (DataRow row in dt.Rows)
            //{
            //    var id = Convert.ToString(row["TermClassId"]).ToLower();
            //    var termValue = Convert.ToString(row["Term"] == DBNull.Value ? null : row["Term"]);
            //    var pathTerm = Convert.ToString(row["PathTerm"] == DBNull.Value ? null : row["PathTerm"]);
            //    var pid = Convert.ToString(row["PId"]).ToLower();
            //    var source = Convert.ToString(row["Source"] == DBNull.Value ? null : row["Source"]).ToLower();
            //    var orderIndex = Convert.ToInt32(row["OrderIndex"] == DBNull.Value ? 0 : row["OrderIndex"]);
            //    results.TreeItems.Add(new TreeItem(id, pathTerm, termValue, orderIndex, source, pid));
            //}
            return results;
        }

        //public async Task<DataSet> Descendant(Guid id, string parent, string cc, string sr, int lvl)
        //{
        //    IDataParameter pTermClassId = new SqlParameter("@TermClassId", SqlDbType.UniqueIdentifier);
        //    if (id == Guid.Empty) { pTermClassId.Value = DBNull.Value; }
        //    else { pTermClassId.Value = id; }

        //    IDataParameter pParent = new SqlParameter("@Term", SqlDbType.NVarChar);
        //    if (string.IsNullOrEmpty(parent)) { pParent.Value = DBNull.Value; }
        //    else { pParent.Value = parent; }

        //    IDataParameter pCc = new SqlParameter("@CCCode", SqlDbType.VarChar);
        //    if (string.IsNullOrEmpty(cc)) { pCc.Value = DBNull.Value; }
        //    else { pCc.Value = cc; }

        //    IDataParameter pSr = new SqlParameter("@SR", SqlDbType.VarChar);
        //    if (string.IsNullOrEmpty(sr)) { pSr.Value = DBNull.Value; }
        //    else { pSr.Value = sr; }

        //    IDataParameter plvl = new SqlParameter("@lvl", SqlDbType.Int);
        //    plvl.Value = lvl;

        //    var helper = new DBHelper("SemanticsDBContext");
        //    return await Task.FromResult(helper.RunProcedureDs("[dbo].[USP_GetTermTree]", pTermClassId, pParent, pCc, pSr, plvl));
        //}
        #endregion

        #region 获得语义关系
        public async Task<string> Formal(string term)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));

            var formalTerm = await (from tr in _dbContext.SD_TermTranslation
                                    join cc in _dbContext.SD_CCTerm on tr.TermClassID equals cc.TermClassID
                                    where tr.Translation.Trim() == term.Trim()
                                    select cc.Term).ToListAsync();
            return await Task.FromResult(formalTerm.Count != 0 ? formalTerm.FirstOrDefault() : term.Trim());
        }

        public async Task<List<SD_CCTerm>> GetSemantics(List<string> terms)
        {
            if (terms.Count==0)
                throw new ArgumentNullException(@"term is null");

            //先获得语义正向或反向关联的ID，即所有叙述词关系
            var lTermId = await _dbContext.SD_Semantics.Where(w => terms.Contains(w.FTerm) || terms.Contains(w.LTerm))
                    .Select((s => s.LTermClassId))
                    .ToListAsync();

            return lTermId.Count != 0
                  ? _dbContext.SD_CCTerm.Where(w => lTermId.Contains(w.TermClassID)).Include("SD_ConceptClass").ToList()
                  : new List<SD_CCTerm>();
        }

        public async Task<List<SD_CCTerm>> GetSemantics(string term)
        {
            if (string.IsNullOrEmpty(term))
                throw new ArgumentNullException(@"term is null");

            //先获得语义正向或反向关联的ID，即所有叙述词关系
            var lTermId = await _dbContext.SD_Semantics.Where(w => w.FTerm.Equals(term) || w.LTerm.Equals(term))
                    .Select((s => s.LTermClassId))
                    .ToListAsync();

            return lTermId.Count != 0
                  ? _dbContext.SD_CCTerm.Where(w => lTermId.Contains(w.TermClassID)).Include("SD_ConceptClass").ToList()
                  : new List<SD_CCTerm>();
            //var AllTermItems = await _dbContext.SD_Semantics.Where(w => w.FTerm.Equals(term) || w.LTerm.Equals(term))
            //        .ToListAsync();
            //var lTermId = AllTermItems.DistinctBy((s => s.LTermClassId)).Select((s => s.LTermClassId))
            //        .ToList();
            //var fTermId = AllTermItems.DistinctBy((s => s.FTermClassId)).Select((s => s.FTermClassId))
            //        .ToList();

            //var resLTerms = lTermId.Count != 0
            //      ? _dbContext.SD_CCTerm.Where(w => lTermId.Contains(w.TermClassID)).Include("SD_ConceptClass").ToList()
            //      : new List<SD_CCTerm>();
            //var resFTerms = fTermId.Count != 0
            //      ? _dbContext.SD_CCTerm.Where(w => fTermId.Contains(w.TermClassID)).Include("SD_ConceptClass").ToList()
            //      : new List<SD_CCTerm>();

            //var result = resLTerms.Union(resFTerms).ToList<SD_CCTerm>();

            //return result;
        }

        public async Task<List<SD_CCTerm>> GetSemantics(string term, string sr)
        {
            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(sr))
                throw new ArgumentNullException(@"term or " + "sr");

            //先获得语义正向关联的ID
            var lTermId = await _dbContext.SD_Semantics.Where(w => w.FTerm.Equals(term) && w.SR.Equals((sr)))
                    .Select((s => s.LTermClassId))
                    .ToListAsync();

            return lTermId.Count != 0
                  ? _dbContext.SD_CCTerm.Where(w => lTermId.Contains(w.TermClassID)).Include("SD_ConceptClass").ToList()
                  : new List<SD_CCTerm>();
        }

        public async Task<List<SD_CCTerm>> GetReverseSemantics(string term, string sr)
        {
            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(sr))
                throw new ArgumentNullException(@"term or " + "sr");

            //先获得语义反向关联的ID
            var fTermId = await _dbContext.SD_Semantics.Where(w => w.LTerm.Equals(term) && w.SR.Equals((sr)))
                    .Select((s => s.FTermClassId))
                    .ToListAsync();

            return fTermId.Count != 0
                  ? _dbContext.SD_CCTerm.Where(w => fTermId.Contains(w.TermClassID)).Include("SD_ConceptClass").ToList()
                  : new List<SD_CCTerm>();
        }

        #endregion

        #region 获得词库字典

        public async Task<List<WordResult>> GetWholeDict()
        {
            return await _dbContext.SD_CCTerm
                .Where(t => !string.IsNullOrEmpty(t.Term))
                .Select(s => new WordResult
                {
                    Term = s.Term.ToLower(),
                    Cc = s.CCCode == "null" ? "UnKnown" : s.CCCode
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<WordResult>> GetDictByCc(List<string> cc)
        {
            return await _dbContext.SD_CCTerm
                .Where(t => cc.Contains(t.CCCode) && !string.IsNullOrEmpty(t.Term))
                .Select(s => new WordResult
                {
                    Term = s.Term.ToLower(),
                    Cc = s.CCCode == "null" ? "UnKnown" : s.CCCode
                })
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// 获取翻译词词典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetTransDict()
        {
            var query = _dbContext.SD_TermTranslation
                .Join(_dbContext.SD_CCTerm, t => t.TermClassID, c => c.TermClassID, (t, c) => new
                {
                    c.Term,
                    t.Translation
                })
                .GroupBy(e => e.Term)
                .Select(g => new
                {
                    Term = g.Key,
                    Translations = g.Select(s => s.Translation).Distinct().ToList()
                })
                .ToDictionary(d => d.Term, d => d.Translations);
            return  query;
        }

        /// <summary>
        /// 获取同义词字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetAliasDict()
        {
            var query = _dbContext.SD_Semantics
                .Where(t => t.SR.Equals("D"))
                .Select(t => new {t.FTerm, t.LTerm})
                .ToList();
            var dict = new Dictionary<string, List<string>>();
            foreach (var keyValue in query)
            {
                //正向
                if (dict.ContainsKey(keyValue.FTerm))
                {
                    if (!dict[keyValue.FTerm].Contains(keyValue.LTerm) && keyValue.FTerm != keyValue.LTerm)
                        dict[keyValue.FTerm].Add(keyValue.LTerm);
                }
                else
                {
                    dict[keyValue.FTerm] = new List<string> {keyValue.LTerm};
                }
                //反向
                if (dict.ContainsKey(keyValue.LTerm))
                {
                    if (!dict[keyValue.LTerm].Contains(keyValue.FTerm) && keyValue.LTerm != keyValue.FTerm)
                        dict[keyValue.LTerm].Add(keyValue.FTerm);
                }
                else
                {
                    dict[keyValue.LTerm] = new List<string> {keyValue.FTerm};
                }
            }
            return dict;
        }

        #endregion
    }
}
