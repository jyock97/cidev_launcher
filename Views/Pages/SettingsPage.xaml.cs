using cidev_launcher.Models;
using cidev_launcher.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace cidev_launcher.Views.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            AppConfig appConfig = CacheService.Instance.GetAppConfig();
            settingsPage_InactiveNumberBox.Value = appConfig.inactiveTime;
        }

        private void _settingsPage_QuitButton_Click(object sender, RoutedEventArgs e)
        {
            if (settingsPage_QuitTextBox.Text.Equals(CacheService.Instance.GetAppConfig().quitPassword))
            {
                Application.Current.Exit();
            }
        }

        private void _settingsPage_UpdateInactiveTimeButton_Click(object sender, RoutedEventArgs e)
        {
            AppConfig appConfig = CacheService.Instance.GetAppConfig();
            if (!double.IsNaN(settingsPage_InactiveNumberBox.Value))
            {
                appConfig.inactiveTime = (int)settingsPage_InactiveNumberBox.Value;
                CacheService.Instance.SaveNewAppConfig(appConfig);
            }
            settingsPage_InactiveNumberBox.Value = appConfig.inactiveTime;
        }
    }
}
