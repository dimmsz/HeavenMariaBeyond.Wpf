using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;

namespace HeavenMariaBeyond.Service
{
    public interface IMainWindowStatusService
    {
        void Hide(string girlId);
        void DeleteDiary(Diary diary);
        GirlStatusUpdateResult ReverseStatus(string girlId, GirlAttribute attribute, bool isFavorite, bool isLegend);
    }

    public readonly record struct GirlStatusUpdateResult(bool Applied, bool VisibilityChanged);
}
