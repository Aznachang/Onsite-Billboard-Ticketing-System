using AprimoTaskViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace AprimoTaskViewer.Controllers
{
    public class HomeController : Controller
    {
        private TaskContext db = new TaskContext();

        public ActionResult Index()
        {
            return View();
        }

    }   //end of Home Controller
}