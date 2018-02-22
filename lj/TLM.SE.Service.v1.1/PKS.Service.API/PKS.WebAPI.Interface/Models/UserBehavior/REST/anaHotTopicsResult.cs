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
    public class anaHotTopicsResult:anaResultBase<dataHotTopic>
    {
        public override IEnumerable<dataHotTopic> data { get; set; }
    }

    [Serializable]
    public class dataHotTopic {
        public string word { get; set; }
        public double? score { get; set; }
        public double? idf { get; set; }
        public double docFreq { get; set; }
        public double tf { get; set; }
    }
}
