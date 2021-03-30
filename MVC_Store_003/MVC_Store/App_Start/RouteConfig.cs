using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_Store
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Pages","{page}",
                new { controller = "Pages", action = "Index" }
               , new[] { "MVC_Store.Controllers" });

            routes.MapRoute(
                "Default", "",
                new { controller = "Pages", action = "Index" },
                new[] { "MVC_Store.Controllers" });

            routes.MapRoute("PagesManuPartial", "Pages/PagesManuPartial",
                new { controller = "Pages", action = "PagesManuPartial" }
               , new[] { "MVC_Store.Controllers" });

            /* routes.MapRoute(
                 name: "Default",
                 url: "{controller}/{action}/{id}",
                 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
             );*/
        }
    }
}
