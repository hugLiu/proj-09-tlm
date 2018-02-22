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
    /// 信息板块
    /// </summary>
    [Serializable]
    public class BlockBase
    {
        /// <summary>
        /// 板块编号
        /// </summary>
        [DataMember(Name = "BlockId")]
        [JsonProperty("blockid")]
        public string BlockId { get; set; }

        /// <summary>
        /// 板块名称
        /// </summary>
        [DataMember(Name = "BlockName")]
        [JsonProperty("blockname")]
        public string BlockName { get; set; }
    }
}
