using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WeList
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Posts Search|List",
                url: "Posts/List/a/{area_sl}/l/{loc_sl}/c/{cate_sl}/s/{subc_sl}/k/{keyword}",
                defaults: new { controller = "Posts", action = "List", keyword=UrlParameter.Optional}
                );

           routes.MapRoute(
                name: "Posts Search|List area&loc&cate",
                url: "Posts/List/a/{area_sl}/l/{loc_sl}/c/{cate_sl}/k/{keyword}",
                defaults: new { controller = "Posts", action = "List", keyword=UrlParameter.Optional}
                );

           routes.MapRoute(
                name: "Posts Search|List area&cate&subc",
                url: "Posts/List/a/{area_sl}/c/{cate_sl}/s/{subc_sl}/k/{keyword}",
                defaults: new { controller = "Posts", action = "List", keyword=UrlParameter.Optional}
                );

            routes.MapRoute(
                name: "Posts Search|List area&cate",
                url: "Posts/List/a/{area_sl}/c/{cate_sl}/k/{keyword}",
                defaults: new
                {
                    controller = "Posts",
                    action = "List",
                    keyword = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "Posts Search area&loc",
                url: "Posts/List/a/{area_sl}/l/{loc_sl}/k/{keyword}",
                defaults: new
                {
                    controller = "Posts",
                    action = "List",
                    keyword = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "Posts Search area",
                url: "Posts/List/a/{area_sl}/k/{keyword}",
                defaults: new
                {
                    controller = "Posts",
                    action = "List",
                    keyword = UrlParameter.Optional
                }
            );


            routes.MapRoute(
                name:"Index List",
                url:"Index/List/a/{area_sl}/l/{loc_sl}",
                defaults:new {controller="Index",action="List",area_sl=UrlParameter.Optional,loc_sl=UrlParameter.Optional}
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Index", action = "List", id = UrlParameter.Optional }
            );
        }
    }
}
