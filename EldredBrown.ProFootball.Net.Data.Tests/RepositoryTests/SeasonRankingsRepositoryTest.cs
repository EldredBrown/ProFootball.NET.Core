using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class SeasonRankingsRepositoryTest
    {
        private readonly TestableSeasonRankingsRepository _testRepository;

        public SeasonRankingsRepositoryTest()
        {
            _testRepository = new TestableSeasonRankingsRepository();
        }

        [Fact]
        public void GetOffensiveRankingsForSeason_ShouldReturnOffensiveRankings()
        {
            // Arrange
            var seasonYear = 1920;

            var expected = new List<RankingsOffensiveTeamSeason>
            {
                new RankingsOffensiveTeamSeason { }
            };

            _testRepository.OffensiveRankingsToReturn = expected;

            // Act
            var result = _testRepository.GetOffensiveRankingsForSeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetOffensiveRankingsForSeasonAsync_ShouldReturnOffensiveRankings()
        {
            // Arrange
            var seasonYear = 1920;

            var expected = new List<RankingsOffensiveTeamSeason>
            {
                new RankingsOffensiveTeamSeason { }
            };

            _testRepository.OffensiveRankingsToReturn = expected;

            // Act
            var result = await _testRepository.GetOffensiveRankingsForSeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public void GetDefensiveRankingsForSeason_ShouldReturnDefensiveRankings()
        {
            // Arrange
            var seasonYear = 1920;

            var expected = new List<RankingsDefensiveTeamSeason>
            {
                new RankingsDefensiveTeamSeason { }
            };

            _testRepository.DefensiveRankingsToReturn = expected;

            // Act
            var result = _testRepository.GetDefensiveRankingsForSeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetDefensiveRankingsForSeasonAsync_ShouldReturnDefensiveRankings()
        {
            // Arrange
            var seasonYear = 1920;

            var expected = new List<RankingsDefensiveTeamSeason>
            {
                new RankingsDefensiveTeamSeason { }
            };

            _testRepository.DefensiveRankingsToReturn = expected;

            // Act
            var result = await _testRepository.GetDefensiveRankingsForSeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public void GetTotalRankingsForSeason_ShouldReturnTotalRankings()
        {
            // Arrange
            var seasonYear = 1920;

            var expected = new List<RankingsTotalTeamSeason>
            {
                new RankingsTotalTeamSeason { }
            };

            _testRepository.TotalRankingsToReturn = expected;

            // Act
            var result = _testRepository.GetTotalRankingsForSeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTotalRankingsForSeasonAsync_ShouldReturnTotalRankings()
        {
            // Arrange
            var seasonYear = 1920;

            var expected = new List<RankingsTotalTeamSeason>
            {
                new RankingsTotalTeamSeason { }
            };

            _testRepository.TotalRankingsToReturn = expected;

            // Act
            var result = await _testRepository.GetTotalRankingsForSeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        /// <summary>
        /// Testable subclass that overrides the protected EF Core call so it can be
        /// exercised without a real database or relational provider.
        /// </summary>
        private class TestableSeasonRankingsRepository : SeasonRankingsRepository
        {
            // Pass null for dbContext — the override means it is never touched in tests.
            public TestableSeasonRankingsRepository() : base(null!) { }

            public int CapturedSeasonYear { get; private set; }

            public IEnumerable<RankingsOffensiveTeamSeason> OffensiveRankingsToReturn { get; set; }
                = new List<RankingsOffensiveTeamSeason>();

            public IEnumerable<RankingsDefensiveTeamSeason> DefensiveRankingsToReturn { get; set; }
                = new List<RankingsDefensiveTeamSeason>();

            public IEnumerable<RankingsTotalTeamSeason> TotalRankingsToReturn { get; set; }
                = new List<RankingsTotalTeamSeason>();

            protected override IEnumerable<RankingsOffensiveTeamSeason> 
                ExecuteGetOffensiveRankingsForSeason(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return OffensiveRankingsToReturn;
            }

            protected override async Task<IEnumerable<RankingsOffensiveTeamSeason>>
                ExecuteGetOffensiveRankingsForSeasonAsync(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(OffensiveRankingsToReturn);
            }

            protected override IEnumerable<RankingsDefensiveTeamSeason>
                ExecuteGetDefensiveRankingsForSeason(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return DefensiveRankingsToReturn;
            }

            protected override async Task<IEnumerable<RankingsDefensiveTeamSeason>>
                ExecuteGetDefensiveRankingsForSeasonAsync(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(DefensiveRankingsToReturn);
            }

            protected override IEnumerable<RankingsTotalTeamSeason>
                ExecuteGetTotalRankingsForSeason(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return TotalRankingsToReturn;
            }

            protected override async Task<IEnumerable<RankingsTotalTeamSeason>>
                ExecuteGetTotalRankingsForSeasonAsync(int seasonYear)
            {
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(TotalRankingsToReturn);
            }
        }
    }
}
