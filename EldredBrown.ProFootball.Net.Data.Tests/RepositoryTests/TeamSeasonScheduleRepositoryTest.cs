using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class TeamSeasonScheduleRepositoryTest
    {
        private readonly TestableTeamSeasonScheduleRepository _testRepository;

        public TeamSeasonScheduleRepositoryTest()
        {
            _testRepository = new TestableTeamSeasonScheduleRepository();
        }

        [Fact]
        public void GetTeamSeasonScheduleProfile_ShouldReturnOpponentProfiles()
        {
            // Arrange
            var teamId = 1;
            var seasonYear = 1920;

            var expected = new List<TeamSeasonOpponentProfile>
            {
                new()
            };

            _testRepository.ProfileToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleProfile(teamId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleProfileAsync_ShouldReturnOpponentProfiles()
        {
            // Arrange
            var teamId = 1;
            var seasonYear = 1920;

            var expected = new List<TeamSeasonOpponentProfile>
            {
                new()
            };

            _testRepository.ProfileToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleProfileAsync(teamId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public void GetTeamSeasonScheduleTotals_ShouldReturnScheduleTotals()
        {
            // Arrange
            var teamId = 1;
            var seasonYear = 1920;

            var expected = new TeamSeasonScheduleTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleTotals(teamId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleTotalsAsync_ShouldReturnScheduleTotals()
        {
            // Arrange
            var teamId = 1;
            var seasonYear = 1920;

            var expected = new TeamSeasonScheduleTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleTotalsAsync(teamId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public void GetTeamSeasonScheduleAverages_ShouldReturnScheduleAverages()
        {
            // Arrange
            var teamId = 1;
            var seasonYear = 1920;

            var expected = new TeamSeasonScheduleAverages { };

            _testRepository.AveragesToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleAverages(teamId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleAveragesAsync_ShouldReturnScheduleAverages()
        {
            // Arrange
            var teamId = 1;
            var seasonYear = 1920;

            var expected = new TeamSeasonScheduleAverages { };

            _testRepository.AveragesToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleAveragesAsync(teamId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        /// <summary>
        /// Testable subclass that overrides the protected EF Core call so it can be
        /// exercised without a real database or relational provider.
        /// </summary>
        private class TestableTeamSeasonScheduleRepository : TeamSeasonScheduleRepository
        {
            // Pass null for dbContext — the override means it is never touched in tests.
            public TestableTeamSeasonScheduleRepository() : base(null!) { }

            public int? CapturedTeamId { get; private set; }
            public int CapturedSeasonYear { get; private set; }

            public IEnumerable<TeamSeasonOpponentProfile> ProfileToReturn { get; set; }
                = new List<TeamSeasonOpponentProfile>();
            public TeamSeasonScheduleTotals TotalsToReturn { get; set; }
                = new TeamSeasonScheduleTotals { };
            public TeamSeasonScheduleAverages AveragesToReturn { get; set; }
                = new TeamSeasonScheduleAverages { };

            protected override IEnumerable<TeamSeasonOpponentProfile> ExecuteGetTeamSeasonScheduleProfile(int teamId,
                int seasonYear)
            {
                CapturedTeamId = teamId;
                CapturedSeasonYear = seasonYear;
                return ProfileToReturn;
            }

            protected override async Task<IEnumerable<TeamSeasonOpponentProfile>> ExecuteGetTeamSeasonScheduleProfileAsync(
                int teamId, int seasonYear)
            {
                CapturedTeamId = teamId;
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(ProfileToReturn);
            }

            protected override TeamSeasonScheduleTotals ExecuteGetTeamSeasonScheduleTotals(int teamId, int seasonYear)
            {
                CapturedTeamId = teamId;
                CapturedSeasonYear = seasonYear;
                return TotalsToReturn;
            }

            protected override async Task<TeamSeasonScheduleTotals> ExecuteGetTeamSeasonScheduleTotalsAsync(int teamId,
                int seasonYear)
            {
                CapturedTeamId = teamId;
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(TotalsToReturn);
            }

            protected override TeamSeasonScheduleAverages ExecuteGetTeamSeasonScheduleAverages(int teamId,
                int seasonYear)
            {
                CapturedTeamId = teamId;
                CapturedSeasonYear = seasonYear;
                return AveragesToReturn;
            }

            protected override async Task<TeamSeasonScheduleAverages> ExecuteGetTeamSeasonScheduleAveragesAsync(
                int teamId, int seasonYear)
            {
                CapturedTeamId = teamId;
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(AveragesToReturn);
            }
        }
    }
}
