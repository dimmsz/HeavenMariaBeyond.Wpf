using HeavenMariaBeyond.Domain.Entities.Main;

namespace HeavenMariaBeyond.Service
{
    public interface IExternalLauncherService
    {
        void OpenDirectory(Diary diary);
        void OpenDirectory(Girl girl);
        void OpenDirectory(string path);
        void PlayDiaryVideo(Diary diary);
        void OpenUrl(string url);
        void LaunchRelatedApp(string appName);
    }
}
