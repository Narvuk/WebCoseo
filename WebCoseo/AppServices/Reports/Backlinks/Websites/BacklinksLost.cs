using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.Reports.Backlinks.Websites
{
    public class BacklinksLost
    {
        public static BacklinksLost? _instance;

        public BacklinksLost()
        {
            _instance = this;
        }

        public async Task MainTask()
        {
            await Task.Run(() =>
            {
                using (ReportsDbContext reportsContext = new ReportsDbContext())
                {
                    using (WebcoseoDbContext mainContext = new WebcoseoDbContext())
                    {
                        var dtNow = DateTime.Now;
                        int monthNow = DateTime.Now.Month;
                        int yearNow = DateTime.Now.Year;

                        var getWebsites = mainContext.Websites.ToList();

                        foreach (var site in getWebsites)
                        {
                            int siteId = site.Id;
                            int totalBacklinks = mainContext.Backlinks.Where(s => s.WebsiteId == siteId && s.Status == "Lost").Count();

                            var findReport = reportsContext.BL_Websites_Monthly_Lost.Where(x => x.WebsiteSiteId == site.Id && x.DtMonth == monthNow && x.DtYear == yearNow);
                            if (findReport.Count() > 0)
                            {
                                var getReport = reportsContext.BL_Websites_Monthly_Lost.Single(x => x.Id == findReport.First().Id);
                                getReport.Amount = totalBacklinks;
                                getReport.Updated = dtNow;
                                reportsContext.SaveChanges();
                            }
                            if (findReport.Count() <= 0)
                            {
                                reportsContext.BL_Websites_Monthly_Lost.Add(new Models.Reports.Backlinks.Websites.BL_Websites_Monthly_Lost
                                {
                                    WebsiteSiteId = site.Id,
                                    MonthYear = dtNow.ToString("MMM yy"),
                                    DtMonth = monthNow,
                                    DtYear = yearNow,
                                    Created = dtNow,
                                    Updated = dtNow,
                                    Amount = totalBacklinks

                                });
                                reportsContext.SaveChanges();
                            }


                        }
                    }
                }
            });
        }
    }
}
