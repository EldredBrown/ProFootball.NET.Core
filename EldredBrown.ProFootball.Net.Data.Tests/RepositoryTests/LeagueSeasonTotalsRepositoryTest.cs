using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class LeagueSeasonTotalsRepositoryTest
    {
        private readonly TestableLeagueSeasonTotalsRepository _testRepository;

        public LeagueSeasonTotalsRepositoryTest()
        {
            _testRepository = new TestableLeagueSeasonTotalsRepository();
        }

        [Fact]
        public void GetLeagueSeasonTotals_ShouldReturnLeagueSeasonTotals()
        {
            // Arrange
            var leagueId = 1;
            var seasonId = 1920;

            var expected = new LeagueSeasonTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = _testRepository.GetLeagueSeasonTotals(leagueId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedLeagueId.ShouldBe(leagueId);
            _testRepository.CapturedSeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public async Task GetLeagueSeasonTotalsAsync_ShouldReturnLeagueSeasonTotals()
        {
            // Arrange
            var leagueId = 1;
            var seasonId = 1920;

            var expected = new LeagueSeasonTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = await _testRepository.GetLeagueSeasonTotalsAsync(leagueId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedLeagueId.ShouldBe(leagueId);
            _testRepository.CapturedSeasonId.ShouldBe(seasonId);
        }

        /// <summary>
        /// Testable subclass that overrides the protected EF Core call so it can be
        /// exercised without a real database or relational provider.
        /// </summary>
        private class TestableLeagueSeasonTotalsRepository : LeagueSeasonTotalsRepository
        {
            // Pass null for dbContext — the override means it is never touched in tests.
            public TestableLeagueSeasonTotalsRepository() : base(null!) { }

            public int CapturedLeagueId { get; private set; }
            public int CapturedSeasonId { get; private set; }

            public LeagueSeasonTotals TotalsToReturn { get; set; }
                = new LeagueSeasonTotals { };

            protected override LeagueSeasonTotals? ExecuteGetLeagueSeasonTotals(int leagueId, int seasonId)
            {
                CapturedLeagueId = leagueId;
                CapturedSeasonId = seasonId;
                return TotalsToReturn;
            }

            protected override async Task<LeagueSeasonTotals?> ExecuteGetLeagueSeasonTotalsAsync(int leagueId, int seasonId)
            {
                CapturedLeagueId = leagueId;
                CapturedSeasonId = seasonId;
                return await Task.FromResult(TotalsToReturn);
            }
        }
    }
}
