using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
           return RedirectToAction("Index", "Pages");
        }

        public ActionResult SearchData(string searchString)
        {
            var pageNumber = 1;
            List<ProductVM> productVMList = new List<ProductVM>();
            if (searchString == null)
            {
                using (Db db = new Db())
                {
                    productVMList = db.Products.ToArray().Select(x => new ProductVM(x)).ToList();
                    ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                }
                var onePageOfProducts = productVMList.ToPagedList(pageNumber, 6);
                ViewBag.onePageOfProducts = onePageOfProducts;
                productVMList = null;
                return View(productVMList);
            }
            else
            {
                using (Db db = new Db())
                {
                    productVMList = db.Products.ToArray().Where(x => x.Description == searchString || x.CategoryName == searchString
                    || x.Slug == searchString || x.Name == searchString).Select(x => new ProductVM(x)).ToList();
                    ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                }
                var onePageOfProducts = productVMList.ToPagedList(pageNumber, 6);
                ViewBag.onePageOfProducts = onePageOfProducts;
                return View(productVMList);
            }
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

        public ActionResult Price(int? page)
        {
            var pageNumber = page ?? 1;
            List<ProductVM> productVMList;

            using(Db db = new Db())
            {
                productVMList = db.Products.ToArray().OrderBy(x => x.Price).Select(x => new ProductVM(x)).ToList(); 
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            var onePageOfProducts = productVMList.ToPagedList(pageNumber, 6);
            ViewBag.onePageOfProducts = onePageOfProducts;
            return View(productVMList);
        }
        public ActionResult SortOfPriceDec(int? page)
        {
            var pageNumber = page ?? 1;
            List<ProductVM> productVMList;

            using(Db db = new Db())
            {
                productVMList = db.Products.ToArray().OrderBy(x => x.Price).Select(x => new ProductVM(x)).ToList();
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            productVMList.Reverse();
            var onePageOfProducts = productVMList.ToPagedList(pageNumber, 6);
            ViewBag.onePageOfProducts = onePageOfProducts;
            return View(productVMList);
        }

        //GET: Shop/Category/name
        //Get Category
        public ActionResult Category(string name, int? page)
        {
            var pageNumber = page ?? 1;
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
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                //Set chose a category
                ViewBag.SelectedCat = catId.ToString();
                //Check on null
                if (productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }
            var onePageOfProducts = productVMList.ToPagedList(pageNumber, 6);
            ViewBag.onePageOfProducts = onePageOfProducts;
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

        public ActionResult GetAllProduct(int? page)
        {
            var pageNumber = 1;
            List<ProductVM> productVMList = new List<ProductVM>();

                using (Db db = new Db())
                {
                    productVMList = db.Products.ToArray().Select(x => new ProductVM(x)).ToList();
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            var onePageOfProducts = productVMList.ToPagedList(pageNumber, 6);
            ViewBag.onePageOfProducts = onePageOfProducts;
            return View(productVMList);
            }

    }
}