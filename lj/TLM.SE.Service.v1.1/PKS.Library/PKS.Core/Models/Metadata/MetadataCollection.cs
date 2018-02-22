using System;
using System.Collections.Generic;
using PKS.Utils;

namespace PKS.Models
{
    /// <summary>元数据集合</summary>
    [Serializable]
    public class MetadataCollection<T> : List<T>
    {
        /// <summary>构造函数</summary>
        public MetadataCollection()
        {
        }
        /// <summary>构造函数</summary>
        public MetadataCollection(IEnumerable<T> values) : base(values)
        {
        }
        /// <summary>生成JSON串</summary>
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
