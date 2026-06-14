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
            var seasonId = 1920;

            var expected = new List<TeamSeasonOpponentProfile>
            {
                new()
            };

            _testRepository.ProfileToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleProfile(teamId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleProfileAsync_ShouldReturnOpponentProfiles()
        {
            // Arrange
            var teamId = 1;
            var seasonId = 1920;

            var expected = new List<TeamSeasonOpponentProfile>
            {
                new()
            };

            _testRepository.ProfileToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleProfileAsync(teamId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public void GetTeamSeasonScheduleTotals_ShouldReturnScheduleTotals()
        {
            // Arrange
            var teamId = 1;
            var seasonId = 1920;

            var expected = new TeamSeasonScheduleTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleTotals(teamId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleTotalsAsync_ShouldReturnScheduleTotals()
        {
            // Arrange
            var teamId = 1;
            var seasonId = 1920;

            var expected = new TeamSeasonScheduleTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleTotalsAsync(teamId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public void GetTeamSeasonScheduleAverages_ShouldReturnScheduleAverages()
        {
            // Arrange
            var teamId = 1;
            var seasonId = 1920;

            var expected = new TeamSeasonScheduleAverages { };

            _testRepository.AveragesToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleAverages(teamId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleAveragesAsync_ShouldReturnScheduleAverages()
        {
            // Arrange
            var teamId = 1;
            var seasonId = 1920;

            var expected = new TeamSeasonScheduleAverages { };

            _testRepository.AveragesToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleAveragesAsync(teamId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamId.ShouldBe(teamId);
            _testRepository.CapturedSeasonId.ShouldBe(seasonId);
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
            public int CapturedSeasonId { get; private set; }

            public IEnumerable<TeamSeasonOpponentProfile> ProfileToReturn { get; set; }
                = new List<TeamSeasonOpponentProfile>();
            public TeamSeasonScheduleTotals TotalsToReturn { get; set; }
                = new TeamSeasonScheduleTotals { };
            public TeamSeasonScheduleAverages AveragesToReturn { get; set; }
                = new TeamSeasonScheduleAverages { };

            protected override IEnumerable<TeamSeasonOpponentProfile> ExecuteGetTeamSeasonScheduleProfile(int teamId,
                int seasonId)
            {
                CapturedTeamId = teamId;
                CapturedSeasonId = seasonId;
                return ProfileToReturn;
            }

            protected override async Task<IEnumerable<TeamSeasonOpponentProfile>> ExecuteGetTeamSeasonScheduleProfileAsync(
                int teamId, int seasonId)
            {
                CapturedTeamId = teamId;
                CapturedSeasonId = seasonId;
                return await Task.FromResult(ProfileToReturn);
            }

            protected override TeamSeasonScheduleTotals ExecuteGetTeamSeasonScheduleTotals(int teamId, int seasonId)
            {
                CapturedTeamId = teamId;
                CapturedSeasonId = seasonId;
                return TotalsToReturn;
            }

            protected override async Task<TeamSeasonScheduleTotals> ExecuteGetTeamSeasonScheduleTotalsAsync(int teamId,
                int seasonId)
            {
                CapturedTeamId = teamId;
                CapturedSeasonId = seasonId;
                return await Task.FromResult(TotalsToReturn);
            }

            protected override TeamSeasonScheduleAverages ExecuteGetTeamSeasonScheduleAverages(int teamId,
                int seasonId)
            {
                CapturedTeamId = teamId;
                CapturedSeasonId = seasonId;
                return AveragesToReturn;
            }

            protected override async Task<TeamSeasonScheduleAverages> ExecuteGetTeamSeasonScheduleAveragesAsync(
                int teamId, int seasonId)
            {
                CapturedTeamId = teamId;
                CapturedSeasonId = seasonId;
                return await Task.FromResult(AveragesToReturn);
            }
        }
    }
}
