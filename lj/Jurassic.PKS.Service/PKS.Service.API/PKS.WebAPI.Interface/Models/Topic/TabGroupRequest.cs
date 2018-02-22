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
    /// 板块分栏-请求
    /// </summary>
    [Serializable]
    public class TabGroupRequest : TabBase
    {
        /// <summary>
        /// 信息搜索query
        /// </summary>
        [DataMember(Name = "Query")]
        [JsonProperty("query")]
        public JObject Query { get; set; }
    }
}
