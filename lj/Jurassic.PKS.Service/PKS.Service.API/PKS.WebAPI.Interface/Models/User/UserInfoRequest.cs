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
    /// 用户信息-请求
    /// </summary>
    [Serializable]
    public class UserInfoRequest
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [DataMember(Name = "Id")]
        [JsonProperty("id")]
        public string Id { get; set; }

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
        public string[] Role { get; set; }
    }
}
