using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private IWebHostEnvironment hosting;
        private readonly IWebHostEnvironment _hosting;
        public ProductController(ApplicationDbContext db,IWebHostEnvironment hosting)
        {

            _db = db;
            _hosting = hosting;

        }
        public IActionResult Index()
        {
            var dsSanPham = _db.Products.Include(x => x.Category).ToList();
            return View(dsSanPham);
        }
        public IActionResult Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
         public IActionResult DeleteConfirmed(int id)
            {
                var product = _db.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }
                if (!String.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_hosting.WebRootPath, product.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                _db.Products.Remove(product);
                _db.SaveChanges();
                TempData["success"] = "Product deleted success";
                return RedirectToAction("Index");
         }
        public IActionResult Add()
        {
            
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }
        [HttpPost]
        public IActionResult Add( Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null)
                {
                   
                    product.ImageUrl = SaveImage(ImageUrl);
                }
               
                _db.Products.Add(product);
                _db.SaveChanges();
                TempData["success"] = "Product inserted success";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }

        private string SaveImage(IFormFile image)
        {
           
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        
            var path = Path.Combine(_hosting.WebRootPath, @"images/products");
            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }
            return @"images/products/" + filename;
        }
    }
}
