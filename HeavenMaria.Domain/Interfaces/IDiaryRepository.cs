using HeavenMaria.Domain.Entities.Main;

namespace HeavenMaria.Domain.Interfaces
{
    public interface IDiaryRepository
    {
        IEnumerable<DiaryOld> GetDiariesWithoutBody(string girlId);
        IEnumerable<DiaryOld> GetMovieDiaries(string girlId);
        DiaryOld GetDiaryByDiaryCode(string diaryCode);

        IEnumerable<DiaryOld> GetDiariesByGirlId_2(string girlId);
        IEnumerable<Diary> GetDiariesByGirlId(string girlId);

        int DeleteDiary(DiaryOld diary);
        int DeleteDiary(Diary diary);
    }
}
