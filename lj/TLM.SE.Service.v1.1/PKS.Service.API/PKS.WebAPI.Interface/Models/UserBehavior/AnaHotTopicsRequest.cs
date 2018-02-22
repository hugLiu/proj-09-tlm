using Newtonsoft.Json;
using PKS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Models.UserBehavior
{
    /// <summary>
    /// 分析热点条目请求体
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(LowerCaseNamingStrategy))]
    public class AnaHotTopicsRequest: AnaRequestBase
    {
        /// <summary>
        /// 板块编号
        /// </summary>
        [DataMember(Name = "BlockInfo")]
        [JsonProperty("blockinfo")]
        public BlockBase BlockInfo { get; set; }

        /// <summary>
        /// 分析项
        /// </summary>
        [DataMember(Name = "Analysis")]
        [JsonProperty("analysis")]
        public Dictionary<string, object> Analysis { get; set; }
    }
}
