using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace WebCoseo.Views.MySites.Details
{
    /// <summary>
    /// Interaction logic for SiteMaps.xaml
    /// </summary>
    public partial class SiteMaps : UserControl
    {
        public static SiteMaps? _instance;
        public int GetMySiteId { set; get; }
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public SiteMaps()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int MySiteId)
        {
            GetMySiteId = MySiteId;
            GetSiteMaps();

        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            GetSiteMaps();
            Get_Total_Pages();
        }

        public void AddSiteMap_Click(object sender, RoutedEventArgs e)
        {
            ViewDetails._instance.GetView_AddSiteMap();
        }

        private void ClearSiteMaps_List()
        {
            SiteMaps_List.ItemsSource = null;
            SiteMaps_List.Items.Clear();
        }

        public void GetSiteMaps()
        {
            ClearSiteMaps_List();
            SiteMaps_SearchBox.Text = "";
            Combobox_ItemsPerPage.Text = pageSize.ToString();
            CurrentPage_Label.Text = pageNumber.ToString();
            using AppServices.WebcoseoDbContext context = new();
            SiteMaps_List.ItemsSource = context.MySitesSiteMaps.Where(x => x.MySiteId == GetMySiteId).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
        }

        private void SiteMaps_Search_Click(object sender, RoutedEventArgs e)
        {
            ClearSiteMaps_List();
            using AppServices.WebcoseoDbContext context = new();
            var mysitesfound = context.MySitesSiteMaps.Where(l => l.SitemapUrl.ToLower().Contains(SiteMaps_SearchBox.Text.ToLower()));
            SiteMaps_List.ItemsSource = mysitesfound.ToList();
        }

        private void SiteMaps_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearSiteMaps_List();
            GetSiteMaps();
        }

        private async void DeleteSiteMap_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            using AppServices.WebcoseoDbContext context = new();
            // do check before remove
            //count my sites pages
            if (MessageBox.Show("Are you sure you wish to remove this sitemap?",
                       "Confirm Removal of Sitemap",
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var getSiteMap = context.MySitesSiteMaps.Where(s => s.Id == idconv).Single();
                context.MySitesSiteMaps.Remove(getSiteMap);

                context.SaveChanges();
                GetSiteMaps();
            }
        }


        private async void SiteMapCheckNow_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            using AppServices.WebcoseoDbContext context = new();
            // do check before remove
            //count my sites pages

            var getSiteMap = context.MySitesSiteMaps.Where(s => s.Id == idconv).Single();
            getSiteMap.NextRead = DateTime.Now.AddMinutes(1);
            getSiteMap.Status = "Pending";

            context.SaveChanges();
            GetSiteMaps();
        }


        public void Get_Total_Pages()
        {
            using (AppServices.WebcoseoDbContext context = new())
            {
                totalItems = context.MySitesSiteMaps.Where(x => x.MySiteId == GetMySiteId).Count();
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
            GetSiteMaps();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                GetSiteMaps();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            GetSiteMaps();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            GetSiteMaps();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            GetSiteMaps();
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
                    GetSiteMaps();
                }
            }

        }
    }
}
