using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebCoseo.Views.MySites.Actions;
using System.Windows.Input;

namespace WebCoseo.Views.MySites.Details
{
    /// <summary>
    /// Interaction logic for SitePages.xaml
    /// </summary>
    public partial class SitePages : UserControl
    {
        public static SitePages? _instance;
        public int GetMySiteId { set; get; }
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public SitePages()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int MySiteId)
        {
            GetMySiteId = MySiteId;
            GetSitePages();

        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            GetSitePages();
            Get_Total_Pages();
        }

        private void ClearSitePages_List()
        {
            SitePages_List.ItemsSource = null;
            SitePages_List.Items.Clear();
        }

        public void GetSitePages()
        {
            ClearSitePages_List();
            SitePages_SearchBox.Text = "";
            Combobox_ItemsPerPage.Text = pageSize.ToString();
            CurrentPage_Label.Text = pageNumber.ToString();
            using AppServices.WebcoseoDbContext context = new();
            SitePages_List.ItemsSource = context.MySitesPages.Where(x => x.MySiteId == GetMySiteId).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
        }

        private void SitePages_Search_Click(object sender, RoutedEventArgs e)
        {
            ClearSitePages_List();
            using AppServices.WebcoseoDbContext context = new();
            var mysitesfound = context.MySitesPages.Where(l => l.PageName.ToLower().Contains(SitePages_SearchBox.Text.ToLower()));
            SitePages_List.ItemsSource = mysitesfound.ToList();
        }

        private void SitePages_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearSitePages_List();
            GetSitePages();
        }

        private void EditSitePage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            EditSitePage._instance.LoadDetails(idconv);
            ViewDetails._instance.GetView_EditSitePage();
        }

        private async void DeleteMySitePage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            using AppServices.WebcoseoDbContext context = new();
            // do check before remove
            //count my sites pages
            var getSite = context.MySites.Single(sp => sp.Id == GetMySiteId);
            var countbl = context.Backlinks.Where(bl => bl.MySitePageId == idconv).Count();
            if (countbl > 0)
            {
                if (MessageBox.Show("Do you want to remove your site page it contains backlinks?",
                    "Confirm Removal of Site Page",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await Task.Run(() =>
                    {

                        // if contains backlinks remove these
                        var getBacklinks = context.Backlinks.Where(mspage => mspage.MySitePageId == idconv);
                        if (getBacklinks.Any())
                        {
                            foreach (var backlink in getBacklinks)
                            {
                                context.Backlinks.Remove(backlink);
                                getSite.BacklinksCount--;
                            }
                        }

                    });
                }
                else
                {
                    return;
                }
            }
            var getSitePage = context.MySitesPages.Where(s => s.Id == idconv).Single();
            context.MySitesPages.Remove(getSitePage);

            getSite.PagesCount--;

            context.SaveChanges();
            GetSitePages();
        }

        public void Get_Total_Pages()
        {
            using (AppServices.WebcoseoDbContext context = new())
            {
                totalItems = context.MySitesPages.Where(x => x.MySiteId == GetMySiteId).Count();
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
            GetSitePages();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                GetSitePages();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            GetSitePages();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            GetSitePages();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            GetSitePages();
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
                    GetSitePages();
                }
            }

        }
    }
}
