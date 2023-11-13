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

            DisableButtons();

            // Store the item to be used in binding to UI
            SelectedGame = e.Parameter as CachedGame;

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ExplorePageToGamePage");
            if (anim != null)
            {
                anim.TryStart(gamePage_Thumbnail);
            }


            if (CacheService.Instance.IsUpdating(SelectedGame))
            {
                gamePage_UpdateProgressRing.Visibility = Visibility.Visible;
                gamePage_UpdateProgressRing.IsIndeterminate = true;

                CacheService.Instance.UpdateSubscribeProgressRing(UpdateCallback);
                CacheService.Instance.UpdateSubscribeEndCallback(UpdateEndCallback);
                CacheService.Instance.DownloadSubscribeProgressRing(UpdateCallback);
                CacheService.Instance.DownloadSubscribeEndCallback(UpdateEndCallback);
                CacheService.Instance.DownloadSubscribeProgressRing(UpdateCallback);
                CacheService.Instance.DownloadSubscribeEndCallback(UpdateEndCallback);
            }
            else if (CacheService.Instance.IsDownloading(SelectedGame))
            {
                gamePage_DownloadProgressRing.Visibility = Visibility.Visible;
                gamePage_DownloadProgressRing.IsIndeterminate = true;

                CacheService.Instance.DownloadSubscribeProgressRing(DownloadCallback);
                CacheService.Instance.DownloadSubscribeEndCallback(DownloadEndCallback);
            }
            else if (CacheService.Instance.IsDeleting(SelectedGame))
            {
                gamePage_DeleteProgressRing.Visibility = Visibility.Visible;
                gamePage_DeleteProgressRing.IsIndeterminate = true;

                CacheService.Instance.DownloadSubscribeProgressRing(DeleteCallback);
                CacheService.Instance.DownloadSubscribeEndCallback(DeleteEndCallback);
            }



            SetupThumbnail();
            SetupHeader();

            SetCTAVisibilities();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            CacheService.Instance.UnsubscribeAll();
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

        private async void _gamePage_DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            gamePage_DownloadProgressRing.Visibility = Visibility.Visible;
            gamePage_DownloadProgressRing.IsIndeterminate = true;

            CacheService.Instance.DownloadSubscribeProgressRing(DownloadCallback);
            CacheService.Instance.DownloadSubscribeEndCallback(DownloadEndCallback);

            await CacheService.Instance.DownloadGame(SelectedGame);

            CacheService.Instance.UnsubscribeAll();
        }

        private void _gamePage_PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void _gamePage_UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            gamePage_UpdateProgressRing.Visibility = Visibility.Visible;
            gamePage_UpdateProgressRing.IsIndeterminate = true;

            CacheService.Instance.UpdateSubscribeProgressRing(UpdateCallback);
            CacheService.Instance.UpdateSubscribeEndCallback(UpdateEndCallback);
            await CacheService.Instance.UpdateGame(SelectedGame);

            CacheService.Instance.UnsubscribeAll();
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
                gamePage_Header.Visibility = Visibility.Visible;

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
            bool isUpdating = CacheService.Instance.IsUpdating(SelectedGame);
            bool isDownloading = CacheService.Instance.IsDownloading(SelectedGame);
            bool isDeleting = CacheService.Instance.IsDeleting(SelectedGame);
            bool isGameDownloadedOrUpdating = isUpdating || SelectedGame.isGameDownloaded;
            gamePage_DownloadButton.Visibility = isGameDownloadedOrUpdating ? Visibility.Collapsed : Visibility.Visible;
            gamePage_PlayButton.Visibility = isGameDownloadedOrUpdating ? Visibility.Visible : Visibility.Collapsed;
            gamePage_UpdateButton.Visibility = isGameDownloadedOrUpdating ? Visibility.Visible : Visibility.Collapsed;
            gamePage_DeleteButton.Visibility = isGameDownloadedOrUpdating ? Visibility.Visible : Visibility.Collapsed;


            gamePage_DownloadButton.IsEnabled = !isUpdating && !isDownloading && !isDeleting;
            gamePage_PlayButton.IsEnabled = !isUpdating && !isDownloading && !isDeleting;
            gamePage_UpdateButton.IsEnabled = !isUpdating && !isDownloading && !isDeleting;
            gamePage_DeleteButton.IsEnabled = !isUpdating && !isDownloading && !isDeleting;
        }


        private void UpdateCallback(int progress)
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateProgressRing(gamePage_UpdateProgressRing, progress));
        }
        private void UpdateEndCallback(CachedGame result)
        {
            ProcessEndCallback(result, gamePage_UpdateProgressRing);
        }
        private void DownloadCallback(int progress)
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateProgressRing(gamePage_DownloadProgressRing, progress));
        }
        private void DownloadEndCallback(CachedGame result)
        {
            ProcessEndCallback(result, gamePage_DownloadProgressRing);
        }
        private void DeleteCallback(int progress)
        {
            DispatcherQueue.TryEnqueue(() =>
                UpdateProgressRing(gamePage_DeleteProgressRing, progress));
        }
        private void DeleteEndCallback(CachedGame result)
        {
            ProcessEndCallback(result, gamePage_DeleteProgressRing);
        }
        private void ProcessEndCallback(CachedGame result, ProgressRing progressRing)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                SelectedGame = result;

                SetupThumbnail();
                SetupHeader();

                progressRing.Visibility = Visibility.Collapsed;
                
                SetCTAVisibilities();
            });
        }
    }
}
