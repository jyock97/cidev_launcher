using cidev_launcher.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace cidev_launcher.Views.Controls
{
    public sealed class GameCard : StackPanel
    {
        public GameCard(CachedGame cachedGame)
        {
            this.Margin = new Thickness(16);
            this.Orientation = Orientation.Vertical;
            this.Width = 200;
            this.Height = 200;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(cachedGame.thumbnailImgPath));
            img.Stretch = Stretch.UniformToFill;

            TextBlock txt = new TextBlock();
            txt.Text = cachedGame.gameInfo.gameTitle;

            this.Children.Add(img);
            this.Children.Add(txt);
        }

        public void Expand()
        {
            this.Width = 400;

            Image img = new Image();
            var i = this.Children[0] as Image;
            img.Source = i.Source;
            img.Stretch = Stretch.UniformToFill;

            this.Children.Add(img);
        }
    }
}
