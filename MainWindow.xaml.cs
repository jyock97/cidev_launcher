using cidev_launcher.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net;

namespace cidev_launcher
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void _OnNavigationChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;
            Type pageType = typeof(ExplorePage);
            if (args.IsSettingsSelected)
            {
                pageType = typeof(SettingsPage);
            }
            else if (selectedItem == mainWindow_NavigationItemExplore)
            {
                pageType = typeof(ExplorePage);
            }
            else if (selectedItem == mainWindow_NavigationItemGames)
            {
                pageType = typeof(DownloadGamesPage);
            }

            mainWindow_Frame.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
        }
    }
}
