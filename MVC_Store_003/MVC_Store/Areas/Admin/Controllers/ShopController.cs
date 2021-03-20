using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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

         //Post: Admin/Sho/AddNewCategory 
         [HttpPost]
        public string AddNewCategory(string catName)
        {
            // Declare a new param ID
            string id;
            
            using(Db db = new Db())
            {
                //Check NameCategory on Unique
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";
                //initializa a model DTO
                CategoryDTO dto = new CategoryDTO();

                //Fill data in a model
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;
                //Save model
                db.Categories.Add(dto);
                db.SaveChanges();
                //Get Id
                id = dto.Id.ToString();

            }

            //return ID in VM
            return id;
        }

        //Get: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                CategoryDTO dto;
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //Get Category
                CategoryDTO dto = db.Categories.Find(id);
                //Delete Category
                db.Categories.Remove(dto);

                //Save changes in database
                db.SaveChanges();
            }
            //Add message about succesfull delete
            TempData["SM"] = "You have deleted a category!";

            //Redirect user to index

            return RedirectToAction("Categories");
        }

        //Post: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                // Check a name category on the unique
                if(db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }
                //Get a model DTO

                CategoryDTO dto = db.Categories.Find(id);

                //Change a model DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Save changes
                db.SaveChanges();
            }
            //Return Result
            return "ok";
        }

        //Create method for add products
        //Get: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Initialize model
            ProductVM model = new ProductVM();

            using(Db db = new Db())
            {
                //add list category from database in model
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }
            //return model in VM
            return View(model);
        }

        //Create method for add products
        //POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase imageForSave)
        {
            //Check the model for Valid
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }
            //Check a name product for unique

            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }

            //declare param ProductDTO
            int id;

            //Initialize and save model based ProductDTO
            using (Db db = new Db())
            {
                ProductDTO dto = new ProductDTO();

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                //Внимательно посмотреть имена если нужно CategoryName
                dto.CategoryName = catDTO.Name;

                db.Products.Add(dto);
                db.SaveChanges();
                id = dto.Id;
            }

            //Add message to TempData
            TempData["SM"] = "You have added a product!";


            #region Upload Image
            //Create a need link derectory
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            //Check avalability derectory
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            //Check file loaded
            if (imageForSave != null && imageForSave.ContentLength > 0)
            {
                string ext = imageForSave.ContentType.ToLower();

                //Check expansion file
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(model);
                    }
                }

                //declare param with name image
                string imageName = imageForSave.FileName;

                //Save a name image in model DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                //Set full path and short path
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                //Save original image
                imageForSave.SaveAs(path);

                //Create and save a small copy
                WebImage img = new WebImage(imageForSave.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }
            #endregion
          
            // Redirect user
            return RedirectToAction("AddProduct");

        }


        //Create Product List
        //Post: Admin/Shop/Products

        [HttpGet]

        public ActionResult Products(int? page, int? catId)
        {
            // Declare ProductVM of type List
            List<ProductVM> listOfProductVM;
            //Set number of page
            var pageNumber = page ?? 1;
            
            using (Db db = new Db())
            {//Intialize ProductVM and fill data
               
                listOfProductVM = db.Products.ToArray()
                     .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                     .Select(x => new ProductVM(x)).ToList();
                //Fill Category list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                //Set chose a category
                ViewBag.SelectedCat = catId.ToString();
            }
            //Set Navigation for pages
            //Цифра меняет количество товаров на страницы
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.onePageOfProducts = onePageOfProducts;
            // return VM with data
            return View(listOfProductVM);
        }
    }
}