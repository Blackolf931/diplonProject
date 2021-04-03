using MVC_Store.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //Declare a list of type CartVM
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            //Check on null the cart
            if(cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty!";
                return View();
            }
            //Plus all Sum and set Viewbag 
            decimal total = 0m;

            foreach(var el in cart)
            {
                total += el.Total;
            }
            ViewBag.FinishTotal = total; 
            //return list in VM
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //Declare the model CartVM
            CartVM model = new CartVM();

            //Declare a perem for total
            int qty = 0;

            //Declare a perem for price 
            decimal price = 0m;

            //Check a session cart
            if (Session["cart"] != null)
            {
                //Get Sum quantity and price products
                var list = (List<CartVM>)Session["cart"];

                foreach(var el in list)
                {
                    qty += el.Quantity;
                    price += el.Quantity * el.Price;
                }
            }
            else
            {
                //Initialize a cart in default values
                model.Quantity = 0;
                model.Price = 0m;
            }
            //return partial View with model
            return PartialView("_cartPartial", model);
        }

    }
}