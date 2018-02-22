using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Models
{
    /// <summary>
    /// 信息条目关联请求体
    /// </summary>
    [Serializable]
    public class TopicRelationRequest : RequestBase
    {
        /// <summary>
        /// 主数据体
        /// 主要包含iiid、研究目标或其他业务板块目标
        /// "masterdata": {
        ///     "iiid": "80a84801-4b62-4789-8085-9700f50b7b5e",
        ///     "well": "库101"
        ///     "pt": "钻井日报"
        /// }
        /// </summary>
        [DataMember(Name = "MasterData")]
        [JsonProperty("masterdata")]
        public JObject MasterData { get; set; }

        /// <summary>
        /// 主数据字段（选择影响结果的主数据字段集合）
        /// </summary>
        /// "masterfield": ["well", "uid"]
        [DataMember(Name = "MasterFields")]
        [JsonProperty("masterfields")]
        public List<string> MasterFields { get; set; }

        /// <summary>
        /// 关联数据字段（语义关联的字段）
        /// </summary>
        /// "masterfield": ["well", "uid", "pt", "ba"]
        [DataMember(Name = "RelationFields")]
        [JsonProperty("relationfields")]
        public List<string> RelationFields { get; set; }

        /// <summary>
        /// 关联逻辑操作符
        /// </summary>
        /// "masterfield": ["$and", "$or", "$not"]
        [DataMember(Name = "RelationSymbol")]
        [JsonProperty("relationsymbol")]
        public string RelationSymbol { get ; set; }

        /// <summary>
        /// 是否启用语义联想，如果不启用则根据获取主数据字段匹配关联数据字段
        /// "intelligent": true
        /// </summary>
        [DataMember(Name = "RelationFields")]
        [JsonProperty("intelligent")]
        public bool Intelligent { get; set; }

        /// <summary>
        /// 信息搜索query
        /// </summary>
        [DataMember(Name = "Query")]
        [JsonProperty("query")]
        public JObject Query { get; set; }
    }
}
