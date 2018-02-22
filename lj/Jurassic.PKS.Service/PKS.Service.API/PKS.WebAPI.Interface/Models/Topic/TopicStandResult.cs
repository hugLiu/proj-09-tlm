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
    public class TopicStandResult
    {
        /// <summary>
        /// 返回结果条目数
        /// </summary>
        [DataMember(Name = "Total")]
        [JsonProperty("total")]
        public double? Total { get; set; }

        /// <summary>
        /// 返回结果集合
        /// </summary>
        [DataMember(Name = "Result")]
        [JsonProperty("result")]
        public object Result { get; set; }
    }
}
