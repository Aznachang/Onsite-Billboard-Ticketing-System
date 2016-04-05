using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AprimoTaskViewer.Models
{
    public class MarketingManager
    {
        [Key]
        public int ManagerId { get; set; }

        [Display(Name = "Manager Name")]
        public string ManagerName { get; set; }

        [Display(Name = "Marketing Unit")]
        public string MarketingUnit { get; set; }
    }
}