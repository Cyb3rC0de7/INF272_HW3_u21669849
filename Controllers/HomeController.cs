using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using u21669849_HW3.Models;

namespace u21669849_HW3.Controllers
{
    public class HomeController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: Home/Index
        public async Task<ActionResult> Index()
        {
            // Load Staff with related Store
            ViewBag.Staffs = await db.staffs
                .Include(s => s.stores)
                .ToListAsync();

            // Load Customers
            ViewBag.Customers = await db.customers
                .ToListAsync();

            // Load Products with Brand and Category
            ViewBag.Products = await db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .ToListAsync();

            // Load Brands for filter dropdown
            ViewBag.Brands = await db.brands.ToListAsync();

            // Load Categories for filter dropdown
            ViewBag.Categories = await db.categories.ToListAsync();

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}