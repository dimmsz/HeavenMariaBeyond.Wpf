using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;

namespace HeavenMariaBeyond.Service.Tests;

public class MainWindowStatusServiceTests
{
    [Fact]
    public void Hide_CallsRepositoryHide()
    {
        var repository = new SpyGirlStatusRepository();
        var sut = new MainWindowStatusService(repository, new DiaryRepositoryStub());

        sut.Hide("G1");

        Assert.Equal(1, repository.HideCalls);
        Assert.Equal("G1", repository.LastGirlId);
    }

    [Fact]
    public void ReverseStatus_FavoriteTrue_CallsUnfavorite()
    {
        var repository = new SpyGirlStatusRepository();
        var sut = new MainWindowStatusService(repository, new DiaryRepositoryStub());

        var result = sut.ReverseStatus("G1", GirlAttribute.Favorite, isFavorite: true, isLegend: false);

        Assert.True(result.Applied);
        Assert.False(result.VisibilityChanged);
        Assert.Equal(1, repository.UnfavoriteCalls);
        Assert.Equal(0, repository.FavoriteCalls);
    }

    [Fact]
    public void ReverseStatus_FavoriteFalse_CallsFavorite()
    {
        var repository = new SpyGirlStatusRepository();
        var sut = new MainWindowStatusService(repository, new DiaryRepositoryStub());

        var result = sut.ReverseStatus("G1", GirlAttribute.Favorite, isFavorite: false, isLegend: false);

        Assert.True(result.Applied);
        Assert.False(result.VisibilityChanged);
        Assert.Equal(1, repository.FavoriteCalls);
        Assert.Equal(0, repository.UnfavoriteCalls);
    }

    [Fact]
    public void ReverseStatus_Hidden_CallsHide_AndMarksVisibilityChanged()
    {
        var repository = new SpyGirlStatusRepository();
        var sut = new MainWindowStatusService(repository, new DiaryRepositoryStub());

        var result = sut.ReverseStatus("G1", GirlAttribute.Hidden, isFavorite: false, isLegend: false);

        Assert.True(result.Applied);
        Assert.True(result.VisibilityChanged);
        Assert.Equal(1, repository.HideCalls);
    }

    [Fact]
    public void ReverseStatus_LegendTrue_CallsUnmarkLegend()
    {
        var repository = new SpyGirlStatusRepository();
        var sut = new MainWindowStatusService(repository, new DiaryRepositoryStub());

        var result = sut.ReverseStatus("G1", GirlAttribute.Legend, isFavorite: false, isLegend: true);

        Assert.True(result.Applied);
        Assert.False(result.VisibilityChanged);
        Assert.Equal(1, repository.UnmarkLegendCalls);
        Assert.Equal(0, repository.MarkLegendCalls);
    }

    [Fact]
    public void ReverseStatus_LegendFalse_CallsMarkLegend()
    {
        var repository = new SpyGirlStatusRepository();
        var sut = new MainWindowStatusService(repository, new DiaryRepositoryStub());

        var result = sut.ReverseStatus("G1", GirlAttribute.Legend, isFavorite: false, isLegend: false);

        Assert.True(result.Applied);
        Assert.False(result.VisibilityChanged);
        Assert.Equal(1, repository.MarkLegendCalls);
        Assert.Equal(0, repository.UnmarkLegendCalls);
    }

    [Fact]
    public void ReverseStatus_None_DoesNothing()
    {
        var repository = new SpyGirlStatusRepository();
        var sut = new MainWindowStatusService(repository, new DiaryRepositoryStub());

        var result = sut.ReverseStatus("G1", GirlAttribute.None, isFavorite: true, isLegend: true);

        Assert.False(result.Applied);
        Assert.False(result.VisibilityChanged);
        Assert.Equal(0, repository.HideCalls + repository.UnhideCalls + repository.FavoriteCalls + repository.UnfavoriteCalls + repository.MarkLegendCalls + repository.UnmarkLegendCalls);
    }

    private sealed class SpyGirlStatusRepository : IGirlStatusRepository
    {
        public int HideCalls { get; private set; }
        public int UnhideCalls { get; private set; }
        public int FavoriteCalls { get; private set; }
        public int UnfavoriteCalls { get; private set; }
        public int MarkLegendCalls { get; private set; }
        public int UnmarkLegendCalls { get; private set; }
        public string LastGirlId { get; private set; } = string.Empty;

        public Status GetStatus(string girlId) => new();
        public void SetHidden(IReadOnlyCollection<string> girlIds, bool hidden) => throw new NotSupportedException();

        public int Hide(string girlId)
        {
            HideCalls++;
            LastGirlId = girlId;
            return 1;
        }

        public int Unhide(string girlId)
        {
            UnhideCalls++;
            LastGirlId = girlId;
            return 1;
        }

        public int Favorite(string girlId)
        {
            FavoriteCalls++;
            LastGirlId = girlId;
            return 1;
        }

        public int Unfavorite(string girlId)
        {
            UnfavoriteCalls++;
            LastGirlId = girlId;
            return 1;
        }

        public int MarkAsLegend(string girlId)
        {
            MarkLegendCalls++;
            LastGirlId = girlId;
            return 1;
        }

        public int UnMarkAsLegend(string girlId)
        {
            UnmarkLegendCalls++;
            LastGirlId = girlId;
            return 1;
        }
    }
}
