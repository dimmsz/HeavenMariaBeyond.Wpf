using HeavenMariaBeyond.Domain.Entities.Dtos;

namespace HeavenMariaBeyond.Service
{
    public interface ISelectModeService
    {
        IEnumerable<GirlWithStatusDto> GetAllGirlsSortedByName();
        void SetHidden(IReadOnlyCollection<string> girlIds, bool hidden);
    }
}
