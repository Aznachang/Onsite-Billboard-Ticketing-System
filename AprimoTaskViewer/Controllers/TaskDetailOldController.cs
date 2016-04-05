using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq.Dynamic;
using System.Globalization;

namespace AprimoTaskViewer.Models
{
    public class TaskDetailOldController : Controller
    {
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

        //Import Excel File For Processing
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile, string statusId)
        {
            //Init. OldTasks and NewTasks
            TaskDetailOld oldTask = new TaskDetailOld();
            //TaskDetailNew newTask = new TaskDetailNew();

 /** Disallow empty/null excel files - send ajax message **/

            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Please select an excel file";
                return View("Index");
            }

            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx") || excelfile.FileName.EndsWith("csv"))
                {    
                    /** Logic for extracting excel file's data by uploading it to the local server disk **/       
                    
                    string path = Server.MapPath("~/Content/" + excelfile.FileName);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    excelfile.SaveAs(path);

                    //Init. Excel objects
                    Excel.Application app;
                    Excel.Workbook workBook;
                    Excel.Worksheet workSheet;
                    Excel.Range range;

                    //Read the data from the excel file 
                    app = new Excel.Application();
                    workBook = app.Workbooks.Open(path);
                    workSheet = (Excel.Worksheet)workBook.ActiveSheet;
                    range = workSheet.UsedRange;

                    /*DateTime - 'Begin' and 'End'*/
                    DateTime BeginDateTime = new DateTime();
                    DateTime EndDateTime = new DateTime();

                    /*String - 'Begin' and 'End'*/
                    string endDate;
                    string beginDate;

                    //Logic for accessing Frame 5 Excel docs (.xls or .xlsx) - see if file type (.xls(x)) & 'B2' cell == 'Frame 5'
                    //Frame 5
                    if ((excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx")) && ((range.Cells[2, 2]).Text).ToString().Contains("Frame 5"))
                    {

                        for (int row = 5; row < range.Rows.Count; row += 9)
                        {
                            //Get Task Name
                            //oldTask.TaskName = int.Parse(((Excel.Range)range.Cells[row, 2]).Text);
                            if (((range.Cells[row, 2]).Text).ToString() != "")
                                oldTask.TaskName = ((range.Cells[row, 2]).Text).ToString();
                            else if (((range.Cells[row++, 2]).Text).ToString() != "")
                                oldTask.TaskName = ((range.Cells[row++, 2]).Text).ToString();
                            else
                                row = 90001;

                            //Get Begin Date
                            beginDate = ((range.Cells[10, 2]).Text).ToString();
                            BeginDateTime = DateTime.Parse(beginDate);
                            oldTask.BeginDate = BeginDateTime;

                            //Get End Date
                            endDate = ((range.Cells[11, 2]).Text).ToString();
                            EndDateTime = DateTime.Parse(endDate);
                            oldTask.EndDate = EndDateTime;

                            //Compares EndDate to Today's Date
                            if (oldTask.EndDate >= DateTime.Today)
                                oldTask.StatusId = 4;     //running
                            if (oldTask.EndDate < DateTime.Today)
                                oldTask.StatusId = 5;     //completed

                            //Get Frame #
                            oldTask.FrameNumber = 5;

                            //Get AprimoNumber --> OldTask Datatable
                            oldTask.AprimoNumber = int.Parse(((range.Cells[1, 2]).Text).ToString());

                            //Get Manager
                            oldTask.ManagerId = 3;      //Frame 5 - Joe MacFarland    

                            db.OldTasks.Add(oldTask);   //add OldTask row record to DataTable!
                            db.SaveChanges();           //after all excel data rows are added - save changes to Database
                        }
                    }
              //Logic for accessing Frame 1-4 Excel docs (.csv)
                    if (excelfile.FileName.EndsWith("csv"))
                    {
                        //starts at campaign name
                        for (int row = 6; row < range.Rows.Count; row += 11)
                        {
                            //check to see if Campaign name is 'null' then proceed
                            if (((range.Cells[row, 3]).Text).ToString() != "")
                            {
                                //Get Task Name - [6,3] 
                                oldTask.TaskName = ((range.Cells[row, 3]).Text).ToString();

                                //Get Begin Date - [12, 3]
                                beginDate = ((range.Cells[row+=6, 3]).Text).ToString() +
                                            ((range.Cells[row, 5]).Text +",").ToString() +  
                                        ((range.Cells[row, 7]).Text).ToString();

                                BeginDateTime = DateTime.ParseExact(beginDate, "MMMd,yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                                oldTask.BeginDate = BeginDateTime;

                                //Get End Date - [16,3]
                                endDate = ((range.Cells[row += 4, 3]).Text).ToString() +
                                            ((range.Cells[row, 5]).Text +",").ToString() +
                                        ((range.Cells[row, 7]).Text).ToString();

                                //EndDateTime
                                EndDateTime = DateTime.ParseExact(endDate, "MMMd,yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                                oldTask.EndDate = EndDateTime;

                                //Compares EndDate to Today's Date
                                if (oldTask.EndDate >= DateTime.Today)
                                    oldTask.StatusId = 4;     //running
                                if (oldTask.EndDate < DateTime.Today)
                                    oldTask.StatusId = 5;     //completed

                                //Get Frame # - row 25 (16+16)
                                oldTask.FrameNumber = int.Parse(((range.Cells[row += 9, 3]).Text).ToString());

                                //Get AprimoNumber --> OldTask Datatable
                                oldTask.AprimoNumber = int.Parse(((range.Cells[8, 3]).Text).ToString());

                                //Get Manager
                                if (oldTask.TaskName.Contains("Motors") || oldTask.TaskName.Contains("eGW"))
                                {
                                    oldTask.ManagerId = 2; //Cindy Puma   
                                }
                                else if (!oldTask.TaskName.Contains("Motors") && !oldTask.TaskName.Contains("eGW"))
                                {
                                    oldTask.ManagerId = 5; //Sarah Peterson
                                }     

                                db.OldTasks.Add(oldTask);   //add OldTask row record to DataTable!
                                db.SaveChanges();           //after all excel data rows are added - save changes to Database
                            } //end of if (campaign name exists)

                            else
                            {
                                row = 9001; //shitty way off breaking off the loop
                            }

                        }   //end of For Loop
                    }   //end of if

                        //Close Workbook and Quit
                        workBook.Close(true, null, null);
                        app.Quit();

                        releaseObject(workSheet);
                        releaseObject(workBook);
                        releaseObject(app);
                    //return View("Index", "TaskDetailOld");     //change this to the TaskDetailController                  
                    //}
                }   //end of right file type: 'csv', 'xls', or 'xlsx'
                else
                {
                    ViewBag.Error = "File type is incorrect. Select file type with .xls or .xlsx";
                    return View("Index");
                }
                ViewBag.Success = "Upload was Successful!";
                return RedirectToAction("Index");
            }   //end of else
        }   //end of if

        /** Garbage Collect - when excel file is finished for reading/querying! **/
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                ViewBag.Error("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        public ActionResult Search(string SearchBox, string dateRangeId)
        {
            var oldtasks = from t in db.OldTasks select t;

            DateRange dr = new DateRange();
            //dr.DateRangeId = int.Parse(dateRangeId);
            ViewBag.DateHistory = new SelectList(db.DateHistory, "DateRangeId", "DateHistory", dr.DateRangeId);

            DateTime week1Ago = DateTime.Today.AddDays(-7);
            DateTime week2Ago = DateTime.Today.AddDays(-14);
            DateTime month1Ago = DateTime.Today.AddMonths(-1);
            DateTime month3Ago = DateTime.Today.AddMonths(-3);

            if (!String.IsNullOrEmpty(SearchBox) && dateRangeId == "")
            {
                DateTime searchDateOld;

                /*See if it is a DateTime format String - boolean
                If true: searchDateNew stores 'SearchBox' date value*/
                bool isDateSearch = DateTime.TryParse(SearchBox, out searchDateOld);

                //DateSearch = true --> find 'Date' entered
                if (isDateSearch)
                {
                    oldtasks = oldtasks.Where(s => s.BeginDate.Equals(searchDateOld)
                                              || s.EndDate.Equals(searchDateOld));
                }

                //execute if not DateTime format
                else
                {
                    int value;
                    if (int.TryParse(SearchBox, out value))
                    {
                        oldtasks = from z in db.OldTasks
                                    where
                                         z.FrameNumber.ToString().Equals(SearchBox)
                                         || z.AprimoNumber.ToString().Equals(SearchBox)
                                    select z;
                        //return View("Index", oldtasks.ToList());
                    }

                    if (!int.TryParse(SearchBox, out value))
                    {
                        oldtasks = from a in db.OldTasks
                                   where
                                       a.TaskName.Contains(SearchBox)
                                       || a.employee.ManagerName.Contains(SearchBox)  //'ManagerName' employee (nav. property)
                                       || a.status.StatusName.Contains(SearchBox)   //'StatusName' status (nav. property)
                                   select a;
                    }
                }
            }

            //Combination of 'SearchBox' and 'DropdownList'
            else if (!String.IsNullOrEmpty(SearchBox) && dateRangeId != "")
            {
                //See if 'SearchBox' input is a 'DateTime' date input
                DateTime searchDateOld2;
                bool isDateSearch = DateTime.TryParse(SearchBox, out searchDateOld2);
                
                //Frame Number/Aprimo Number 
                int value;
                bool isNotDateSearch = int.TryParse(SearchBox, out value);

                dr.DateRangeId = int.Parse(dateRangeId);

                //Query Based on 'Begin Date' or 'End Date'
                if (isDateSearch)
                {
                   if (dr.DateRangeId == 1)
                    {
                        oldtasks = db.OldTasks.Include(d => d.employee).Include(d => d.status)
                                              .Where(d => (d.BeginDate.Equals(searchDateOld2)
                                                       || d.EndDate.Equals(searchDateOld2))
                                                       && d.BeginDate >= week1Ago);
                    }

                    else if (dr.DateRangeId == 2)
                    {
                        oldtasks = db.OldTasks.Include(d => d.employee).Include(d => d.status)
                                               .Where(d => (d.BeginDate.Equals(searchDateOld2)
                                                        || d.EndDate.Equals(searchDateOld2))
                                                        && d.BeginDate >= week2Ago);
                    }
                    else if (dr.DateRangeId == 3)
                    {
                        oldtasks = db.OldTasks.Include(d => d.employee).Include(d => d.status)
                                              .Where(d => (d.BeginDate.Equals(searchDateOld2)
                                                       || d.EndDate.Equals(searchDateOld2))
                                                       && d.BeginDate >= month1Ago);
                    }
                    else if (dr.DateRangeId == 4)
                    {
                        oldtasks = db.OldTasks.Include(d => d.employee).Include(d => d.status)
                                              .Where(d => (d.BeginDate.Equals(searchDateOld2)
                                                       || d.EndDate.Equals(searchDateOld2))
                                                       && d.BeginDate >= month3Ago);
                    }
                }//end

                //Query Based on 'Employye' or 'TaskName' or 'Status'
                else if (!isNotDateSearch)
                {
                    if (dr.DateRangeId == 1)
                    {
                        oldtasks = db.OldTasks.Include(n => n.employee).Include(n => n.status)
                                               .Where(n => (n.TaskName.Contains(SearchBox)
                                                    || n.employee.ManagerName.Contains(SearchBox)
                                                    || n.status.StatusName.Contains(SearchBox))
                                                    && n.BeginDate >= week1Ago);
                    }

                    else if (dr.DateRangeId == 2)
                    {
                        oldtasks = db.OldTasks.Include(n => n.employee).Include(n => n.status)
                                              .Where(n => (n.TaskName.Contains(SearchBox)
                                                   || n.employee.ManagerName.Contains(SearchBox)
                                                   || n.status.StatusName.Contains(SearchBox))
                                                   && n.BeginDate >= week2Ago);
                    }
                    else if (dr.DateRangeId == 3)
                    {
                        oldtasks = db.OldTasks.Include(n => n.employee).Include(n => n.status)
                                              .Where(n => (n.TaskName.Contains(SearchBox)
                                                   || n.employee.ManagerName.Contains(SearchBox)
                                                   || n.status.StatusName.Contains(SearchBox))
                                                   && n.BeginDate >= month1Ago);
                    }
                    else if (dr.DateRangeId == 4)
                    {
                        oldtasks = db.OldTasks.Include(n => n.employee).Include(n => n.status)
                                              .Where(n => (n.TaskName.Contains(SearchBox)
                                                   || n.employee.ManagerName.Contains(SearchBox)
                                                   || n.status.StatusName.Contains(SearchBox))
                                                   && n.BeginDate >= month3Ago);
                    }
                }//end

                //Query Based on 'Frame Number' or 'Aprimo #'
                else if (isNotDateSearch)
                {
                    if (dr.DateRangeId == 1)
                    {
                        oldtasks = db.OldTasks.Include(b => b.employee).Include(b => b.status)
                                               .Where(b => (b.FrameNumber.ToString().Equals(SearchBox)
                                                        || b.AprimoNumber.ToString().Equals(SearchBox))
                                                        && b.BeginDate >= week1Ago);
                    }

                    else if (dr.DateRangeId == 2)
                    {
                        oldtasks = db.OldTasks.Include(b => b.employee).Include(b => b.status)
                                               .Where(b => (b.FrameNumber.ToString().Equals(SearchBox)
                                                        || b.AprimoNumber.ToString().Equals(SearchBox))
                                                        && b.BeginDate >= week2Ago);
                    }
                    else if (dr.DateRangeId == 3)
                    {
                        oldtasks = db.OldTasks.Include(b => b.employee).Include(b => b.status)
                                               .Where(b => (b.FrameNumber.ToString().Equals(SearchBox)
                                                        || b.AprimoNumber.ToString().Equals(SearchBox))
                                                        && b.BeginDate >= month1Ago);
                    }
                    else if (dr.DateRangeId == 4)
                    {
                        oldtasks = db.OldTasks.Include(b => b.employee).Include(b => b.status)
                                                .Where(b => (b.FrameNumber.ToString().Equals(SearchBox)
                                                         || b.AprimoNumber.ToString().Equals(SearchBox))
                                                         && b.BeginDate >= month3Ago);
                    }
                }

            } //end of else if block
            
            //case for Date History
            else if (String.IsNullOrEmpty(SearchBox) && dateRangeId != "")
            {
                dr.DateRangeId = int.Parse(dateRangeId);
                if (dr.DateRangeId == 1)
                {
                    oldtasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).Where(t => t.BeginDate >= week1Ago);
                }

                else if (dr.DateRangeId == 2)
                {
                    oldtasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).Where(t => t.BeginDate >= week2Ago);
                }
                else if (dr.DateRangeId == 3)
                {
                    oldtasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).Where(t => t.BeginDate >= month1Ago);
                }
                else if (dr.DateRangeId == 4)
                {
                    oldtasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).Where(t => t.BeginDate >= month3Ago);
                }

            }

            //Sort the query at the end - in ascending order
            oldtasks = oldtasks.AsQueryable().OrderBy("ManagerId ASC, StatusId ASC, FrameNumber ASC, BeginDate ASC");

            //Return List of Tasks Data Rows Based on:
            //Date (boolean) or Textbox String
            return View("Index", oldtasks.ToList());    //(Task Index Page, list of tasks) - http://localhost:55531/TaskDetailOld/Search
        }

        // GET: TaskDetailOld
        public ActionResult Index(string sortOrder)
        {
            ViewBag.DateHistory = new SelectList(db.DateHistory, "DateRangeId", "DateHistory");

            ViewBag.TaskNameSortParm = String.IsNullOrEmpty(sortOrder) ? "taskname" : "";
            ViewBag.BeginDateSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_date" : "";
            ViewBag.FrameNumberSortParm = String.IsNullOrEmpty(sortOrder) ? "frame" : "";
            ViewBag.AprimoSortParm = String.IsNullOrEmpty(sortOrder) ? "aprimo" : "";
            ViewBag.ManagerSortParm = String.IsNullOrEmpty(sortOrder) ? "manager" : "";

            var oldTasks = from t in db.OldTasks
                           select t;

            switch (sortOrder)
            {
                case "manager":
                    oldTasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).OrderBy(t =>t.employee.ManagerName);
                    break;
                case "begin_date":
                    oldTasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).OrderBy(t => t.BeginDate);
                    break;
                case "taskname":
                    oldTasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).OrderBy(t => t.TaskName);
                    break;
                case "frame":
                    oldTasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).OrderBy(t => t.FrameNumber);
                    break;
                case "aprimo":
                    oldTasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).OrderBy(t => t.AprimoNumber);
                    break;
            
                default:
                    oldTasks = db.OldTasks.Include(t => t.employee).Include(t => t.status).OrderBy("ManagerId ASC, StatusId ASC,FrameNumber ASC,  BeginDate ASC");
                     //oldTasks = oldTasks.AsQueryable().OrderBy("ManagerId ASC, StatusId ASC, BeginDate ASC, FrameNumber ASC");
                    break;
            }
            //By default 
            return View(oldTasks.ToList());
        }

        // GET: TaskDetailOld/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailOld taskDetailOld = db.OldTasks.Find(id);
            if (taskDetailOld == null)
            {
                return HttpNotFound();
            }
            return View(taskDetailOld);
        }

        // GET: TaskDetailOld/Create
        public ActionResult Create()
        {
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName");
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName");
            return View();
        }

        // POST: TaskDetailOld/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskDetailId,BeginDate,EndDate,TaskName,FrameNumber,AprimoNumber,ManagerId,StatusId")] TaskDetailOld taskDetailOld)
        {
            if (ModelState.IsValid)
            {
                db.OldTasks.Add(taskDetailOld);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailOld.ManagerId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailOld.StatusId);
            return View(taskDetailOld);
        }

        // GET: TaskDetailOld/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailOld taskDetailOld = db.OldTasks.Find(id);
            if (taskDetailOld == null)
            {
                return HttpNotFound();
            }
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailOld.ManagerId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailOld.StatusId);

            return View(taskDetailOld);
        }

        // POST: TaskDetailOld/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskDetailId,BeginDate,EndDate,TaskName,FrameNumber,AprimoNumber,ManagerId,StatusId")] TaskDetailOld taskDetailOld)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taskDetailOld).State = EntityState.Modified;
                db.SaveChanges();

                if (taskDetailOld.StatusId == 1 || taskDetailOld.StatusId == 2)
                {
                    TaskDetailNew taskDetailNew = new TaskDetailNew();

                    taskDetailNew.TaskName = taskDetailOld.TaskName;
                    taskDetailNew.FrameNumber = taskDetailOld.FrameNumber;
                    taskDetailNew.AprimoNumber = taskDetailOld.AprimoNumber;
                    taskDetailNew.BeginDate = taskDetailOld.BeginDate;
                    taskDetailNew.EndDate = taskDetailOld.EndDate;
                    taskDetailNew.ManagerId = taskDetailOld.ManagerId;
                    taskDetailNew.StatusId = taskDetailOld.StatusId;
                    taskDetailNew.employee = taskDetailOld.employee;
                    taskDetailNew.status = taskDetailOld.status;

                    db.NewTasks.Add(taskDetailNew);
                    db.OldTasks.Remove(taskDetailOld);
                    db.SaveChanges();
                    //  return View("TaskDetailCancel/Index", taskDetailCancel);
                }
                return RedirectToAction("Index");
            }
            ViewBag.ManagerId = new SelectList(db.Managers, "ManagerId", "ManagerName", taskDetailOld.ManagerId);
            ViewBag.StatusId = new SelectList(db.Statuses, "StatusId", "StatusName", taskDetailOld.StatusId);
            return View(taskDetailOld);
        }

        // GET: TaskDetailOld/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaskDetailOld taskDetailOld = db.OldTasks.Find(id);
            if (taskDetailOld == null)
            {
                return HttpNotFound();
            }
            return View(taskDetailOld);
        }

        // POST: TaskDetailOld/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaskDetailOld taskDetailOld = db.OldTasks.Find(id);
            db.OldTasks.Remove(taskDetailOld);
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
