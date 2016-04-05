using AprimoTaskViewer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AprimoTaskViewer.Models
{
    public class TaskInitializer : DropCreateDatabaseIfModelChanges<TaskContext>
    {
        protected override void Seed(TaskContext context)
        {
            var managers = new List<MarketingManager>
            {
                new MarketingManager {ManagerName = "Annie Phan", MarketingUnit = "Electronics"},
                new MarketingManager {ManagerName = "Cindy Puma", MarketingUnit = "Motors & Giving Works"},
                new MarketingManager {ManagerName = "Joe MacFarland", MarketingUnit = "Frame 5"},
                new MarketingManager {ManagerName = "Leilani Carrara", MarketingUnit = "Live Auction"},
                new MarketingManager {ManagerName = "Sarah Peterson", MarketingUnit = "Fashion" },
            };
            foreach (var temp in managers)
            {
                //loop through and add each Objects in Employees List to 'Employees'
                context.Managers.Add(temp);
            }
            context.SaveChanges();

            var statuses = new List<Status>
            {
                new Status {StatusName = "Waiting for ASSets..."},
                new Status {StatusName = "Assets Ready! Work on it!"},
                new Status {StatusName = "Cancelled"},
                new Status {StatusName = "Running"},
                new Status {StatusName = "Completed" }

            };
            foreach (var temp in statuses)
            {
                //loop through and add each Objects in Status List to 'Statuses'
                context.Statuses.Add(temp);
            }
            context.SaveChanges();

            var history = new List<DateRange>
            {
                new DateRange {DateHistory = "1 Week ago"},
                new DateRange {DateHistory = "2 Weeks ago"},
                new DateRange {DateHistory = "1 Month ago"},
                new DateRange {DateHistory = "3 Months ago"},
            };
            foreach (var temp in history)
            {
                //loop through and add each Objects in Status List to 'DateHistory'
                context.DateHistory.Add(temp);
            }
            context.SaveChanges();

            var NewTasks = new List<TaskDetailNew>
            {
                new TaskDetailNew {TaskName = "Foreverlux Fragrances Fashion", FrameNumber = 2, AprimoNumber = 31413, BeginDate = DateTime.Parse("2015-10-02"), EndDate= DateTime.Parse("2015-10-10"), ManagerId = 5, StatusId = 1},
            };
            foreach (var temp in NewTasks)
            {
                //loop through and add each Objects in NewTasks List to 'NewTasks'
                context.NewTasks.Add(temp);
            }
            context.SaveChanges();

            var OldTasks = new List<TaskDetailOld>
            {
                new TaskDetailOld {TaskName = "David's Bridal Charity Program", FrameNumber = 4, AprimoNumber = 30505, BeginDate = DateTime.Parse("2015-09-25"), EndDate= DateTime.Parse("2015-10-05"), ManagerId = 2, StatusId = 5},
                new TaskDetailOld {TaskName = "Sotheby's Contemporary", FrameNumber = 3, AprimoNumber = 31603, BeginDate = DateTime.Parse("2015-09-23"), EndDate= DateTime.Parse("2015-09-27"), ManagerId = 4, StatusId = 5},
            };
            foreach (var temp in OldTasks)
            {
                //loop through and add each Objects in NewTasks List to 'NewTasks'
                context.OldTasks.Add(temp);
            }
            context.SaveChanges();

            var CancelTasks = new List<TaskDetailCancel>
            {
                new TaskDetailCancel {TaskName = "Generic Live Auction September", FrameNumber = 3, AprimoNumber = 31704, ManagerId = 4, StatusId = 3},
            };
            foreach (var temp in CancelTasks)
            {
                //loop through and add each Objects in NewTasks List to 'NewTasks'
                context.CancelTasks.Add(temp);
            }
            context.SaveChanges();
        }
    }
}