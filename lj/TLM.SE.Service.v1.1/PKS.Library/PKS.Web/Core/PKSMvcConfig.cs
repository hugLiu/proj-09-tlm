using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PKS.Web.MVC.Filter;

namespace PKS.Web.MVC
{
    /// <summary>MVC全局配置</summary>
    public static class PKSMvcConfig
    {
        /// <summary>注册全局过滤器</summary>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(WebBootstrapper.Get<PKSAuthorizeAttribute>()); //授权认证暂时取消
            filters.Add(WebBootstrapper.Get<PKSExceptionFilterAttribute>());
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }
        /// <summary>注册全局过滤器</summary>
        public static void RegisterGlobalFiltersForPortalMgmt(GlobalFilterCollection filters)
        {
            filters.Clear();
            //filters.Add(WebBootstrapper.Get<PKSMgmtAuthorizeAttribute>()); //授权认证暂时取消
            //filters.Add(WebBootstrapper.Get<PKSMgmtExceptionFilterAttribute>()); //授权认证暂时取消
        }
    }
}