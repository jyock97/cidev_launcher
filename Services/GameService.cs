using cidev_launcher.Models;
using System;
using System.Diagnostics;
using System.Timers;

namespace cidev_launcher.Services
{
    public class GameService
    {
        private static GameService _instance;
        public static GameService Instance
        {
            get
            {
                if (_instance == null) { _instance = new GameService(); }
                return _instance;
            }
        }

        public bool IsGameRunning { get; private set; }

        private Timer timer;
        private Process currentProcess;
        private int inactiveTime = -1;
        private int latestKeyTime = -1;

        public void StartGame(string gamefile, Action processEndCallback)
        {
            if (currentProcess != null)
            {
                Debug.WriteLine($"\t[GameService][Error] Trying to start the game: [{gamefile}] while anotherone is running [{currentProcess.StartInfo.FileName}]");

                return;
            }

            timer = new Timer(500);
            timer.AutoReset = true;
            timer.Elapsed += (sender, args) =>
            {
                ValidateInactivity();
            };

            currentProcess = new Process();
            currentProcess.StartInfo.FileName = gamefile;
            currentProcess.StartInfo.UseShellExecute = false;
            currentProcess.StartInfo.CreateNoWindow = true;
            currentProcess.EnableRaisingEvents = true;
            currentProcess.Exited += (sender, args) =>
            {
                Debug.WriteLine($"\t[GameService][Exited] {currentProcess.StartInfo.FileName}");
                currentProcess.Dispose();

                timer.Stop();
                currentProcess = null;

                IsGameRunning = false;
                processEndCallback?.Invoke();
            };
            currentProcess.Disposed += (sender, args) =>
            {
                Debug.WriteLine($"\t[GameService][Disposed]  {currentProcess.StartInfo.FileName}");
            };

            IsGameRunning = true;
            latestKeyTime = Environment.TickCount;
            currentProcess.Start();
            timer.Start();
        }

        private void ValidateInactivity()
        {
            inactiveTime = Environment.TickCount - EventHookService.Instance.LastKeyTime;
            Debug.WriteLine($"\t[GameService][ValidateInactivity] Tick inactive time: {inactiveTime}");

            AppConfig appConfig = CacheService.Instance.GetAppConfig();
            if (appConfig.inactiveTime != -1 && inactiveTime > appConfig.inactiveTime)
            {
                currentProcess.Kill();
            }
        }
    }
}
