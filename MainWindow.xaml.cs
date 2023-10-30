using cidev_launcher.Models;
using cidev_launcher.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace cidev_launcher
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();


            List<Game> games = ItchioService.Instance.GetCidevGames();

            foreach (Game game in games)
            {
                Debug.WriteLine(game.gameTitle);
                myButton.Content = game.gameTitle;

                Debug.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                Debug.WriteLine(System.IO.Directory.GetCurrentDirectory());

                
            }


            //myImage.Source = new BitmapImage(new Uri("https://img.itch.zone/aW1nLzM5MTY0MjQucG5n/original/nt2wSa.png"));
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";

            // Create the file picker
            var filePicker = new FileOpenPicker();

            // Get the current window's HWND by passing in the Window object
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            // Use file picker like normal!
            filePicker.FileTypeFilter.Add("*");
            Windows.Storage.StorageFile file = await filePicker.PickSingleFileAsync();

            string path1 = AppDomain.CurrentDomain.BaseDirectory;
            string path2 = System.IO.Directory.GetCurrentDirectory();
            await Windows.Storage.FileIO.WriteTextAsync(file, path1);
            await Windows.Storage.FileIO.WriteTextAsync(file, path2);
            await Windows.Storage.FileIO.WriteTextAsync(file, $"{path1}\n{path2}");
        }
    }
}
