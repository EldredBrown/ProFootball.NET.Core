using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Shouldly;
using Xunit;

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
            var teamName = "Test Team";
            var seasonYear = 1920;

            var expected = new List<TeamSeasonOpponentProfile>
            {
                new TeamSeasonOpponentProfile { }
            };

            _testRepository.ProfileToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleProfile(teamName, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamName.ShouldBe(teamName);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleProfileAsync_ShouldReturnOpponentProfiles()
        {
            // Arrange
            var teamName = "Test Team";
            var seasonYear = 1920;

            var expected = new List<TeamSeasonOpponentProfile>
            {
                new TeamSeasonOpponentProfile { }
            };

            _testRepository.ProfileToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleProfileAsync(teamName, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamName.ShouldBe(teamName);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public void GetTeamSeasonScheduleTotals_ShouldReturnScheduleTotals()
        {
            // Arrange
            var teamName = "Test Team";
            var seasonYear = 1920;

            var expected = new TeamSeasonScheduleTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleTotals(teamName, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamName.ShouldBe(teamName);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleTotalsAsync_ShouldReturnScheduleTotals()
        {
            // Arrange
            var teamName = "Test Team";
            var seasonYear = 1920;

            var expected = new TeamSeasonScheduleTotals { };

            _testRepository.TotalsToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamName.ShouldBe(teamName);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public void GetTeamSeasonScheduleAverages_ShouldReturnScheduleAverages()
        {
            // Arrange
            var teamName = "Test Team";
            var seasonYear = 1920;

            var expected = new TeamSeasonScheduleAverages { };

            _testRepository.AveragesToReturn = expected;

            // Act
            var result = _testRepository.GetTeamSeasonScheduleAverages(teamName, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamName.ShouldBe(teamName);
            _testRepository.CapturedSeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTeamSeasonScheduleAveragesAsync_ShouldReturnScheduleAverages()
        {
            // Arrange
            var teamName = "Test Team";
            var seasonYear = 1920;

            var expected = new TeamSeasonScheduleAverages { };

            _testRepository.AveragesToReturn = expected;

            // Act
            var result = await _testRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedTeamName.ShouldBe(teamName);
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

            public string? CapturedTeamName { get; private set; }
            public int CapturedSeasonYear { get; private set; }

            public IEnumerable<TeamSeasonOpponentProfile> ProfileToReturn { get; set; }
                = new List<TeamSeasonOpponentProfile>();
            public TeamSeasonScheduleTotals TotalsToReturn { get; set; }
                = new TeamSeasonScheduleTotals { };
            public TeamSeasonScheduleAverages AveragesToReturn { get; set; }
                = new TeamSeasonScheduleAverages { };

            protected override IEnumerable<TeamSeasonOpponentProfile> ExecuteGetTeamSeasonScheduleProfile(
                string teamName, int seasonYear)
            {
                CapturedTeamName = teamName;
                CapturedSeasonYear = seasonYear;
                return ProfileToReturn;
            }

            protected override async Task<IEnumerable<TeamSeasonOpponentProfile>> ExecuteGetTeamSeasonScheduleProfileAsync(
                string teamName, int seasonYear)
            {
                CapturedTeamName = teamName;
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(ProfileToReturn);
            }

            protected override TeamSeasonScheduleTotals ExecuteGetTeamSeasonScheduleTotals(
                string teamName, int seasonYear)
            {
                CapturedTeamName = teamName;
                CapturedSeasonYear = seasonYear;
                return TotalsToReturn;
            }

            protected override async Task<TeamSeasonScheduleTotals> ExecuteGetTeamSeasonScheduleTotalsAsync(
                string teamName, int seasonYear)
            {
                CapturedTeamName = teamName;
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(TotalsToReturn);
            }

            protected override TeamSeasonScheduleAverages ExecuteGetTeamSeasonScheduleAverages(
                string teamName, int seasonYear)
            {
                CapturedTeamName = teamName;
                CapturedSeasonYear = seasonYear;
                return AveragesToReturn;
            }

            protected override async Task<TeamSeasonScheduleAverages> ExecuteGetTeamSeasonScheduleAveragesAsync(
                string teamName, int seasonYear)
            {
                CapturedTeamName = teamName;
                CapturedSeasonYear = seasonYear;
                return await Task.FromResult(AveragesToReturn);
            }
        }
    }
}
