using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using u21669849_HW3.Models;

namespace u21669849_HW3.Controllers
{
    public class HomeController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // HOME PAGE - with page merging for Staff, Customers, and Products
        public async Task<ActionResult> Home(int staffPage = 1, int staffPageSize = 10,
            int customerPage = 1, int customerPageSize = 10,
            int productPage = 1, int productPageSize = 10,
            int? brandFilter = null, int? categoryFilter = null)
        {
            // Filter products by brand and category if specified
            var productsQuery = db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .AsQueryable();

            if (brandFilter.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.brand_id == brandFilter.Value);
            }

            if (categoryFilter.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.category_id == categoryFilter.Value);
            }

            var staff = await db.staffs
                .Include(s => s.stores)
                .OrderBy(s => s.staff_id)
                .Skip((staffPage - 1) * staffPageSize)
                .Take(staffPageSize)
                .ToListAsync();

            var customers = await db.customers
                .OrderBy(c => c.customer_id)
                .Skip((customerPage - 1) * customerPageSize)
                .Take(customerPageSize)
                .ToListAsync();

            var products = await productsQuery
                .OrderBy(p => p.product_id)
                .Skip((productPage - 1) * productPageSize)
                .Take(productPageSize)
                .ToListAsync();

            ViewBag.StaffCurrentPage = staffPage;
            ViewBag.StaffPageSize = staffPageSize;
            ViewBag.StaffTotalPages = (int)Math.Ceiling((double)await db.staffs.CountAsync() / staffPageSize);

            ViewBag.CustomerCurrentPage = customerPage;
            ViewBag.CustomerPageSize = customerPageSize;
            ViewBag.CustomerTotalPages = (int)Math.Ceiling((double)await db.customers.CountAsync() / customerPageSize);

            ViewBag.ProductCurrentPage = productPage;
            ViewBag.ProductPageSize = productPageSize;
            ViewBag.ProductTotalPages = (int)Math.Ceiling((double)await productsQuery.CountAsync() / productPageSize);

            ViewBag.BrandFilter = brandFilter;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.Brands = await db.brands.ToListAsync();
            ViewBag.Categories = await db.categories.ToListAsync();

            var viewModel = new BikeStoreCombined
            {
                Staff = staff,
                Customers = customers,
                Products = products,
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            return View(viewModel);
        }

        // MAINTAIN PAGE - Edit, Update, Delete Staff, Customers, and Products
        public async Task<ActionResult> Maintain(int staffPage = 1, int staffPageSize = 10,
            int customerPage = 1, int customerPageSize = 10,
            int productPage = 1, int productPageSize = 10)
        {
            var staff = await db.staffs
                .Include(s => s.stores)
                .OrderBy(s => s.staff_id)
                .Skip((staffPage - 1) * staffPageSize)
                .Take(staffPageSize)
                .ToListAsync();

            var customers = await db.customers
                .OrderBy(c => c.customer_id)
                .Skip((customerPage - 1) * customerPageSize)
                .Take(customerPageSize)
                .ToListAsync();

            var products = await db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .OrderBy(p => p.product_id)
                .Skip((productPage - 1) * productPageSize)
                .Take(productPageSize)
                .ToListAsync();

            ViewBag.StaffCurrentPage = staffPage;
            ViewBag.StaffPageSize = staffPageSize;
            ViewBag.StaffTotalPages = (int)Math.Ceiling((double)await db.staffs.CountAsync() / staffPageSize);

            ViewBag.CustomerCurrentPage = customerPage;
            ViewBag.CustomerPageSize = customerPageSize;
            ViewBag.CustomerTotalPages = (int)Math.Ceiling((double)await db.customers.CountAsync() / customerPageSize);

            ViewBag.ProductCurrentPage = productPage;
            ViewBag.ProductPageSize = productPageSize;
            ViewBag.ProductTotalPages = (int)Math.Ceiling((double)await db.products.CountAsync() / productPageSize);

            var viewModel = new BikeStoreCombined
            {
                Staff = staff,
                Customers = customers,
                Products = products,
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            return View(viewModel);
        }

        // REPORT PAGE
        public async Task<ActionResult> Report()
        {
            // Popular Products Report - Count products in orders
            var popularProducts = await db.order_items
                .GroupBy(oi => new { oi.product_id, oi.products.product_name })
                .Select(g => new PopularProduct
                {
                    ProductId = g.Key.product_id,
                    ProductName = g.Key.product_name,
                    OrderCount = g.Count(),
                    TotalQuantity = g.Sum(x => x.quantity)
                })
                .OrderByDescending(x => x.OrderCount)
                .ToListAsync();

            var viewModel = new BikeStoreCombined
            {
                Staff = await db.staffs.ToListAsync(),
                Customers = await db.customers.ToListAsync(),
                Products = await db.products.Include(p => p.brands).Include(p => p.categories).ToListAsync(),
                Orders = await db.orders.ToListAsync(),
                OrderItems = await db.order_items.ToListAsync()
            };

            ViewBag.PopularProducts = popularProducts;

            // Document Archive
            var reportDirectory = Server.MapPath("~/Reports");
            if (!Directory.Exists(reportDirectory))
            {
                Directory.CreateDirectory(reportDirectory);
            }

            var files = Directory.GetFiles(reportDirectory)
                                 .Select(file => new FileInfo(file))
                                 .Select(fileInfo => new ReportFile
                                 {
                                     FileName = fileInfo.Name,
                                     FilePath = $"/Reports/{fileInfo.Name}",
                                     FileSize = fileInfo.Length
                                 })
                                 .ToList();

            ViewBag.SavedReports = files;

            var reportFiles = Directory.GetFiles(reportDirectory)
                .Select(Path.GetFileName)
                .ToList();

            ViewBag.ReportFiles = reportFiles;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveReport(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var reportDirectory = Server.MapPath("~/Reports");
                if (!Directory.Exists(reportDirectory))
                {
                    Directory.CreateDirectory(reportDirectory);
                }
                var path = Path.Combine(reportDirectory, Path.GetFileName(file.FileName));
                file.SaveAs(path);
            }
            return RedirectToAction("Report");
        }

        public FileResult DownloadReport(string filename)
        {
            var path = Path.Combine(Server.MapPath("~/Reports"), filename);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        [HttpPost]
        public ActionResult DeleteReport(string filename)
        {
            var path = Path.Combine(Server.MapPath("~/Reports"), filename);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return RedirectToAction("Report");
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