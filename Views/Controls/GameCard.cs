using cidev_launcher.Models;
using cidev_launcher.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using System;
using Windows.UI;

namespace cidev_launcher.Views.Controls
{
    public sealed class GameCard : StackPanel
    {
        public CachedGame CachedGame { get; private set; }
        public UIElement ThumbnailImage { get; private set; }

        public GameCard(CachedGame cachedGame)
        {
            this.CachedGame = cachedGame;

            this.Margin = new Thickness(16);
            this.Orientation = Orientation.Vertical;
            this.Width = 200;
            this.Height = 224;

            Rectangle thumbnailSkeleton = new Rectangle();
            thumbnailSkeleton.Width = 200;
            thumbnailSkeleton.Height = 200;
            thumbnailSkeleton.Fill = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
            
            Rectangle titleSkeleton = new Rectangle();
            titleSkeleton.Width = 200;
            titleSkeleton.Height = 16;
            titleSkeleton.Margin = new Thickness(0, 8, 0, 0);
            titleSkeleton.Fill = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

            this.Children.Add(thumbnailSkeleton);
            this.Children.Add(titleSkeleton);
            
            ThumbnailImage = thumbnailSkeleton;

            // Download Thumbnails
            CacheService.Instance.DownloadThumbnail(cachedGame).ContinueWith(t =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    cachedGame.thumbnailImgPath = t.Result;

                    this.Children.Clear();

                    if (cachedGame.thumbnailImgPath != null)
                    {
                        Image img;
                        img = new Image();
                        img.Source = new BitmapImage(new Uri(cachedGame.thumbnailImgPath));
                        img.Stretch = Stretch.UniformToFill;

                        ThumbnailImage = img;

                        this.Children.Add(img);
                    }
                    else
                    {
                        ThumbnailImage = thumbnailSkeleton;
                        this.Children.Add(thumbnailSkeleton);
                    }

                    TextBlock txt = new TextBlock();
                    txt.Margin = new Thickness(0, 8, 0, 0);
                    txt.Text = cachedGame.gameInfo.gameTitle;
                    this.Children.Add(txt);
                });
            });
        }
    }
}
