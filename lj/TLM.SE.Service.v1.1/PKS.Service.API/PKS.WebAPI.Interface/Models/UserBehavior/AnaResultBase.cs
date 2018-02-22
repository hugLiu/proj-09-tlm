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
    public class AnaResultBase<T>
    {
        /// <summary>
        /// 成功标志
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
        /// <summary>
        /// 返回代码
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
        /// <summary>
        /// 分析结果
        /// </summary>
        [JsonProperty("result")]
        public virtual IEnumerable<T> AnaResult { get; set; }
        /// <summary>
        /// 结果数量
        /// </summary>
        [JsonProperty("total")]
        public double? Total { get; set; }
    }
}
