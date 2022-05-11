using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Account;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_Store.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        //GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //checked model on Valid
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            //Compare password
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                //Check username on unique
                if (db.Users.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", $"Username {model.UserName} is taken");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }
                //Create copy class UserDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    AddresToDelivery = model.AddressToDelivery,
                    PhoneNumber = model.PhoneNumber,
                    EmailAdress = model.EmailAddress,
                    UserName = model.UserName,
                    Password = model.Password
                };
                //Fill copy class
                db.Users.Add(userDTO);

                //Save data
                db.SaveChanges();

                //Add role in database
                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };
                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }
            //Set message for tempdata
            TempData["SM"] = "You are now registered and can login.";

            //Redirect user
            return RedirectToAction("Login");
        }

        //GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            //Coniform that user no auntification

            if(!string.IsNullOrEmpty(User.Identity.Name))
                return RedirectToAction("user-profile");

            return View();
        }

        //POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //Check model on Valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Check user on valid
            bool isvalid = false;
            using(Db db = new Db())
            {
                if(db.Users.Any(x => x.UserName.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isvalid = true;
                }
                if (!isvalid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }

        //GET: /account/logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        [Authorize]
        public ActionResult UserNavPartial()
        {
            //Get name user
            string userName = User.Identity.Name;

            //Declare the model
            UserNavPartialVM model;

            using (Db db = new Db())
            {
                //Get user
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                //Fill the model from context 
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            //return PartialView
            return PartialView(model);
        }
        //GET: /account/user-profile
        
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            // Get user name
            string userName = User.Identity.Name;
            //Declare model
            UserProfileVM model;
            using (Db db = new Db())
            {
                //Get user
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);
                //Initialize model with data
                model = new UserProfileVM(dto);
               
            }
            return View("UserProfile",model);
        }

        //POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameISChanged = false;

            //Check model on Valid
            if (!ModelState.IsValid)
            {
                return View("UserProfile",model);
            }
            //Check password if user change password
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View("UserProfile", model);
                }
            }
            using(Db db = new Db())
            {
                //Get name user 
                string userName = User.Identity.Name;
                if (userName != model.UserName)
                {
                    userName = model.UserName;
                    userNameISChanged = true;
                }
                //Check name on Unique
                if(db.Users.Where(x => x.Id != model.Id).Any(x => x.UserName == userName))
                {
                    ModelState.AddModelError("", $"Username {model.UserName} already exists.");
                    model.UserName = "";
                    return View("UserProfile", model);
                }
                //Change context model
                UserDTO dto = db.Users.Find(model.Id);
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAddress;
                dto.UserName = model.UserName;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }
                //Save changes
                db.SaveChanges();
            }
            //Set SM
            TempData["SM"] = "You have edited you profile!";

            if (!userNameISChanged)
            {
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }

        [Authorize(Roles = "User")]
        public ActionResult Orders()
        {
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();
            using(Db db = new Db())
            {
                UserDTO user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

                int userId = user.Id;

                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId).ToArray().Select(x => new OrderVM(x)).ToList();

                foreach(var order in orders)
                {
                    Dictionary<string, int> productAndQty = new Dictionary<string, int>();

                    decimal total = 0m;

                    List<OrderDetailsDTO> orderDetailsDto = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    foreach(var orderDetails in orderDetailsDto)
                    {
                        ProductDTO product = db.Products.FirstOrDefault(x => x.Id == orderDetails.ProductId);

                        decimal price = product.Price;

                        string productName = product.Name;

                        productAndQty.Add(productName, orderDetails.Quantity);

                        total += orderDetails.Quantity * price;
                    }
                    ordersForUser.Add(new OrdersForUserVM() { 
                    
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQuantity = productAndQty,
                        CreatedAt = order.CreatedAt
                    });
                }
}
            return View(ordersForUser);
        }
    }
}