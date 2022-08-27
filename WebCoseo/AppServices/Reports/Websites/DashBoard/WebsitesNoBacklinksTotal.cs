using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.Reports.Websites.DashBoard
{
    public class WebsitesNoBacklinksTotal
    {
        public static WebsitesNoBacklinksTotal? _instance;

        public WebsitesNoBacklinksTotal()
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
                        var nblcount = 0;
                        foreach (var site in getWebsites)
                        {
                            var getCount = mainContext.Backlinks.Where(gbl => gbl.WebsiteId == site.Id && gbl.Status == "Active").Count();
                            if (getCount == 0)
                            {
                                nblcount++;
                            }
                        }

                        int noBacklinks = nblcount;

                        var findReport = reportsContext.Websites_Dash_Monthly_NoBacklinks_Total.Where(x => x.DtMonth == monthNow && x.DtYear == yearNow);
                        if (findReport.Count() > 0)
                        {
                            var getReport = reportsContext.Websites_Dash_Monthly_NoBacklinks_Total.Single(x => x.Id == findReport.First().Id);
                            getReport.Amount = noBacklinks;
                            getReport.Updated = dtNow;
                            reportsContext.SaveChanges();
                        }
                        if (findReport.Count() <= 0)
                        {
                            reportsContext.Websites_Dash_Monthly_NoBacklinks_Total.Add(new Models.Reports.Websites.Dashboard.Websites_Dash_Monthly_NoBacklinks_Total
                            {
                                MonthYear = dtNow.ToString("MMM yy"),
                                DtMonth = monthNow,
                                DtYear = yearNow,
                                Created = dtNow,
                                Updated = dtNow,
                                Amount = noBacklinks
                            });
                            reportsContext.SaveChanges();
                        }



                    }
                }
            });
        }
    }
}
