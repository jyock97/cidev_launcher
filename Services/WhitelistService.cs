using cidev_launcher.Models;
using System.Collections.Generic;

namespace cidev_launcher.Services
{
    public class WhitelistService
    {
        private static WhitelistService _instance;
        public static WhitelistService Instance
        {
            get
            {
                if (_instance == null) { _instance = new WhitelistService(); }
                return _instance;
            }
        }

        public List<Game> GetWhitelistGames()
        {
            return new List<Game>()
            {
                //new Game(){
                //   gameTitle= "WaterLava",
                //   description= "Juego desarrollado en el primer GameJam creado por CIDeV",
                //   thumbnailImgUrl= "https://img.itch.zone/aW1hZ2UvMjA1ODU0Lzk2MzY0MC5wbmc=/315x250%23c/GwUluW.png",
                //   headerImgUrl = null,
                //   pageUrl= "https://bogamedia.itch.io/agualava",
                //   downloadUrl=  "https://rednose-interactive.itch.io/witchcraft-tales/file/6707982"
                //},

                //new Game(){
                //  gameTitle=  "WitchCraftTales",
                //  description= "Vive la historia de como 3 Brujas de Salem huyen de la famosa caceria de Salem",
                //  thumbnailImgUrl=  "https://img.itch.zone/aW1nLzExNDI1OTk1LnBuZw==/315x250%23c/VyTz0m.png",
                //  headerImgUrl =  "https://img.itch.zone/aW1nLzExNDI2MTI3LnBuZw==/original/%2BU9z%2FB.png",
                //  pageUrl=  "https://rednose-interactive.itch.io/witchcraft-tales",
                //  downloadUrl=  "https://rednose-interactive.itch.io/witchcraft-tales/file/6707982"
                //},

                //new Game(){
                //  gameTitle=  "123",
                //  description= "12345678901234567890123456789012 12345678901234567890123456789012 12345678901234567890123456789012",
                //  thumbnailImgUrl=  "https://img.itch.zone/aW1nLzI5OTkxOTguanBn/315x250%23c/Xar8%2FH.jpg",
                //  headerImgUrl =  "https://img.itch.zone/aW1nLzk0MzE4OTUuZ2lm/original/ClEOfL.gif",
                //  pageUrl=  "https://modus-interactive.itch.io/shaderlab",
                //  downloadUrl=  "https://rednose-interactive.itch.io/witchcraft-tales/file/6707982"

                //},
            };

        }
    }
}
