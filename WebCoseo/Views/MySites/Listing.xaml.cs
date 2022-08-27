using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WebCoseo.Views.MySites
{
    /// <summary>
    /// Interaction logic for MySitesListing.xaml
    /// </summary>
    public partial class Listing : UserControl
    {
        public static Listing? _instance;
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public Listing()
        {
            _instance = this;
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            GetMySites();
        }

        private void ClearMySites_List()
        {
            MySites_List.ItemsSource = null;
            MySites_List.Items.Clear();
        }

        public void GetMySites()
        {
            ClearMySites_List();
            MySites_SearchBox.Text = "";
            Combobox_ItemsPerPage.Text = pageSize.ToString();
            CurrentPage_Label.Text = pageNumber.ToString();
            using AppServices.CoreDbContext coreContext = new();
            using AppServices.WebcoseoDbContext webcoseoContext = new();
            
            MySites_List.ItemsSource = webcoseoContext.MySites.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            
            
        }

        private void SearchMySites_Click(object sender, RoutedEventArgs e)
        {
            ClearMySites_List();
            using AppServices.CoreDbContext coreContext = new();
            using AppServices.WebcoseoDbContext webcoseoContext = new();
            
            var mysitesfound = webcoseoContext.MySites.Where(l => l.SiteName.ToLower().Contains(MySites_SearchBox.Text.ToLower()))
                .Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            MySites_List.ItemsSource = mysitesfound.ToList();
            
            
            
        }

        private void ResetMySites_Click(object sender, RoutedEventArgs e)
        {
            ClearMySites_List();
            GetMySites();
        }

        private void MySites_List_Details_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            ViewDetails._instance.LoadId(idconv);
            ViewDetails._instance.Overview_Default_LoadPage();
            Main._instance.ViewMySitesDetails();

        }

        private void AddNewSite_Click(object sender, RoutedEventArgs e)
        {
            using AppServices.CoreDbContext coreContext = new();
            using AppServices.WebcoseoDbContext webcoseoContext = new();
            
            Main._instance.ViewMySitesAddNew();
            
            
        }

        private void EditSite_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            Edit._instance.LoadInitial(idconv);
            Main._instance.ViewMySitesEdit();
        }

        private async void DeleteMySite_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                // do check before remove
                //count my sites pages
                var countsp = context.MySitesPages.Where(msp => msp.MySiteId == idconv).Count();
                if (countsp > 0)
                {
                    if (MessageBox.Show("Do you want to remove your website it contains sites pages or backlinks?",
                        "Confirm Removal of Site",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        await Task.Run(() =>
                        {
                            var getPages = context.MySitesPages.Where(msp => msp.MySiteId == idconv);
                            foreach (var page in getPages)
                            {
                                // if contains backlinks remove these
                                var getBacklinks = context.Backlinks.Where(mspage => mspage.MySitePageId == page.Id);
                                if (getBacklinks.Count() > 0)
                                {
                                    foreach (var backlink in getBacklinks)
                                    {
                                        context.Backlinks.Remove(backlink);
                                    }
                                }
                                context.MySitesPages.Remove(page);
                            }
                        });
                    }
                    else
                    {
                        return;
                    }
                }
                var getSite = context.MySites.Where(s => s.Id == idconv).Single();
                context.MySites.Remove(getSite);
                context.SaveChanges();
                GetMySites();
            }

        }

        public void Get_Total_Pages()
        {
            using (AppServices.WebcoseoDbContext context = new())
            {
                totalItems = context.MySites.Count();
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
            GetMySites();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                GetMySites();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            GetMySites();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            GetMySites();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            GetMySites();
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
                    GetMySites();
                }
            }

        }

    }
}
