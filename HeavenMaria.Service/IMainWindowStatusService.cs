using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Enums;

namespace HeavenMaria.Service
{
    public interface IMainWindowStatusService
    {
        void Hide(string girlId);
        void DeleteDiary(DiaryOld diary);
        void DeleteDiary(Diary diary);
        GirlStatusUpdateResult ReverseStatus(string girlId, GirlAttribute attribute, bool isFavorite, bool isLegend);
    }

    public readonly record struct GirlStatusUpdateResult(bool Applied, bool VisibilityChanged);
}
