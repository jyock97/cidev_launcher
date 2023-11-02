using cidev_launcher.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.IO;

namespace cidev_launcher.Views.Pages
{
    public sealed partial class GamePage : Page
    {
        public CachedGame SelectedGame { get; private set; }
        public GamePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Store the item to be used in binding to UI
            SelectedGame = e.Parameter as CachedGame;

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ExplorePageToGamePage");
            if (anim != null)
            {
                anim.TryStart(gamePage_Thumbnail);
            }


            if (!File.Exists(SelectedGame.headerImgPath))
            {
                gamePage_Header.Visibility = Visibility.Collapsed;
            }
            else
            {
                gamePage_Header.Visibility = Visibility.Visible;
            }

            gamePage_Header.Visibility = File.Exists(SelectedGame.headerImgPath)? Visibility.Visible : Visibility.Collapsed;

            gamePage_DownloadButton.Visibility = SelectedGame.isGameDownloaded ? Visibility.Collapsed : Visibility.Visible;
            gamePage_PlayButton.Visibility = SelectedGame.isGameDownloaded ? Visibility.Visible : Visibility.Collapsed;
            gamePage_UpdateButton.Visibility = SelectedGame.isGameDownloaded ? Visibility.Visible : Visibility.Collapsed;
            gamePage_DeleteButton.Visibility = SelectedGame.isGameDownloaded ? Visibility.Visible : Visibility.Collapsed;
        }

        private void _gamePage_BackButton_Loaded(object sender, RoutedEventArgs e)
        {
            gamePage_BackButton.Focus(FocusState.Programmatic);
        }

        private void _gamePage_BackButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("GamePageToExplorePage", gamePage_Thumbnail);
            animation.Configuration = new BasicConnectedAnimationConfiguration();
            Frame.GoBack(new SuppressNavigationTransitionInfo());
        }

        private void gamePage_DownloadButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void gamePage_PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void gamePage_UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void gamePage_DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
