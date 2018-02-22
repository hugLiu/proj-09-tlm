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
    public class AnaYourLikesResult : AnaResultBase<string>
    {
        /// <summary>
        /// 分析结果
        /// </summary>
        [JsonProperty("result")]
        public override IEnumerable<string> AnaResult { get; set; }
    }
}
