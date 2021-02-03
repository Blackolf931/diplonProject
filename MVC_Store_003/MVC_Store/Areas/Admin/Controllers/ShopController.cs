using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            //Declare the model type of List
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {

                //intialize the model with data 
                categoryVMList = db.Categories.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();

            }
            //return List in View
            return View(categoryVMList);
        }
    }
}