using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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
                PagesDTO dto = new PagesDTO
                {
                    //Set title model
                    Title = model.Title.ToUpper()
                };

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

        //GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Declare model PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Get page
                PagesDTO dto = db.Pages.Find(id);

                //Check on available page
                if (dto == null)
                {
                    return Content("This page not found");
                }
                //Initialize model dto
                model = new PageVM(dto);
            }
            //return model in view

            return View(model);
        }

        //GET: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Check model on valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Get id page
                int id = model.Id;
                //Declare param for slug
                string slug = "home";

                //Get page (id)
                PagesDTO dto = db.Pages.Find(id);

                //Set name из get model DTO
                dto.Title = model.Title;

                //Check slug on unique and set param
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Check title and slug on unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }
                //set other value to class DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Save in database
                db.SaveChanges();
            }

            //Set message for user
            TempData["SM"] = "You have edited the page.";

            //Redirect user
            return RedirectToAction("EditPage");
        }

        //GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            PageVM model;
            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("The page does not exist.");
                }
                model = new PageVM(dto);
            }
            return View(model);
        }

        //GET: Admin/Pages/DeletePage/id

        public ActionResult DeletePage(int id)
        {

            using (Db db = new Db())
            {
                //Get Page
                PagesDTO dto = db.Pages.Find(id);
                //Delete Page
                db.Pages.Remove(dto);

                //Save changes in database
                db.SaveChanges();
            }
            //Add message about succesfull delete
            TempData["SM"] = "You have deleted a page!";

            //Redirect user to index

            return RedirectToAction("Index");
        }

        //Create sort method
        //GET: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;

                PagesDTO dto;

                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }

        }

        //GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Объявляем модель
            SidebarVM model;
            using (Db db = new Db())
            {
                //Get data from DTO
                SidebarDTO dto = db.Sidebars.Find(1); //Bad code you aren't use жесткие param
                //Fill model
                model = new SidebarVM(dto);
            }
            //return view with model

            return View(model);
        }

        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                //Get data from DTO
                SidebarDTO dto = db.Sidebars.Find(1);//BadCode

                //Set data in Body
                dto.Body = model.Body;

                //Save changes
                db.SaveChanges();
            }
            //Set message to TempData
            TempData["SM"] = "You have edited the sidebar!";
            //Redirect user
            return RedirectToAction("EditSidebar");
        }

    }
}