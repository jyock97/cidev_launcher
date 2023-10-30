using cidev_launcher.Models;
using System.Collections.Generic;

// Get download link from here... The id need to be scraped from the HTML
// https://rednose-interactive.itch.io/witchcraft-tales/file/7427162
namespace cidev_launcher.Services
{
    public class ItchioService
    {
        private static ItchioService _instance;
        public static ItchioService Instance
        {
            get
            {
                if (_instance == null) { _instance = new ItchioService(); }
                return _instance;
            }
        }

        public List<Game> GetCidevGames(int page = 1)
        {
            return new List<Game>()
            {
                new Game("Title1",
                    "https://img.itch.zone/aW1hZ2UvMjA1ODU0Lzk2MzY0MC5wbmc=/315x250%23c/GwUluW.png",
                    "",
                    "https://bogamedia.itch.io/agualava",
                    "https://w3g3a5v6.ssl.hwcdn.net/upload2/game/205854/701518?GoogleAccessId=uploader@moonscript2.iam.gserviceaccount.com&Expires=1698472799&Signature=CngGzvqZ%2BtejTnaDzogl%2BB7F%2Fup%2FZV5ww9qe41OW9BbOP%2Fs5xOrWfj2TOb7NRpIKBkrdir2zf6gzZtt0mGHiLBupq1Nh3ut3PkhibXht7juxofgUK2O5BsES48cZLNsdYaIuVKcl%2FSw1MNF8UXlzePxuTZjuYUNjXxrriYGNc7G8lUFJq6HQDztDYkt0DoMNkKQFrl3EXAX7S9cAz2L%2B320HwxGe9W6VnMxysg7SnN1JgjcPn7fkwyN4amzxoe064BM2HKtnSId1gMsLWoB6an4dMmPlfFCw26OzZ46eGVOSWrJDOCVQ%2FlQPzm8b0dk3YVoTPzZfUuyfRZC0INxbVQ==&hwexp=1698473059&hwsig=49c70be5e9bb9e061023adad0077ae18"),

                new Game("WitchCraftTales",
                    "https://img.itch.zone/aW1hZ2UvMjA1ODU0Lzk2MzY0MC5wbmc=/315x250%23c/GwUluW.png",
                    "https://img.itch.zone/aW1nLzExNDI2MTI3LnBuZw==/original/%2BU9z%2FB.png",
                    "https://rednose-interactive.itch.io/witchcraft-tales",
                    "https://w3g3a5v6.ssl.hwcdn.net/upload2/game/1749062/7427162?GoogleAccessId=uploader@moonscript2.iam.gserviceaccount.com&Expires=1698472714&Signature=FQxaIQ0RmIYcAWqc7%2F2VyNQaVuI5%2Bf8Pha0kd47Crj8Q%2FdDjnGdyg2dtdtVfMZBqTlA1jel46uG%2Fx9RiBfSDgkDtYAgW2SS7b5Ay9xDP3Tp58bowHRw68u07y4YRnhkhgqz6SOdGuuGZqZ6nYWYLSTLs3xz17VG5u1E0DGzQcnasZ8TkGD%2FanqX%2BsOkmIv5WBjJYu84Ig73VC%2BmafIY8Psj4z4wxp4IlM1uUjXTvrI7dIjvAwkGfcA5AvgDc%2BJwAHR0jGTQO24qaPlKmrzcxrm76LUYRfamR9nWvVQ6mh0IMsfbpjNkXduGcWquRGxZsTpgLNQWJe9B6hbK4wUfzaw==&hwexp=1698472974&hwsig=a51ef6c03924ad951d6cd3e7964a6884")
            };
        }
    }
}
