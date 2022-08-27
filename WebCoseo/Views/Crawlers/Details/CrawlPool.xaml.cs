using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace WebCoseo.Views.Crawlers.Details
{
    /// <summary>
    /// Interaction logic for CrawlPool.xaml
    /// </summary>
    public partial class CrawlPool : UserControl
    {
        public static CrawlPool? _instance;
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public CrawlPool()
        {
            _instance = this;
            InitializeComponent();
            LoadInitial();
        }

        public void LoadInitial()
        {
            Get_Total_Pages();
            Get_CrawlPool();
        }

        private void ClearCrawlPool_List()
        {
            CrawlPool_List.ItemsSource = null;
            CrawlPool_List.Items.Clear();
        }

        public void Reset_CrawlPool_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext context = new AppServices.CrawlersDbContext())
            {
                if (MessageBox.Show("Do you really want to clear and delete the Crawl Pool?",
                    "Reset and Clear Crawl Pool",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var commandText = "DELETE FROM CrawlPool";
                    context.Database.ExecuteSqlRaw(commandText);
                    var commandText2 = "UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='CrawlPool'";
                    context.Database.ExecuteSqlRaw(commandText2);
                    context.SaveChanges();

                    ClearCrawlPool_List();
                    Get_CrawlPool();
                }

            }
        }

        public void Get_CrawlPool()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                CrawlPool_SearchBox.Text = "";
                Combobox_ItemsPerPage.Text = pageSize.ToString();
                CurrentPage_Label.Text = pageNumber.ToString();
                CrawlPool_List.ItemsSource = crawlersContext.CrawlPool.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                Get_Total_Pages();

            }
        }

        private void CrawlPool_Search_Click(object sender, RoutedEventArgs e)
        {
            ClearCrawlPool_List();
            using AppServices.CrawlersDbContext crawlersContext = new();
            var skipCrawlFound = crawlersContext.CrawlPool.Where(l => l.Url.ToLower().Contains(CrawlPool_SearchBox.Text.ToLower()));
            CrawlPool_List.ItemsSource = skipCrawlFound.ToList();
        }

        private void CrawlPool_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearCrawlPool_List();
            Get_CrawlPool();
        }

        public void Get_Total_Pages()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                totalItems = crawlersContext.CrawlPool.Count();
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
            Get_CrawlPool();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                Get_CrawlPool();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            Get_CrawlPool();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            Get_CrawlPool();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            Get_CrawlPool();
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
                    Get_CrawlPool();
                }
            }

        }

        public void Delete_CrawlerPage_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                var button = sender as MenuItem;
                var code = button.Tag;
                var id = code.ToString();
                var idconv = int.Parse(id);

                if (MessageBox.Show("Are you sure you wish to remove this page from the crawl pool?",
                   "Confirm Removal of Page",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var getCrawlPage = crawlersContext.CrawlPool.Single(x => x.Id == idconv);

                    crawlersContext.CrawlPool.Remove(getCrawlPage);
                    crawlersContext.SaveChanges();
                    Get_CrawlPool();
                }
                else
                {
                    return;
                }
            }

        }
    }
}
