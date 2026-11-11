using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Sub;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;
using HeavenMariaBeyond.Infrastructure.MariaDB;

namespace HeavenMariaBeyond.Infrastructure.Repositories
{
    public class GirlWithStatusRepository : IGirlWithStatusRepository
    {
        private readonly ReadSQL _readSQL;

        public GirlWithStatusRepository(IConnectionStringProvider connectionStringProvider)
        {
            _readSQL = new ReadSQL(connectionStringProvider);
        }

        public IEnumerable<GirlWithStatusDto> GetVisibleGirls()
        {
            var filter = new GirlStatusFilter
            {
                Hidden = false
            };

            return _readSQL.GetGirlsWithFilter(filter);
        }

        public IEnumerable<GirlWithStatusDto> GetAllGirls()
        {
            var filter = new GirlStatusFilter { };

            var girlsWithStatus = _readSQL.GetGirlsWithFilter(filter);

            return girlsWithStatus;
        }

        public IEnumerable<GirlWithStatusDto> GetGirlsByKey(GirlKeyType girlKeyType)
        {
            switch (girlKeyType)
            {
                case GirlKeyType.All:
                    return GetAllGirls()
                        .OrderBy(x => x.DisplayName);
                case GirlKeyType.NotHidden:
                    return GetVisibleGirls()
                        .OrderBy(x => x.DisplayName);
                case GirlKeyType.Favorite:
                    var filter = new GirlStatusFilter
                    {
                        Favorite = true
                    };
                    return _readSQL.GetGirlsWithFilter(filter)
                        .OrderBy(x => x.DisplayName);
                case GirlKeyType.Legend:
                    filter = new GirlStatusFilter
                    {
                        Legend = true
                    };
                    return _readSQL.GetGirlsWithFilter(filter)
                        .OrderBy(x => x.DisplayName);
                default:
                    return Enumerable.Empty<GirlWithStatusDto>();
            }
        }

        public IEnumerable<GirlWithStatusDto> GetGirlsWithFilter(GirlStatusFilter filter)
        {
            var girlsWithStatus = _readSQL.GetGirlsWithFilter(filter);
            return girlsWithStatus;
        }

        public GirlWithStatusDto GetGirlById(string girlId)
        {
            var girlsWithStatus = _readSQL.GetGirlById(girlId);
            return girlsWithStatus;
        }
    }
}
