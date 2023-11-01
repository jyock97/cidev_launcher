using cidev_launcher.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace cidev_launcher.Views.Pages
{
    public sealed partial class GamePage : Page
    {
        public CachedGame SelectedGame { get; set; }
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
    }
}
