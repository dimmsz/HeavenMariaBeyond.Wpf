using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Interfaces;
using HeavenMaria.Infrastructure.MariaDB;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.NetworkInformation;

namespace HeavenMaria.Infrastructure.Repositories
{
    public class DiaryRepository : IDiaryRepository
    {
        private readonly ReadSQL _readSQL;
        private readonly WriteSQL _writeSQL;

        public DiaryRepository(IConnectionStringProvider connectionStringProvider,
            ICompressionService compressionService)
        {
            _readSQL = new ReadSQL(connectionStringProvider, compressionService);
            _writeSQL = new WriteSQL(connectionStringProvider, compressionService);
        }

        public int DeleteDiary(DiaryOld diary)
        {
            var deletedCount = _writeSQL.DeleteDiary(diary);
            return deletedCount;
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

        public IEnumerable<DiaryOld> GetDiariesWithoutBody(string girlId)
        {
            var diaries = _readSQL.GetDiariesWithoutDiaryBody(girlId);
            return diaries;
        }

        public DiaryOld GetDiaryByDiaryCode(string diaryCode)
        {
            var diary = _readSQL.GetDiaryByDiaryCode(diaryCode);
            return diary;
        }

        public IEnumerable<DiaryOld> GetMovieDiaries(string girlId)
        {
            var diaries = _readSQL.GetMovieDiaries(girlId);
            return diaries;
        }

        IEnumerable<DiaryOld> IDiaryRepository.GetDiariesByGirlId_2(string girlId)
        {
            throw new NotImplementedException();
        }

        DiaryOld IDiaryRepository.GetDiaryByDiaryCode(string diaryCode)
        {
            var diary = _readSQL.GetDiaryByDiaryCode(diaryCode);
            return diary;
        }
    }
}
