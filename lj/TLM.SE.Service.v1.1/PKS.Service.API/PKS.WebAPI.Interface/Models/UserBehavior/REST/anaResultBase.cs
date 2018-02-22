using Newtonsoft.Json;
using PKS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Models.UserBehavior.REST
{
    /// <summary>
    /// RESTful API返回结果
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(LowerCaseNamingStrategy))]
    [Serializable]
    public class anaResultBase<T>
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public virtual IEnumerable<T> data { get; set; }
        public double? total { get; set; }
    }
}
