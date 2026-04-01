using HeavenMaria.Domain.Interfaces;

namespace HeavenMaria.Service
{
    public class DiaryDisplayService : IDiaryDisplayService
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IDiaryHtmlGenerator _diaryHtmlGenerator;

        public DiaryDisplayService(IDiaryRepository diaryRepository, IDiaryHtmlGenerator diaryHtmlGenerator)
        {
            _diaryRepository = diaryRepository;
            _diaryHtmlGenerator = diaryHtmlGenerator;
        }

        public async Task<string> BuildDiaryHtml(string diaryCode)
        {
            var diary = _diaryRepository.GetDiaryByDiaryCode(diaryCode);
            return await _diaryHtmlGenerator.MakeDiaryHtml(diary);
        }
    }
}
