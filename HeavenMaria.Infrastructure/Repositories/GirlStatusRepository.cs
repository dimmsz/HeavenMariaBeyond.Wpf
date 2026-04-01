using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Enums;
using HeavenMaria.Domain.Interfaces;
using HeavenMaria.Infrastructure.MariaDB;

namespace HeavenMaria.Infrastructure.Repositories
{
    public class GirlStatusRepository : IGirlStatusRepository
    {
        private readonly ReadSQL _readSQL;
        private readonly WriteSQL _writeSQL;

        public GirlStatusRepository(IConnectionStringProvider connectionStringProvider,
            ICompressionService compressionService)
        {
            _readSQL = new ReadSQL(connectionStringProvider, compressionService);
            _writeSQL = new WriteSQL(connectionStringProvider, compressionService);
        }

        public Status GetStatus(string girlId)
        {
            return _readSQL.GetStatus(girlId);
        }

        public int Favorite(string girlId)
        {
            var currentStatus = _readSQL.GetStatus(girlId);
            if (currentStatus.Favorite == false)
            {
                int result = _writeSQL.ToggleGirlAttribute(girlId, GirlAttribute.Favorite);
                return result;
            }
            else
            {
                return 0;
            }
        }

        public int Unfavorite(string girlId)
        {
            var currentStatus = _readSQL.GetStatus(girlId);
            if (currentStatus.Favorite == true)
            {
                var result = _writeSQL.ToggleGirlAttribute(girlId, GirlAttribute.Favorite);
                return result;
            }
            else
            {
                return 0;
            }
        }

        public int Hide(string girlId)
        {
            var currentStatus = _readSQL.GetStatus(girlId);
            if (currentStatus.Hidden == false)
            {
                var result = _writeSQL.ToggleGirlAttribute(girlId, GirlAttribute.Hidden);
                return result;
            }
            else
            {
                return 0;
            }
        }

        public int Unhide(string girlId)
        {
            var currentStatus = _readSQL.GetStatus(girlId);
            if (currentStatus.Hidden == true)
            {
                var result = _writeSQL.ToggleGirlAttribute(girlId, GirlAttribute.Hidden);
                return result;
            }
            else
            {
                return 0;
            }
        }

        public int MarkAsLegend(string girlId)
        {
            var currentStatus = _readSQL.GetStatus(girlId);
            if (currentStatus.Legend == false)
            {
                var result = _writeSQL.ToggleGirlAttribute(girlId, GirlAttribute.Legend);
                return result;
            }
            else
            {
                return 0;
            }
        }

        public int UnMarkAsLegend(string girlId)
        {
            var currentStatus = _readSQL.GetStatus(girlId);
            if (currentStatus.Legend == true)
            {
                var result = _writeSQL.ToggleGirlAttribute(girlId, GirlAttribute.Legend);
                return result;
            }
            else
            {
                return 0;
            }
        }
    }
}
