using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Sub;
using HeavenMariaBeyond.Domain.Enums;

namespace HeavenMariaBeyond.Domain.Interfaces
{
    public interface IGirlWithStatusRepository
    {
        IEnumerable<GirlWithStatusDto> GetAllGirls();
        IEnumerable<GirlWithStatusDto> GetGirlsWithFilter(GirlStatusFilter filter);
        IEnumerable<GirlWithStatusDto> GetGirlsByKey(GirlKeyType girlKeyType);
        IEnumerable<GirlWithStatusDto> GetVisibleGirls();

        GirlWithStatusDto GetGirlById(string girlId);
    }
}
