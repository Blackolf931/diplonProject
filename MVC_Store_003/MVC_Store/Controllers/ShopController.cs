using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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
        //GET: Shop/Category/name

            //Get Category
        public ActionResult Category(string name)
        {
            //Declare List of tupe List<>
            List<ProductVM> productVMList;
            //Get idCategory
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = dto.Id;
                //Initialize a list of data
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();
                //Get a name category
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                //Check on null
                if(productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }
            //return Vm with model
            return View(productVMList);
        }

        //GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {

            //Declare DTO and VM
            ProductDTO dto;
            ProductVM model;

            //Initialize idProduct
            int id = 0;

            //Check on usability
            using (Db db = new Db())
            {

                if (!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "shop");
                }

                //Initialize model ProductDTO data
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //Get idProduct
                id = dto.Id;

                //Initialize the model VM with data
                model = new ProductVM(dto);
            }
            //Get all images from Gallery
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));
            //Return the model in VM
            return View("ProductDetails", model);
        }
    }
}