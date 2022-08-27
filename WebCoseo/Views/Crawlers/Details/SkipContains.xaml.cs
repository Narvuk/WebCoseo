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

namespace WebCoseo.Views.Crawlers.Details
{
    /// <summary>
    /// Interaction logic for SkipContains.xaml
    /// </summary>
    public partial class SkipContains : UserControl
    {
        public static SkipContains? _instance;
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public SkipContains()
        {
            _instance = this;
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            Get_SkipContains();
        }

        private void ClearSkipContains_List()
        {
            SkipContains_List.ItemsSource = null;
            SkipContains_List.Items.Clear();
        }

        public void Get_SkipContains()
        {
            ClearSkipContains_List();
            SkipContains_SearchBox.Text = "";
            Combobox_ItemsPerPage.Text = pageSize.ToString();
            CurrentPage_Label.Text = pageNumber.ToString();
            using AppServices.CrawlersDbContext crawlersContext = new();
            SkipContains_List.ItemsSource = crawlersContext.SkipContains.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
        }

        private void SkipContains_Search_Click(object sender, RoutedEventArgs e)
        {
            ClearSkipContains_List();
            using AppServices.CrawlersDbContext crawlersContext = new();
            var skipCrawlFound = crawlersContext.SkipContains.Where(l => l.Phrase.ToLower().Contains(SkipContains_SearchBox.Text.ToLower()))
                .Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            SkipContains_List.ItemsSource = skipCrawlFound.ToList();
        }

        private void SkipContains_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearSkipContains_List();
            Get_SkipContains();
        }


        public void Get_Total_Pages()
        {
            using (AppServices.CrawlersDbContext crawlersContext = new AppServices.CrawlersDbContext())
            {
                totalItems = crawlersContext.SkipContains.Count();
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
            Get_SkipContains();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                Get_SkipContains();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            Get_SkipContains();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            Get_SkipContains();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            Get_SkipContains();
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
                    Get_SkipContains();
                }
            }

        }

        public void AddNew_SkipContains_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails._instance.GetView_AddSkipContains();
        }

        public void Remove_SkipContains_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            using AppServices.CrawlersDbContext crawlersContext = new();
            // do check before remove
            var getSkipContains = crawlersContext.SkipContains.Single(x => x.Id == idconv);

            if (MessageBox.Show("Are you sure you wish to remove and allow this url contains to be crawled again?",
                "Confirm Removal of Skipped Contains",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                crawlersContext.Remove(getSkipContains);
                crawlersContext.SaveChanges();
                Get_SkipContains();
            }
            else
            {
                return;
            }


        }
    }
}
