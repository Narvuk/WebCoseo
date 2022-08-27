using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.Reports.MySites.DashBoard
{
    public class MySitesTotal
    {
        public static MySitesTotal? _instance;

        public MySitesTotal()
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


                        int mysitesTotal = mainContext.MySites.Count();

                        var findReport = reportsContext.MySites_Dash_Monthly_Total.Where(x => x.DtMonth == monthNow && x.DtYear == yearNow);
                        if (findReport.Count() > 0)
                        {
                            var getReport = reportsContext.MySites_Dash_Monthly_Total.Single(x => x.Id == findReport.First().Id);
                            getReport.Amount = mysitesTotal;
                            getReport.Updated = dtNow;
                            reportsContext.SaveChanges();
                        }
                        if (findReport.Count() <= 0)
                        {
                            reportsContext.MySites_Dash_Monthly_Total.Add(new Models.Reports.MySites.Dashboard.MySites_Dash_Monthly_Total
                            {
                                MonthYear = dtNow.ToString("MMM yy"),
                                DtMonth = monthNow,
                                DtYear = yearNow,
                                Created = dtNow,
                                Updated = dtNow,
                                Amount = mysitesTotal,
                            });
                            reportsContext.SaveChanges();
                        }



                    }
                }
            });
        }
    }
}
