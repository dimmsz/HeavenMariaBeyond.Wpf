using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Interfaces;
using HeavenMariaBeyond.Infrastructure.Compression;
using HeavenMariaBeyond.Infrastructure.Generators;
using HeavenNetCollector.Infrastructure.Providers;
using System.IO;

namespace HeavenMariaBeyond.Service.Tests;

public class DiaryDisplayServiceTests
{
    private readonly ZstdCompressionService _compression = new();

    [Fact]
    public async Task BuildDiaryHtml_DisplaysDiaryWithoutOptionalImageMetadata()
    {
        var diary = new Diary
        {
            DiaryCode = "D1",
            GirlID = "G1",
            DiaryTitle = "A < B",
            FrameTypeString = "photoframe",
            DiaryBody = _compression.CompressToByteArray("<article><p>Diary text</p></article>")
        };

        var html = await CreateService(new DiaryRepositoryStub { Diary = diary }).BuildDiaryHtml("D1");

        Assert.Contains("Diary text", html);
        Assert.Contains("A &lt; B", html);
    }

    [Theory]
    [InlineData("diary_info_contents")]
    [InlineData("diary_detail")]
    public async Task BuildDiaryHtml_TrimsOnlyLeadingBreaksAndKeepsPhoto(string bodyClass)
    {
        var diary = new Diary
        {
            DiaryCode = "D1",
            GirlID = "G1",
            DiaryBody = _compression.CompressToByteArray($"""
                <article><div class="diary_item">
                    <div class="diary_img_contents"><p><img src="cover.jpg"></p></div>
                    <div class="{bodyClass}"><p><span><br> <br>本文の最初<br><br>次の行</span></p></div>
                </div></article>
                """)
        };

        var html = await CreateService(new DiaryRepositoryStub { Diary = diary }).BuildDiaryHtml("D1");

        Assert.Contains("<img src=\"cover.jpg\">", html);
        Assert.Contains("本文の最初<br><br>次の行</span>", html);
        Assert.DoesNotContain("<span><br>", html);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("<html><p>Failed download</p></html>")]
    public async Task BuildDiaryHtml_ReportsMissingArticleOrBody(string? body)
    {
        var diary = new Diary
        {
            DiaryCode = "D1",
            DiaryBody = body == null ? null : _compression.CompressToByteArray(body)
        };

        await Assert.ThrowsAsync<InvalidDataException>(
            () => CreateService(new DiaryRepositoryStub { Diary = diary }).BuildDiaryHtml("D1"));
    }

    [Fact]
    public async Task BuildDiaryHtml_DistinguishesMissingRowFromDatabaseFailure()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateService(new DiaryRepositoryStub()).BuildDiaryHtml("missing"));

        var failure = new IOException("Database unavailable");
        var error = await Assert.ThrowsAsync<IOException>(
            () => CreateService(new DiaryRepositoryStub { Failure = failure }).BuildDiaryHtml("D1"));
        Assert.Same(failure, error);
    }

    private DiaryDisplayService CreateService(IDiaryRepository repository)
        => new(repository, new DiaryHtmlGenerator(_compression, new PathProvider()));
}

internal sealed class DiaryRepositoryStub : IDiaryRepository
{
    public Diary? Diary { get; init; }
    public Exception? Failure { get; init; }

    public Task<Diary?> GetDiaryByDiaryCodeAsync(string diaryCode, CancellationToken cancellationToken)
        => Failure == null ? Task.FromResult(Diary) : Task.FromException<Diary?>(Failure);

    public int DeleteDiary(Diary diary) => throw new NotSupportedException();
    public IEnumerable<Diary> GetDiariesByGirlId(string girlId) => throw new NotSupportedException();
    public IEnumerable<Diary> GetDiariesWithoutBody(string girlId) => throw new NotSupportedException();
    public IEnumerable<Diary> GetMovieDiaries(string girlId) => throw new NotSupportedException();
}
