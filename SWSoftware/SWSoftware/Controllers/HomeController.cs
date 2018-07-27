using SWSoftware.DAL;
using SWSoftware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SWSoftware.Controllers
{
    public class HomeController : Controller
    {
        private SalesContext db = new SalesContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ViewUser()
        {
            return View(db.Products.ToList());
        }

        public ActionResult ViewCart()
        {
            var cartItemsIds = db.CartItem.Select(w => w.ProductId).Distinct().ToList();
            ViewBag.totalItems = 0;
            ViewBag.totalPrice = 0;

            foreach (var productId in cartItemsIds)
            {
                var firstSale = db.CartItem.Where(w => w.Product.SaleID == 2 && w.ProductId == productId).ToList();

                if (firstSale.Count > 0)
                {
                    ViewBag.totalItems = ViewBag.totalItems + (firstSale.Count * 2);
                    foreach (var sale in firstSale)
                        ViewBag.totalPrice = ViewBag.totalPrice + sale.Product.Price;
                }

                var secondSale = db.CartItem.Where(w => w.Product.SaleID == 3 && w.ProductId == productId).ToList();

                if (secondSale.Count > 0)
                {
                    ViewBag.totalItems = ViewBag.totalItems + secondSale.Count;
                    var qtyProductsInSale = Convert.ToInt32(secondSale.Count / 3);

                    if (qtyProductsInSale != 0)
                    {
                        ViewBag.totalPrice = ViewBag.totalPrice + (qtyProductsInSale * 10);
                        var remainder = secondSale.Count % 3;

                        if (remainder != 0)
                        {
                            for (int i = 0; i < remainder; i++)
                                ViewBag.totalPrice = ViewBag.totalPrice + db.Products.Find(productId).Price;
                        }
                    }
                    else
                    {
                        var remainder = secondSale.Count % 3;
                        if (remainder != 0)
                        {
                            for (int i = 0; i < remainder; i++)
                                ViewBag.totalPrice = ViewBag.totalPrice + db.Products.Find(productId).Price;
                        }
                    }
                }
            }

            return View(db.CartItem.ToList());
        }

        [HttpPost]
        public ActionResult AddToCart([Bind(Include = "ID,Quantity")] Product product)
        {
            var productInfo = db.Products.Find(product.ID);

            if (productInfo != null)
            {
                for (int i = 0; i < product.Quantity; i++)
                {
                    var cartItem = new CartItem()
                    {
                        ProductId = product.ID,
                        DateCreated = DateTime.Now
                    };

                    db.CartItem.Add(cartItem);
                }
                db.SaveChanges();
            }

            return RedirectToAction("ViewCart");
        }

        public ActionResult DeleteFromCart(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItem.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }

            db.CartItem.Remove(cartItem);
            db.SaveChanges();

            return RedirectToAction("ViewCart");
        }

        public ActionResult ViewDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
    }
}