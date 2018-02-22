using System;
using System.Net.Security;
using System.Security.Principal;
using System.Threading.Tasks;
using PKS.Core;
using PKS.Models;
using PKS.WebAPI.Models;
using PKS.DbServices;
using PKS.DbServices.SysFrame;
using System.Collections.Generic;
using PKS.DBModels;
using System.Linq;
using System.Security.Policy;
using CacheManager.Core;
using Nest;
using PKS.Utils;

namespace PKS.WebAPI.Services
{
    /// <summary>安全服务实现</summary>
    public class SecurityService : AppService, ISecurityService, ISingletonAppService
    {
        /// <summary>登录</summary>
        public LoginResult Login(LoginRequest request)
        {
            return Task.Run(() => LoginAsync(request)).Result;
        }

        /// <summary>登录</summary>
        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            var result = await GetService<IdentityService>().LoginAsync(request);
            if (result.Success)
            {
                var cacheItem = new CacheItem<object>(result.Token, CacheConst.AuthenticationRegion, result.Principal, ExpirationMode.Absolute, TimeSpan.FromMinutes(PKSWebConsts.TokenExpireInterval));
                this.MemcachedCacher.TryAddOrUpdate(cacheItem);
            }
            return result;
        }
        /// <summary>获取登录用户信息</summary>
        public IPKSPrincipal GetPrincipal(string token)
        {
            return this.MemcachedCacher.TryGetOrAddValue<IPKSPrincipal>(token, CacheConst.AuthenticationRegion, GetCacheItem_Principal);
        }
        /// <summary>获取登录用户信息</summary>
        public Task<IPKSPrincipal> GetPrincipalAsync(string token)
        {
            return Task.FromResult(GetPrincipal(token));
        }
        /// <summary>获得缓存项_登录用户信息</summary>
        private CacheItem<object> GetCacheItem_Principal(string token, string region)
        {
            var principal = GetPrincipalFromDb(token);
            if (principal == null) return null;
            return new CacheItem<object>(token, region, principal, ExpirationMode.Absolute, TimeSpan.FromMinutes(PKSWebConsts.TokenExpireInterval));
        }
        /// <summary>从数据库获取登录用户信息</summary>
        private IPKSPrincipal GetPrincipalFromDb(string token)
        {
            return Task.Run(() => GetService<IdentityService>().GetPrincipalAsync(token)).Result;
        }
        /// <summary>注销用户</summary>
        public void Logout(string token)
        {
            Task.Run(() => LogoutAsync(token));
        }

        /// <summary>注销用户</summary>
        public async Task LogoutAsync(string token)
        {
            await GetService<IdentityService>().LogoutAsync(token);
            this.MemcachedCacher.TryRemove(token, CacheConst.AuthenticationRegion);
        }
        /// <summary>获得指定角色的门户菜单</summary>
        public PortalMenu GetPortalMenu(int roleId)
        {
            return Task.Run(() => GetPortalMenuAsync(roleId)).Result;
        }

        /// <summary>获得指定角色的门户菜单</summary>
        public async Task<PortalMenu> GetPortalMenuAsync(int roleId)
        {
            return await GetService<RolePermissionsService>().GetPortalMenuAsync(roleId);
        }
        /// <summary>获得指定用户的权限集合</summary>
        public Dictionary<string, bool> GetPermissions(int userId)
        {
            return Task.Run(() => GetPermissionsAsync(userId)).Result;
        }
        /// <summary>获得指定用户的权限集合</summary>
        public async Task<Dictionary<string, bool>> GetPermissionsAsync(int userId)
        {
            return await GetService<RolePermissionsService>().GetPermissionsAsync(userId);
        }
    }
}