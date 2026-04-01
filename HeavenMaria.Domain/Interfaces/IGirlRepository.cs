using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Entities.Sub;

namespace HeavenMaria.Domain.Interfaces
{
    public interface IGirlRepository
    {
        IEnumerable<Girl> GetGirlsByKey(string key);

        Girl GetGirlById(string id);
    }
}
