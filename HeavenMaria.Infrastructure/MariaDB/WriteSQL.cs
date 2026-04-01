using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Enums;
using HeavenMaria.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace HeavenMaria.Infrastructure.MariaDB
{
    public class WriteSQL
    {
        private readonly string _conStr = string.Empty;
        private readonly ICompressionService _compressionService;

        public WriteSQL(IConnectionStringProvider connectionStringProvider,
            ICompressionService compressionService)
        {
            _conStr = connectionStringProvider.GetConnectionString();
            _compressionService = compressionService;
        }

        /// <summary>
        /// 任意の嬢を非表示にする
        /// </summary>
        /// <param name="girl"></param>
        public void UpdateGirlHidden(string girlId)
        {
            string sql =
$@"UPDATE Status SET Hidden = true WHERE GirlID = '{girlId}';";

            using (var connection = new MySqlConnection(_conStr))
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 任意の嬢のステータスを反転させる
        /// </summary>
        /// <param name="girl"></param>
        public int ToggleGirlAttribute(string girlId, GirlAttribute attribute)
        {
            string sql = $@"UPDATE Status SET {attribute} = NOT {attribute} WHERE GirlID = '{girlId}';";

            using (var connection = new MySqlConnection(_conStr))
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public int DeleteDiary(DiaryOld diary)
        {
            string sql = $@"DELETE FROM DiariesTable WHERE DiaryCode = '{diary.DiaryCode}';";

            using (var connection = new MySqlConnection(_conStr))
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public int DeleteDiary(Diary diary)
        {
            string sql = $@"DELETE FROM Diaries WHERE DiaryCode = '{diary.DiaryCode}';";

            using (var connection = new MySqlConnection(_conStr))
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
    }
}
