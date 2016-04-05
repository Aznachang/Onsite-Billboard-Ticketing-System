using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AprimoTaskViewer.Models
{
    public class TaskContext : DbContext
    {
        //Create Datatables under 'TaskContext' Database
        public DbSet<TaskDetailNew> NewTasks { get; set; }
        public DbSet<TaskDetailOld> OldTasks { get; set; }
        public DbSet<TaskDetailCancel> CancelTasks { get; set; }
        public DbSet<MarketingManager> Managers { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<DateRange> DateHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}