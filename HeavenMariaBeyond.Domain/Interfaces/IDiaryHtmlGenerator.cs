using HeavenMariaBeyond.Domain.Entities.Main;

namespace HeavenMariaBeyond.Domain.Interfaces
{
    public interface IDiaryHtmlGenerator
    {
        Task<string> MakeDiaryHtml(Diary diary, CancellationToken cancellationToken = default);
    }
}
