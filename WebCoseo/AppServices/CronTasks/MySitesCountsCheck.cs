using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebCoseo.AppServices.CronTasks;
using System.Threading;

namespace WebCoseo.AppServices.CronTasks
{
    class MySitesCountsCheck
    {
        public static MySitesCountsCheck? _instance;
        bool isRunning = false;
        DateTime TimeNow = DateTime.Now;
        DateTime NextCheck = DateTime.Now;
        public MySitesCountsCheck()
        {
            _instance = this;
        }

        public async void MainTask(int cronid)
        {
            isRunning = true;
            //MainWindow._instance.UpdateWindowStatusLabel("");
            //Views.CronTasks.CronTasksMain._instance.CT_Update_TelegramBot_Status("Running");
            await Task.Run(async () =>
            {
                while (isRunning == true)
                {
                    using CoreDbContext coreContext = new();
                    using (WebcoseoDbContext context = new WebcoseoDbContext())
                    {

                        var getTask = context.CronTasks.Single(ct => ct.Id == cronid);
                        if (getTask.Status.ToString() == "Running")
                        {
                            try
                            {
                                TimeNow = DateTime.Now;
                                if (NextCheck <= TimeNow)
                                {
                                    var getMySites = context.MySites.OrderBy(x => x.Id);
                                    if (getMySites != null)
                                    {
                                        foreach (var mysite in getMySites)
                                        {
                                            mysite.BacklinksCount = context.Backlinks.Count(x => x.MySiteId == mysite.Id);
                                            mysite.PagesCount = context.MySitesPages.Count(x => x.MySiteId == mysite.Id);
                                        }
                                    }
                                    // Do action for all call and do check for latest call then search new
                                    context.SaveChanges();
                                    NextCheck = TimeNow.AddMinutes(10);
                                    
                                }
                                // 15 Minutes = 900000
                                // 10 Minutes = 600000
                                // 5 Minutes = 300000
                                Thread.Sleep(10000);
                            }
                            catch (Exception e)
                            {

                                if (e.Message == "SQLite Error 5: 'database is locked'.")
                                {
                                    isRunning = true;
                                    Thread.Sleep(10000);
                                }
                                else
                                {
                                    isRunning = false;
                                    context.AppLogs.Add(new Models.AppLogs
                                    {
                                        Created = DateTime.Now,
                                        Type = "Error",
                                        Section = "Application",
                                        Message = e.Message.ToString(),

                                    });
                                    context.SaveChanges();
                                }

                            }
                        }
                        if (getTask.Status.ToString() == "Stopped")
                        {
                            isRunning = false;
                            context.AppLogs.Add(new Models.AppLogs
                            {
                                Created = DateTime.Now,
                                Type = "Warning",
                                Section = "Application",
                                Message = "Sucessfully Stopped " + getTask.Name.ToString() + " Cron Task Service",

                            });
                            context.SaveChanges();

                        }
                    }
                }
            });
            //MainWindow._instance.UpdateWindowStatusLabel("");
            //Views.CronTasks.CronTasksMain._instance.CT_Update_TelegramBot_Status("Stopped");
        }
    }
}
