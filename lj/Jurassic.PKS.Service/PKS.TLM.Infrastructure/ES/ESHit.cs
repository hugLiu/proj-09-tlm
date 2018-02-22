using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.ES.Model.EsRawResult
{
    public class ESHit
    {
        public double? total { get; set; }
        public double? max_score { get; set; }
        public List<ESItem> hits { get; set; }
    }
}
