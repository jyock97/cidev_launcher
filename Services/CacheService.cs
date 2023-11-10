using cidev_launcher.Models;
using cidev_launcher.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
        private const string GameDirectoryName = "Game";

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
            cachedGames = new List<CachedGame>();
            Dictionary<string, CachedGame> cachedGamesDic = SearchCacheGamesAsync(games);
            foreach (CachedGame cachedGame in cachedGamesDic.Values)
            {
                cachedGames.Add(cachedGame);
            }

            return cachedGames;
        }

        public async Task<CachedGame> DownloadThumbnail(CachedGame cachedGame)
        {
            string thumbnailPath = GetFilePath(cachedGame.gameInfo.gameTitle, cachedGame.gameInfo.thumbnailImgUrl, ThumbnailFileName);
            thumbnailPath = await DownloadFile(cachedGame.gameInfo.thumbnailImgUrl, thumbnailPath).ConfigureAwait(false);

            cachedGame.thumbnailImgPath = thumbnailPath;
            SaveCacheMetaFile(cachedGame);

            return cachedGame;
        }

        public async Task<CachedGame> DownloadHeader(CachedGame cachedGame)
        {
            string headerPath = GetFilePath(cachedGame.gameInfo.gameTitle, cachedGame.gameInfo.headerImgUrl, HeaderFileName);
            headerPath = await DownloadFile(cachedGame.gameInfo.headerImgUrl, headerPath).ConfigureAwait(false);
            cachedGame.headerImgPath = headerPath;
            SaveCacheMetaFile(cachedGame);

            return cachedGame;
        }

        public async Task<CachedGame> DownloadGame(CachedGame cachedGame, Action<int> updateProgressCallback)
        {
            cachedGame.isGameDownloaded = true;
            updateProgressCallback(-1);

            Debug.WriteLine($"\t[CacheService][Dowload Game] {cachedGame.gameInfo.gameTitle}");
            string GameFilePath = null;
            try
            {
                // Download the Game from the server
                HttpClient httpClient = new HttpClient();

                HttpResponseMessage DownloadUrlResponse = await httpClient.PostAsync(cachedGame.gameInfo.downloadUrl, null);
                string downloadGameUrlStr = await DownloadUrlResponse.Content.ReadAsStringAsync();
                DownloadGameUrl downloadGameUrl = JsonSerializer.Deserialize<DownloadGameUrl>(downloadGameUrlStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                HttpRequestMessage requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Get;
                requestMessage.RequestUri = new Uri(downloadGameUrl.url);
                HttpResponseMessage downloadGameResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

                long contentLength = downloadGameResponse.Content.Headers.ContentLength.GetValueOrDefault();
                if (contentLength == 0)
                {
                    updateProgressCallback(-1);
                }

                string fileName = downloadGameResponse.Content.Headers.ContentDisposition?.FileName;
                fileName = fileName != null ? fileName.Trim('"') : "default.zip";

                Debug.WriteLine($"[CacheService][Dowload Game] DownloadFile {fileName}");

                Stream contentStream = downloadGameResponse.Content.ReadAsStream();
                GameFilePath = GetFilePath(cachedGame.gameInfo.gameTitle, fileName, Hash.GetHashString(cachedGame.gameInfo.gameTitle));
                FileStream gameFile = File.OpenWrite(GameFilePath);


                int downloadChunkSize = 1000000; // 1MB
                var buffer = new byte[downloadChunkSize];
                long totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
                {
                    Debug.WriteLine($"[CacheService][Dowload Game] Downloading {totalBytesRead} from {contentLength}");

                    await gameFile.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                    totalBytesRead += bytesRead;
                    if (contentLength > 0)
                    {
                        updateProgressCallback((int)((((double)totalBytesRead) / ((double)contentLength)) * 100));
                    }
                }

                gameFile.Flush();
                gameFile.Close();
                cachedGame.downloadPath = GameFilePath;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"\t[CacheService][Dowload Game] Error while downloading game {cachedGame.gameInfo.gameTitle} | {e.Message}");
            }

            try
            {
                // Unzip the file
                updateProgressCallback(-1);
                string downloadDirectoryPath = $"{GetCacheDirectoryPath(cachedGame.gameInfo.gameTitle)}\\{GameDirectoryName}";
                await Task.Run(() => ZipFile.ExtractToDirectory(GameFilePath, downloadDirectoryPath));

                string[] gameExeArray = Directory.GetFiles(downloadDirectoryPath, "*.exe", SearchOption.AllDirectories)
                    .Where(exe => !exe.Contains("UnityCrashHandler")).ToArray();
                string gameExe = gameExeArray.Length > 0 ? gameExeArray[0] : null;

                if (gameExe == null)
                {
                    Debug.WriteLine($"\t[CacheService][Dowload Game] Error no exe found on game's directory {downloadDirectoryPath}");
                }

                cachedGame.downloadExePath = gameExe;
                SaveCacheMetaFile(cachedGame);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"\t[CacheService][Dowload Game] Error while Extracting zip game {cachedGame.gameInfo.gameTitle} | {e.Message}");
            }

            return cachedGame;
        }

        public async Task<CachedGame> DeleteGame(CachedGame cachedGame)
        {
            string cacheDirectory = GetCacheDirectoryPath(cachedGame.gameInfo.gameTitle);

            await Task.Run(() =>
            {
                foreach (string filePath in Directory.EnumerateFiles(cacheDirectory))
                {
                    File.Delete(filePath);
                }
                foreach (string directoryPath in Directory.EnumerateDirectories(cacheDirectory))
                {
                    Directory.Delete(directoryPath, true);
                }
            });

            CachedGame newCachedGame = new CachedGame();
            newCachedGame.gameInfo = cachedGame.gameInfo;
            newCachedGame.cachedDirectory = cacheDirectory;

            SaveCacheMetaFile(newCachedGame);

            return newCachedGame;
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
                    SaveCacheMetaFile(cachedGame);

                    cachedGamesDict[gameHash] = cachedGame;
                }
            }
            return cachedGamesDict;
        }

        private void SaveCacheMetaFile(CachedGame cachedGame)
        {
            string serialized = JsonSerializer.Serialize(cachedGame, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true });
            File.WriteAllText($"{GetCacheDirectoryPath(cachedGame.gameInfo.gameTitle)}.meta", serialized);
        }
    }
}
