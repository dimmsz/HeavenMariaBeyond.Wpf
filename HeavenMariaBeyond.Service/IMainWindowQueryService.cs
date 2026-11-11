using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;

namespace HeavenMariaBeyond.Service
{
    public interface IMainWindowQueryService
    {
        IEnumerable<GirlWithStatusDto> GetVisibleGirlsSortedByName();
        IEnumerable<Diary> GetDiaryPartsByGirlIdDescending(string girlId);
        IEnumerable<Diary> GetMovieDiaries(string girlId);
        IEnumerable<GirlWithStatusDto> GetGirlsByKey(GirlKeyType keyType);
        GirlWithStatusDto GetGirlById(string girlId);
    }
}
