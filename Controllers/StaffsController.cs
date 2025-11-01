using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using u21669849_HW3.Models;

namespace u21669849_HW3.Controllers
{
    public class staffsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: staffs
        public async Task<ActionResult> Index()
        {
            var staffs = db.staffs.Include(s => s.stores).Include(s => s.staffs1);
            return View(await staffs.ToListAsync());
        }

        // GET: staffs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staffs staffs = await db.staffs.FindAsync(id);
            if (staffs == null)
            {
                return HttpNotFound();
            }
            return View(staffs);
        }

        // GET: staffs/Create
        public ActionResult Create()
        {
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name");
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name");
            return View();
        }

        // POST: staffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "staff_id,first_name,last_name,email,phone,active,store_id,manager_id")] staffs staffs)
        {
            if (ModelState.IsValid)
            {
                db.staffs.Add(staffs);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staffs.store_id);
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staffs.manager_id);
            return View(staffs);
        }

        // GET: staffs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staffs staffs = await db.staffs.FindAsync(id);
            if (staffs == null)
            {
                return HttpNotFound();
            }
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staffs.store_id);
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staffs.manager_id);
            return View(staffs);
        }

        // POST: staffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "staff_id,first_name,last_name,email,phone,active,store_id,manager_id")] staffs staffs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staffs).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.store_id = new SelectList(db.stores, "store_id", "store_name", staffs.store_id);
            ViewBag.manager_id = new SelectList(db.staffs, "staff_id", "first_name", staffs.manager_id);
            return View(staffs);
        }

        // GET: staffs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            staffs staffs = await db.staffs.FindAsync(id);
            if (staffs == null)
            {
                return HttpNotFound();
            }
            return View(staffs);
        }

        // POST: staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            staffs staffs = await db.staffs.FindAsync(id);
            db.staffs.Remove(staffs);
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