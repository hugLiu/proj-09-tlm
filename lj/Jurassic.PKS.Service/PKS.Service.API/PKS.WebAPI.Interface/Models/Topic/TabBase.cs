using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PKS.WebAPI.Models
{
    /// <summary>
    /// 信息板块分栏基类
    /// </summary>
    [Serializable]
    public class TabBase
    {
        /// <summary>
        /// 分栏编号
        /// </summary>
        [DataMember(Name = "TabId")]
        [JsonProperty("tabid")]
        public string TabId { get; set; }

        /// <summary>
        /// 分栏名称
        /// </summary>
        [DataMember(Name = "TabName")]
        [JsonProperty("tabname")]
        public string TabName { get; set; }

        /// <summary>
        /// 搜索类别
        /// </summary>
        [DataMember(Name = "SearchType")]
        [JsonProperty("searchtype")]
        public TopicSearchType SearchType { get; set; }
    }

    /// <summary>
    /// 信息条目搜索类别
    /// </summary>
    [Serializable]
    public enum TopicSearchType
    {
        //普通请求不需要处理
        None = 0,
        /// <summary>
        /// 获取热点信息条目
        /// </summary>
        GetHotTopics = 1,
        /// <summary>
        /// 获取个人最近浏览条目
        /// </summary>
        GetRecency = 2,
        /// <summary>
        /// 获取猜你喜欢推荐条目
        /// </summary>
        GetYourLikes = 3,
        /// <summary>
        /// 获取头条信息条目
        /// </summary>
        GetHeadlines = 4,
        /// <summary>
        /// 获取关联推荐信息
        /// </summary>
        GetRelTopics = 5
    }
}
