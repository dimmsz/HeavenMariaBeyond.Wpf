using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Sub;
using HeavenMaria.Domain.Enums;
using HeavenMaria.Domain.Interfaces;
using HeavenMaria.Infrastructure.MariaDB;
using System.Text;

namespace HeavenMaria.Infrastructure.Repositories
{
    public class GirlWithStatusRepository : IGirlWithStatusRepository
    {
        private readonly ReadSQL _readSQL;

        public GirlWithStatusRepository(IConnectionStringProvider connectionStringProvider,
            ICompressionService compressionService)
        {
            _readSQL = new ReadSQL(connectionStringProvider, compressionService);
        }

        public IEnumerable<GirlWithStatusDto> GetVisibleGirls()
        {
            var filter = new GirlStatusFilter
            {
                Hidden = false
            };

            var girlsWithStatus = _readSQL.GetGirlsWithFilter(filter);

            return girlsWithStatus;
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
