using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using PKS.Core;
using PKS.Models;
using PKS.Utils;
using PKS.Web;
using PKS.WebAPI.Services;

namespace PKS.WebAPI.Filter
{
    /// <summary>PKS授权验证特性</summary>
    public class PKSAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>PKS授权验证特性</summary>
        public PKSAuthorizeAttribute(ISecurityService securityService)
        {
            SecurityService = securityService;
        }

        /// <summary>安全服务实例</summary>
        public ISecurityService SecurityService { get; set; }

        /// <summary>是否授权</summary>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var context = actionContext.ControllerContext;
            var authorizationHeader = context.Request.Headers.Authorization;
            if (authorizationHeader == null || authorizationHeader.Parameter.IsNullOrEmpty())
            {
#if DEBUG
                authorizationHeader =
                    new AuthenticationHeaderValue(PKSWebExtension.AuthorizationSchema, PKSWebConsts.Token_Debug);
                context.Request.Headers.Authorization = authorizationHeader;
#else
                ExceptionCodes.OperationUnauthorized.ThrowUserFriendly("请求未授权", "Authorization头不存在或为空");
#endif
            }
            var token = authorizationHeader.Parameter;
            var principal = SecurityService.GetPrincipal(token);
            if (principal == null)
            {
                ExceptionCodes.OperationUnauthorized.ThrowUserFriendly("请求未授权", "Token无效");
            }
            context.RequestContext.Principal = principal;
            return base.IsAuthorized(actionContext);
        }

        /// <summary>处理未授权的请求</summary>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            ExceptionCodes.OperationUnauthorized.ThrowUserFriendly("请求未授权", "Token认证失败");
        }
    }
}