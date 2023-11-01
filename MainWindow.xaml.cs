using cidev_launcher.Models;
using cidev_launcher.Services;
using cidev_launcher.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;


namespace cidev_launcher
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        public void TransitionToGamePage(CachedGame cachedGame)
        {
            mainWindow_Frame.Navigate(typeof(GamePage), null, new DrillInNavigationTransitionInfo());
        }

        private void _OnNavigationChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var recomendedTransition = args.RecommendedNavigationTransitionInfo;

            Type pageType = typeof(ExplorePage);
            NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;

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
                pageType = typeof(GamePage);
            }

            mainWindow_Frame.Navigate(pageType, null, recomendedTransition);
        }
    }
}
