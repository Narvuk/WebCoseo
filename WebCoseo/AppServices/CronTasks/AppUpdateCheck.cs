using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WebCoseo.AppServices.CronTasks
{
    public class AppUpdateCheck
    {
        public static AppUpdateCheck? _instance;
        bool isRunning = false;
        DateTime TimeNow = DateTime.Now;
        DateTime NextCheck = DateTime.Now;
        public AppUpdateCheck()
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
                                    using (var httpClient = new HttpClient())
                                    {
                                        var json = await httpClient.GetStringAsync("https://updates.narvuk.com/webcoseo.json");
                                        //var data = await json.Content.ReadAsStringAsync();
                                        dynamic data = JsonConvert.DeserializeObject(json);
                                        int cv = data.currentversion;
                                        int systemversion = int.Parse(coreContext.SystemConfig.Where(cv => cv.ConfigKey == "sys_app_version_check").Single().ConfigValue);


                                        if (cv > systemversion)
                                        {
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                MainWindow._instance.IsUpdateAvailable("new");
                                            });
                                        }
                                        if (cv <= systemversion)
                                        {
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                MainWindow._instance.IsUpdateAvailable("current");
                                            });
                                        }

                                    }
                                    NextCheck = TimeNow.AddMinutes(30);
                                }
                                // Do action for all call and do check for latest call then search new

                                // 15 Minutes = 900000
                                // 10 Minutes = 600000
                                // 5 Minutes = 300000
                                Thread.Sleep(10000);
                            }
                            catch (Exception e)
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
