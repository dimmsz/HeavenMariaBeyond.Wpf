using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Interfaces;

namespace HeavenMariaBeyond.Service
{
    public class SelectModeService : ISelectModeService
    {
        private readonly IGirlWithStatusRepository _girlWithStatusRepository;
        private readonly IGirlStatusRepository _girlStatusRepository;

        public SelectModeService(
            IGirlWithStatusRepository girlWithStatusRepository,
            IGirlStatusRepository girlStatusRepository)
        {
            _girlWithStatusRepository = girlWithStatusRepository;
            _girlStatusRepository = girlStatusRepository;
        }

        public IEnumerable<GirlWithStatusDto> GetAllGirlsSortedByName()
        {
            return _girlWithStatusRepository
                .GetAllGirls()
                .OrderBy(x => x.DisplayName);
        }

        public void SetHidden(IReadOnlyCollection<string> girlIds, bool hidden)
        {
            _girlStatusRepository.SetHidden(girlIds, hidden);
        }
    }
}
