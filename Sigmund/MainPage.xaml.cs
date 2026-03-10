using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Windows.Phone.Management.Deployment;
using System.Diagnostics;

// https://www.roblox.com/catalog/21311601/Sigmund

namespace Sigmund
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshAppList();
        }

        public class AppInfo
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public string Publisher { get; set; }
            public string InstallLocation { get; set; }
            public string InstalledDate { get; set; }
            public bool IsFramework { get; set; }
            public string ProductId { get; set; }
        }

        private void RefreshAppList()
        {
            var allApps = GetInstalledApps();
            if (allApps == null) return;

            var userApps = allApps.Where(a => !a.IsFramework).ToList();

            AppListBox.ItemsSource = userApps;
            AppCountText.Text = string.Format("{0} user apps found", userApps.Count);
        }

        public List<AppInfo> GetInstalledApps()
        {
            var appList = new List<AppInfo>();
            try
            {
                var packages = InstallationManager.FindPackages();

                foreach (var package in packages)
                {
                    // PS I'm not getting everythin cuz some are only for the CURRENT app (useless)
                    appList.Add(new AppInfo
                    {
                        Name = package.Id.Name,
                        Publisher = package.Id.Publisher,
                        Version = string.Format("{0}.{1}.{2}.{3}",
                            package.Id.Version.Major,
                            package.Id.Version.Minor,
                            package.Id.Version.Build,
                            package.Id.Version.Revision),
                        InstallLocation = package.InstalledLocation.Path,
                        InstalledDate = "Installed: " + package.InstallDate.ToString("yyyy-MM-dd"),
                        IsFramework = package.IsFramework
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Access Denied: Check capabilities.");
                Debug.WriteLine("Error: " + ex.Message);
            }
            return appList;
        }

        private void AppListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppInfo selected = AppListBox.SelectedItem as AppInfo;
            if (selected != null)
            {
                NavigationService.Navigate(new Uri("/DetailPage.xaml?name=" + selected.Name, UriKind.Relative));
                AppListBox.SelectedItem = null;
            }
        }
    }
}