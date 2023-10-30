namespace cidev_launcher.Models
{
    public class Game
    {
        public string gameTitle { get; private set; }
        public string thumbnailImgUrl { get; private set; }
        public string headerImgUrl { get; private set; }
        public string pageUrl { get; private set; }
        public string downloadUrl { get; private set; }

        public Game(string newGameTitle, string newThumbnailImgUrl,
            string newHeaderImgUrl, string newPageUrl, string newDownloadUrl)
        {
            this.gameTitle = newGameTitle;
            this.thumbnailImgUrl = newThumbnailImgUrl;
            this.headerImgUrl = newHeaderImgUrl;
            this.pageUrl = newPageUrl;
            this.downloadUrl = newDownloadUrl;
        }
    }
}
