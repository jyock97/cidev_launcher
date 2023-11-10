using cidev_launcher.Models;
using cidev_launcher.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
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

            SetupThumbnail();
            SetupHeader();

            SetCTAVisibilities();
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

        private void _gamePage_DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            CacheService.Instance.DownloadGame(SelectedGame, (int downloadPercentage) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                    UpdateProgressRing(gamePage_DownloadProgressRing, downloadPercentage));

            }).ContinueWith((t) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SelectedGame = t.Result;

                    gamePage_DownloadProgressRing.Visibility = Visibility.Collapsed;

                    SetCTAVisibilities();
                });
            });
        }

        private void _gamePage_PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void _gamePage_UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            gamePage_UpdateProgressRing.Visibility = Visibility.Visible;
            gamePage_UpdateProgressRing.IsIndeterminate = true;

            await CacheService.Instance.DeleteGame(SelectedGame);
            await CacheService.Instance.DownloadThumbnail(SelectedGame).ContinueWith((t) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SelectedGame = t.Result;

                    SetupThumbnail();
                });
            });
            await CacheService.Instance.DownloadHeader(SelectedGame).ContinueWith((t) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SelectedGame = t.Result;

                    SetupHeader();
                });
            });

            await CacheService.Instance.DownloadGame(SelectedGame, (int downloadPercentage) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                    UpdateProgressRing(gamePage_UpdateProgressRing, downloadPercentage));

            }).ContinueWith((t) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    SelectedGame = t.Result;

                    gamePage_UpdateProgressRing.Visibility = Visibility.Collapsed;

                    SetCTAVisibilities();
                });
            });
        }

        private void _gamePage_DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            gamePage_DeleteProgressRing.Visibility = Visibility.Visible;
            CacheService.Instance.DeleteGame(SelectedGame)
                .ContinueWith((t) =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SelectedGame = t.Result;

                        gamePage_DeleteProgressRing.Visibility = Visibility.Collapsed;

                        SetCTAVisibilities();
                    });
                });
        }

        private void SetupThumbnail()
        {
            if (!File.Exists(SelectedGame.thumbnailImgPath))
            {
                CacheService.Instance.DownloadThumbnail(SelectedGame).ContinueWith(t =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SelectedGame = t.Result;

                        if (SelectedGame.thumbnailImgPath != null)
                        {
                            gamePage_Thumbnail.Source = new BitmapImage(new Uri(SelectedGame.thumbnailImgPath));
                            gamePage_Thumbnail.Visibility = Visibility.Visible;
                        }
                    });
                });
            }
            else
            {
                gamePage_Thumbnail.Source = new BitmapImage(new Uri(SelectedGame.thumbnailImgPath));
                gamePage_Thumbnail.Visibility = Visibility.Visible;
            }
        }

        private void SetupHeader()
        {
            gamePage_Header.Visibility = Visibility.Collapsed;
            if (SelectedGame.gameInfo.headerImgUrl != null)
            {
                if (!File.Exists(SelectedGame.headerImgPath))
                {
                    CacheService.Instance.DownloadHeader(SelectedGame).ContinueWith(t =>
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            SelectedGame = t.Result;

                            if (SelectedGame.headerImgPath != null)
                            {
                                gamePage_Header.Source = new BitmapImage(new Uri(SelectedGame.headerImgPath));
                                gamePage_Header.Visibility = Visibility.Visible;
                            }
                        });
                    });
                }
                else
                {
                    gamePage_Header.Source = new BitmapImage(new Uri(SelectedGame.headerImgPath));
                    gamePage_Header.Visibility = Visibility.Visible;
                }
            }
        }

        private void UpdateProgressRing(ProgressRing progressRing, int downloadPercentage)
        {
            if (downloadPercentage > 0)
            {
                progressRing.Visibility = Visibility.Visible;
                progressRing.IsIndeterminate = false;
                progressRing.Value = downloadPercentage;
            }
            else
            {
                progressRing.Visibility = Visibility.Visible;
                progressRing.IsIndeterminate = true;
            }
        }

        private void DisableButtons()
        {
            gamePage_DownloadButton.IsEnabled = false;
            gamePage_PlayButton.IsEnabled = false;
            gamePage_UpdateButton.IsEnabled = false;
            gamePage_DeleteButton.IsEnabled = false;
        }
        private void SetCTAVisibilities()
        {
            gamePage_DownloadButton.Visibility = SelectedGame.isGameDownloaded ? Visibility.Collapsed : Visibility.Visible;
            gamePage_PlayButton.Visibility = SelectedGame.isGameDownloaded ? Visibility.Visible : Visibility.Collapsed;
            gamePage_UpdateButton.Visibility = SelectedGame.isGameDownloaded ? Visibility.Visible : Visibility.Collapsed;
            gamePage_DeleteButton.Visibility = SelectedGame.isGameDownloaded ? Visibility.Visible : Visibility.Collapsed;

            gamePage_DownloadButton.IsEnabled = true;
            gamePage_PlayButton.IsEnabled = true;
            gamePage_UpdateButton.IsEnabled = true;
            gamePage_DeleteButton.IsEnabled = true;
        }
    }
}
