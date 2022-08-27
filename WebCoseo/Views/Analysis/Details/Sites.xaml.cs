using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WebCoseo.Views.Analysis.Details
{
    /// <summary>
    /// Interaction logic for Sites.xaml
    /// </summary>
    public partial class Sites : UserControl
    {
        public static Sites _instance;
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        string sitesStatus = "Crawling";
        public Sites()
        {
            _instance = this;
            InitializeComponent();
            LoadInitial();
        }

        public void LoadInitial()
        {
            Get_Total_Pages();
            Get_Approved_Sites();
        }

        public void Get_Approved_Sites()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                ComboBox_Status.Text = sitesStatus;
                Combobox_ItemsPerPage.Text = pageSize.ToString();
                CurrentPage_Label.Text = pageNumber.ToString();
                Approved_Sites_List.ItemsSource = crawlersContext.CrawlSites.Where(x => x.Status == sitesStatus).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            }
        }


        public void Get_Total_Pages()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                totalItems = crawlersContext.CrawlSites.Where(x => x.Status == sitesStatus).Count();
                if (totalItems % pageSize == 0)
                {
                    totalPages = totalItems / pageSize;
                    TotalPages_Label.Text = totalPages.ToString();
                }
                else
                {
                    totalPages = totalItems / pageSize + 1;
                    TotalPages_Label.Text = totalPages.ToString();
                }
            }
        }

        public void NextPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = pageNumber + 1;
            Get_Approved_Sites();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                Get_Approved_Sites();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            Get_Approved_Sites();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            Get_Approved_Sites();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            Get_Approved_Sites();
            Get_Total_Pages();
        }

        private void CurrentPage_Label_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                int numbervalue;
                bool selectionIsNumber = int.TryParse(CurrentPage_Label.Text, out numbervalue);
                if (selectionIsNumber)
                {
                    pageNumber = int.Parse(CurrentPage_Label.Text);
                    Get_Approved_Sites();
                }
            }

        }

        public void List_Status_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_Status.SelectedItem;
            sitesStatus = selectedItem.Content.ToString();
            Get_Approved_Sites();
            Get_Total_Pages();
        }

        public void RemoveSite_FromCrawler_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                if (MessageBox.Show("Are you sure you wish to remove site from crawlers, this will remove everything from Pool and Crawled Pages?",
                   "Confirm Removal of Site",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var getCrawlSite = crawlersContext.CrawlSites.Single(x => x.Id == idconv);
                    var getCrawlPool = crawlersContext.CrawlPool.Where(x => x.CrawlSitesId == idconv);
                    var getCrawlPages = crawlersContext.CrawledPages.Where(x => x.CrawlSitesId == idconv);
                    foreach (var page in getCrawlPages)
                    {
                        crawlersContext.CrawledPages.Remove(page);
                    }
                    foreach (var poolpage in getCrawlPool)
                    {
                        crawlersContext.CrawlPool.Remove(poolpage);
                    }
                    crawlersContext.CrawlSites.Remove(getCrawlSite);
                    crawlersContext.SaveChanges();
                    Get_Approved_Sites();
                }
                else
                {
                    return;
                }
            }

        }

        public void Site_Resume_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);
                var getCrawlSite = crawlersContext.CrawlSites.Single(x => x.Id == idconv);
                getCrawlSite.Status = "Crawling";
                Run_PauseResume(idconv, "Waiting");
                crawlersContext.SaveChanges();
                Get_Approved_Sites();
            }
        }

        public void Site_Pause_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);
                var getCrawlSite = crawlersContext.CrawlSites.Single(x => x.Id == idconv);
                getCrawlSite.Status = "Paused";
                Run_PauseResume(idconv, "Paused");
                crawlersContext.SaveChanges();
                Get_Approved_Sites();
            }
        }


        public void Run_PauseResume(int siteid, string status)
        {
            Task.Run(async() =>
            {
                using AppServices.CrawlersDbContext crawlersContext = new();
                if (status == "Paused")
                {
                    crawlersContext.CrawlPool.Where(x => x.CrawlSitesId == siteid).ToList().ForEach(a => a.Status = "Paused");
                    crawlersContext.CrawledPages.Where(x => x.CrawlSitesId == siteid && x.Status == "Waiting").ToList().ForEach(a => a.Status = "Paused");
                }
                if (status == "Waiting")
                {
                    crawlersContext.CrawlPool.Where(x => x.CrawlSitesId == siteid).ToList().ForEach(a => a.Status = "Waiting");
                    crawlersContext.CrawledPages.Where(x => x.CrawlSitesId == siteid && x.Status == "Paused").ToList().ForEach(a => a.Status = "Waiting");
                }
                await crawlersContext.SaveChangesAsync();
            });
        }


    }
}
