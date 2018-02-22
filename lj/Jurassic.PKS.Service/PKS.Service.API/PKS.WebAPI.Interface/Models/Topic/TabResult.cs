using Newtonsoft.Json;
using PKS.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Models
{
    public class TabResult : TabBase
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

        /// <summary>
        /// 返回分组聚合结果
        /// </summary>
        [DataMember(Name = "Group")]
        [JsonProperty("group")]
        public object Group { get; set; }

    }
}
