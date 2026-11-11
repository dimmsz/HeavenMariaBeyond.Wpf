namespace HeavenMariaBeyond.Service
{
    public interface IDiaryDisplayService
    {
        Task<string> BuildDiaryHtml(string diaryCode, CancellationToken cancellationToken = default);
    }
}
