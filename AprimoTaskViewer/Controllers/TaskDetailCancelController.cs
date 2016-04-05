using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace AprimoTaskViewer.Models
{
    public class TaskDetailCancelController : Controller
    {
        private TaskContext db = new TaskContext();

        public ActionResult Search(string SearchBox)
        {
            var canceltasks = from t in db.CancelTasks select t;
            var canceltasks2 = from z in db.CancelTasks select z;

            if (!String.IsNullOrEmpty(SearchBox))
            {
                int value;
                if (int.TryParse(SearchBox, out value))
                {
                    canceltasks2 = from z in db.CancelTasks
                               where
                                    z.FrameNumber.ToString().Equals(SearchBox)
                                    || z.AprimoNumber.ToString().Equals(SearchBox)
                                  //|| z.ManagerId.Equals(SearchBox)
                                  //|| z.StatusId.Equals(SearchBox)
                               select z;
                    return View("Index", canceltasks2.ToList());
                }
                else
                {
                    canceltasks = from t in db.CancelTasks
                               where
                                   t.TaskName.Contains(SearchBox)
                                   || t.employee.ManagerName.Contains(SearchBox)
                                   || t.status.StatusName.Contains(SearchBox)
                               select t;
                }
            }

            //Return List of Tasks Data Rows Based on:
            //Date (boolean) or Textbox String
            canceltasks = canceltasks.AsQueryable().OrderBy("ManagerId ASC, StatusId ASC, FrameNumber ASC");
            return View("Index", canceltasks.ToList());    //(Task Index Page, list of tasks) 
        }

        // GET: TaskDetailCancel
        public ActionResult Index()
        {

            //var canceltasks = from t in db.CancelTasks
            //                  where
            //                      t.status.StatusName.Contains("Cancelled")
            //                  select t;

            var canceltasks = db.CancelTasks.Include(t => t.employee).Include(t => t.status);
            canceltasks = canceltasks.AsQueryable().OrderBy("ManagerId ASC, StatusId ASC, FrameNumber ASC");
            return View(canceltasks.ToList());
        }

        // GET: TaskDetailCancel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailCancel taskDetailCancel = db.CancelTasks.Find(id);
            if (taskDetailCancel == null)
            {
                return HttpNotFound();
            }
            return View(taskDetailCancel);
        }

        // GET: TaskDetailCancel/Create
        public ActionResult Create()
        {
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName");
            return View();
        }

        // POST: TaskDetailCancel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskDetailId,TaskName,FrameNumber,AprimoNumber,ManagerId,StatusId")] TaskDetailCancel taskDetailCancel)
        {
            if (ModelState.IsValid)
            {
                db.CancelTasks.Add(taskDetailCancel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailCancel.ManagerId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailCancel.StatusId);
            return View(taskDetailCancel);
        }

        // GET: TaskDetailCancel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailCancel taskDetailCancel = db.CancelTasks.Find(id);
            if (taskDetailCancel == null)
            {
                return HttpNotFound();
            }
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailCancel.ManagerId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailCancel.StatusId);
            return View(taskDetailCancel);
        }

        // POST: TaskDetailCancel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskDetailId,TaskName,FrameNumber,AprimoNumber,ManagerId,StatusId")] TaskDetailCancel taskDetailCancel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taskDetailCancel).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailCancel.ManagerId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailCancel.StatusId);
            return View(taskDetailCancel);
        }

        // GET: TaskDetailCancel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailCancel taskDetailCancel = db.CancelTasks.Find(id);
            if (taskDetailCancel == null)
            {
                return HttpNotFound();
            }
            return View(taskDetailCancel);
        }

        // POST: TaskDetailCancel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaskDetailCancel taskDetailCancel = db.CancelTasks.Find(id);
            db.CancelTasks.Remove(taskDetailCancel);
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
