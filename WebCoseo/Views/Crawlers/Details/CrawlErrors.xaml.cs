using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebCoseo.Views.Crawlers.Details
{
    /// <summary>
    /// Interaction logic for CrawlErrors.xaml
    /// </summary>
    public partial class CrawlErrors : UserControl
    {
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public CrawlErrors()
        {
            InitializeComponent();
            LoadInitial();
        }

        public void LoadInitial()
        {
            Get_Total_Pages();
            Get_Error_Crawls();
        }

        public void Get_Error_Crawls()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                
                Combobox_ItemsPerPage.Text = pageSize.ToString();
                CurrentPage_Label.Text = pageNumber.ToString();
                Error_Crawl_List.ItemsSource = crawlersContext.CrawlPool.Where(x => x.Status == "Error").Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            }
        }

        public void Get_Total_Pages()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                totalItems = crawlersContext.CrawlPool.Where(x => x.Status == "Error").Count();
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
            Get_Error_Crawls();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                Get_Error_Crawls();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            Get_Error_Crawls();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            Get_Error_Crawls();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            Get_Error_Crawls();
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
                    Get_Error_Crawls();
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
                    Get_Error_Crawls();
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

                if (MessageBox.Show("Confirm that you wish to add this page back to waiting to see if it works on next crawl?",
                   "Confirm adding to waiting list",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    var getCrawlPage = crawlersContext.CrawlPool.Single(x => x.Id == idconv);
                    getCrawlPage.Status = "Waiting";
                    crawlersContext.SaveChanges();
                    Get_Error_Crawls();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
