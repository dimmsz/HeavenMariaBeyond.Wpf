namespace HeavenMaria.Service
{
    public interface IDiaryDisplayService
    {
        Task<string> BuildDiaryHtml(string diaryCode);
    }
}
