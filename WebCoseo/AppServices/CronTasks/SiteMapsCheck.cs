using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Xml;

namespace WebCoseo.AppServices.CronTasks
{
    public class SiteMapsCheck
    {
        public static SiteMapsCheck? _instance;
        public SiteMapsCheck()
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
                                int getSitemapsDelay = int.Parse(coreContext.Settings.Single(x => x.SettingKey == "sitemaps_new_delay").SettingValue);
                                int getSitemapsNextCheck = int.Parse(coreContext.Settings.Single(x => x.SettingKey == "sitemaps_next_check_days").SettingValue);
                                var getSiteMaps = context.MySitesSiteMaps.Where(x => x.NextRead <= DateTime.Now);
                                if (getSiteMaps != null)
                                {
                                    foreach (var siteMap in getSiteMaps)
                                    {
                                        string sitemapUrl = siteMap.SitemapUrl;
                                        WebClient wc = new WebClient();
                                        wc.Encoding = System.Text.Encoding.UTF8;
                                        string xml = wc.DownloadString(sitemapUrl);

                                        XmlDocument xmlDoc = new XmlDocument();
                                        xmlDoc.LoadXml(xml);
                                        
                                        if (xmlDoc.DocumentElement.Name == "sitemapindex")
                                        {
                                            XmlNodeList xnList = xmlDoc.GetElementsByTagName("sitemap");
                                            foreach (XmlNode xn in xnList)
                                            {
                                                if (xn["loc"] != null)
                                                {
                                                    string getsitemapUrl = xn["loc"].InnerText;
                                                    var findSite = context.MySites.Single(x => x.Id == siteMap.MySiteId);
                                                    var checkSitemap = context.MySitesSiteMaps.Count(x => x.SitemapUrl == getsitemapUrl && x.MySiteId == siteMap.MySiteId);
                                                    if (checkSitemap == 0)
                                                    {
                                                        context.MySitesSiteMaps.Add(new Models.MySitesSiteMaps
                                                        {
                                                            MySiteId = siteMap.MySiteId,
                                                            Status = "Pending",
                                                            SitemapUrl = getsitemapUrl,
                                                            NextRead = DateTime.Now.AddMinutes(getSitemapsDelay),
                                                            Created = DateTime.Now,
                                                            Updated = DateTime.Now
                                                        });

                                                    }
                                                }
                                                
                                            }
                                            siteMap.LastRead = DateTime.Now;
                                            siteMap.NextRead = DateTime.Now.AddDays(getSitemapsNextCheck);
                                            siteMap.IsSitemapIndex = true;
                                            siteMap.Status = "Success";
                                            context.SaveChanges();
                                        }
                                        if (xmlDoc.DocumentElement.Name == "urlset")
                                        {
                                            XmlNodeList xnList = xmlDoc.GetElementsByTagName("url");
                                            foreach (XmlNode xn in xnList)
                                            {
                                                if (xn["loc"] != null)
                                                {
                                                    string getPageUrl = xn["loc"].InnerText;
                                                    var findSite = context.MySites.Single(x => x.Id == siteMap.MySiteId);
                                                    var checkPage = context.MySitesPages.Count(x => x.Url == getPageUrl && x.MySiteId == siteMap.MySiteId);
                                                    if (checkPage == 0)
                                                    {

                                                        context.MySitesPages.Add(new Models.MySitesPages
                                                        {
                                                            MySiteId = siteMap.MySiteId,
                                                            PageName = getPageUrl,
                                                            Url = getPageUrl,
                                                            Description = "Added Via Sitemap",
                                                            BacklinksCount = 0,
                                                            Created = DateTime.Now,
                                                            Updated = DateTime.Now
                                                        });

                                                    }
                                                }
                                            }
                                            siteMap.LastRead = DateTime.Now;
                                            siteMap.NextRead = DateTime.Now.AddDays(getSitemapsNextCheck);
                                            siteMap.IsSitemapIndex = false;
                                            siteMap.Status = "Success";
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                // Do action for all call and do check for latest call then search new

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
