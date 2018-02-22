using System.ComponentModel.DataAnnotations;
using PKS.Utils;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PKS.Models;

namespace PKS.WebAPI.Models
{
    /// <summary>登录结果</summary>
    public class LoginResult
    {
        /// <summary>是否成功</summary>
        public bool Success { get; set; }
        /// <summary>错误信息</summary>
        public string ErrorMessage { get; set; }
        /// <summary>认证令牌</summary>
        public string Token { get; set; }
        /// <summary>用户身份</summary>
        public IPKSPrincipal Principal { get; set; }
        /// <summary>生成JSON串</summary>
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}