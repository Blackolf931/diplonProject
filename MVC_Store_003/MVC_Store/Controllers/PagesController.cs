using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index{page}
        public ActionResult Index(string page = "")
        {
            // Getting or Setting short slug
            if (page == "")
            {
                page = "home";
            }
            //Declare model and class DTO
            PageVM model;
            PagesDTO dto;
            // Проверяем доступна ли страница
            using(Db db = new Db())
            {
                if(!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    TempData["SM"] = "Page not found";
                    return RedirectToAction("Index", new { page = "" });
                }
            }
            //Get a DTO page
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }
            //Set a title page
            ViewBag.PageTitle = dto.Title;
            //Check sidebar on get
            if(dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }
            //Fill model data
            model = new PageVM(dto);

            return View(model);
        }

        public ActionResult PagesManuPartial()
        {
            //Initialize List PageVm
            List<PageVM> pageVMList;
            //Get all pages without home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home")
                    .Select(x => new PageVM(x)).ToList();
            }
            //return частичное представление и List with data
            return PartialView("_PagesManuPartial", pageVMList);
        }
    }
}