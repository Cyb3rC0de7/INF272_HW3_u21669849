using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using u21669849_HW3.Models;

namespace u21669849_HW3.Controllers
{
    public class StaffController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: Staff
        public async Task<ActionResult> Index()
        {
            var staffs = db.staffs.Include(s => s.stores);
            return View(await staffs.ToListAsync());
        }

        // GET: Staff/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staffs staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // GET: Staff/Create
        public ActionResult Create()
        {
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name");
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name");
            return View();
        }

        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "staff_id,first_name,last_name,email,phone,active,store_id,manager_id")] staffs staff)
        {
            if (ModelState.IsValid)
            {
                db.staffs.Add(staff);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staff.manager_id);
            return View(staff);
        }

        // GET: Staff/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staffs staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staff.manager_id);
            return View(staff);
        }

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "staff_id,first_name,last_name,email,phone,active,store_id,manager_id")] staffs staff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staff).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staff.store_id);
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staff.manager_id);
            return View(staff);
        }

        // GET: Staff/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staffs staff = await db.staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            staffs staff = await db.staffs.FindAsync(id);
            db.staffs.Remove(staff);
            await db.SaveChangesAsync();
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
    }
}