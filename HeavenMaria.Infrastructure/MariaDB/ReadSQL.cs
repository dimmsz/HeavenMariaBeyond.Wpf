using Dapper;
using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Entities.Sub;
using HeavenMaria.Domain.Enums;
using HeavenMaria.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace HeavenMaria.Infrastructure.MariaDB
{
    public class ReadSQL
    {
        private readonly string _conStr = string.Empty;
        private readonly ICompressionService _compressionService;

        public ReadSQL(IConnectionStringProvider connectionStringProvider,
            ICompressionService compressionService)
        {
            _conStr = connectionStringProvider.GetConnectionString();
            _compressionService = compressionService;
        }

        public GirlWithStatusDto GetGirlById(string girlId)
        {
            // 1. テーブル名を明示して曖昧さを回避 (Girls.GirlId)
            // 2. パラメータ（@Id）を使用してSQLインジェクションを防止
            string sql = @"SELECT * FROM Girls INNER JOIN Status ON Girls.GirlId = Status.GirlId WHERE Girls.GirlId = @Id;";

            using (var connection = new MySqlConnection(_conStr))
            {
                // 3. パラメータを匿名オブジェクトで渡す
                // 4. 見つからない場合に備えてOrDefaultを使用
                return connection.QuerySingleOrDefault<GirlWithStatusDto>(sql, new { Id = girlId })
                    ?? throw new InvalidOperationException($"Girl not found. GirlId={girlId}");
            }
        }

        public IEnumerable<GirlWithStatusDto> GetAllGirls()
        {
            string sql = $@"SELECT * FROM Girls INNER JOIN Status ON Girls.GirlId = Status.GirlID;";
            using (var connection = new MySqlConnection(_conStr))
            {
                var girlsWithStatus = connection.Query<GirlWithStatusDto>(sql);
                return girlsWithStatus;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="girlId"></param>
        /// <returns></returns>
        public IEnumerable<DiaryOld> GetDiariesByGirlId_2(string girlId)
        {
            string sql = $@"SELECT DiaryDate,DiaryCode,DiaryTitle,DiaryUrl,GirlId FROM DiariesTable WHERE GirlID = '{girlId}';";

            using (var connection = new MySqlConnection(_conStr))
            {
                return connection.Query<DiaryOld>(sql, null, null, true, 360);
            }
        }

        public IEnumerable<Diary> GetDiariesByGirlId(string girlId)
        {
            string sql = $@"SELECT DiaryDate,DiaryCode,DiaryTitle,DiaryUrl,GirlId FROM Diaries WHERE GirlID = '{girlId}';";

            using (var connection = new MySqlConnection(_conStr))
            {
                return connection.Query<Diary>(sql, null, null, true, 360);
            }
        }

        /// <summary>
        /// 任意の嬢のDiaryEntity(DiaryBody除く)のコレクションを返す
        /// </summary>
        /// <param name="girlId"></param>
        /// <returns></returns>
        public IEnumerable<DiaryOld> GetDiariesWithoutDiaryBody(string girlId)
        {
            string sql = $@"SELECT DiaryDate,DiaryCode,DiaryTitle,FrameTypeString FROM DiariesTable WHERE GirlID = '{girlId}';";

            using (var connection = new MySqlConnection(_conStr))
            {
                return connection.Query<DiaryOld>(sql);
            }
        }

        /// <summary>
        /// 日記コードからDiaryEntityを返す
        /// </summary>
        /// <param name="diaryCode"></param>
        /// <returns></returns>
        public DiaryOld GetDiaryByDiaryCode(string diaryCode)
        {
            string sql = $@"SELECT * FROM DiariesTable WHERE DiaryCode = '{diaryCode}';";

            using (var connection = new MySqlConnection(_conStr))
            {
                return connection.QuerySingleOrDefault<DiaryOld>(sql);
            }
        }

        /// <summary>       
        /// 日記の削除
        /// </summary>
        /// <param name="code"></param>
        public int DeleteDiary(string code)
        {
            string sql = @"DELETE FROM DiariesTable WHERE DiaryCode = @code;";

            using (var connection = new MySqlConnection(_conStr))
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@code", code);

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="girlID"></param>
        /// <returns></returns>
        public IEnumerable<DiaryOld> GetLatestDiariesByGirlID(string girlID)
        {
            string sql =
                $@"SELECT * FROM diariestable WHERE DiaryDate > CURDATE() - INTERVAL 3 MONTH AND GirlID = '{girlID}'";

            using (var connection = new MySqlConnection(_conStr))
            {
                try
                {
                    var diary = connection.Query<DiaryOld>(sql);
                    return diary;
                }
                catch
                {
                    return new List<DiaryOld>();
                }
            }
        }

        public IEnumerable<DiaryOld> GetMovieDiaries(string girlId)
        {
            string sql = $@"SELECT * FROM DiariesTable WHERE FrameTypeString = 'movieframe' AND GirlID = '{girlId}';";
            using (var connection = new MySqlConnection(_conStr))
            {
                return connection.Query<DiaryOld>(sql);
            }
        }

        public Status GetStatus(string girlId)
        {
            string sql = $@"SELECT * FROM Status WHERE GirlID = '{girlId}';";
            using (var connection = new MySqlConnection(_conStr))
            {
                return connection.QuerySingle<Status>(sql);
            }
        }

        public IEnumerable<GirlWithStatusDto> GetGirlsWithFilter(GirlStatusFilter filter)
        {
            var girls = GetGirls();
            var allStatus = GetAllStatus();

            List<GirlWithStatusDto> query = girls.Join(
                allStatus,
                g => g.GirlId,      // 外側（girls）のキー
                s => s.GirlId,      // 内側（status）のキー
                (g, s) => new GirlWithStatusDto(g, s)).ToList();


            var filteredGirls = query.Where(g =>
                (filter.Favorite == null || g.Status.Favorite == filter.Favorite) &&
                (filter.Hidden == null || g.Status.Hidden == filter.Hidden) &&
                (filter.Legend == null || g.Status.Legend == filter.Legend)).ToList();

            return filteredGirls;
        }

        private IEnumerable<Girl> GetGirls()
        {
            string sql = @"SELECT * FROM Girls";
            using (var connection = new MySqlConnection(_conStr))
            {
                connection.Open();

                var girls = connection.Query<Girl>(sql);
                return girls;
            }
        }

        private IEnumerable<Status> GetAllStatus()
        {
            string sql = @"SELECT * FROM Status";
            using (var connection = new MySqlConnection(_conStr))
            {
                connection.Open();

                var allStatus = connection.Query<Status>(sql);
                return allStatus;
            }
        }
    }
}
