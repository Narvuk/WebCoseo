using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.Helpers
{
    public class checkDbCoredata
    {
        public static checkDbCoredata? _instance;
        public checkDbCoredata()
        {
            _instance = this;
        }

        public async Task RunCheck()
        {
            await Task.Run(() =>
            {
                SystemConfig_DB_Check();
            });
            await Task.Run(() =>
            {
                Settings_DB_Check();
            });
            await Task.Run(() =>
            {
                CT_DB_Check();
            });

        }

        public void CT_DB_Check()
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                try { context.CronTasks.Where(s => s.CronKey == "app_update_service").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.CronTasks.Add(new Models.CronTasks
                    {
                        Id = 1,
                        Status = "Stopped",
                        Name = "Application Update Service",
                        CronKey = "app_update_service",
                        DelayTimer = 15000
                    });
                    context.SaveChanges();
                    CT_DB_Check();
                }
                try { context.CronTasks.Where(s => s.CronKey == "backlinks_checker_service").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.CronTasks.Add(new Models.CronTasks
                    {
                        Id = 2,
                        Status = "Stopped",
                        Name = "Backlinks Checker Service",
                        CronKey = "backlinks_checker_service",
                        DelayTimer = 3000
                    });
                    context.SaveChanges();
                    CT_DB_Check();
                }
                try { context.CronTasks.Where(s => s.CronKey == "crawler_crawlpool_service").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.CronTasks.Add(new Models.CronTasks
                    {
                        Id = 3,
                        Status = "Stopped",
                        Name = "Crawl Pool Service",
                        CronKey = "crawler_crawlpool_service",
                        DelayTimer = 3000
                    });
                    context.SaveChanges();
                    CT_DB_Check();
                }
                try { context.CronTasks.Where(s => s.CronKey == "crawler_crawlpages_check_service").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.CronTasks.Add(new Models.CronTasks
                    {
                        Id = 4,
                        Status = "Stopped",
                        Name = "Crawled Pages Check Service",
                        CronKey = "crawler_crawlpages_check_service",
                        DelayTimer = 3000
                    });
                    context.SaveChanges();
                    CT_DB_Check();
                }
                try { context.CronTasks.Where(s => s.CronKey == "reports_service").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.CronTasks.Add(new Models.CronTasks
                    {
                        Id = 5,
                        Status = "Stopped",
                        Name = "Reports Service",
                        CronKey = "reports_service",
                        DelayTimer = 3000
                    });
                    context.SaveChanges();
                    CT_DB_Check();
                }

                try { context.CronTasks.Where(s => s.CronKey == "analysis_htmlchecker").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.CronTasks.Add(new Models.CronTasks
                    {
                        Id = 6,
                        Status = "Stopped",
                        Name = "Analysis Html Checker Service",
                        CronKey = "analysis_htmlchecker",
                        DelayTimer = 3000
                    });
                    context.SaveChanges();
                    CT_DB_Check();
                }
                try { context.CronTasks.Where(s => s.CronKey == "sitemaps_checker").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.CronTasks.Add(new Models.CronTasks
                    {
                        Id = 7,
                        Status = "Stopped",
                        Name = "Sitemaps Checker Service",
                        CronKey = "sitemaps_checker",
                        DelayTimer = 3000
                    });
                    context.SaveChanges();
                    CT_DB_Check();
                }
                try { context.CronTasks.Where(s => s.CronKey == "mysites_counts_checker").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.CronTasks.Add(new Models.CronTasks
                    {
                        Id = 8,
                        Status = "Stopped",
                        Name = "MySites Counts Checker Service",
                        CronKey = "mysites_counts_checker",
                        DelayTimer = 3000
                    });
                    context.SaveChanges();
                    CT_DB_Check();
                }
            }
        }

        public void Settings_DB_Check()
        {
            using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
            {
                try { context.Settings.Where(s => s.SettingKey == "backlinks_next_check").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 1,
                        Name = "Backlinks Next Check",
                        Description = "Time between checks for backlinks in hours from when a backlink was last checked",
                        SettingKey = "backlinks_next_check",
                        SettingValue = "24",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }
                try { context.Settings.Where(s => s.SettingKey == "crawlers_crawlpool_maxcrawlers").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 2,
                        Name = "Crawlers Pool Max Crawlers",
                        Description = "Maximum crawlers to use when when using the crawler pool crawling the web for new and existing websites",
                        SettingKey = "crawlers_crawlpool_maxcrawlers",
                        SettingValue = "5",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }
                try { context.Settings.Where(s => s.SettingKey == "crawlers_crawlpool_maxpersite").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 3,
                        Name = "Max Crawlers Per Site",
                        Description = "Maximum amount of crawlers for 1 websites, to prevent using too many resources crawling just 1 website or getting website timeouts",
                        SettingKey = "crawlers_crawlpool_maxpersite",
                        SettingValue = "2",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }
                try { context.Settings.Where(s => s.SettingKey == "api_pagepeeker").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 4,
                        Name = "PagePeeker API Key",
                        Description = "Api to use pagepeeker website thumbs use setting 'Free' to use the free api but if you have paid use the code given by them",
                        SettingKey = "api_pagepeeker",
                        SettingValue = "Free",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }

                try { context.Settings.Where(s => s.SettingKey == "analysis_maxscanners").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 5,
                        Name = "Analysis Max Scanners",
                        Description = "Max scanners that analysis html code, please note this can be cpu and ram intensive on larger pages",
                        SettingKey = "analysis_maxscanners",
                        SettingValue = "2",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }

                try { context.Settings.Where(s => s.SettingKey == "analysis_maxscanners_persite").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 6,
                        Name = "Analysis Max Scanners Per Website",
                        Description = "Maximum amount of scanners for 1 websites, to prevent using too many resources crawling just 1 website or getting website timeouts",
                        SettingKey = "analysis_maxscanners_persite",
                        SettingValue = "2",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }

                try { context.Settings.Where(s => s.SettingKey == "sitemaps_new_delay").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 7,
                        Name = "New Sitemaps Delay Minutes",
                        Description = "When a new sitemap url is found or added to save the software overworking how many minutes delay for next check",
                        SettingKey = "sitemaps_new_delay",
                        SettingValue = "10",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }

                try { context.Settings.Where(s => s.SettingKey == "sitemaps_next_check_days").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 8,
                        Name = "Sitemaps Next Check",
                        Description = "The days between checks for checking on new urls within the sitemaps for busy sites 2-7 days less busy bit longer",
                        SettingKey = "sitemaps_next_check_days",
                        SettingValue = "7",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }
                try { context.Settings.Where(s => s.SettingKey == "crawledpages_next_check_days").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.Settings.Add(new Models.Settings
                    {
                        Id = 9,
                        Name = "Crawled Pages Next Check",
                        Description = "The days days to wait for the next check for automated adding page back to crawl pool to rescan and analysis, These rechecks get added to priority",
                        SettingKey = "crawledpages_next_check_days",
                        SettingValue = "14",
                    });
                    context.SaveChanges();
                    Settings_DB_Check();
                }
            }
        }


        public void SystemConfig_DB_Check()
        {
            using (AppServices.CoreDbContext context = new AppServices.CoreDbContext())
            {
                try { context.SystemConfig.Where(s => s.ConfigKey == "sys_app_version").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.SystemConfig.Add(new Models.SystemConfig
                    {
                        Id = 1,
                        Name = "Application Version",
                        Description = "Current Running Application Version",
                        ConfigKey = "sys_app_version",
                        ConfigValue = "0.0.1"
                    });
                    context.SaveChanges();
                    SystemConfig_DB_Check();
                }

                try { context.SystemConfig.Where(s => s.ConfigKey == "sys_app_version_check").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.SystemConfig.Add(new Models.SystemConfig
                    {
                        Id = 2,
                        Name = "Current Version Check",
                        Description = "Use this value to check for updates",
                        ConfigKey = "sys_app_version_check",
                        ConfigValue = "000100"
                    });
                    context.SaveChanges();
                    SystemConfig_DB_Check();
                }

                try { context.SystemConfig.Where(s => s.ConfigKey == "sys_app_version_stability").Single().Id.ToString(); }
                catch (Exception)
                {
                    context.SystemConfig.Add(new Models.SystemConfig
                    {
                        Id = 3,
                        Name = "Software Version Stability",
                        Description = "Software Version Stability Such as Alpha, Beta, RC1, Stable",
                        ConfigKey = "sys_app_version_stability",
                        ConfigValue = "Beta"
                    });
                    context.SaveChanges();
                    SystemConfig_DB_Check();
                }
                
            }
        }
    }
}
