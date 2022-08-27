using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WebCoseo.Views.Analysis.Details
{
    /// <summary>
    /// Interaction logic for FoundSites.xaml
    /// </summary>
    public partial class FoundSites : UserControl
    {
        public static FoundSites _instance;
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        string foundStatus = "New";
        public FoundSites()
        {
            _instance = this;
            InitializeComponent();
            LoadInitial();
        }

        public void LoadInitial()
        {
            Get_Total_Pages();
            Get_Found_Sites();
        }

        public void Get_Found_Sites()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                ComboBox_Status.Text = foundStatus;
                Combobox_ItemsPerPage.Text = pageSize.ToString();
                CurrentPage_Label.Text = pageNumber.ToString();
                Found_Sites_List.ItemsSource = crawlersContext.NewSites.Where(x => x.Status == foundStatus).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            }
        }


        public void Get_Total_Pages()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                totalItems = crawlersContext.NewSites.Where(x => x.Status == foundStatus).Count();
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
            Get_Found_Sites();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                Get_Found_Sites();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            Get_Found_Sites();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            Get_Found_Sites();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            Get_Found_Sites();
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
                    Get_Found_Sites();
                }
            }

        }

        public void List_Status_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_Status.SelectedItem;
            foundStatus = selectedItem.Content.ToString();
            Get_Found_Sites();
            Get_Total_Pages();
        }

        public void Remove_FromCrawler_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                if (MessageBox.Show("Confirm Removal of found site this may appear again when doing web crawling and analysis to prevent appearing use Not Used Button?",
                   "Confirm Removal of Site",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var getFoundSite = crawlersContext.NewSites.Single(x => x.Id == idconv);
                    crawlersContext.NewSites.Remove(getFoundSite);
                    crawlersContext.SaveChanges();
                    Get_Found_Sites();
                }
                else
                {
                    return;
                }
            }

        }

        public void NotUsed_FromCrawler_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                if (MessageBox.Show("Confirm that you do not wish to use this website in your websites, crawler and analysis?",
                   "Confirm not using Site",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var getFoundSite = crawlersContext.NewSites.Single(x => x.Id == idconv);

                    getFoundSite.Status = "NotUsed";
                    crawlersContext.SaveChanges();
                    Get_Found_Sites();
                }
                else
                {
                    return;
                }
            }

        }

        public void Import_FromCrawler_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                if (MessageBox.Show("Confirm that you wish to import this website to your websites, future crawling and analysis?",
                   "Confirm importing of site",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    using (AppServices.WebcoseoDbContext context = new())
                    {

                        var getFoundSite = crawlersContext.NewSites.Single(x => x.Id == idconv);
                        var Domain = getFoundSite.Url.Replace("https://", "");

                        context.Websites.Add(new Models.Websites
                        {
                            Name = Domain,
                            Url = getFoundSite.Url,
                            Description = Domain + " has been imported from the analysis found sites",
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        });
                        context.SaveChanges();

                        getFoundSite.Status = "Imported";
                        crawlersContext.SaveChanges();
                        Get_Found_Sites();
                    }
                }
                else
                {
                    return;
                }
            }

        }

        public void LoadView_Website_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                var getWebsite = crawlersContext.NewSites.Single(x => x.Id == idconv);
                FoundSitesBrowser._instance.WebsitesBrowser_LoadPage(getWebsite.Url);
                FoundSitesBrowser._instance.foundSiteId = idconv;
                ViewDetails._instance.GetView_ViewWebsite();
            }


        }

    }
}
