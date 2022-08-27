using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WebCoseo.Views.Websites
{
    /// <summary>
    /// Interaction logic for Listing.xaml
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
            GetWebsites();
        }

        private void ClearWebsites_List()
        {
            Websites_List.ItemsSource = null;
            Websites_List.Items.Clear();
        }

        public void GetWebsites()
        {
            ClearWebsites_List();
            Websites_SearchBox.Text = "";
            Combobox_ItemsPerPage.Text = pageSize.ToString();
            CurrentPage_Label.Text = pageNumber.ToString();
            using AppServices.CoreDbContext coreContext = new();
            using AppServices.WebcoseoDbContext context = new();
            
            var websitesList = context.Websites.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            Websites_List.ItemsSource = websitesList;
            
            
        }

        private void SearchWebsites_Click(object sender, RoutedEventArgs e)
        {
            ClearWebsites_List();
            using AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext();
            using AppServices.CoreDbContext coreContext = new();
            
            var websitesfound = context.Websites.Where(l => l.Name.ToLower().Contains(Websites_SearchBox.Text.ToLower()));
            Websites_List.ItemsSource = websitesfound.ToList();
            
        }

        private void ResetWebsites_Click(object sender, RoutedEventArgs e)
        {
            ClearWebsites_List();
            GetWebsites();
        }

        private void Websites_List_Details_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            // Load the id of the reacord and anything that needs to load with it
            ViewDetails._instance.LoadId(idconv);
            // When clicking into view details load overview page first
            ViewDetails._instance.Overview_Default_LoadPage();
            Main._instance.ViewWebsitesDetails();

        }

        private void AddWebsite_Click(object sender, RoutedEventArgs e)
        {
            using AppServices.CoreDbContext coreContext = new();
            using AppServices.WebcoseoDbContext webcoseoContext = new();
            
            Main._instance.ViewAddNew();
            
        }

        private void EditSite_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            Edit._instance.LoadInitial(idconv);
            Main._instance.ViewEdit();
        }

        private async void DeleteWebsite_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            using AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext();
            // do check before remove
            var countbl = context.Backlinks.Where(wbl => wbl.WebsiteId == idconv).Count();
            if (countbl > 0)
            {
                if (MessageBox.Show("Do you want to remove website it contains backlinks?",
                    "Confirm Removal of Site",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await Task.Run(() =>
                    {
                        var getBacklinks = context.Backlinks.Where(gbl => gbl.WebsiteId == idconv);
                        foreach (var backlink in getBacklinks)
                        {
                            context.Backlinks.Remove(backlink);
                        }
                    });
                }
                else
                {
                    return;
                }
            }

            var getSite = context.Websites.Where(s => s.Id == idconv).Single();
            context.Websites.Remove(getSite);
            context.SaveChanges();
            GetWebsites();
            //HomeMain._instance.UpdateDashboard();

        }

        public void Get_Total_Pages()
        {
            using (AppServices.WebcoseoDbContext context = new())
            {
                totalItems = context.Websites.Count();
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
            GetWebsites();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                GetWebsites();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            GetWebsites();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            GetWebsites();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            GetWebsites();
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
                    GetWebsites();
                }
            }

        }
    }
}
