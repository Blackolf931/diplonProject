using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Suplier.Controllers
{
    public class SuplierDashBoardController : Controller
    {
        // GET: Suplier/SuplierDashBoard
        public ActionResult Index()
        {
            ViewBag.Title = "Index";
            return View();
        }
    }
}