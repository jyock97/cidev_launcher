using cidev_launcher.Models;
using cidev_launcher.Services;
using cidev_launcher.Views.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;

namespace cidev_launcher.Views.Pages
{
    public sealed partial class DownloadGamesPage : Page
    {
        private static CachedGame SelectedGame;
        private static GameCard SelectedGameUIElement;
        private UIElement connectedAnimDestElement;

        public DownloadGamesPage()
        {
            this.InitializeComponent();
        }

        private void _downloadGamesPageGridView_Loaded(object sender, RoutedEventArgs e)
        {
            List<Game> games = WhitelistService.Instance.GetWhitelistGames();
            List<CachedGame> cachedGames = CacheService.Instance.GetCachedGames(games);
            foreach (CachedGame cachedGame in cachedGames)
            {
                if (cachedGame.isGameDownloaded)
                {
                    GameCard gameCard = new GameCard(cachedGame);
                    downloadGamesPage_GridView.Items.Add(gameCard);
                    if (SelectedGame != null && SelectedGame.Equals(cachedGame))
                    {
                        SelectedGameUIElement = gameCard;
                    }
                }
            }

            if (SelectedGame != null)
            {
                if (SelectedGameUIElement != null)
                {
                    // If the connected item appears outside the viewport, scroll it into view.
                    downloadGamesPage_GridView.ScrollIntoView(SelectedGameUIElement, ScrollIntoViewAlignment.Default);
                    downloadGamesPage_GridView.UpdateLayout();

                    downloadGamesPage_GridView.Focus(FocusState.Programmatic);

                    connectedAnimDestElement = SelectedGameUIElement.ThumbnailImage;
                }
                else
                {
                    connectedAnimDestElement = downloadGamesPage_GridView;
                }

                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("GamePageToExplorePage");
                if (anim != null)
                {
                    anim.TryStart(connectedAnimDestElement);
                }

                SelectedGame = null;
                SelectedGameUIElement = null;
                connectedAnimDestElement = null;
            }
        }

        private void _downloadGamesPageGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is GameCard container)
            {
                SelectedGame = container.CachedGame;

                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView()
                    .PrepareToAnimate("ExplorePageToGamePage", container.ThumbnailImage);
                animation.Configuration = new DirectConnectedAnimationConfiguration();
                Frame.Navigate(typeof(GamePage), SelectedGame, new SuppressNavigationTransitionInfo());
            }
        }
    }
}
