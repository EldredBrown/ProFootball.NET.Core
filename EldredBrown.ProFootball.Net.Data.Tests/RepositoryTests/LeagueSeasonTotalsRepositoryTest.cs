using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Shouldly;
using Xunit;

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
            var leagueName = "NFL";
            var seasonYear = 1920;

            var expected = new LeagueSeasonTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = _testRepository.GetLeagueSeasonTotals(leagueName, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedLeagueName.ShouldBe(leagueName);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetLeagueSeasonTotalsAsync_ShouldReturnLeagueSeasonTotals()
        {
            // Arrange
            var leagueName = "NFL";
            var seasonYear = 1920;

            var expected = new LeagueSeasonTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = await _testRepository.GetLeagueSeasonTotalsAsync(leagueName, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedLeagueName.ShouldBe(leagueName);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        /// <summary>
        /// Testable subclass that overrides the protected EF Core call so it can be
        /// exercised without a real database or relational provider.
        /// </summary>
        private class TestableLeagueSeasonTotalsRepository : LeagueSeasonTotalsRepository
        {
            // Pass null for dbContext — the override means it is never touched in tests.
            public TestableLeagueSeasonTotalsRepository() : base(null!) { }

            public string? CapturedLeagueName { get; private set; }
            public int CapturedSeasonYear { get; private set; }

            public LeagueSeasonTotals TotalsToReturn { get; set; }
                = new LeagueSeasonTotals { };

            protected override LeagueSeasonTotals ExecuteGetLeagueSeasonTotals(
                string leagueName, int seasonYear)
            {
                CapturedLeagueName = leagueName;
                CapturedSeasonYear = seasonYear;
                return TotalsToReturn;
            }

            protected override async Task<LeagueSeasonTotals> ExecuteGetLeagueSeasonTotalsAsync(
                string leagueName, int seasonYear)
            {
                CapturedLeagueName = leagueName;
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(TotalsToReturn);
            }
        }
    }
}
