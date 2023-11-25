using cidev_launcher.Models;
using cidev_launcher.Services;
using cidev_launcher.Views.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;


namespace cidev_launcher.Views.Pages
{
    public sealed partial class ExplorePage : Page
    {
        private static CachedGame SelectedGame;
        private static GameCard SelectedGameUIElement;

        public ExplorePage()
        {
            this.InitializeComponent();
        }

        private void _explorePageGridView_Loaded(object sender, RoutedEventArgs e)
        {
            List<Game> games = WhitelistService.Instance.GetWhitelistGames();
            List<CachedGame> cachedGames = CacheService.Instance.GetCachedGames(games);
            foreach (CachedGame cachedGame in cachedGames)
            {
                GameCard gameCard = new GameCard(cachedGame);
                explorePage_GridView.Items.Add(gameCard);
                if (SelectedGame != null && SelectedGame.Equals(cachedGame))
                {
                    SelectedGameUIElement = gameCard;
                }
            }

            if (SelectedGameUIElement != null)
            {
                // If the connected item appears outside the viewport, scroll it into view.
                explorePage_GridView.ScrollIntoView(SelectedGameUIElement, ScrollIntoViewAlignment.Default);
                explorePage_GridView.UpdateLayout();

                explorePage_GridView.Focus(FocusState.Programmatic);

                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("GamePageToExplorePage");
                if (anim != null)
                {
                    anim.TryStart(SelectedGameUIElement.ThumbnailImage);
                }

                SelectedGame = null;
                SelectedGameUIElement = null;
            }
        }

        private void _explorePageGridView_ItemClick(object sender, ItemClickEventArgs e)
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
