using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AprimoTaskViewer.Models
{
    public class DateRange
    {
        [Key]
        public int DateRangeId { get; set; }

        [Display(Name = "Date Range")]
        public string DateHistory { get; set; }
    }
}