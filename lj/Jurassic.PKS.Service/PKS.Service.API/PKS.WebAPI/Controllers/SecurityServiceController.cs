using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PKS.Utils;
using PKS.WebAPI.Models;
using PKS.WebAPI.Services;
using System.Collections.Generic;
using PKS.Models;
using PKS.Web;
using System;
using PKS.Core;

namespace PKS.WebAPI.Controllers
{
    /// <summary>安全服务控制器</summary>
    public class SecurityServiceController : PKSApiController
    {
        /// <summary>构造函数</summary>
        public SecurityServiceController(ISecurityService service)
        {
            ServiceImpl = service;
        }

        /// <summary>服务实例</summary>
        private ISecurityService ServiceImpl { get; }

        /// <summary>获得服务信息</summary>
        protected override ServiceInfo GetServiceInfo()
        {
            return new ServiceInfo
            {
                Description = "安全服务用于认证授权和门户数据访问"
            };
        }

        /// <summary>登录</summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<LoginResult> Login(LoginRequest request)
        {
            if (request.UserHostAddress.IsNullOrEmpty())
            {
                request.UserHostAddress = this.Request.RequestUri.Authority;
            }
            return await ServiceImpl.LoginAsync(request);
        }

        /// <summary>获取登录用户信息</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IPKSPrincipal> GetPrincipal()
        {
            var token = this.Request.RequestUri.ParseQueryString()["token"];
            if (token.IsNullOrEmpty()) token = this.Token;
            return await ServiceImpl.GetPrincipalAsync(token);
        }

        /// <summary>注销用户</summary>
        [HttpGet]
        public async Task<IHttpActionResult> Logout()
        {
            var token = this.Request.RequestUri.ParseQueryString()["token"];
            if (token.IsNullOrEmpty()) token = this.Token;
            await ServiceImpl.LogoutAsync(token);
            return Ok();
        }

        /// <summary>获得指定角色或登录用户角色的门户菜单</summary>
        [HttpGet]
        public async Task<PortalMenu> GetPortalMenu()
        {
            var roleId = this.Request.RequestUri.ParseQueryString()["roleId"];
            if (roleId.IsNullOrEmpty()) roleId = this.PKSUser.Roles.First().Id;
            return await ServiceImpl.GetPortalMenuAsync(roleId.ToInt32());
        }
        /// <summary>获得指定用户的权限集合</summary>
        [HttpGet]
        public async Task<Dictionary<string, bool>> GetPermissions()
        {
            var userId = this.Request.RequestUri.ParseQueryString()["userId"];
            if (userId.IsNullOrEmpty()) userId = this.PKSUser.Identity.Id;
            return await ServiceImpl.GetPermissionsAsync(userId.ToInt32());
        }
        /// <summary>清空外部缓存</summary>
        [HttpGet]
        public IHttpActionResult ClearCache()
        {
            GetService<ICacheProvider>().ExternalCacher.Clear();
            return Ok();
        }
    }
}