using Dapper;
using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Entities.Sub;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;
using MySql.Data.MySqlClient;

namespace HeavenMariaBeyond.Infrastructure.MariaDB
{
    public class ReadSQL
    {
        private const string DiaryListColumns =
            "diary_date, diary_code, diary_title, diary_url, girl_id, diary_image_list";
        private const int RecentDiaryMonths = 3;
        private static readonly TimeSpan DiaryListTimeout = TimeSpan.FromMinutes(6);
        private const string GirlsWithStatusSql = """
            SELECT g.*, s.GirlId, s.Favorite, s.Hidden, s.Legend
            FROM Girls g
            LEFT JOIN GirlStatus s ON g.GirlId = s.GirlId
            """;
        private readonly IConnectionStringProvider _connectionStringProvider;

        public ReadSQL(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public GirlWithStatusDto GetGirlById(string girlId)
        {
            return QueryGirlsWithStatus(GirlsWithStatusSql + " WHERE g.GirlId = @girlId", new { girlId })
                .SingleOrDefault() ?? throw new InvalidOperationException($"Girl not found. GirlId={girlId}");
        }

        public IEnumerable<GirlWithStatusDto> GetAllGirls()
        {
            return GetGirlsWithFilter(new GirlStatusFilter());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="girlId"></param>
        /// <returns></returns>
        public IEnumerable<Diary> GetDiariesByGirlId(string girlId)
        {
            string sql = $"SELECT {DiaryListColumns} FROM Diaries WHERE girl_id = @girlId;";

            using (var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                return connection.Query<Diary>(sql, new { girlId }, commandTimeout: (int)DiaryListTimeout.TotalSeconds);
            }
        }

        /// <summary>
        /// 任意の嬢のDiaryEntity(DiaryBody除く)のコレクションを返す
        /// </summary>
        /// <param name="girlId"></param>
        /// <returns></returns>
        public IEnumerable<Diary> GetDiariesWithoutDiaryBody(string girlId)
        {
            const string sql = "SELECT diary_date, diary_code, diary_title, frame_type FROM Diaries WHERE girl_id = @girlId;";

            using (var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                return connection.Query<Diary>(sql, new { girlId });
            }
        }

        /// <summary>
        /// 日記コードからDiaryEntityを返す
        /// </summary>
        /// <param name="diaryCode"></param>
        /// <returns></returns>
        public async Task<Diary?> GetDiaryByDiaryCodeAsync(string diaryCode, CancellationToken cancellationToken)
        {
            const string sql = "SELECT * FROM Diaries WHERE diary_code = @diaryCode;";
            using var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString());
            return await connection.QuerySingleOrDefaultAsync<Diary>(
                new CommandDefinition(sql, new { diaryCode }, cancellationToken: cancellationToken))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="girlID"></param>
        /// <returns></returns>
        public IEnumerable<Diary> GetLatestDiariesByGirlID(string girlID)
        {
            const string sql =
                "SELECT * FROM Diaries WHERE diary_date > CURDATE() - INTERVAL @months MONTH AND girl_id = @girlID;";

            using (var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                return connection.Query<Diary>(sql, new { months = RecentDiaryMonths, girlID });
            }
        }

        public IEnumerable<Diary> GetMovieDiaries(string girlId)
        {
            string sql = $"SELECT {DiaryListColumns}, frame_type FROM Diaries WHERE frame_type = @frameType AND girl_id = @girlId;";
            using (var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                return connection.Query<Diary>(sql, new { frameType = nameof(Domain.DisplayHtml.DiaryEnums.FrameType.movieframe), girlId });
            }
        }

        public Status GetStatus(string girlId)
        {
            string sql = $@"SELECT GirlID, Favorite, Hidden, Legend
                            FROM GirlStatus
                            WHERE GirlID = @girlId;";
            using (var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                return connection.QuerySingle<Status>(sql, new { girlId });
            }
        }

        public IEnumerable<GirlWithStatusDto> GetGirlsWithFilter(GirlStatusFilter filter)
        {
            return QueryGirlsWithStatus(GirlsWithStatusSql).Where(g =>
                (filter.Favorite == null || g.Favorite == filter.Favorite) &&
                (filter.Hidden == null || g.Hidden == filter.Hidden) &&
                (filter.Legend == null || g.Legend == filter.Legend));
        }

        private IEnumerable<GirlWithStatusDto> QueryGirlsWithStatus(string sql, object? parameters = null)
        {
            using var connection = new MySqlConnection(_connectionStringProvider.GetConnectionString());
            return connection.Query<Girl, Status, GirlWithStatusDto>(sql,
                (girl, status) => new GirlWithStatusDto(girl, status ?? new Status { GirlId = girl.GirlId }),
                parameters, splitOn: "GirlId");
        }
    }
}
