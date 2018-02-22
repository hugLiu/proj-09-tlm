using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Models
{
    /// <summary>
    /// 响应结果
    /// </summary>
    [Serializable]
    public class TopicResult
    {
        /// <summary>
        /// 信息板块
        /// </summary>
        [DataMember(Name = "BlockId")]
        [JsonProperty("blockid")]
        public string BlockId { get; set; }

        /// <summary>
        /// 信息分栏结果
        /// </summary>
        [DataMember(Name = "Tabs")]
        [JsonProperty("tabs")]
        public List<TabResult> Tabs { get; set; }
    }
}
