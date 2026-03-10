using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using Windows.Phone.Management.Deployment;
using Windows.Management.Deployment;

namespace Sigmund
{
    public partial class DetailPage : PhoneApplicationPage
    {
        private Windows.ApplicationModel.Package _package;

        public DetailPage() { InitializeComponent(); }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string name;
            if (NavigationContext.QueryString.TryGetValue("name", out name))
            {
                _package = InstallationManager.FindPackages().FirstOrDefault(p => p.Id.Name == name);

                if (_package != null)
                {
                    TxtName.Text = _package.Id.Name;
                    TxtPublisher.Text = _package.Id.Publisher;
                    TxtVersion.Text = string.Format("{0}.{1}.{2}.{3}",
                        _package.Id.Version.Major, _package.Id.Version.Minor,
                        _package.Id.Version.Build, _package.Id.Version.Revision);
                    TxtDate.Text = _package.InstallDate.ToString("yyyy-MM-dd HH:mm");
                    TxtPath.Text = _package.InstalledLocation.Path;
                }
            }
        }

        private void BtnLaunch_Click(object sender, RoutedEventArgs e)
        {
            try { _package.Launch(""); }
            catch { MessageBox.Show("This component cannot be launched directly."); }
        }

        private async void BtnUninstall_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to uninstall this app ?", "Confirm", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    // Uninstaller method via InstallationManager
                    await InstallationManager.RemovePackageAsync(_package.Id.FullName, RemovalOptions.None);
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Uninstall failed: " + ex.Message);
                }
            }
        }
    }
}