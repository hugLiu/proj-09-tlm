using System.ComponentModel.DataAnnotations;
using PKS.Models;
using PKS.Utils;

namespace PKS.WebAPI.Models
{
    /// <summary>准确查询结果</summary>
    public class MatchResult<T> where T : class
    {
        /// <summary>总数</summary>
        public int Total { get; set; }
        /// <summary>索引集合</summary>
        public MetadataCollection<T> Metadatas { get; set; }
        /// <summary>生成JSON串</summary>
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}