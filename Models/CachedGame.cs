namespace cidev_launcher.Models
{
    public class CachedGame
    {
        public Game gameInfo { get; set; }
        public string cachedDirectory { get; set; }
        public bool isGameDownloaded { get; set; }
        public string thumbnailImgPath { get; set; }
        public string headerImgPath { get; set; }
        public string downloadPath { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as CachedGame;

            if (item == null)
            {
                return false;
            }

            return this.cachedDirectory.Equals(item.cachedDirectory);
        }

        public override int GetHashCode()
        {
            return this.cachedDirectory.GetHashCode();
        }
    }
}
