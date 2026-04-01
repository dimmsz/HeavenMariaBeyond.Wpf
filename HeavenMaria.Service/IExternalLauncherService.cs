using HeavenMaria.Domain.Entities.Main;

namespace HeavenMaria.Service
{
    public interface IExternalLauncherService
    {
        void OpenDirectory(DiaryOld diary);
        void OpenDirectory(Girl girl);
        void OpenDirectory(string path);
        void PlayDiaryVideo(DiaryOld diary);
        void PlayDiaryVideo(Diary diary);
        void OpenUrl(string url);
        void LaunchRelatedApp(string appName);
    }
}
