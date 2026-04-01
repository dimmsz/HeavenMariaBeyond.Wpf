using HeavenMaria.Domain;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Interfaces;
using System.Diagnostics;

namespace HeavenMaria.Service
{
    public class ExternalLauncherService : IExternalLauncherService
    {
        private readonly IPathProvider _pathProvider;

        public ExternalLauncherService(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }

        public void OpenDirectory(DiaryOld diary)
        {
            var dir = _pathProvider.GetDiaryDirectoryPath(diary);
            OpenDirectoryByPath(dir);
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

        public void PlayDiaryVideo(DiaryOld diary)
        {
            var dir = _pathProvider.GetDiaryDirectoryPath(diary);
            var video = Directory.GetFiles(dir, "*.MP4").FirstOrDefault();
            if (video != null)
            {
                Process.Start(video);
            }
        }

        private void OpenDirectoryByPath(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = dirPath,
                    UseShellExecute = true // Ç±ÇÍÇ™èdóv
                };
                Process.Start(psi);
            }
        }

        public void OpenUrl(string url)
        {
            var pi = new ProcessStartInfo
            {
                FileName = "msedge",
                Arguments = url,
                UseShellExecute = true,
            };

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

        public void PlayDiaryVideo(Diary diary)
        {
            throw new NotImplementedException();
        }
    }
}
