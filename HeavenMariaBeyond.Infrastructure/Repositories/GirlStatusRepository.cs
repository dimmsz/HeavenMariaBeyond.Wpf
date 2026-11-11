using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;
using HeavenMariaBeyond.Infrastructure.MariaDB;

namespace HeavenMariaBeyond.Infrastructure.Repositories
{
    public class GirlStatusRepository : IGirlStatusRepository
    {
        private readonly ReadSQL _readSQL;
        private readonly WriteSQL _writeSQL;

        public GirlStatusRepository(IConnectionStringProvider connectionStringProvider)
        {
            _readSQL = new ReadSQL(connectionStringProvider);
            _writeSQL = new WriteSQL(connectionStringProvider);
        }

        public Status GetStatus(string girlId) => _readSQL.GetStatus(girlId);

        public int Favorite(string girlId) => _writeSQL.UpdateGirlAttribute(girlId, GirlAttribute.Favorite, true);
        public int Unfavorite(string girlId) => _writeSQL.UpdateGirlAttribute(girlId, GirlAttribute.Favorite, false);
        public int Hide(string girlId) => _writeSQL.UpdateGirlAttribute(girlId, GirlAttribute.Hidden, true);
        public int Unhide(string girlId) => _writeSQL.UpdateGirlAttribute(girlId, GirlAttribute.Hidden, false);
        public int MarkAsLegend(string girlId) => _writeSQL.UpdateGirlAttribute(girlId, GirlAttribute.Legend, true);
        public int UnMarkAsLegend(string girlId) => _writeSQL.UpdateGirlAttribute(girlId, GirlAttribute.Legend, false);

        public void SetHidden(IReadOnlyCollection<string> girlIds, bool hidden)
            => _writeSQL.SetGirlsHidden(girlIds, hidden);
    }
}
