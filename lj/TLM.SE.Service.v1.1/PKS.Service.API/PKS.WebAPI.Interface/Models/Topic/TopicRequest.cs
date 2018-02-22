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
    /// 请求参数
    /// </summary>
    [Serializable]
    public class TopicRequest : RequestBase
    {
        /// <summary>
        /// 信息栏目-数据请求
        /// </summary>
        [DataMember(Name = "Blocks")]
        [JsonProperty("blocks")]
        public List<BlockRequest> Blocks { get; set; }
    }
}
