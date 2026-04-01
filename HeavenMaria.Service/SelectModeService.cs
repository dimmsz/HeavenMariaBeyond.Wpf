using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Interfaces;

namespace HeavenMaria.Service
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

        public void UpdateHiddenByCurrentState(string girlId, bool currentlyHidden)
        {
            if (currentlyHidden)
            {
                _girlStatusRepository.Unhide(girlId);
                return;
            }

            _girlStatusRepository.Hide(girlId);
        }
    }
}
