using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.PKS.Service;
using Nest;
using PKS.Models;
using PKS.WebAPI.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace PKS.WebAPI.ES
{
    /// <summary>
    /// ES搜索Provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchProvider<T> where T : class
    {
        /// <summary>
        /// 根据过滤条件构建查询
        /// </summary>
        /// <param name="filterExp">过滤条件表达式</param>
        /// <returns></returns>
        public QueryContainer BuildFilterQuery(Dictionary<string, object[]> filterExp)
        {
            if (filterExp == null|| filterExp.Count==0)
                return null;
            var queryContainerList = new List<QueryContainer>();
            if (filterExp.Count > 0)
            {
                foreach (var itemExp in filterExp)
                {
                    queryContainerList.Add(new TermsQuery { Field = KeyWord(itemExp.Key), Terms = itemExp.Value });
                }
            }
            return new BoolQuery { Must = queryContainerList };
        }

        /// <summary>
        /// 根据过滤条件构建查询（扩展 by panxiang）
        /// </summary>
        /// <param name="subquery">过滤条件表达式</param>
        /// <returns></returns>
        public void BuildFilterQuery(List<QueryContainer> queryContainerList, object[] subquery)
        {
            if (subquery == null || subquery.Length == 0)
                return;
            if (subquery.Length > 0)
            {
                JObject query = null;
                Dictionary<string, object[]> filter = null;
                Dictionary<string, object> range = null;
                foreach (var item in subquery)
                {
                    query = JObject.FromObject(item);
                    JToken f = query.GetValue("filter");
                    if (f != null)
                    {
                        filter = f.ToObject<Dictionary<string, object[]>>();

                        var filterQuery = BuildFilterQuery(filter);

                        queryContainerList.Add(filterQuery);
                    }
                    //适应区间范围过滤条件
                    f = query.GetValue("_range");
                    if (f != null)
                    {
                        range = f.ToObject<Dictionary<string, object>>();

                        var rangerQuery = BuildRangeQuery(range);

                        queryContainerList.Add(rangerQuery);
                    }
                }
            }
        }

        ///// <summary>
        ///// 根据输入短语构建全文查询
        ///// </summary>
        ///// <param name="sentence"></param>
        ///// <param name="ranks"></param>
        ///// <returns></returns>
        //public QueryContainer BuildFullTextQuery(string sentence, RankCollection ranks)
        //{
        //    if (string.IsNullOrEmpty(sentence)) return null;
        //    //var fields = new List<string> { "source.*^1", "ep.*^1", "fulltext^1" };

        //    //The query_string and simple_query_string queries query the _all field by default
        //    //unless another field is specified:
        //    return ranks != null && ranks.Count > 0
        //        ? new QueryStringQuery { Query = sentence, Fields = GetBoostFields(ranks).ToArray(), UseDisMax = false }
        //        : new QueryStringQuery { Query = sentence, UseDisMax = false };
        //}

        /// <summary>
        /// 创建自定义得分的查询
        /// </summary>
        /// <param name="sentence">输入短语</param>
        /// <param name="ranks">排序规则</param>
        /// <returns></returns>
        public QueryContainer BuildCustomScoreQuery(string sentence, Dictionary<string,int> ranks)
        {
            if (string.IsNullOrEmpty(sentence)) return null;
            if (ranks != null && ranks.Count > 0)
                return new FunctionScoreQuery
                {
                    Functions = BuildScoreFunctions(sentence, ranks),
                    MinScore = ranks.Select(s => s.Value).Min()
                };
            return new QueryStringQuery { Query = sentence, UseDisMax = false };
        }

        /// <summary>
        /// 构建得分函数
        /// </summary>
        /// <param name="sentence">输入短语</param>
        /// <param name="ranks">排序规则</param>
        /// <returns>返回一个或多个得分函数定义</returns>
        private static IEnumerable<IScoreFunction> BuildScoreFunctions(string sentence, Dictionary<string, int> ranks)
        {
            var scoreFunctions = new List<IScoreFunction>();
            foreach (var rank in ranks)
            {
                var scoreFunction = new WeightFunction()
                {
                    Filter = new MatchQuery() { Field = rank.Key, Query = sentence },
                    Weight = rank.Value
                };
                scoreFunctions.Add(scoreFunction);
            }
            return scoreFunctions;
        }

        ///// <summary>
        ///// 获得带权重的字段
        ///// </summary>
        ///// <param name="ranks">字段权重</param>
        ///// <returns></returns>
        //private static List<string> GetBoostFields(RankCollection ranks)
        //{
        //    var boostFields = new List<string>();
        //    foreach (var rank in ranks)
        //    {
        //        var field = rank.Key + "^" + rank.Value;
        //        boostFields.Add(field);
        //    }
        //    return boostFields;
        //}

        /// <summary>
        /// 合并多个查询为Should组合查询
        /// </summary>
        /// <param name="querys">多个查询条件</param>
        /// <returns></returns>
        public QueryContainer CombineShouldQuery(params QueryContainer[] querys)
        {
            var count = querys.Count(s => s != null);
            return count > 1
               ? new BoolQuery() { Should = querys } : count == 1
               ? querys.First(w => w != null)
               : null;
        }

        /// <summary>
        /// 合并多个查询为Must组合查询
        /// </summary>
        /// <param name="querys">多个查询条件</param>
        /// <returns></returns>
        public QueryContainer CombineMustQuery(params QueryContainer[] querys)
        {
            var count = querys.Count(s => s != null);
            return count > 1
               ? new BoolQuery() { Must = querys } : count == 1
               ? querys.First(w => w != null)
               : null;
        }


        /// <summary>
        /// 合并多个查询为MustNot组合查询
        /// </summary>
        /// <param name="querys">多个查询条件</param>
        /// <returns></returns>
        public QueryContainer CombineMustNotQuery(params QueryContainer[] querys)
        {
            var count = querys.Count(s => s != null);
            return count > 1
                ? new BoolQuery() { MustNot = querys } : count == 1
                ? querys.First(w => w != null)
                : null;
        }

        /// <summary>
        /// 根据排序规则构建排序器
        /// </summary>
        /// <param name="sorts">排序规则</param>
        /// <returns></returns>
        public SortDescriptor<T> BuildSort(List<PKSKeyValuePair<string, object>> sorts)
        {
            if (sorts == null || sorts.Count == 0) return null;

            var sortDesc = new SortDescriptor<T>();
            foreach (var s in sorts)
            {
                var order = Convert.ToInt32(s.Value) == 1 ? SortOrder.Descending : SortOrder.Ascending;
                var sortField = new SortField() { Field = KeyWord(s.Key), Order = order };//取消 Keyword 方法 debug by panxiang
                sortDesc.Field(f => sortField);
                //if (s.Value.Promotion == null || s.Value.Promotion.Count == 0)
                //{
                //    var sortField = new SortField() { Field = s.Key, Order = order };
                //    sortDesc.Field(f => sortField);
                //}
                //else
                //{
                //    var scriptField = new ScriptSort { Script = new InlineScript(GetOrderScript(s.Value, s.Key)), Order = order, Type = "number" };
                //    sortDesc.Script(sc => scriptField);
                //}
            }
            return sortDesc;
        }

        ///// <summary>
        ///// 获得排序脚本
        ///// </summary>
        ///// <param name="sort">排序规则</param>
        ///// <param name="field">排序字段</param>
        ///// <returns></returns>
        //private static string GetOrderScript(Sort sort, string field)
        //{
        //    var value = sort.Promotion;
        //    var sb = new StringBuilder();
        //    for (int i = 0; i < value.Count + 1; i++)
        //    {
        //        string scriptText;
        //        string itemValue;
        //        if (i == 0)
        //        {
        //            itemValue = value[i].Trim();
        //            // scriptText = $"if('{itemValue}' == doc['{field.KeyWord()}'].value)return 0;";
        //            scriptText = $"'{itemValue}'==doc['{field.KeyWord()}'].value?0";
        //        }
        //        else if (i == value.Count)
        //        {
        //            // scriptText = $"else return {value.Count}";
        //            scriptText = $":{i}";
        //            for (var j = 0; j < value.Count - 1; j++)
        //            {
        //                scriptText += ")";
        //            }
        //        }
        //        else
        //        {
        //            itemValue = value[i];
        //            //scriptText = $"else if('{itemValue}' == doc['{field.KeyWord()}'].value)return {i};";
        //            scriptText = $":('{itemValue}'==doc['{field.KeyWord()}'].value?{i}";
        //        }
        //        sb.Append(scriptText);
        //    }

        //    return sb.ToString();
        //}

        /// <summary>
        /// 根据聚合条件构建聚合器
        /// </summary>
        /// <param name="groups">聚合条件</param>
        /// <returns></returns>
        public AggregationContainerDescriptor<T> BuildAggs(SearchGroupRules groups)
        {
            if (groups == null || groups.Fields.Count == 0) return null;

            var aggsDesc = new AggregationContainerDescriptor<T>();
            foreach (var gfield in groups.Fields)
            {
                aggsDesc.Terms(gfield, s => s.Field(KeyWord(gfield)).Size(groups.Top).OrderDescending("_count"));
            }
            return aggsDesc;
        }

        /// <summary>
        /// 构建返回字段过滤器
        /// </summary>
        /// <param name="fields">字段投影</param>
        /// <returns></returns>
        public SourceFilterDescriptor<T> BuildFields(SearchSourceFilter fields)
        {
            if (fields == null ) return null;
            var fieldDesc = new SourceFilterDescriptor<T>()
                .IncludeAll();
            if(fields.Excludes!=null&&fields.Excludes.Count>0)
                fieldDesc= fieldDesc.Excludes(ec => ec.Fields(fields.Excludes.ToArray()));
            if (fields.Includes != null && fields.Includes.Count > 0)
                fieldDesc= fieldDesc.Includes(ic => ic.Fields(fields.Includes.ToArray()));

            return fieldDesc;
        }


        public string KeyWord(string field)
        {
            int poi = field.IndexOf(".nokey");
            if (poi >= 0) {
                return field.Remove(poi);
            }
            //return field;
            //有些字段添加.keyword搜索不到数据 debug by panxiang
            return field.Contains("keyword") ? field : field + ".keyword";
        }

        #region 扩展方法
        /// <summary>
        /// 指定区间范围查询
        /// </summary>
        /// <param name="rangeExp">过滤条件表达式</param>
        /// <returns></returns>
        public QueryContainer BuildRangeQuery(Dictionary<string, object> rangeExp)
        {
            if (rangeExp == null || rangeExp.Count == 0)
                return null;
            var queryContainerList = new List<QueryContainer>();
            if (rangeExp.Count > 0)
            {
                foreach (var itemExp in rangeExp)
                {
                    var o = JObject.FromObject(itemExp.Value);
                    var t = o.GetValue("type");
                    var gt = o.GetValue("gt");
                    var gte = o.GetValue("gte");
                    var lt = o.GetValue("lt");
                    var lte = o.GetValue("lte");
                    var relation = o.GetValue("relation");
                    if (t == null)
                        return new BoolQuery();

                    switch (t.ToString().ToLower())
                    {
                        case "date":
                            {//日期型区间
                                var rangeQuery = new DateRangeQuery { Field = itemExp.Key };
                                if (gt != null)
                                    rangeQuery.GreaterThan = DateTime.Parse(gt.ToString());
                                if (gte != null)
                                    rangeQuery.GreaterThanOrEqualTo = DateTime.Parse(gte.ToString());
                                if (lt != null)
                                    rangeQuery.LessThan = DateTime.Parse(lt.ToString());
                                if (lte != null)
                                    rangeQuery.LessThanOrEqualTo = DateTime.Parse(lte.ToString());
                                queryContainerList.Add(rangeQuery);
                                break;
                            }
                        case "numeric":
                            {//数字型区间
                                var rangeQuery = new NumericRangeQuery { Field = itemExp.Key };
                                if (gt != null)
                                    rangeQuery.GreaterThan = double.Parse(gt.ToString());
                                if (gte != null)
                                    rangeQuery.GreaterThanOrEqualTo = double.Parse(gte.ToString());
                                if (lt != null)
                                    rangeQuery.LessThan = double.Parse(lt.ToString());
                                if (lte != null)
                                    rangeQuery.LessThanOrEqualTo = double.Parse(lte.ToString());
                                if (relation != null)
                                {
                                    switch(relation.ToString().ToLower())
                                    {
                                        case "contains": {
                                                rangeQuery.Relation = RangeRelation.Contains;
                                                break;
                                            }
                                        case "intersects":
                                            {
                                                rangeQuery.Relation = RangeRelation.Intersects;
                                                break;
                                            }
                                        case "within":
                                            {
                                                rangeQuery.Relation = RangeRelation.Within;
                                                break;
                                            }
                                    }
                                }
                                queryContainerList.Add(rangeQuery);
                                break;
                            }
                        case "term":
                            {//字符型区间
                                var rangeQuery = new TermRangeQuery { Field = itemExp.Key };
                                if (gt != null)
                                    rangeQuery.GreaterThan = gt.ToString();
                                if (gte != null)
                                    rangeQuery.GreaterThanOrEqualTo = gte.ToString();
                                if (lt != null)
                                    rangeQuery.LessThan = lt.ToString();
                                if (lte != null)
                                    rangeQuery.LessThanOrEqualTo = lte.ToString();
                                queryContainerList.Add(rangeQuery);
                                break;
                            }
                    }
                }
            }
            return new BoolQuery { Must = queryContainerList };
        }
        #endregion 扩展方法

        #region 搜索方法 薛炜 2017-11-15
        public virtual async Task<T> GetEsdataAsync(SearchMetadataRequest request)
        {
            ESAccess<T> _esAccess = new ESAccess<T>();
            var provider = new SearchProvider<T>();
            var termQuery = new TermQuery { Field = "iiid.keyword", Value = request.IIId };
            var query = new BoolQuery() { Must = new List<QueryContainer> { termQuery } };

            var fields = provider.BuildFields(request.Fields);

            var from = 0;
            var size = 1;

            var searchresponse = await _esAccess.PagingQueryAsync(query, null, fields, null, from, size);

            var response = searchresponse.Documents.FirstOrDefault();

            return await Task.FromResult(response);
        }

        #endregion
    }
}
