using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Entities.Sub;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;

namespace HeavenMariaBeyond.Service.Tests;

public class SelectModeServiceTests
{
    [Fact]
    public void GetAllGirlsSortedByName_ReturnsSortedCollection()
    {
        var girlRepository = new StubGirlWithStatusRepository(new[]
        {
            CreateDto("2", "Mio", hidden: false),
            CreateDto("1", "Aoi", hidden: true),
            CreateDto("3", "Rei", hidden: false),
        });
        var statusRepository = new SpyGirlStatusRepository();
        var sut = new SelectModeService(girlRepository, statusRepository);

        var result = sut.GetAllGirlsSortedByName().ToList();

        Assert.Equal(new[] { "Aoi", "Mio", "Rei" }, result.Select(x => x.DisplayName));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SetHidden_ForwardsTheDesiredStateAsOneBatch(bool hidden)
    {
        var girlRepository = new StubGirlWithStatusRepository(Array.Empty<GirlWithStatusDto>());
        var statusRepository = new SpyGirlStatusRepository();
        var sut = new SelectModeService(girlRepository, statusRepository);

        sut.SetHidden(new[] { "G1", "G2" }, hidden);

        var update = Assert.Single(statusRepository.Updates);
        Assert.Equal(new[] { "G1", "G2" }, update.Ids);
        Assert.Equal(hidden, update.Hidden);
    }

    [Fact]
    public void SetHidden_PropagatesPersistenceFailure()
    {
        var girlRepository = new StubGirlWithStatusRepository(Array.Empty<GirlWithStatusDto>());
        var failure = new InvalidOperationException("Save failed");
        var statusRepository = new SpyGirlStatusRepository { Failure = failure };
        var sut = new SelectModeService(girlRepository, statusRepository);

        Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => sut.SetHidden(new[] { "G1" }, true)));
    }

    private static GirlWithStatusDto CreateDto(string girlId, string displayName, bool hidden)
    {
        var girl = new Girl
        {
            GirlId = girlId,
            DisplayName = displayName,
            ProfileUrl = "https://example.com",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        var status = new Status
        {
            GirlId = girlId,
            UpdatedAt = DateTimeOffset.UtcNow,
            Favorite = false,
            Hidden = hidden,
            Legend = false,
            ActivityStatus = ActivityStatus.Normal,
        };

        return new GirlWithStatusDto(girl, status);
    }

    private sealed class StubGirlWithStatusRepository : IGirlWithStatusRepository
    {
        private readonly IEnumerable<GirlWithStatusDto> _girls;

        public StubGirlWithStatusRepository(IEnumerable<GirlWithStatusDto> girls)
        {
            _girls = girls;
        }

        public IEnumerable<GirlWithStatusDto> GetAllGirls() => _girls;
        public IEnumerable<GirlWithStatusDto> GetGirlsWithFilter(GirlStatusFilter filter) => _girls;
        public IEnumerable<GirlWithStatusDto> GetGirlsByKey(GirlKeyType girlKeyType) => _girls;
        public IEnumerable<GirlWithStatusDto> GetVisibleGirls() => _girls.Where(x => !x.Hidden);
        public GirlWithStatusDto GetGirlById(string girlId) => _girls.First(x => x.GirlId == girlId);
    }

    private sealed class SpyGirlStatusRepository : IGirlStatusRepository
    {
        public List<(IReadOnlyCollection<string> Ids, bool Hidden)> Updates { get; } = new();
        public Exception? Failure { get; init; }

        public Status GetStatus(string girlId) => new();

        public void SetHidden(IReadOnlyCollection<string> girlIds, bool hidden)
        {
            if (Failure != null)
                throw Failure;
            Updates.Add((girlIds, hidden));
        }

        public int Hide(string girlId) => 1;
        public int Unhide(string girlId) => 1;
        public int Favorite(string girlId) => 1;
        public int Unfavorite(string girlId) => 1;
        public int MarkAsLegend(string girlId) => 1;
        public int UnMarkAsLegend(string girlId) => 1;
    }
}
