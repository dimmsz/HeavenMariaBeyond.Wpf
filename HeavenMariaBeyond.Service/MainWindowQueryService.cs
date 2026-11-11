using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;

namespace HeavenMariaBeyond.Service
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

        public IEnumerable<Diary> GetDiaryPartsByGirlIdDescending(string girlId)
        {
            return _diaryRepository
                .GetDiariesByGirlId(girlId)
                .OrderByDescending(x => x.DiaryDate);
        }

        public IEnumerable<Diary> GetMovieDiaries(string girlId)
        {
            return _diaryRepository.GetMovieDiaries(girlId).OrderByDescending(x => x.DiaryDate);
        }

        public IEnumerable<GirlWithStatusDto> GetGirlsByKey(GirlKeyType keyType)
        {
            return _girlWithStatusRepository.GetGirlsByKey(keyType);
        }

        public GirlWithStatusDto GetGirlById(string girlId)
        {
            return _girlWithStatusRepository.GetGirlById(girlId);
        }
    }
}
