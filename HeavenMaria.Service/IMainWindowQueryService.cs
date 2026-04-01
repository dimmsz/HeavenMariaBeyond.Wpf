using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Enums;

namespace HeavenMaria.Service
{
    public interface IMainWindowQueryService
    {
        IEnumerable<GirlWithStatusDto> GetVisibleGirlsSortedByName();
        IEnumerable<Diary> GetDiariesByGirlIdDescending(string girlId);
        IEnumerable<DiaryOld> GetMovieDiaries(string girlId);
        IEnumerable<GirlWithStatusDto> GetGirlsByKey(GirlKeyType keyType);
        GirlWithStatusDto GetGirlById(string girlId);
    }
}
