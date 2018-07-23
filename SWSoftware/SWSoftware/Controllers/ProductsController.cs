using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SWSoftware.DAL;
using SWSoftware.Models;

namespace SWSoftware.Controllers
{
    public class ProductsController : Controller
    {
        private SalesContext db = new SalesContext();

        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        public ActionResult Products()
        {
            var cartItems = db.CartItem.ToList();
            ViewBag.totalItems = 0;
            ViewBag.totalPrice = 0;

            foreach (var item in cartItems)
            {
                var firstSale = db.CartItem.Where(w => w.Product.SaleID == 2 && w.ProductId == item.ProductId).ToList();

                if(firstSale.Count > 0)
                {
                    ViewBag.totalItems = ViewBag.totalItems + (firstSale.Count * 2);
                    foreach (var sale in firstSale)
                        ViewBag.totalPrice = ViewBag.totalPrice + sale.Product.Price;
                }

                var secondSale = db.CartItem.Where(w => w.Product.SaleID == 3 && w.ProductId == item.ProductId).ToList();

                if (secondSale.Count > 0)
                {
                    ViewBag.totalItems = ViewBag.totalItems + secondSale.Count;
                    var teste = Convert.ToInt32(secondSale.Count / 3);

                    if (teste != 0)
                    {
                        ViewBag.totalPrice = teste * 10;
                        var remainder = secondSale.Count % 3;

                        if (remainder != 0)
                        {
                            for (int i = 0; i < remainder; i++)
                                ViewBag.totalPrice = ViewBag.totalPrice + item.Product.Price;
                        }
                    }
                    else
                    {
                        var remainder = secondSale.Count % 3;
                        if (remainder != 0)
                        {
                            for (int i = 0; i < remainder; i++)
                                ViewBag.totalItems = ViewBag.totalItems + item.Product.Price;
                        }
                    }
                }

            }

            return View(cartItems);
        }


        // GET: Products/Details/5
        public ActionResult Details(int? id)
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

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Sales = new SelectList(db.Sales, "ID", "SaleDescription");

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProductName,Price,SaleID,ImageName")] Product product, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (product.SaleID.HasValue)
                    {
                        var sale = db.Sales.Find(product.SaleID);
                        product.SaleDescription = sale.SaleDescription;
                    }

                    db.Products.Add(product);
                    db.SaveChanges();
                    if (file != null)
                        saveUploadedImage(file, product.ID);

                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    ViewBag.CreateStatus = "Falha ao criar produto";
                    return View();
                }
            }

            return View(product);
        }

        [HttpGet]
        public FileContentResult GetThumbnailImage(int? productId)
        {
            ProductImage img = db.ProductImage.FirstOrDefault(p => p.ProductID == productId);

            if (img != null)
                return File(img.ImageThumbnail, img.ImageMimeType.ToString());
            else
                return null;
        }

        [HttpGet]
        public FileContentResult GetActualImage(int? productId)
        {
            ProductImage img = db.ProductImage.FirstOrDefault(p => p.ProductID == productId);

            if (img != null)
                return File(img.ImageData, img.ImageMimeType.ToString());
            else
                return null;
        }


        public ActionResult AddToCart(int id)
        {
            var product = db.Products.Find(id);

            if(product != null)
            {
                var cartItem = new CartItem()
                {
                    ProductId = product.ID,
                    DateCreated = DateTime.Now
                };
                db.CartItem.Add(cartItem);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProductName,Price,SaleID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
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

            return RedirectToAction("Products");
        }

        [HttpGet]
        public ActionResult GetSalesList()
        {
            var sales = db.Sales.ToList();

            return View(sales);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void saveUploadedImage(HttpPostedFileBase image, int productId)
        {
            var img = new ProductImage()
            {
                FileName = image.FileName,
                ImageMimeType = image.ContentLength,
                ImageData = new byte[image.ContentLength],
                ProductID = productId
            };

            image.InputStream.Read(img.ImageData, 0, image.ContentLength);

            var fileName = image.FileName;
            var fileOriginalPath = Server.MapPath("~/Uploads/Originals");
            var fileThumbnailPath = Server.MapPath("~/Uploads/Thumbnails");
            string savedFileName = Path.Combine(fileOriginalPath, fileName);
            image.SaveAs(savedFileName);

            var imageFile = Path.Combine(Server.MapPath("~/Uploads/Originals"), fileName);
            using (var srcImage = Image.FromFile(imageFile))
            using (var newImage = new Bitmap(100, 100))
            using (var graphics = Graphics.FromImage(newImage))
            using (var stream = new MemoryStream())
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(srcImage, new Rectangle(0, 0, 100, 100));
                newImage.Save(stream, ImageFormat.Png);
                var thumbNew = File(stream.ToArray(), "image/png");
                img.ImageThumbnail = thumbNew.FileContents;
            }

            db.ProductImage.Add(img);
            db.SaveChanges();
        }
    }
}

