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
    public class TaskDetailNewController : Controller
    {
        //Get instance and access of the 'TaskContext' database datatables
        private TaskContext db = new TaskContext();

        [HttpPost]
        public ActionResult deleteRow(IEnumerable<int> aprimoRecordDeletebyId)
        {
            //loop through each selected checkbox and pass this 'id' to selected Datatable rows
            foreach (var id in aprimoRecordDeletebyId)
            {
                var aprimo = db.OldTasks.Single(s => s.TaskDetailId == id);

                db.OldTasks.Remove(aprimo);
            }
            db.SaveChanges();   //after all selected check-boxed data rows are deleted - save changes to database
            return RedirectToAction("Index");
        }

        public ActionResult Search(string SearchBox)
        {
            var newtasks = from t in db.NewTasks select t;
            var newtasks2 = from z in db.NewTasks select z;

            DateTime searchDateNew;

            if (!String.IsNullOrEmpty(SearchBox))
            {
                //See if it is a DateTime format String - boolean
                //If true: searchDateNew stores 'SearchBox' date value
                bool isDateSearch = DateTime.TryParse(SearchBox, out searchDateNew);

                //DateSearch = true --> find 'Date' entered
                if (isDateSearch)
                {
                    newtasks = newtasks.Where(s => s.BeginDate.Equals(searchDateNew)
                                              ||s.EndDate.Equals(searchDateNew));
                }

                //execute if not DateTime format
                else
                {
                    int value;
                    if (int.TryParse(SearchBox, out value))
                    {
                        newtasks2 = from z in db.NewTasks
                                   where
                                        z.FrameNumber.ToString().Equals(SearchBox)
                                        || z.AprimoNumber.ToString().Equals(SearchBox)
                                   //|| z.ManagerId.Equals(SearchBox)
                                   //|| z.StatusId.Equals(SearchBox)
                                   select z;
                        return View("Index", newtasks2.ToList());
                    }

                    if (!int.TryParse(SearchBox, out value))
                    {
                        newtasks = from t in db.NewTasks
                                   where
                                       t.TaskName.Contains(SearchBox)
                                       || t.employee.ManagerName.Contains(SearchBox)  //'ManagerName' employee (nav. property)
                                       || t.status.StatusName.Contains(SearchBox)   //'StatusName' status (nav. property)
                                   select t;
                    }
                }
            }

            newtasks = newtasks.AsQueryable().OrderBy("ManagerId ASC, StatusId ASC, BeginDate ASC, FrameNumber ASC");
            //Return List of Tasks Data Rows Based on:
            //Date (boolean) or Textbox String
            return View("Index", newtasks.ToList());    //(Task Index Page, list of tasks) 
        }

        // GET: TaskDetailNew
        //User sees list of 'New Tasks' When clicking on 'New Tasks' Tab Header on the Site
        public ActionResult Index()
        {
            ViewBag.DateHistory = new SelectList(db.DateHistory, "DateRangeId", "DateHistory");
            var newtasks = db.NewTasks.Include(t => t.employee).Include(t => t.status);
            newtasks = newtasks.AsQueryable().OrderBy("ManagerId ASC, StatusId ASC, BeginDate ASC, FrameNumber ASC");
            return View(newtasks.ToList());
        }

        // GET: TaskDetailNew/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailNew taskDetailNew = db.NewTasks.Find(id);
            if (taskDetailNew == null)
            {
                return HttpNotFound();
            }
            return View(taskDetailNew); //Don't need '.ToList()' - only one object/instance of a New Task is being returned back
        }

        // GET: TaskDetailNew/Create
        public ActionResult Create()
        {
            //Calls TaskDetailNewController > Create.cshtml 
            //Enable a DropDownList - Nothing is Selected  (default setting)
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName");    //KEY: ManagerId;  Value: ManagerName
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName");       //KEY: StatusId;  Value: StatusName
            return View();
        }

        // POST: TaskDetailNew/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskDetailId,BeginDate,EndDate,TaskName,FrameNumber,AprimoNumber,ManagerId,StatusId")] TaskDetailNew taskDetailNew)
        {
            if (ModelState.IsValid)
            {
                db.NewTasks.Add(taskDetailNew);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //Save Previous 'ManagerName' and 'Status' selected
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailNew.ManagerId); 
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailNew.StatusId);
            return View(taskDetailNew);
        }

        // GET: TaskDetailNew/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailNew taskDetailNew = db.NewTasks.Find(id);
            if (taskDetailNew == null)
            {
                return HttpNotFound();
            }

            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailNew.ManagerId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailNew.StatusId);
            return View(taskDetailNew);
        }

        // POST: TaskDetailNew/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskDetailId,BeginDate,EndDate,TaskName,FrameNumber,AprimoNumber,ManagerId,StatusId")] TaskDetailNew taskDetailNew)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taskDetailNew).State = EntityState.Modified;
                db.SaveChanges();

                //Move 'Edit' record to 'CancelledTasks' 
                //Cancelled = '3'
                if (taskDetailNew.StatusId == 3)
                {
                    TaskDetailCancel taskDetailCancel = new TaskDetailCancel();

                    taskDetailCancel.TaskName = taskDetailNew.TaskName;
                    taskDetailCancel.FrameNumber = taskDetailNew.FrameNumber;
                    taskDetailCancel.AprimoNumber = taskDetailNew.AprimoNumber;
                    taskDetailCancel.ManagerId = taskDetailNew.ManagerId;
                    taskDetailCancel.StatusId = taskDetailNew.StatusId;
                    taskDetailCancel.employee = taskDetailNew.employee;
                    taskDetailCancel.status = taskDetailNew.status;

                    db.CancelTasks.Add(taskDetailCancel);
                    db.NewTasks.Remove(taskDetailNew);
                    db.SaveChanges();
                  //  return View("TaskDetailCancel/Index", taskDetailCancel);
                }

                //Move 'Edit' record to 'OldTasks' 
                //Running = '4'; Completed = '5'
                if (taskDetailNew.StatusId == 4 || taskDetailNew.StatusId == 5)
                {
                    TaskDetailOld taskDetailOld = new TaskDetailOld();

                    taskDetailOld.TaskName = taskDetailNew.TaskName;
                    taskDetailOld.FrameNumber = taskDetailNew.FrameNumber;
                    taskDetailOld.AprimoNumber = taskDetailNew.AprimoNumber;
                    taskDetailOld.BeginDate = taskDetailNew.BeginDate;
                    taskDetailOld.EndDate = taskDetailNew.EndDate;
                    taskDetailOld.ManagerId = taskDetailNew.ManagerId;
                    taskDetailOld.StatusId = taskDetailNew.StatusId;
                    taskDetailOld.employee = taskDetailNew.employee;
                    taskDetailOld.status = taskDetailNew.status;

                    db.OldTasks.Add(taskDetailOld);
                    db.NewTasks.Remove(taskDetailNew);
                    db.SaveChanges();
                    //return View("TaskDetailOld/Index", taskDetailOld);
                }
                return RedirectToAction("Index");
            }
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailNew.ManagerId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailNew.StatusId);
            return View(taskDetailNew);
        }

        // GET: TaskDetailNew/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailNew taskDetailNew = db.NewTasks.Find(id);
            if (taskDetailNew == null)
            {
                return HttpNotFound();
            }
            return View(taskDetailNew);
        }

        // POST: TaskDetailNew/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaskDetailNew taskDetailNew = db.NewTasks.Find(id);
            db.NewTasks.Remove(taskDetailNew);
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
