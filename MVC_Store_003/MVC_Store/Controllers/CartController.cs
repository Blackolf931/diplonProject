using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                model.Quantity = qty;
                model.Price = price;
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

        public ActionResult AddToCartPartial(int id)
        {
            //Declare the list of type CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Declare a model CartVM
            CartVM model = new CartVM();

            using(Db db = new Db())
            {
                //Get Products
                ProductDTO product = db.Products.Find(id);

                //Check a product on stay in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);
                //Add a product in cart
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }

                //If a product is located in cart then add one
                else
                {
                    productInCart.Quantity ++;
                }
            }
            //Get total count of products and add to model
            int qty = 0;
            decimal price = 0m;

            foreach(var el in cart)
            {
                qty += el.Quantity;
                price += el.Quantity * el.Price;
            }

            model.Quantity = qty;
            model.Price = price;
            //Save State cart in session
            Session["cart"] = cart;

            //return own a partialView
            return PartialView("_AddToCartPartial",model);
        }

        //GET: /cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // Declare List<CartVM>
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using(Db db = new Db())
            {
                //Get the model from a List

                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Add quantity
                model.Quantity++;

                //Save need data
                var result = new { qty = model.Quantity, price = model.Price };
                //return JSOn with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: /cart/DecrementProduct
        public JsonResult DecrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Get the model from a List

                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Decrement quantity
                if(model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                //Save need data
                var result = new { qty = model.Quantity, price = model.Price };
                //return JSOn with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public void RemoveProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                cart.Remove(model);
            }
        }

        public ActionResult PaypalPartial()
        {
            // get list of products in cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            //return partial view with list
            return PartialView(cart);
        }

        //POST: /cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            string userName = User.Identity.Name;

            int orderId = 0;

            using(Db db = new Db())
            {
                OrderDTO orderDto = new OrderDTO();

                var q = db.Users.FirstOrDefault(x => x.UserName == userName);

                int userId = q.Id;

                orderDto.UserId = userId;

                orderDto.CreatedAt = DateTime.Now;
                
                db.Orders.Add(orderDto);

                db.SaveChanges();

                orderId = orderDto.OrderId;

                OrderDetailsDTO orderDetailsDto = new OrderDetailsDTO();
                
                foreach(var el in cart)
                {
                    orderDetailsDto.OrderId = orderId;
                    orderDetailsDto.UserId = userId;
                    orderDetailsDto.ProductId = el.ProductId;
                    orderDetailsDto.Quantity = el.Quantity;

                    db.OrderDetails.Add(orderDetailsDto);
                    db.SaveChanges();
                }
            }
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("a22ef42155c7ec", "393e4c06b7ae69"),
                EnableSsl = true
            };
            client.Send("shop@example.com", "admin@example.com", "New Order", $"You have a new order. Order number: {orderId}");
            Session["cart"] = null;
        }
    }
}