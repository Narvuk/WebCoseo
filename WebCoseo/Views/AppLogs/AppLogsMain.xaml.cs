using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WebCoseo.Views.AppLogs
{
    /// <summary>
    /// Interaction logic for AppLogMain.xaml
    /// </summary>
    public partial class AppLogsMain : UserControl
    {
        public AppLogsMain()
        {
            InitializeComponent();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            ClearDatePickers();
            AppLogs_Today();
        }

        public void AppLogs_Today()
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                ClearLogs_List();
                DateTime todaydate = DateTime.Today.Date;
                AppLogs_List.ItemsSource = context.AppLogs.Where(
                    sd => sd.Created.Value.Date == todaydate.Date).OrderByDescending(od => od.Created).ToList();
            }
        }

        public void Search_AppLogs_Click(object sender, RoutedEventArgs e)
        {
            ClearLogs_List();
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                try
                {
                    ClearLogs_List();
                    DateTime startDate = DateTime.Parse(AppLogs_DateFrom_Pick.ToString());
                    DateTime endDate = DateTime.Parse(AppLogs_DateTo_Pick.ToString());
                    ComboBoxItem TypePick = (ComboBoxItem)AppLogs_Type_Pick.SelectedItem;
                    ComboBoxItem SectionPick = (ComboBoxItem)AppLogs_Section_Pick.SelectedItem;
                    if (TypePick != null)
                    {
                        if (TypePick.Content.ToString() == "All")
                        {
                            if (SectionPick != null)
                            {
                                AppLogs_List.ItemsSource = context.AppLogs.Where(
                                    sd => sd.Created.Value.Date >= startDate.Date && sd.Created.Value.Date <= endDate.Date && sd.Section == SectionPick.Name.ToString()
                                    ).OrderByDescending(od => od.Created).ToList();
                            }
                            else
                            {
                                AppLogs_List.ItemsSource = context.AppLogs.Where(
                                    sd => sd.Created.Value.Date >= startDate.Date && sd.Created.Value.Date <= endDate.Date).OrderByDescending(od => od.Created).ToList();
                            }
                        }
                        else
                        {
                            if (SectionPick != null)
                            {
                                AppLogs_List.ItemsSource = context.AppLogs.Where(
                                sd => sd.Created.Value.Date >= startDate.Date && sd.Created.Value.Date <= endDate.Date && sd.Type == TypePick.Name.ToString()
                                && sd.Section == SectionPick.Content.ToString()).OrderByDescending(od => od.Created).ToList();
                            }
                            else
                            {
                                AppLogs_List.ItemsSource = context.AppLogs.Where(
                                sd => sd.Created.Value.Date >= startDate.Date && sd.Created.Value.Date <= endDate.Date && sd.Type == TypePick.Name.ToString()
                                ).OrderByDescending(od => od.Created).ToList();
                            }
                        }
                    }
                    else
                    {
                        AppLogs_Today();
                    }
                }
                catch (Exception)
                {
                    // Report or do nothing
                }
            }
        }

        public void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ClearLogs_List();
            ClearDatePickers();
            ClearComboBoxes();
            AppLogs_Today();
        }

        public void Remove_All_Logs_Click(object sender, RoutedEventArgs e)
        {
            using (AppServices.WebcoseoDbContext context = new AppServices.WebcoseoDbContext())
            {
                if (MessageBox.Show("Do you really want to clear and delete the app logs?",
                    "Reset and Clear Logs",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var commandText = "DELETE FROM AppLogs";
                    context.Database.ExecuteSqlRaw(commandText);
                    var commandText2 = "UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='AppLogs'";
                    context.Database.ExecuteSqlRaw(commandText2);
                    context.AppLogs.Add(new Models.AppLogs
                    {
                        Created = DateTime.Now,
                        Type = "Information",
                        Section = "Application",
                        Message = "Logs have been sucessfully cleared!"

                    });
                    context.SaveChanges();

                    ClearLogs_List();
                    ClearDatePickers();
                    ClearComboBoxes();
                    AppLogs_Today();
                }

            }
        }

        private void ClearLogs_List()
        {
            AppLogs_List.ItemsSource = null;
            AppLogs_List.Items.Clear();
        }

        private void ClearComboBoxes()
        {
            AppLogs_Type_Pick.SelectedIndex = -1;
            AppLogs_Section_Pick.SelectedIndex = -1;
        }

        private void ClearDatePickers()
        {
            AppLogs_DateFrom_Pick.SelectedDate = DateTime.Now.Date;
            AppLogs_DateTo_Pick.SelectedDate = DateTime.Now.Date;
        }
    }
}
