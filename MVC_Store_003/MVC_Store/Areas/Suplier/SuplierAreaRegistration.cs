using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Suplier
{
    public class SuplierAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Suplier";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Suplier_default",
                "Suplier/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}