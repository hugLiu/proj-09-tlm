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
    public class anaYourLikesResult: anaResultBase<string>
    {
        public override IEnumerable<string> data { get; set; }
    }
}
