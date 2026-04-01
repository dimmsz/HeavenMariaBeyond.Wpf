using HeavenMaria.Domain.Entities.Main;

namespace HeavenMaria.Domain.Interfaces
{
    public interface IDiaryHtmlGenerator
    {
        Task<string> MakeDiaryHtml(DiaryOld diary);
    }
}
