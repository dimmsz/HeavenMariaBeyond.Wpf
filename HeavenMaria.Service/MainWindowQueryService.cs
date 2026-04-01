using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Enums;
using HeavenMaria.Domain.Interfaces;

namespace HeavenMaria.Service
{
    public class MainWindowQueryService : IMainWindowQueryService
    {
        private readonly IGirlWithStatusRepository _girlWithStatusRepository;
        private readonly IDiaryRepository _diaryRepository;

        public MainWindowQueryService(
            IGirlWithStatusRepository girlWithStatusRepository,
            IDiaryRepository diaryRepository)
        {
            _girlWithStatusRepository = girlWithStatusRepository;
            _diaryRepository = diaryRepository;
        }

        public IEnumerable<GirlWithStatusDto> GetVisibleGirlsSortedByName()
        {
            return _girlWithStatusRepository
                .GetVisibleGirls()
                .OrderBy(x => x.DisplayName);
        }

        public IEnumerable<Diary> GetDiariesByGirlIdDescending(string girlId)
        {
            return _diaryRepository
                .GetDiariesByGirlId(girlId)
                .OrderByDescending(x => x.DiaryDate);
        }

        public IEnumerable<DiaryOld> GetMovieDiaries(string girlId)
        {
            return _diaryRepository.GetMovieDiaries(girlId);
        }

        public IEnumerable<GirlWithStatusDto> GetGirlsByKey(GirlKeyType keyType)
        {
            return _girlWithStatusRepository.GetGirlsByKey(keyType);
        }

        public GirlWithStatusDto GetGirlById(string girlId)
        {
            return _girlWithStatusRepository.GetGirlById(girlId);
        }

        IEnumerable<DiaryOld> IMainWindowQueryService.GetDiariesByGirlIdDescending(string girlId)
        {
            return _diaryRepository
             .GetDiariesByGirlId_2(girlId)
             .OrderByDescending(x => x.DiaryDate);
        }
    }
}
