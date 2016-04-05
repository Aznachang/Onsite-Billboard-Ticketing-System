using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AprimoTaskViewer.Models
{
    public class TaskDetail
    {
        [Key]
        public int TaskDetailId { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Display(Name = "Frame #")]
        public int FrameNumber { get; set; }

        [Display(Name = "Aprimo ID")]
        public int AprimoNumber { get; set; }

        [Display(Name = "Manager")]
        public int ManagerId { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }

        //navigational attributes
        public virtual MarketingManager employee { get; set; }
        public virtual Status status { get; set; }
        public virtual DateRange daterange { set; get; }
        //public IEnumerable<SelectListItem> DateRange { get; set; }
    }
}