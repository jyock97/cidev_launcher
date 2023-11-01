using cidev_launcher.Models;
using cidev_launcher.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// TODO Continue working with GamePage (Download exe and create new process)
// TODO Implement Default images (when they are missing or failed to download)

namespace cidev_launcher.Views.Pages
{
    public sealed partial class ExplorePage : Page
    {
        private static CachedGame SelectedGame;

        public ExplorePage()
        {
            this.InitializeComponent();
        }

        private async void _explorePageGridView_Loaded(object sender, RoutedEventArgs e)
        {
            List<Game> games = WhitelistService.Instance.GetWhitelistGames();
            Dictionary<string, CachedGame> cachedGames = await CacheService.Instance.GetCachedGames(games);
            foreach (CachedGame cachedGame in cachedGames.Values)
            {
                //Debug.WriteLine($"\t[Explore Page] gameTitle: {cachedGame.gameInfo.gameTitle}");
                //Debug.WriteLine($"\t[Explore Page] thumbnailImgUrl: {cachedGame.gameInfo.thumbnailImgUrl}");
                //Debug.WriteLine($"\t[Explore Page] headerImgUrl: {cachedGame.gameInfo.headerImgUrl}");
                //Debug.WriteLine($"\t[Explore Page] pageUrl: {cachedGame.gameInfo.pageUrl}");
                //Debug.WriteLine($"\t[Explore Page] downloadUrl: {cachedGame.gameInfo.downloadUrl}");
                //Debug.WriteLine($"\t[Explore Page] shouldExitClearCache: {cachedGame.shouldExitClearCache}");
                //Debug.WriteLine($"\t[Explore Page] thumbnailImgPath: {cachedGame.thumbnailImgPath}");
                //Debug.WriteLine($"\t[Explore Page] headerImgPath: {cachedGame.headerImgPath}");
                //Debug.WriteLine($"\t[Explore Page] downloadPath: {cachedGame.downloadPath}");
            }
            
            explorePage_GridView.ItemsSource = cachedGames.Values;

            if (SelectedGame != null)
            {
                // If the connected item appears outside the viewport, scroll it into view.
                explorePage_GridView.ScrollIntoView(SelectedGame, ScrollIntoViewAlignment.Default);
                explorePage_GridView.UpdateLayout();

                explorePage_GridView.Focus(FocusState.Programmatic);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("GamePageToExplorePage");
            if (anim != null)
            {
                anim.TryStart(explorePage_GridView);
            }
        }

        private void _explorePageGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (explorePage_GridView.ContainerFromItem(e.ClickedItem) is GridViewItem container)
            {
                SelectedGame = container.Content as CachedGame;

                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ExplorePageToGamePage", container);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
                Frame.Navigate(typeof(GamePage), SelectedGame, new SuppressNavigationTransitionInfo());
            }
        }
    }
}
