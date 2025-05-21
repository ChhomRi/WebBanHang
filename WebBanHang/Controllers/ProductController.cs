using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
using System.IO;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private IWebHostEnvironment _hosting;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment hosting)
        {
            _db = db;
            _hosting = hosting;
        }

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var query = _db.Products.Include(x => x.Category).OrderBy(p => p.Id);

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(products);
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
        public IActionResult Add(Product product, IFormFile ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
                return View(product);
            }

            if (ImageUrl != null)
            {
                product.ImageUrl = SaveImage(ImageUrl);
            }

            _db.Products.Add(product);
            _db.SaveChanges();
           TempData["success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                TempData["error"] = "Không tìm thấy sản phẩm để cập nhật.";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });

            return View(product);
        }

        [HttpPost]
        public IActionResult Update(Product product, IFormFile ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
                return View(product);
            }

            var existingProduct = _db.Products.Find(product.Id);
            if (existingProduct == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            if (ImageUrl != null)
            {
                product.ImageUrl = SaveImage(ImageUrl);

                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_hosting.WebRootPath, existingProduct.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                product.ImageUrl = existingProduct.ImageUrl;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ImageUrl = product.ImageUrl;

            _db.SaveChanges();
            TempData["success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var sp = _db.Products.Find(id);
            if (sp == null)
            {
                TempData["error"] = "Không tìm thấy sản phẩm để xoá.";
                return RedirectToAction("Index");
            }

            return View(sp);
        }

        public IActionResult DeleteConfirmed(int id)
        {
            var sp = _db.Products.Find(id);
            if (sp == null)
            {
                TempData["error"] = "Không tìm thấy sản phẩm để xoá.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(sp.ImageUrl))
            {
                var filePath = Path.Combine(_hosting.WebRootPath, sp.ImageUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _db.Products.Remove(sp);
            _db.SaveChanges();
            TempData["success"] = "Xoá sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        private string SaveImage(IFormFile image)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var path = Path.Combine(_hosting.WebRootPath, "images/products");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }

            return "images/products/" + filename;
        }
    }
}
