using HeavenMariaBeyond.Domain;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Interfaces;
using System.Diagnostics;

namespace HeavenMariaBeyond.Service
{
    public class ExternalLauncherService : IExternalLauncherService
    {
        private readonly IPathProvider _pathProvider;

        public ExternalLauncherService(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public void OpenDirectory(Girl girl)
        {
            var dir = _pathProvider.GetGirlProfileDirectoryPath(girl);
            OpenDirectoryByPath(dir);
        }

        public void OpenDirectory(string path)
        {
            OpenDirectoryByPath(path);
        }

        public void OpenDirectory(Diary diary)
        {
            var dir = _pathProvider.GetDiaryDirectoryPath(diary);
            OpenDirectoryByPath(dir);
        }

        public void PlayDiaryVideo(Diary diary)
        {
            var dir = _pathProvider.GetDiaryDirectoryPath(diary);
            var video = Directory.GetFiles(dir, "*.MP4").FirstOrDefault();
            if (video == null)
            {
                throw new FileNotFoundException("The diary directory contains no MP4 file.", dir);
            }

            Process.Start(new ProcessStartInfo(video) { UseShellExecute = true });
        }

        private void OpenDirectoryByPath(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                throw new DirectoryNotFoundException(dirPath);
            }

            Process.Start(new ProcessStartInfo(dirPath) { UseShellExecute = true });
        }

        public void OpenUrl(string url)
        {
            var pi = new ProcessStartInfo
            {
                FileName = "msedge",
                UseShellExecute = true,
            };
            pi.ArgumentList.Add(url);

            Process.Start(pi);
        }

        public void LaunchRelatedApp(string appName)
        {
            switch (appName)
            {
                case "HeavenNetCollector":
                    Process.Start(ApplicationConstData.HeavenNetCollectorWPF_Path);
                    break;
                case "HeavenVideoDownLoader":
                    Process.Start(ApplicationConstData.HeavenVideoDownLoader_Path);
                    break;
                default:
                    break;
            }
        }

    }
}
