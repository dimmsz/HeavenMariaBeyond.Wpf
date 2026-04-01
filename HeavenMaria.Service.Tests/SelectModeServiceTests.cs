using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Entities.Sub;
using HeavenMaria.Domain.Enums;
using HeavenMaria.Domain.Interfaces;

namespace HeavenMaria.Service.Tests;

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

    [Fact]
    public void UpdateHiddenByCurrentState_True_CallsUnhide()
    {
        var girlRepository = new StubGirlWithStatusRepository(Array.Empty<GirlWithStatusDto>());
        var statusRepository = new SpyGirlStatusRepository();
        var sut = new SelectModeService(girlRepository, statusRepository);

        sut.UpdateHiddenByCurrentState("G1", currentlyHidden: true);

        Assert.Equal(1, statusRepository.UnhideCalls);
        Assert.Equal(0, statusRepository.HideCalls);
    }

    [Fact]
    public void UpdateHiddenByCurrentState_False_CallsHide()
    {
        var girlRepository = new StubGirlWithStatusRepository(Array.Empty<GirlWithStatusDto>());
        var statusRepository = new SpyGirlStatusRepository();
        var sut = new SelectModeService(girlRepository, statusRepository);

        sut.UpdateHiddenByCurrentState("G1", currentlyHidden: false);

        Assert.Equal(1, statusRepository.HideCalls);
        Assert.Equal(0, statusRepository.UnhideCalls);
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
        public int HideCalls { get; private set; }
        public int UnhideCalls { get; private set; }

        public Status GetStatus(string girlId) => new();

        public int Hide(string girlId)
        {
            HideCalls++;
            return 1;
        }

        public int Unhide(string girlId)
        {
            UnhideCalls++;
            return 1;
        }

        public int Favorite(string girlId) => 1;
        public int Unfavorite(string girlId) => 1;
        public int MarkAsLegend(string girlId) => 1;
        public int UnMarkAsLegend(string girlId) => 1;
    }
}
