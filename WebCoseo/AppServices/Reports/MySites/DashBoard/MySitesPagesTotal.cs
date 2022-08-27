using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.Reports.MySites.DashBoard
{
    public class MySitesPagesTotal
    {
        public static MySitesPagesTotal? _instance;

        public MySitesPagesTotal()
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


                        int mySitesPagesTotal = mainContext.MySitesPages.Count();

                        var findReport = reportsContext.MySitesPages_Dash_Monthly_Total.Where(x => x.DtMonth == monthNow && x.DtYear == yearNow);
                        if (findReport.Count() > 0)
                        {
                            var getReport = reportsContext.MySitesPages_Dash_Monthly_Total.Single(x => x.Id == findReport.First().Id);
                            getReport.Amount = mySitesPagesTotal;
                            getReport.Updated = dtNow;
                            reportsContext.SaveChanges();
                        }
                        if (findReport.Count() <= 0)
                        {
                            reportsContext.MySitesPages_Dash_Monthly_Total.Add(new Models.Reports.MySites.Dashboard.MySitesPages_Dash_Monthly_Total
                            {
                                MonthYear = dtNow.ToString("MMM yy"),
                                DtMonth = monthNow,
                                DtYear = yearNow,
                                Created = dtNow,
                                Updated = dtNow,
                                Amount = mySitesPagesTotal,
                            });
                            reportsContext.SaveChanges();
                        }



                    }
                }
            });
        }
    }
}
