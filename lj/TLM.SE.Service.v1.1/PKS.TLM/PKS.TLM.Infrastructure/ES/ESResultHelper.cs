﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PKS.WebAPI.ES.Model.EsRawResult
{
    public static class ESResultHelper
    {
        public static double? GetTotal(this ESRoot esRoot)
        {
            return esRoot.hits.total;
        }

        public static IEnumerable<Dictionary<string, object>> GetSource(this ESRoot esRoot)
        {
            var res = new List<Dictionary<string, object>>();
            var hits = esRoot.hits?.hits;
            if (hits == null)
                throw new NullReferenceException("搜索服务内部错误");
            foreach (var o in hits)
            {
                res.Add(o._source);
            }
            return res;
        }
    }
}
