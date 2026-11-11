using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Interfaces;
using HeavenMariaBeyond.Infrastructure.MariaDB;

namespace HeavenMariaBeyond.Infrastructure.Repositories
{
    public class DiaryRepository : IDiaryRepository
    {
        private readonly ReadSQL _readSQL;
        private readonly WriteSQL _writeSQL;

        public DiaryRepository(IConnectionStringProvider connectionStringProvider)
        {
            _readSQL = new ReadSQL(connectionStringProvider);
            _writeSQL = new WriteSQL(connectionStringProvider);
        }

        public int DeleteDiary(Diary diary)
        {
            var deletedCount = _writeSQL.DeleteDiary(diary);
            return deletedCount;
        }

        public IEnumerable<Diary> GetDiariesByGirlId(string girlId)
        {
            var diaries = _readSQL.GetDiariesByGirlId(girlId);
            return diaries;
        }

        public IEnumerable<Diary> GetDiariesWithoutBody(string girlId)
        {
            var diaries = _readSQL.GetDiariesWithoutDiaryBody(girlId);
            return diaries;
        }

        public Task<Diary?> GetDiaryByDiaryCodeAsync(string diaryCode, CancellationToken cancellationToken)
            => _readSQL.GetDiaryByDiaryCodeAsync(diaryCode, cancellationToken);

        public IEnumerable<Diary> GetMovieDiaries(string girlId)
        {
            var diaries = _readSQL.GetMovieDiaries(girlId);
            return diaries;
        }
    }
}
