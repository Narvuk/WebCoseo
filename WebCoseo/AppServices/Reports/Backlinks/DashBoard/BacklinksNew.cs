using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.Reports.Backlinks.DashBoard
{
    public class BacklinksNew
    {
        public static BacklinksNew? _instance;

        public BacklinksNew()
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


                        int totalBacklinks = mainContext.Backlinks.Where(x => x.Status == "New").Count();

                        var findReport = reportsContext.BL_Dash_Monthly_New_Total.Where(x => x.DtMonth == monthNow && x.DtYear == yearNow);
                        if (findReport.Count() > 0)
                        {
                            var getReport = reportsContext.BL_Dash_Monthly_New_Total.Single(x => x.Id == findReport.First().Id);
                            getReport.Amount = totalBacklinks;
                            getReport.Updated = dtNow;
                            reportsContext.SaveChanges();
                        }
                        if (findReport.Count() <= 0)
                        {
                            reportsContext.BL_Dash_Monthly_New_Total.Add(new Models.Reports.Backlinks.Dashboard.BL_Dash_Monthly_New_Total
                            {
                                MonthYear = dtNow.ToString("MMM yy"),
                                DtMonth = monthNow,
                                DtYear = yearNow,
                                Created = dtNow,
                                Updated = dtNow,
                                Amount = totalBacklinks,
                            });
                            reportsContext.SaveChanges();
                        }



                    }
                }
            });
        }
    }
}
