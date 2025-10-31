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

        // GET: Home
        public async Task<ActionResult> Index(int staffPage = 1, int staffPageSize = 10,
                                              int customerPage = 1, int customerPageSize = 10,
                                              int productPage = 1, int productPageSize = 10,
                                              int? brandId = null, int? categoryId = null)
        {
            // Staff pagination
            var staffs = await db.staffs
                .Include(s => s.stores)
                .OrderBy(s => s.staff_id)
                .Skip((staffPage - 1) * staffPageSize)
                .Take(staffPageSize)
                .ToListAsync();

            // Customer pagination
            var customers = await db.customers
                .OrderBy(c => c.customer_id)
                .Skip((customerPage - 1) * customerPageSize)
                .Take(customerPageSize)
                .ToListAsync();

            // Product pagination with filtering
            var productsQuery = db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .AsQueryable();

            if (brandId.HasValue)
                productsQuery = productsQuery.Where(p => p.brand_id == brandId);
            if (categoryId.HasValue)
                productsQuery = productsQuery.Where(p => p.category_id == categoryId);

            var products = await productsQuery
                .OrderBy(p => p.product_id)
                .Skip((productPage - 1) * productPageSize)
                .Take(productPageSize)
                .ToListAsync();

            // ViewBag for pagination and filtering
            ViewBag.StaffCurrentPage = staffPage;
            ViewBag.StaffPageSize = staffPageSize;
            ViewBag.StaffTotalPages = (int)Math.Ceiling((double)await db.staffs.CountAsync() / staffPageSize);

            ViewBag.CustomerCurrentPage = customerPage;
            ViewBag.CustomerPageSize = customerPageSize;
            ViewBag.CustomerTotalPages = (int)Math.Ceiling((double)await db.customers.CountAsync() / customerPageSize);

            ViewBag.ProductCurrentPage = productPage;
            ViewBag.ProductPageSize = productPageSize;
            ViewBag.ProductTotalPages = (int)Math.Ceiling((double)await productsQuery.CountAsync() / productPageSize);

            ViewBag.Brands = new SelectList(await db.brands.ToListAsync(), "brand_id", "brand_name");
            ViewBag.Categories = new SelectList(await db.categories.ToListAsync(), "category_id", "category_name");
            ViewBag.SelectedBrandId = brandId;
            ViewBag.SelectedCategoryId = categoryId;

            var viewModel = new Combined
            {
                Staffs = staffs,
                Customers = customers,
                Products = products,
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            return View(viewModel);
        }

        // GET: Maintain
        public async Task<ActionResult> Maintain(int staffPage = 1, int staffPageSize = 10,
                                                int customerPage = 1, int customerPageSize = 10,
                                                int productPage = 1, int productPageSize = 10)
        {
            var staffs = await db.staffs
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

            // Pagination ViewBag
            ViewBag.StaffCurrentPage = staffPage;
            ViewBag.StaffPageSize = staffPageSize;
            ViewBag.StaffTotalPages = (int)Math.Ceiling((double)await db.staffs.CountAsync() / staffPageSize);

            ViewBag.CustomerCurrentPage = customerPage;
            ViewBag.CustomerPageSize = customerPageSize;
            ViewBag.CustomerTotalPages = (int)Math.Ceiling((double)await db.customers.CountAsync() / customerPageSize);

            ViewBag.ProductCurrentPage = productPage;
            ViewBag.ProductPageSize = productPageSize;
            ViewBag.ProductTotalPages = (int)Math.Ceiling((double)await db.products.CountAsync() / productPageSize);

            var viewModel = new Combined
            {
                Staffs = staffs,
                Customers = customers,
                Products = products
            };

            return View(viewModel);
        }

        // GET: Report
        public async Task<ActionResult> Report()
        {
            // Sales Report Data
            var salesReport = await db.order_items
                .Include(oi => oi.products)
                .Include(oi => oi.orders)
                .Include(oi => oi.orders.customers)
                .Include(oi => oi.orders.staffs)
                .GroupBy(oi => new { oi.products.product_name, oi.orders.customers.first_name, oi.orders.customers.last_name, oi.orders.staffs.first_name, oi.orders.staffs.last_name })
                .Select(g => new SalesReport
                {
                    ProductName = g.Key.product_name,
                    CustomerName = g.Key.first_name + " " + g.Key.last_name,
                    StaffName = g.Key.first_name + " " + g.Key.last_name,
                    TotalSold = g.Sum(oi => oi.quantity),
                    TotalRevenue = g.Sum(oi => oi.quantity * oi.list_price * (1 - oi.discount))
                })
                .ToListAsync();

            // Popular Products Data
            var popularProducts = await db.order_items
                .Include(oi => oi.products)
                .GroupBy(oi => new { oi.products.product_id, oi.products.product_name })
                .Select(g => new PopularProducts
                {
                    ProductId = g.Key.product_id,
                    ProductName = g.Key.product_name,
                    OrderCount = g.Count(),
                    TotalQuantity = g.Sum(oi => oi.quantity)
                })
                .OrderByDescending(p => p.OrderCount)
                .Take(10)
                .ToListAsync();

            ViewBag.SalesReport = salesReport;
            ViewBag.PopularProducts = popularProducts;

            // Archive files
            var reportDirectory = Server.MapPath("~/Reports");
            var files = System.IO.Directory.GetFiles(reportDirectory)
                                 .Select(file => new System.IO.FileInfo(file))
                                 .Select(fileInfo => new ReportFile
                                 {
                                     FileName = fileInfo.Name,
                                     FilePath = $"/Reports/{fileInfo.Name}",
                                     FileSize = fileInfo.Length
                                 })
                                 .ToList();

            ViewBag.SavedReports = files;

            return View();
        }

        [HttpPost]
        public ActionResult SaveReport(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var path = System.IO.Path.Combine(Server.MapPath("~/Reports"), System.IO.Path.GetFileName(file.FileName));
                file.SaveAs(path);
            }
            return RedirectToAction("Report");
        }

        public FileResult DownloadReport(string filename)
        {
            var path = System.IO.Path.Combine(Server.MapPath("~/Reports"), filename);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        [HttpPost]
        public ActionResult DeleteReport(string filename)
        {
            var path = System.IO.Path.Combine(Server.MapPath("~/Reports"), filename);
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