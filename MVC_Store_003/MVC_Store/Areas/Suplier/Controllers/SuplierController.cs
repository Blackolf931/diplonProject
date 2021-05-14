using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MVC_Store.Areas.Suplier.Controllers
{
    public class SuplierController : Controller
    {
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Initialize model
            ProductVM model = new ProductVM();

            using (Db db = new Db())
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
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);

            }
            #endregion

            // Redirect user
            return RedirectToAction("AddProduct");

        }


        public ActionResult GetWayBill()
        {
            //Initialize model
            List<WayBillVM> wayBillVm;
            using (Db db = new Db())
            {
                wayBillVm = db.WayBill.ToArray().Select(x => new WayBillVM(x)).ToList();
            }
            //return model in VM
            return View(wayBillVm);
        }

        [HttpGet]
        public ActionResult CreateWayBill()
        {
            //Initialize model
            WayBillVM model = new WayBillVM();

            using (Db db = new Db())
            {
                //add list category from database in model
                model.ProductsInWayBill = new SelectList(db.Products.ToList(), "id", "Name");
            }
            //return model in VM
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateWayBill(WayBillVM model, int[] IdProductInWayBill)
        {
            //Check the model for Valid
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.ProductsInWayBill = new SelectList(db.Products.ToList(), "Id", "Name");
                    return View(model);
                }
            }
            if (IdProductInWayBill.Length <= 0)
            {
                ModelState.AddModelError("", "You should choice a one products!");
                return View(model);
            }

            //Check a name product for unique

            using (Db db = new Db())
            {
               if (db.WayBill.Any(x => x.DocumentNumber == model.DocumentNumber))
                {
                    model.ProductsInWayBill = new SelectList(db.Products.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That document number is taken!");
                    return View(model);
                }
            }
            List<WayBillVM> wayBillVm;
            using (Db db = new Db())
            {
                wayBillVm = db.WayBill.ToArray().Select(x => new WayBillVM(x)).ToList();
            }
            //declare param ProductDTO
            int id = 0;
            for (int i = 0; i < IdProductInWayBill.Length; i++) 
            {
                id = 0;
                id = AddProductInWayBill(wayBillVm, IdProductInWayBill[i]);
            }



            //Initialize and save model based ProductDTO
            using (Db db = new Db())
            {
                WayBillDTO dto = new WayBillDTO();

                dto.Base = model.Base;
                dto.CreatedAt = DateTime.Now;
                dto.DocumentNumber = id;
                dto.Payer = model.Payer;
                dto.ProviderName = model.ProviderName;
                dto.RecipientName = model.RecipientName;
                dto.TheCargoWasRealesed = model.RecipientName;
                dto.IdProductInWayBill = 0;

                db.WayBill.Add(dto);
                db.SaveChanges();
               // id = dto.Id;
            }

            //Add message to TempData
            TempData["SM"] = "You have created a WayBill!";

            // Redirect user*/
            return RedirectToAction("CreateWayBill");
        }
        private int AddProductInWayBill(List<WayBillVM> wayBillVm, int id)
        {
            using (Db db = new Db())
            {
                ProductInWayBillDTO dto = new ProductInWayBillDTO();
                id = wayBillVm[wayBillVm.Count-1].IdWayBill +1;
                dto.IdWayBill = id;
                dto.CountProduct = 1;
                dto.IdProduct = id;
                db.ProductInWayBillDTO.Add(dto);
                db.SaveChanges();
            }
            return id;
        }
    }
}