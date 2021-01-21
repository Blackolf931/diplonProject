using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declare list for view (PageVM)
            List<PageVM> pageList;
            //Set list(Db)
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
           //return list to view

           return View(pageList);
        
        }

        //GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        //POST: Admin/pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check model on Valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //Declare param for short description(slug)
                string slug;

                //initialize PageDto
                PagesDTO dto = new PagesDTO();

                //Set title model
                dto.Title = model.Title.ToUpper();

                //Check have got sgort description
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Check on unique short description and  title
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exists");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug already exists");
                    return View(model);
                }

                //set another value to model
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Save model in database
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            //Send message view TempData
            TempData["SM"] = "You have added a new page";

            //Redirect user to Index 
            return RedirectToAction("Index");
        }
    }
}