using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class SeasonStandingsRepositoryTest
    {
        private readonly TestableSeasonStandingsRepository _testRepository;

        public SeasonStandingsRepositoryTest()
        {
            _testRepository = new TestableSeasonStandingsRepository();
        }

        [Fact]
        public void GetSeasonStandings_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnSeasonStandings()
        {
            // Arrange
            var expected = new List<StandingsTeamSeason>
            {
                new() { }
            };
            _testRepository.SeasonStandingsToReturn = expected;

            var seasonId = 1920;

            // Act
            var result = _testRepository.GetSeasonStandings(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonId);
        }

        [Fact]
        public async Task GetSeasonStandingsAsync_ShouldSucceed()
        {
            // Arrange
            var expected = new List<StandingsTeamSeason>
            {
                new() { }
            };
            _testRepository.SeasonStandingsToReturn = expected;

            var seasonId = 1920;

            // Act
            var result = await _testRepository.GetSeasonStandingsAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonId);
        }

        /// <summary>
        /// Testable subclass that overrides the protected EF Core call so it can be
        /// exercised without a real database or relational provider.
        /// </summary>
        private class TestableSeasonStandingsRepository : SeasonStandingsRepository
        {
            // Pass null for dbContext — the override means it is never touched in tests.
            public TestableSeasonStandingsRepository() : base(null!) { }

            public int CapturedSeasonYear { get; private set; }

            public IEnumerable<StandingsTeamSeason> SeasonStandingsToReturn { get; set; }
                = new List<StandingsTeamSeason>();

            protected override IEnumerable<StandingsTeamSeason>
                ExecuteGetSeasonStandings(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return SeasonStandingsToReturn;
            }

            protected override async Task<IEnumerable<StandingsTeamSeason>>
                ExecuteGetSeasonStandingsAsync(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(SeasonStandingsToReturn);
            }
        }
    }
}
