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
    [JsonObject(NamingStrategyType = typeof(LowerCaseNamingStrategy))]
    public class AnaYourLikesRequest : AnaRequestBase
    {
        /// <summary>
        /// 分析项
        /// </summary>
        [DataMember(Name = "Analysis")]
        [JsonProperty("analysis")]
        public Dictionary<string, object> Analysis { get; set; }
    }
}
