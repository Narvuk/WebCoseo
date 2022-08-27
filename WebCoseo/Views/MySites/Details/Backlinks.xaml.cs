using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WebCoseo.Views.Websites.Actions;
using System.Windows.Input;
using System.Threading.Tasks;

namespace WebCoseo.Views.MySites.Details
{
    /// <summary>
    /// Interaction logic for Backlinks.xaml
    /// </summary>
    public partial class Backlinks : UserControl
    {
        public static Backlinks? _instance;
        public int GetMySiteId { set; get; }
        public string BacklinksStatus { set; get; } = "All";
        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public Backlinks()
        {
            _instance = this;
            InitializeComponent();
        }

        public void LoadDetails(int MySiteId)
        {
            GetMySiteId = MySiteId;
            GetBacklinks();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            GetBacklinks();
            Get_Total_Pages();
        }

        private void ClearBacklinks_List()
        {
            WebsiteBacklinks_List.ItemsSource = null;
            WebsiteBacklinks_List.Items.Clear();
        }

        public void GetBacklinks()
        {
            ClearBacklinks_List();
            Backlinks_SearchBox.Text = "";
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                ComboBox_Status.Text = BacklinksStatus;
                Combobox_ItemsPerPage.Text = pageSize.ToString();
                CurrentPage_Label.Text = pageNumber.ToString();
                if (BacklinksStatus == "All")
                {
                    WebsiteBacklinks_List.ItemsSource = context.Backlinks.Where(x => x.MySiteId == GetMySiteId)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                } else
                {
                    WebsiteBacklinks_List.ItemsSource = context.Backlinks.Where(x => x.MySiteId == GetMySiteId && x.Status == BacklinksStatus)
                        .Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                }
            }
        }

        private void Backlinks_Search_Click(object sender, RoutedEventArgs e)
        {
            ClearBacklinks_List();
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                var backlinksfound = context.Backlinks.Where(l => l.BacklinkUrl.ToLower().Contains(Backlinks_SearchBox.Text.ToLower()) && l.MySiteId == GetMySiteId);
                WebsiteBacklinks_List.ItemsSource = backlinksfound.ToList();
            }
        }

        private void Backlinks_Reset_Click(object sender, RoutedEventArgs e)
        {
            ClearBacklinks_List();
            GetBacklinks();
        }


        private void EditBacklink_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            Actions.EditBacklink._instance.LoadDetails(idconv);
            ViewDetails._instance.GetView_EditBacklink();

        }

        private void ViewBacklink_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);
            BacklinkView._instance.LoadDetails(idconv);
            ViewDetails._instance.GetView_ViewBacklink();

        }


        private async void DeleteBacklink_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            using AppServices.WebcoseoDbContext context = new();
            // do check before remove
           
            if (MessageBox.Show("Do you want to remove this backlink?",
                "Confirm Removal of Backlink",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
               
                var getBacklink = context.Backlinks.Single(s => s.Id == idconv);
                context.Backlinks.Remove(getBacklink);
                context.SaveChanges();
                GetBacklinks();

            }
            else
            {
                return;
            }
            
        }

        public void Get_Total_Pages()
        {
            using (AppServices.WebcoseoDbContext context = new())
            {
                if (BacklinksStatus == "All")
                {
                    totalItems = context.Backlinks.Where(x => x.MySiteId == GetMySiteId).Count();
                }
                else
                {
                    totalItems = context.Backlinks.Where(x => x.MySiteId == GetMySiteId && x.Status == BacklinksStatus).Count();
                }
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
            GetBacklinks();
        }

        public void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (pageNumber >= 1)
            {
                pageNumber = pageNumber - 1;
                GetBacklinks();
            }
        }

        public void LastPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = totalPages;
            GetBacklinks();
        }

        public void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            pageNumber = 1;
            GetBacklinks();
        }

        public void Items_PerPage_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Combobox_ItemsPerPage.SelectedItem;
            pageSize = int.Parse(selectedItem.Content.ToString());
            GetBacklinks();
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
                    GetBacklinks();
                }
            }

        }

        public void List_Status_Change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_Status.SelectedItem;
            BacklinksStatus = selectedItem.Content.ToString();
            GetBacklinks();
            Get_Total_Pages();
        }
    }
}
