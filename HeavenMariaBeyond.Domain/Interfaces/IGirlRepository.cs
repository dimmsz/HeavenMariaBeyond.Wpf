using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Entities.Sub;

namespace HeavenMariaBeyond.Domain.Interfaces
{
    public interface IGirlRepository
    {
        IEnumerable<Girl> GetGirlsByKey(string key);

        Girl GetGirlById(string id);
    }
}
