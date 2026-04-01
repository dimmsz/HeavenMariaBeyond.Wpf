using HeavenMaria.Domain.Entities.Main;

namespace HeavenMaria.Domain.Interfaces
{
    public interface IPathProvider
    {
        string GetOldGirlDirectoryPath(Girl girl);
        string GetDiaryDirectoryPath(DiaryOld diary);
        string GetDiaryDirectoryPath(Diary diary);
        string GetGirlProfileDirectoryPath(Girl girl);
    }
}
