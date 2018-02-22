using Newtonsoft.Json;
using PKS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Models.UserBehavior
{
    [JsonObject(NamingStrategyType = typeof(LowerCaseNamingStrategy))]
    public class AnaHotTopicsResult: AnaResultBase<AnaResult4HotTopic>
    {
        /// <summary>
        /// 分析结果
        /// </summary>
        [JsonProperty("result")]
        public override IEnumerable<AnaResult4HotTopic> AnaResult { get; set; }
    }

    public class AnaResult4HotTopic
    {
        /// <summary>
        /// 分析结果 - 热词/热点条目标题
        /// </summary>
        [JsonProperty("word")]
        public string Word { get; set; }
        /// <summary>
        /// 分析结果 - 评分
        /// </summary>
        [JsonProperty("score")]
        public double? Score { get; set; }
        /// <summary>
        /// 分析结果 - 逆向文档频率
        /// </summary>
        [JsonProperty("idf")]
        public double? IDF { get; set; }
        /// <summary>
        /// 分析结果 - 获得多少文档包含此Term
        /// </summary>
        [JsonProperty("docFreq")]
        public double DocFreq { get; set; }
        /// <summary>
        /// 分析结果 - 词频
        /// </summary>
        [JsonProperty("tf")]
        public double TF { get; set; }
    }
}
