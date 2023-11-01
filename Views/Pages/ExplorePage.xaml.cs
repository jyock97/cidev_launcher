using cidev_launcher.Models;
using cidev_launcher.Services;
using cidev_launcher.Views.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WinRT;

// TODO Re arange images when clicked on the GameCard, make that Gamecard bigger and add buttons, etc
// TODO Implement Default images (when they are missing or failed to download)

namespace cidev_launcher.Views.Pages
{
    public sealed partial class ExplorePage : Page
    {
        public ExplorePage()
        {
            this.InitializeComponent();

            List<Game> games = WhitelistService.Instance.GetWhitelistGames();
            var cachedGames = CacheService.Instance.GetCachedGames(games);
            foreach (CachedGame cachedGame in cachedGames.Result.Values)
            {
                GameCard gameCard = new GameCard(cachedGame);
                TextBlock t = new TextBlock();

                _exploreGridView.Items.Add(gameCard);

                Debug.WriteLine($"\t[Explore Page] gameTitle: {cachedGame.gameInfo.gameTitle}");
                Debug.WriteLine($"\t[Explore Page] thumbnailImgUrl: {cachedGame.gameInfo.thumbnailImgUrl}");
                Debug.WriteLine($"\t[Explore Page] headerImgUrl: {cachedGame.gameInfo.headerImgUrl}");
                Debug.WriteLine($"\t[Explore Page] pageUrl: {cachedGame.gameInfo.pageUrl}");
                Debug.WriteLine($"\t[Explore Page] downloadUrl: {cachedGame.gameInfo.downloadUrl}");
                Debug.WriteLine($"\t[Explore Page] shouldExitClearCache: {cachedGame.shouldExitClearCache}");
                Debug.WriteLine($"\t[Explore Page] thumbnailImgPath: {cachedGame.thumbnailImgPath}");
                Debug.WriteLine($"\t[Explore Page] headerImgPath: {cachedGame.headerImgPath}");
                Debug.WriteLine($"\t[Explore Page] downloadPath: {cachedGame.downloadPath}");
            }
        }

        private void _exploreGridView_GameClick(object sender, ItemClickEventArgs e)
        {
            //Frame mainWindow = this.Parent.As<Frame>();
            //if (mainWindow != null)
            //{
            //    //mainWindow.TransitionToGamePage(new CachedGame());
            //}

            if (e.ClickedItem.GetType() == typeof(GameCard))
            {
                GameCard gameCard = e.ClickedItem as GameCard;

                gameCard.Expand();
            }
            else
            {
                Debug.WriteLine("\n\n NO");
            }
        }
        private void n_exploreGridView_GameClick()
        {
            //if (sender.GetType() == typeof(GameCard))
            //{
            //    Debug.WriteLine("\t[ExplorePage] GameCard");
            //}

            //myButton.Content = "Clicked";

            //// Create the file picker
            //var filePicker = new FileOpenPicker();

            //// Get the current window's HWND by passing in the Window object
            //var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            //// Associate the HWND with the file picker
            //WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            //// Use file picker like normal!
            //filePicker.FileTypeFilter.Add("*");
            //Windows.Storage.StorageFile file = await filePicker.PickSingleFileAsync();

            //string path1 = AppDomain.CurrentDomain.BaseDirectory;
            //string path2 = System.IO.Directory.GetCurrentDirectory();
            //await Windows.Storage.FileIO.WriteTextAsync(file, path1);
            //await Windows.Storage.FileIO.WriteTextAsync(file, path2);
            //await Windows.Storage.FileIO.WriteTextAsync(file, $"{path1}\n{path2}");
        }
    }
}
