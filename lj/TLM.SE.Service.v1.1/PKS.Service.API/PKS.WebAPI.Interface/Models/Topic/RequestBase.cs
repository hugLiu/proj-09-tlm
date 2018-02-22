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
    /// 数据请求基类
    /// </summary>
    [Serializable]
    public class RequestBase
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        [DataMember(Name = "UserInfo")]
        [JsonProperty("userinfo")]
        public UserInfoRequest UserInfo { get; set; }

        /// <summary>
        /// 信息搜索默认配置
        /// </summary>
        [DataMember(Name = "configure")]
        [JsonProperty("configure")]
        public JObject configure { get; set; }
    }
}
