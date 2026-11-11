using Dapper;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace HeavenMariaBeyond.Infrastructure.MariaDB
{
    public class WriteSQL
    {
        private readonly IConnectionStringProvider _connectionStringProvider;

        public WriteSQL(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public void SetGirlsHidden(IReadOnlyCollection<string> girlIds, bool hidden)
        {
            if (girlIds.Count == 0)
            {
                return;
            }

            using var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString());
            connection.Open();
            using var transaction = connection.BeginTransaction();
            connection.Execute(GetSetAttributeSql(GirlAttribute.Hidden),
                girlIds.Distinct().Order(StringComparer.Ordinal).Select(girlId => new { girlId, value = hidden }),
                transaction);
            transaction.Commit();
        }

        public int UpdateGirlAttribute(string girlId, GirlAttribute attribute, bool value)
        {
            using var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString());
            int affectedRows = connection.Execute(GetSetAttributeSql(attribute), new { girlId, value });
            return affectedRows > 0 ? 1 : 0;
        }

        private static string GetSetAttributeSql(GirlAttribute attribute)
        {
            string column = attribute switch
            {
                GirlAttribute.Favorite => "Favorite",
                GirlAttribute.Hidden => "Hidden",
                GirlAttribute.Legend => "Legend",
                _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, "Unsupported girl attribute.")
            };

            return $"""
                INSERT INTO GirlStatus (GirlId, Favorite, Hidden, Legend)
                VALUES (@girlId, {InsertValue(GirlAttribute.Favorite)}, {InsertValue(GirlAttribute.Hidden)}, {InsertValue(GirlAttribute.Legend)})
                ON DUPLICATE KEY UPDATE {column} = @value;
                """;

            string InsertValue(GirlAttribute flag) => attribute == flag ? "@value" : "FALSE";
        }

        public int DeleteDiary(Diary diary)
        {
            const string sql = "DELETE FROM Diaries WHERE diary_code = @DiaryCode;";
            using var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString());
            return connection.Execute(sql, new { diary.DiaryCode });
        }
    }
}
