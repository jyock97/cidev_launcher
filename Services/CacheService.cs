﻿using cidev_launcher.Models;
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
        private const string ThumbnailFileName = "Thumbnail";
        private const string HeaderFileName = "Header";
        private string DefaultImgPath = $"{AppDomain.CurrentDomain.BaseDirectory}Assets\\DefaultImg.png";

        private List<CachedGame> cachedGames;

        public static CacheService Instance
        {
            get
            {
                if (_instance == null) { _instance = new CacheService(); }
                return _instance;
            }
        }

        public List<CachedGame> GetCachedGames(List<Game> games)
        {
            if (cachedGames == null)
            {
                cachedGames = new List<CachedGame>();
                Dictionary<string, CachedGame> cachedGamesDic = SearchCacheGamesAsync(games);
                foreach(CachedGame cachedGame in cachedGamesDic.Values)
                {
                    cachedGames.Add(cachedGame);
                }
            }

            return cachedGames;
        }

        public async Task<string> DownloadThumbnail(CachedGame cachedGame)
        {
            string thumbnailPath = GetFilePath(cachedGame.gameInfo.gameTitle, cachedGame.gameInfo.thumbnailImgUrl, ThumbnailFileName);
            thumbnailPath = await DownloadFile(cachedGame.gameInfo.thumbnailImgUrl, thumbnailPath).ConfigureAwait(false);

            return thumbnailPath;
        }

        public async Task<string> DownloadHeader(CachedGame cachedGame)
        {
            string headerPath = GetFilePath(cachedGame.gameInfo.gameTitle, cachedGame.gameInfo.headerImgUrl, HeaderFileName);
            headerPath = await DownloadFile(cachedGame.gameInfo.headerImgUrl, headerPath).ConfigureAwait(false);

            return headerPath;
        }

        private string GetCacheDirectoryPath(string gameTitle)
        {
            string gameHash = Hash.GetHashString(gameTitle);
            return $"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}\\{gameHash}";
        }

        private string GetFilePath(string gameTitle, string fileUrl, string fileName)
        {
            string fileExtention = Path.GetExtension(fileUrl);

            return $"{GetCacheDirectoryPath(gameTitle)}\\{fileName}{fileExtention}";
        }

        private async Task<string> DownloadFile(string url, string filePath)
        {
            Debug.WriteLine($"\t[CacheService][Dowload] {url}");
            string resultPath = null;
            try
            {
                HttpClient httpClient = new HttpClient();
                byte[] imageByte = await httpClient.GetByteArrayAsync(url).ConfigureAwait(false);
                File.WriteAllBytes(filePath, imageByte);
                resultPath = filePath;
            }
            catch (Exception)
            {
                Debug.WriteLine($"\t[CacheService][Dowload] Error while downloading {url}");
            }

            return resultPath;
        }

        private Dictionary<string, CachedGame> SearchCacheGamesAsync(List<Game> games)
        {
            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}"))
            {
                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}");
            }

            Dictionary<string, CachedGame> cachedGamesDict = new Dictionary<string, CachedGame>();

            // Finding saved Games
            foreach (string file in Directory.EnumerateFiles($"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}", "*.meta"))
            {
                CachedGame cachedGame = JsonSerializer.Deserialize<CachedGame>(File.ReadAllText(file), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Debug.WriteLine($"\t[CacheService][Cache HIT] {cachedGame.gameInfo.gameTitle}");

                string gameHash = Hash.GetHashString(cachedGame.gameInfo.gameTitle);
                cachedGamesDict[gameHash] = cachedGame;
            }

            // Saving game assets
            foreach (Game game in games)
            {
                string gameHash = Hash.GetHashString(game.gameTitle);
                if (!cachedGamesDict.ContainsKey(gameHash))
                {
                    Debug.WriteLine($"\t[CacheService][Cache MISS] {game.gameTitle}");

                    string directoryPath = $"{AppDomain.CurrentDomain.BaseDirectory}{CachedPath}\\{gameHash}";
                    Directory.CreateDirectory(directoryPath);

                    string thumbnailPath = GetFilePath(game.gameTitle, game.thumbnailImgUrl, ThumbnailFileName);
                    thumbnailPath = File.Exists(thumbnailPath) ? thumbnailPath : null;

                    string headerPath = GetFilePath(game.gameTitle, game.headerImgUrl, HeaderFileName);
                    headerPath = File.Exists(headerPath) ? headerPath : null;

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

                    cachedGamesDict[gameHash] = cachedGame;
                }
            }
            return cachedGamesDict;
        }
    }
}
