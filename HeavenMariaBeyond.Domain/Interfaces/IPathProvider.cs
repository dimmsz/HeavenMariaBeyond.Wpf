using HeavenMariaBeyond.Domain.Entities.Main;

namespace HeavenMariaBeyond.Domain.Interfaces
{
    public interface IPathProvider
    {
        string DiaryRootDirectory { get; }
        string GetOldGirlDirectoryPath(Girl girl);
        string GetDiaryDirectoryPath(Diary diary);
        string GetGirlProfileDirectoryPath(Girl girl);
    }
}
