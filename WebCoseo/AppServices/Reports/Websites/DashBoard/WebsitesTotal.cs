using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.Reports.Websites.DashBoard
{
    public class WebsitesTotal
    {
        public static WebsitesTotal? _instance;

        public WebsitesTotal()
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


                        int totalWebsites = mainContext.Websites.Count();

                        var findReport = reportsContext.Websites_Dash_Monthly_Total.Where(x => x.DtMonth == monthNow && x.DtYear == yearNow);
                        if (findReport.Count() > 0)
                        {
                            var getReport = reportsContext.Websites_Dash_Monthly_Total.Single(x => x.Id == findReport.First().Id);
                            getReport.Amount = totalWebsites;
                            getReport.Updated = dtNow;
                            reportsContext.SaveChanges();
                        }
                        if (findReport.Count() <= 0)
                        {
                            reportsContext.Websites_Dash_Monthly_Total.Add(new Models.Reports.Websites.Dashboard.Websites_Dash_Monthly_Total
                            {
                                MonthYear = dtNow.ToString("MMM yy"),
                                DtMonth = monthNow,
                                DtYear = yearNow,
                                Created = dtNow,
                                Updated = dtNow,
                                Amount = totalWebsites
                            });
                            reportsContext.SaveChanges();
                        }



                    }
                }
            });
        }
    }
}
