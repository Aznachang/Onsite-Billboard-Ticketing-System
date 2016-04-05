using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AprimoTaskViewer.Models;

namespace AprimoTaskViewer.Controllers
{
    public class MarketingManagerController : Controller
    {
        private TaskContext db = new TaskContext();

        // GET: MarketingManager
        public ActionResult Index()
        {
            return View(db.Managers.ToList());
        }

        // GET: MarketingManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarketingManager marketingManager = db.Managers.Find(id);
            if (marketingManager == null)
            {
                return HttpNotFound();
            }
            return View(marketingManager);
        }

        // GET: MarketingManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MarketingManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ManagerId,ManagerName,MarketingUnit")] MarketingManager marketingManager)
        {
            if (ModelState.IsValid)
            {
                db.Managers.Add(marketingManager);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(marketingManager);
        }

        // GET: MarketingManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarketingManager marketingManager = db.Managers.Find(id);
            if (marketingManager == null)
            {
                return HttpNotFound();
            }
            return View(marketingManager);
        }

        // POST: MarketingManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ManagerId,ManagerName,MarketingUnit")] MarketingManager marketingManager)
        {
            if (ModelState.IsValid)
            {
                db.Entry(marketingManager).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(marketingManager);
        }

        // GET: MarketingManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarketingManager marketingManager = db.Managers.Find(id);
            if (marketingManager == null)
            {
                return HttpNotFound();
            }
            return View(marketingManager);
        }

        // POST: MarketingManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MarketingManager marketingManager = db.Managers.Find(id);
            db.Managers.Remove(marketingManager);
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
    }
}
