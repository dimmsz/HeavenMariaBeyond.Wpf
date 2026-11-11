using HeavenMariaBeyond.Domain.Entities.Main;

namespace HeavenMariaBeyond.Domain.Interfaces
{
    public interface IDiaryRepository
    {
        IEnumerable<Diary> GetDiariesWithoutBody(string girlId);
        IEnumerable<Diary> GetMovieDiaries(string girlId);
        Task<Diary?> GetDiaryByDiaryCodeAsync(string diaryCode, CancellationToken cancellationToken);
        IEnumerable<Diary> GetDiariesByGirlId(string girlId);

        int DeleteDiary(Diary diary);
    }
}
