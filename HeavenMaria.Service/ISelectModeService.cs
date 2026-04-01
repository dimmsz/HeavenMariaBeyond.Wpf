using HeavenMaria.Domain.Entities.Dtos;

namespace HeavenMaria.Service
{
    public interface ISelectModeService
    {
        IEnumerable<GirlWithStatusDto> GetAllGirlsSortedByName();
        void UpdateHiddenByCurrentState(string girlId, bool currentlyHidden);
    }
}
