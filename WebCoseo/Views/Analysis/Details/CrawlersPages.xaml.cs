using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace WebCoseo.Views.Analysis.Details
{
    /// <summary>
    /// Interaction logic for CrawlersPages.xaml
    /// </summary>
    public partial class CrawlersPages : UserControl
    {
        public static CrawlersPages _instance;

        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        string pagesStatus = "Waiting";
        public CrawlersPages()
        {
            _instance = this;
            InitializeComponent();
            LoadInitial();
        }

        public void LoadInitial()
        {
            Get_Total_Pages();
            Get_Pages_Scan();
        }

        private void ClearCrawlerPages_List()
        {
            Pages_Scan_List.ItemsSource = null;
            Pages_Scan_List.Items.Clear();
        }

        public void Reset_CrawlerPages_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext context = new AppServices.CrawlersDbContext())
            {
                if (MessageBox.Show("Do you really want to clear and delete Crawler Pages?",
                    "Reset and Clear Crawler Pages",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var commandText = "DELETE FROM CrawledPages";
                    context.Database.ExecuteSqlRaw(commandText);
                    var commandText2 = "UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='CrawledPages'";
                    context.Database.ExecuteSqlRaw(commandText2);
                    context.SaveChanges();

                    ClearCrawlerPages_List();
                    Get_Pages_Scan();
                }

            }
        }

        public void Get_Pages_Scan()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                // Paginition with skip((page - 1) * pagesize).Take(pagesize) ///
                CrawlerPages_SearchBox.Text = "";
                ComboBox_Status.Text = pagesStatus;
                Combobox_ItemsPerPage.Text = pageSize.ToString();
                CurrentPage_Label.Text = pageNumber.ToString();
                Pages_Scan_List.ItemsSource = crawlersContext.CrawledPages.Where(x => x.Status == pagesStatus).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            }
        }

        private void CrawlerPages_Search_Click(object sender, RoutedEventArgs e)
        {
            ClearCrawlerPages_List();
            using AppServices.CrawlersDbContext crawlersContext = new();
            var skipCrawlFound = crawlersContext.CrawledPages.Where(l => l.Url.ToLower().Contains(CrawlerPages_SearchBox.Text.ToLower()));
            Pages_Scan_List.ItemsSource = skipCrawlFound.ToList();
        }

        private void CrawlerPages_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearCrawlerPages_List();
            Get_Pages_Scan();
        }

        public void Get_Total_Pages()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                totalItems = crawlersContext.CrawledPages.Where(x => x.Status == pagesStatus).Count();
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
            Get_Pages_Scan();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                Get_Pages_Scan();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            Get_Pages_Scan();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            Get_Pages_Scan();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            Get_Pages_Scan();
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
                    Get_Pages_Scan();
                }
            }

        }

        public void List_Status_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_Status.SelectedItem;
            pagesStatus = selectedItem.Content.ToString();
            Get_Pages_Scan();
            Get_Total_Pages();
        }

        public void Delete_CrawlerPage_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                if (MessageBox.Show("Are you sure you wish to remove this page, it could appear again when crawling unless when domain is on skip crawl list?",
                   "Confirm Removal of Page",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var getCrawlPage = crawlersContext.CrawledPages.Single(x => x.Id == idconv);
                   
                    crawlersContext.CrawledPages.Remove(getCrawlPage);
                    crawlersContext.SaveChanges();
                    Get_Pages_Scan();
                }
                else
                {
                    return;
                }
            }

        }

        public void CrawlPool_CrawlerPage_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                if (MessageBox.Show("Are you sure you wish to add this to a waiting list to be crawled again?",
                   "Confirm adding to waiting list",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    var getCrawlPage = crawlersContext.CrawledPages.Single(x => x.Id == idconv);
                    
                    var checkInPool = crawlersContext.CrawlPool.Count(x => x.Url == getCrawlPage.Url);
                    if (checkInPool == 0)
                    {
                        crawlersContext.Add(new Models.Crawlers.CrawlPool
                        {
                            CrawlSitesId = getCrawlPage.CrawlSitesId,
                            Url = getCrawlPage.Url,
                            Status = "Waiting",
                            IsPriority = true,
                            Created = DateTime.Now,
                        });
                        getCrawlPage.Status = "ReCrawling";
                        crawlersContext.SaveChanges();
                        MessageBox.Show("The url has been added to the crawl pool to recheck", "Added To Crawl Pool");
                    }
                    if (checkInPool > 0)
                    {
                        MessageBox.Show("The url is already in the crawl pool waiting to be crawled", "Already In Crawl Pool");
                    }

                    Get_Pages_Scan();
                }
                else
                {
                    return;
                }
            }
        }

        public void Waiting_CrawlerPage_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                if (MessageBox.Show("Are you sure you wish to add this to a waiting list to be scanned?",
                   "Confirm adding to waiting list",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    var getCrawlPage = crawlersContext.CrawledPages.Single(x => x.Id == idconv);
                    getCrawlPage.Status = "Waiting";
                    crawlersContext.SaveChanges();
                    Get_Pages_Scan();
                }
                else
                {
                    return;
                }
            }
        }

        public void Excluded_CrawlerPage_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);
                if (MessageBox.Show("Are you sure you to exclude this page from being crawled?",
                   "Confirm excluding page",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var getCrawlPage = crawlersContext.CrawledPages.Single(x => x.Id == idconv);
                    getCrawlPage.Status = "Excluded";
                    crawlersContext.SaveChanges();
                    Get_Pages_Scan();
                } 
                else
                {
                    return;
                }
            }
        }
    }
}
