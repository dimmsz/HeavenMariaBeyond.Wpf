using HeavenMariaBeyond.Domain.Interfaces;

namespace HeavenMariaBeyond.Service
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

        public async Task<string> BuildDiaryHtml(string diaryCode, CancellationToken cancellationToken = default)
        {
            var diary = await _diaryRepository.GetDiaryByDiaryCodeAsync(diaryCode, cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Diary not found. DiaryCode={diaryCode}");

            return await _diaryHtmlGenerator.MakeDiaryHtml(diary, cancellationToken).ConfigureAwait(false);
        }
    }
}
