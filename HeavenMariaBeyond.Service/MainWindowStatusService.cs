using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;

namespace HeavenMariaBeyond.Service
{
    public class MainWindowStatusService : IMainWindowStatusService
    {
        private readonly IGirlStatusRepository _statusRepository;
        private readonly IDiaryRepository _diaryRepository;

        public MainWindowStatusService(IGirlStatusRepository statusRepository, IDiaryRepository diaryRepository)
        {
            _statusRepository = statusRepository;
            _diaryRepository = diaryRepository;
        }

        public void DeleteDiary(Diary diary)
        {
            _diaryRepository.DeleteDiary(diary);
        }

        public void Hide(string girlId)
        {
            _statusRepository.Hide(girlId);
        }

        public GirlStatusUpdateResult ReverseStatus(string girlId, GirlAttribute attribute, bool isFavorite, bool isLegend)
        {
            switch (attribute)
            {
                case GirlAttribute.Favorite:
                    if (isFavorite)
                    {
                        _statusRepository.Unfavorite(girlId);
                    }
                    else
                    {
                        _statusRepository.Favorite(girlId);
                    }
                    return new GirlStatusUpdateResult(Applied: true, VisibilityChanged: false);

                case GirlAttribute.Hidden:
                    Hide(girlId);
                    return new GirlStatusUpdateResult(Applied: true, VisibilityChanged: true);

                case GirlAttribute.Legend:
                    if (isLegend)
                    {
                        _statusRepository.UnMarkAsLegend(girlId);
                    }
                    else
                    {
                        _statusRepository.MarkAsLegend(girlId);
                    }
                    return new GirlStatusUpdateResult(Applied: true, VisibilityChanged: false);

                default:
                    return new GirlStatusUpdateResult(Applied: false, VisibilityChanged: false);
            }
        }
    }
}
