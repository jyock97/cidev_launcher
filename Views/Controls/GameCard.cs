using cidev_launcher.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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
    }
}
