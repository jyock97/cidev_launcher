namespace cidev_launcher.Models
{
    public class AppConfig
    {
        public string quitPassword { get; set; }
        public int inactiveTime { get; set; }

        public AppConfig() 
        {
            quitPassword = "let me out please";
            inactiveTime = 60000; // 1m
        }
    }
}
