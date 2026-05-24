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
        public void GetSeasonStandings_ShouldSucceed()
        {
            // Arrange
            var seasonYear = 1920;

            var expected = new List<SeasonTeamStanding>
            {
                new SeasonTeamStanding { }
            };

            _testRepository.SeasonStandingsToReturn = expected;

            // Act
            var result = _testRepository.GetSeasonStandings(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetSeasonStandingsAsync_ShouldSucceed()
        {
            // Arrange
            var seasonYear = 1920;

            var expected = new List<SeasonTeamStanding>
            {
                new SeasonTeamStanding { }
            };

            _testRepository.SeasonStandingsToReturn = expected;

            // Act
            var result = _testRepository.GetSeasonStandings(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
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

            public IEnumerable<SeasonTeamStanding> SeasonStandingsToReturn { get; set; }
                = new List<SeasonTeamStanding>();

            protected override IEnumerable<SeasonTeamStanding>
                ExecuteGetSeasonStandings(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return SeasonStandingsToReturn;
            }

            protected override async Task<IEnumerable<SeasonTeamStanding>>
                ExecuteGetSeasonStandingsAsync(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(SeasonStandingsToReturn);
            }
        }
    }
}
