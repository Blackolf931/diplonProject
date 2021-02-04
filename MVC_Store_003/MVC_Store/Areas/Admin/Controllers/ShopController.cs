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

        //Ger: Admin/Shop/ReorderCategories
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
    }
}