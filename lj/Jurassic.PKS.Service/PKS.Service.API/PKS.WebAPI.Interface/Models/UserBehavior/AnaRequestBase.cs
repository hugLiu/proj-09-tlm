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
    public class AnaRequestBase: UserInfoRequest
    {
        /// <summary>
        /// 限制返回结果数量
        /// </summary>
        [DataMember(Name = "ResultLimit")]
        [JsonProperty("resultlimit")]
        public int ResultLimit { get; set; }
    }
}
