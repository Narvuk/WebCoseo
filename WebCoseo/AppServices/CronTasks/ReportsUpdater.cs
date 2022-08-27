using System.Threading;
using System.Threading.Tasks;

namespace WebCoseo.AppServices.CronTasks
{
    public class ReportsUpdater
    {
        public static ReportsUpdater? _instance;

        public bool isRunning { get; set; }
        public ReportsUpdater()
        {
            _instance = this;
        }

        public async void MainTask(int cronid)
        {
            isRunning = true;
            await Task.Run(async () =>
            {
                while (isRunning == true)
                {
                    // Backlinks Reports
                    await Task.Run(async () =>
                    {
                        // Total
                        var startBacklinksTotal = new Reports.Backlinks.Websites.BacklinksTotal();
                        await startBacklinksTotal.MainTask();
                        var startDash_BacklinksTotal = new Reports.Backlinks.DashBoard.BacklinksTotal();
                        await startDash_BacklinksTotal.MainTask();
                        // Active
                        var startBacklinksActive = new Reports.Backlinks.Websites.BacklinksActive();
                        await startBacklinksActive.MainTask();
                        var startDash_BacklinksActive = new Reports.Backlinks.DashBoard.BacklinksActive();
                        await startDash_BacklinksActive.MainTask();
                        // New
                        var startBacklinksNew = new Reports.Backlinks.Websites.BacklinksNew();
                        await startBacklinksNew.MainTask();
                        var startDash_BacklinksNew = new Reports.Backlinks.DashBoard.BacklinksNew();
                        await startDash_BacklinksNew.MainTask();
                        // Lost
                        var startBacklinksLost = new Reports.Backlinks.Websites.BacklinksLost();
                        await startBacklinksLost.MainTask();
                        var startDash_BacklinksLost = new Reports.Backlinks.DashBoard.BacklinksLost();
                        await startDash_BacklinksLost.MainTask();
                    });
                    // MySites
                    await Task.Run(async () =>
                    {
                        var startDash_MySitesTotal = new Reports.MySites.DashBoard.MySitesTotal();
                        await startDash_MySitesTotal.MainTask();
                        var startDash_MySitesPages_Total = new Reports.MySites.DashBoard.MySitesPagesTotal();
                        await startDash_MySitesPages_Total.MainTask();

                    });
                    // Websites
                    await Task.Run(async () =>
                    {
                        var startDash_WebsitesTotal = new Reports.Websites.DashBoard.WebsitesTotal();
                        await startDash_WebsitesTotal.MainTask();
                        var startDash_WebsitesNoBacklinksTotal = new Reports.Websites.DashBoard.WebsitesNoBacklinksTotal();
                        await startDash_WebsitesNoBacklinksTotal.MainTask();
                        // as no reports coded yet sleep so does not run away with resources
                        Thread.Sleep(10000);
                    });
                }
            });
        }

        public void StopReportsUpdater()
        {
            isRunning = false;
        }
    }
}
