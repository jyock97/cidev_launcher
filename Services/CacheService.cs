using cidev_launcher.Models;
using cidev_launcher.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace cidev_launcher.Services
{
    public class CacheService
    {
        private static CacheService _instance;

        private const string CachedPath = "_Cached";
        private string DefaultImgPath = $"{AppDomain.CurrentDomain.BaseDirectory}Assets\\DefaultImg.png";

        private Dictionary<string, CachedGame> cachedGames;

        public static CacheService Instance
        {
            get
            {
                if (_instance == null) { _instance = new CacheService(); }
                return _instance;
            }
        }

        public async Task<Dictionary<string, CachedGame>> GetCachedGames(List<Game> games)
        {
            if (cachedGames == null)
            {
                cachedGames = await SearchCacheGamesAsync(games).ConfigureAwait(false);
            }

            return cachedGames;
        }

        private async Task<Dictionary<string, CachedGame>> SearchCacheGamesAsync(List<Game> games)
        {
            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}"))
            {
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}");
            }

            cachedGames = new Dictionary<string, CachedGame>();

            // Finding saved Games
            foreach (string file in Directory.EnumerateFiles($"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}", "*.meta"))
            {
                CachedGame cachedGame = JsonSerializer.Deserialize<CachedGame>(File.ReadAllText(file), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Debug.WriteLine($"\t[CacheService][Cache HIT] {cachedGame.gameInfo.gameTitle}");

                string gameHash = Hash.GetHashString(cachedGame.gameInfo.gameTitle);
                cachedGames[gameHash] = cachedGame;
            }

            // Saving game assets
            foreach (Game game in games)
            {
                string gameHash = Hash.GetHashString(game.gameTitle);
                if (!cachedGames.ContainsKey(gameHash))
                {
                    Debug.WriteLine($"\t[CacheService][Cache MISS] {game.gameTitle}");

                    string directoryPath = $"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}\\{gameHash}";
                    Directory.CreateDirectory(directoryPath);

                    string thumbnailFileExtention = Path.GetExtension(game.thumbnailImgUrl);
                    string thumbnailPath = $"{directoryPath}\\thumbnail{thumbnailFileExtention}";
                    thumbnailPath = await TryDownloadImage(game.thumbnailImgUrl, thumbnailPath).ConfigureAwait(false);

                    string headerFileExtention = Path.GetExtension(game.headerImgUrl);
                    string headerPath = $"{directoryPath}\\header{headerFileExtention}";
                    headerPath = await TryDownloadImage(game.headerImgUrl, headerPath).ConfigureAwait(false);


                    CachedGame cachedGame = new CachedGame()
                    {
                        gameInfo = game,
                        cachedDirectory = directoryPath,
                        isGameDownloaded = false,
                        thumbnailImgPath = thumbnailPath,
                        headerImgPath = headerPath
                    };
                    string serialized = JsonSerializer.Serialize(cachedGame, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true });
                    File.WriteAllText($"{directoryPath}.meta", serialized);

                    cachedGames[gameHash] = cachedGame;
                }
            }
            return cachedGames;
        }

        private async Task<string> TryDownloadImage(string url, string filePath)
        {
            string resultPath = DefaultImgPath;
            try
            {
                if (url != null)
                {
                    HttpClient httpClient = new HttpClient();
                    Debug.WriteLine($"\t[CacheService][Dowload] {url}");
                    byte[] imageByte = await httpClient.GetByteArrayAsync(url).ConfigureAwait(false);

                    File.WriteAllBytes(filePath, imageByte);
                    resultPath = filePath;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine($"\t[CacheService][Dowload] Error while download {url}");
            }

            return resultPath;
        }
    }
}
