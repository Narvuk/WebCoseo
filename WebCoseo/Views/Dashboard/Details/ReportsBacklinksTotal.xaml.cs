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
using WebCoseo.AppServices;

namespace WebCoseo.Views.Dashboard.Details
{
    /// <summary>
    /// Interaction logic for ReportsBacklinksTotal.xaml
    /// </summary>
    public partial class ReportsBacklinksTotal : UserControl
    {
        public static ReportsBacklinksTotal _instance;

        int pageNumber = 1;
        int pageSize = 30;
        int totalPages = 1;
        int totalItems = 0;
        public ReportsBacklinksTotal()
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

        public void Get_Pages_Scan()
        {
            using ReportsDbContext reportsContext = new();

            // Paginition with skip((page - 1) * pagesize).Take(pagesize) ///
            Combobox_ItemsPerPage.Text = pageSize.ToString();
            CurrentPage_Label.Text = pageNumber.ToString();
            Reports_BacklinksTotal_List.ItemsSource = reportsContext.BL_Dash_Monthly_Total.OrderByDescending(x => x.Id).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();


        }

        public void Get_Total_Pages()
        {
            using ReportsDbContext reportsContext = new();
            totalItems = reportsContext.BL_Dash_Monthly_Total.Count();
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


        public void Delete_Report_Click(object sender, RoutedEventArgs e)
        {
            using ReportsDbContext reportsContext = new();

            var button = sender as MenuItem;
            var code = button.Tag;
            var id = code.ToString();
            var idconv = int.Parse(id);

            if (MessageBox.Show("Are you sure you wish to remove this report?",
                "Confirm Removal of Report",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var getReport = reportsContext.BL_Dash_Monthly_Total.Single(x => x.Id == idconv);

                reportsContext.BL_Dash_Monthly_Total.Remove(getReport);
                reportsContext.SaveChanges();
                Get_Pages_Scan();
            }
            else
            {
                return;
            }


        }
    }
}
