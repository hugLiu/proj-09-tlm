using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using PKS.Utils;
using PKS.Web;
using System.ComponentModel.DataAnnotations;

namespace PKS.Models
{
    /// <summary></summary>
    [JsonObject(NamingStrategyType = typeof(LowerCaseNamingStrategy))]
    public class UserBehavior: Dictionary<string, object>,IParameterValidation
    {
        #region 构造函数
        /// <summary>构造函数</summary>
        public UserBehavior() { }
        #endregion

        #region 数据成员

        /// <summary>索引ID,元数据唯一标识</summary>
        [DataMember(Name = "LLId")]
        [JsonProperty("llid")]
        public string LLId { get; set; }

        /// <summary>日志日期</summary>
        [DataMember(Name = "LogDate")]
        [JsonProperty("logdate")]
        public DateTime? LogDate { get; set; }
        /// <summary>停留日期</summary>
        [DataMember(Name = "StopDate")]
        [JsonProperty("stopdate")]
        public DateTime? StopDate { get; set; }
        /// <summary>页面索引ID</summary>
        [DataMember(Name = "IIId")]
        [JsonProperty("iiid")]
        public string IIId { get; set; }
        /// <summary>页面</summary>
        [DataMember(Name = "PageUrl")]
        [JsonProperty("pageurl")]
        public string PageUrl { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [DataMember(Name = "Pt")]
        [JsonProperty("pt")]
        public string Pt { get; set; }
        /// <summary>
        /// 业务
        /// </summary>
        [DataMember(Name = "Pc")]
        [JsonProperty("pc")]
        public string Pc { get; set; }
        /// <summary>
        /// 领域
        /// </summary>
        [DataMember(Name = "Bd")]
        [JsonProperty("bd")]
        public string Bd { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMember(Name = "Name")]
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 用户单位
        /// </summary>
        [DataMember(Name = "Organization")]
        [JsonProperty("organization")]
        public string Organization { get; set; }

        /// <summary>
        /// 用户部门
        /// </summary>
        [DataMember(Name = "Department")]
        [JsonProperty("department")]
        public string Department { get; set; }

        /// <summary>
        /// 用户岗位
        /// </summary>
        [DataMember(Name = "Position")]
        [JsonProperty("position")]
        public string Position { get; set; }

        /// <summary>
        /// 用户角色（数组）
        /// </summary>
        [DataMember(Name = "Role")]
        [JsonProperty("role")]
        public string Role { get; set; }

        ///// <summary>获取标签值</summary>
        //public object GetValue(string tagName, bool ignoreTagTypeCheck = false)
        //{
        //    var tagValue = this.GetValueOrDefaultBy(tagName, null);
        //    if (ignoreTagTypeCheck) return tagValue;
        //    if (tagValue == null) return null;
        //    if (tagValue is DateTime)
        //    {
        //        return (DateTime)tagValue;
        //    }
        //    return tagValue;
        //}

        ///// <summary>获取日期标签值</summary>
        //private DateTime? GetDateValue(string tagName)
        //{
        //    var tagValue = this.GetValueOrDefaultBy(tagName, null);
        //    if (tagValue == null) return null;
        //    return (DateTime)tagValue;
        //}


        ///// <summary>设置日期标签值</summary>
        //private void SetDateValue(string tagName, DateTime? value)
        //{
        //    if (value.HasValue)
        //    {
        //        base[tagName] = value.Value;
        //    }
        //    else
        //    {
        //        base.Remove(tagName);
        //    }
        //}

        ///// <summary>清理null或空标签键值</summary>
        //public void ClearNullOrEmpty()
        //{
        //    var keys = GetNullOrEmptyKeys();
        //    foreach (var key in keys)
        //    {
        //        base.Remove(key);
        //    }
        //}
        ///// <summary>获得null或空标签键</summary>
        //public string[] GetNullOrEmptyKeys()
        //{
        //    return this.Where(e => IsNullOrEmpty(e.Value)).Select(e => e.Key).ToArray();
        //}
        ///// <summary>获得null或空标签键</summary>
        //public bool IsNullOrEmpty(object value)
        //{
        //    if (value == null) return true;
        //    var sValue = value.As<string>();
        //    if (sValue != null) return sValue.Length == 0;
        //    var aValue = value.As<object[]>();
        //    if (aValue != null) return aValue.Length == 0;
        //    var cValue = value.As<ICollection>();
        //    if (cValue != null) return cValue.Count == 0;
        //    return false;
        //}
        /// <summary>生成JSON串</summary>
        public override string ToString()
        {
            return this.ToJson();
        }

        #endregion
    }
}
