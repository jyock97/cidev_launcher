# CIDeV Launcher

Launcher used by CIDeV community to showcase their games.


## Configure Local Games

* On the application directory create a "_Cached" directory
* Create a directory for the game with a ".meta" file with the same name
* Set up meta file
```
{
  "gameInfo": {
    "gameTitle": "Name Of The Game",
    "description": "Description Of The Game",
    "thumbnailImgUrl": "URL",// Ignore this if you set it up locally
    "headerImgUrl": "URL",// Ignore this if you set it up locally
    "pageUrl": "URL",// Ignore this if you set it up locally
    "downloadUrl": "URL"// Ignore this if you set it up locally
  },
  "cachedDirectory": "C:\\path_to_cidev_launcher\\_Cached\\GAME_NAME",
  "isGameDownloaded": true,
  "thumbnailImgPath": "C:\\path_to_cidev_launcher\\_Cached\\GAME_NAME\\Thumbnail.png",
  "headerImgPath": "C:\\path_to_cidev_launcher\\_Cached\\GAME_NAME\\Header.png",
  "downloadPath": "PATH", // Ignore this if you set it up locally
  "downloadExePath": "C:\\path_to_cidev_launcher\\_Cached\\GAME_NAME\\DownloadDir\\Application.exe"
}
```
* Setup the Game directory acording to your configuration above