
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Areas.Customer.Models;
using WebBanHang.Models;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int catid = 1)
        {
            var dsLoai = _db.Categories.OrderBy(x => x.DisplayOrder)
               .Select(c => new CategoryViewModel
               {
                   Id = c.Id,
                   Name = c.Name,
                   ToTalProduct = _db.Products.Where(p => p.CategoryId == c.Id).Count()
               })
               .ToList();

            var dsSanPham = _db.Products.Where(p => p.CategoryId == catid).ToList();
            var catName = _db.Categories.Find(catid).Name;
            ViewBag.DSLOAI = dsLoai;
            ViewBag.CATEGORY_NAME = catName;
            return View(dsSanPham);
        }
        public IActionResult LoadProduct(int catid = 1)
        {

            var dsSanPham = _db.Products.Where(p => p.CategoryId == catid).ToList();
            var catName = _db.Categories.Find(catid).Name;
            ViewBag.CATEGORY_NAME = catName;
            return PartialView("_ProductPartial",dsSanPham);
        }
        public IActionResult GetCategory()
        {
            var dsTheLoai = _db.Categories.ToList();
            return PartialView("CategoryPartial", dsTheLoai);
        }
    }
}
