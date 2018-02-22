using System;
using System.Collections.Generic;
using System.Drawing;

namespace PKS.WebAPI.Models
{
    /// <summary>门户菜单</summary>
    [Serializable]
    public class PortalMenu
    {
        /// <summary>角色默认打开的菜单</summary>
        public string DefaultUrl { get; set; }
        /// <summary>角色有权限的菜单信息集合</summary>
        public List<MenuInfo> Menus { get; set; }
    }
}
