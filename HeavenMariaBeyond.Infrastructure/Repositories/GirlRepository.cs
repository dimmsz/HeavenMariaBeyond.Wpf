using Dapper;
using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Entities.Sub;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace HeavenMariaBeyond.Domain.Repositories
{
    public class GirlRepository : IGirlRepository
    {
        private readonly string _conStr;

        public GirlRepository(IConnectionStringProvider connectionStringProvider)
        {
            _conStr = connectionStringProvider.GetConnectionString();
        }

        public IEnumerable<Girl> GetAllGirls()
        {
            return GetGirlsByKey("All");
        }

        public Girl GetGirlById(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Girl> GetNotHiddenGirls()
        {
            return GetGirlsByKey("NotHidden");
        }

        private IEnumerable<Girl> GetGirlsByKey(string key)
        {
            var girlKey = Enum.Parse<GirlKeyType>(key);

            string sql = string.Empty;
            switch (girlKey)
            {
                case GirlKeyType.All:
                    sql = $@"SELECT * FROM GirlsTable INNER JOIN GirlStatus ON GirlsTable.GirlID = GirlStatus.GirlID Order By GirlNameRaw ASC;";
                    break;
                case GirlKeyType.NotHidden:
                    sql = $@"SELECT * FROM GirlsTable INNER JOIN GirlStatus ON GirlsTable.GirlID = GirlStatus.GirlID WHERE Hidden = false Order By GirlNameRaw ASC;";
                    break;
                case GirlKeyType.Favorite:
                    sql = $@"SELECT * FROM GirlsTable INNER JOIN GirlStatus ON GirlsTable.GirlID = GirlStatus.GirlID WHERE Favorite = true Order By GirlNameRaw ASC;";
                    break;
                case GirlKeyType.Legend:
                    sql = $@"SELECT * FROM GirlsTable INNER JOIN GirlStatus ON GirlsTable.GirlID = GirlStatus.GirlID WHERE Legend = true Order By GirlNameRaw ASC;";
                    break;
                default: break;
            }

            using (var connection = new MySqlConnection(_conStr))
            {
                return connection.Query<Girl>(sql);
            }
        }

        IEnumerable<Girl> IGirlRepository.GetGirlsByKey(string key)
        {
            return GetGirlsByKey(key);
        }
    }
}
