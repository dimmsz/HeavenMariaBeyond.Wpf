using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Sub;
using HeavenMaria.Domain.Enums;

namespace HeavenMaria.Domain.Interfaces
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
