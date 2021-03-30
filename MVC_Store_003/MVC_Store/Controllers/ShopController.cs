using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("index", "Pages");
        }
        public ActionResult CategoryMenuPartial()
        {
            // Decalre model of type CategoryVM
            List<CategoryVM> categoryVMList;
            
            //Initialize model
            using(Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            //return View with model
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }
    }
}