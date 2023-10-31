namespace cidev_launcher.Models
{
    public class CachedGame
    {
        public Game gameInfo { get; set; }
        public string cachedDirectory { get; set; }
        public bool shouldExitClearCache { get; set; }
        public string thumbnailImgPath { get; set; }
        public string headerImgPath { get; set; }
        public string downloadPath { get; set; }
    }
}
