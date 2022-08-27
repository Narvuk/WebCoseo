using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.CronTasks
{
    public class BacklinksChecker
    {
        public static BacklinksChecker _instance;

        public BacklinksChecker()
        {
            _instance = this;
        }

        public async void MainTask(int cronid)
        {
            bool isRunning = true;
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
                            var dtnow = DateTime.Now;
                            int nextCheckTimer = int.Parse(coreContext.Settings.Single(x => x.SettingKey == "backlinks_next_check").SettingValue);
                            var BacklinksNextCheck = context.Backlinks.Where(lc => lc.NextCheck <= dtnow).OrderBy(x => x.LastChecked);
                            if (BacklinksNextCheck.Count() > 0)
                            {
                                foreach (var backlink in BacklinksNextCheck)
                                {

                                    // In future allow user to set check time hours in settings
                                    // var getblchecktime

                                    var getBacklink = context.Backlinks.Where(bl => bl.Id == backlink.Id).Single();
                                    var getMySitePage = context.MySitesPages.Where(msp => msp.Id == getBacklink.MySitePageId).Single();
                                    var BacklinkUrl = getBacklink.BacklinkUrl;
                                    var MySitePageUrl = getMySitePage.Url;
                                    HtmlWeb web = new HtmlWeb();

                                    var htmlDoc = await web.LoadFromWebAsync(BacklinkUrl);

                                    // Use below as idea to check if page gives success 200 response
                                    //if (web.StatusCode == HttpStatusCode.OK)
                                    //{}

                                    if (htmlDoc.DocumentNode.SelectSingleNode($"//a[starts-with(@href, '{MySitePageUrl}')]") != null)
                                    {
                                        getBacklink.Status = "Active";
                                    }
                                    else
                                    {
                                        getBacklink.Status = "Lost";
                                    }
                                    getBacklink.LastChecked = dtnow;
                                    getBacklink.NextCheck = dtnow.AddHours(nextCheckTimer);
                                    context.SaveChanges();

                                    /*Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        //Views.HomeMain._instance.UpdateDashboard();
                                        //Views.HomeMain._instance.LastCheckedBacklink(getBacklink.BacklinkUrl, getBacklink.Status);
                                        //MainWindow._instance.UpdateWindowStatusLabel("We are at End has it saved");
                                    });*/

                                    // 15 Minutes = 900000
                                    // 10 Minutes = 600000
                                    // 5 Minutes = 300000
                                    Thread.Sleep(30000);

                                }
                            }
                            else
                            {
                                // if no backlinks are found dont over run the program delay for 5 minutes
                                Thread.Sleep(300000);
                            }

                            }
                            catch (Exception e)
                            {

                                if (e.Message == "SQLite Error 5: 'database is locked'.")
                                {
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