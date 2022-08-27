using System.Threading.Tasks;

namespace WebCoseo.Helpers.Counters
{
    public class BacklinksDbdata
    {
        public static BacklinksDbdata? _instance;
        public BacklinksDbdata()
        {
            _instance = this;
        }

        public async Task RunCheck()
        {
            await Task.Run(() =>
            {
                Backlinks_Counters_Check();
            });
        }


        public void Backlinks_Counters_Check()
        {
            using (AppServices.CountersDbContext counterContext = new AppServices.CountersDbContext())
            {
                /* try { counterContext.CrawlerCounters.Where(s => s.CounterKey == "crawlers_pool").Single().Id.ToString(); }
                 catch (Exception)
                 {
                     counterContext.CrawlerCounters.Add(new Models.Counters.CrawlerCounters
                     {
                         Id = 1,
                         Name = "Crawlers Pool Count",
                         CounterKey = "crawlers_pool",
                         CounterValue = 0
                     });
                     counterContext.SaveChanges();
                     Crawler_Counters_Check();
                 }*/

            }

        }

    }
}
