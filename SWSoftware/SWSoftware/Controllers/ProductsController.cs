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
        public ActionResult Create([Bind(Include = "ID,ProductName,ProductDescription,Price,SaleID,ImageName")] Product product, HttpPostedFileBase file)
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
        public ActionResult Edit([Bind(Include = "ID,ProductName,ProductDescription,Price,SaleID,ImageName")] Product product)
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

