using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Data;
using System.Data.Common;
using Xunit;

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

        [Fact]
        public void GetDataForRankingsUpdate_ReturnsMappedResultSets()
        {
            // Arrange
            // Fake the connection string
            var fakeDbConnection = A.Fake<DbConnection>();
            A.CallTo(() => fakeDbConnection.ConnectionString).Returns("Server=fake;");

            // Fake the connection string provider.
            var fakeConnectionStringProvider = A.Fake<IConnectionStringProvider>();
            A.CallTo(() => fakeConnectionStringProvider.GetConnectionString())
                .Returns("Server=fake;Database=test;");

            // Fake the reader
            var fakeReader = A.Fake<DbDataReader>();

            int readCallCount = 0;
            A.CallTo(() => fakeReader.Read())
                .ReturnsLazily(() => readCallCount++ < 3); // 3 result sets, one row each

            A.CallTo(() => fakeReader.NextResult()).Returns(true);
            A.CallTo(() => fakeReader.FieldCount).Returns(2);
            A.CallTo(() => fakeReader.GetName(0)).Returns("Col1");
            A.CallTo(() => fakeReader.GetName(1)).Returns("Col2");
            A.CallTo(() => fakeReader.IsDBNull(A<int>._)).Returns(false);
            A.CallTo(() => fakeReader.GetValue(0)).Returns("Value1");
            A.CallTo(() => fakeReader.GetValue(1)).Returns(42);

            // Fake the command
            var fakeCommand = A.Fake<IDbCommand>();
            A.CallTo(() => fakeCommand.ExecuteReader()).Returns(fakeReader);
            A.CallTo(() => fakeCommand.Parameters).Returns(A.Fake<DbParameterCollection>());

            // Fake the connection
            var fakeConnection = A.Fake<IDbConnection>();
            A.CallTo(() => fakeConnection.CreateCommand()).Returns(fakeCommand);

            // Fake the connection factory
            var fakeFactory = A.Fake<IDbConnectionFactory>();
            A.CallTo(() => fakeFactory.CreateConnection(A<string>.Ignored)).Returns(fakeConnection);

            var testRepository = new SeasonRankingsRepository(null!, fakeConnectionStringProvider, fakeFactory);

            var teamSeason = new TeamSeason { TeamId = 1, SeasonId = 1, LeagueId = 1 };

            // Act
            var result = testRepository.GetDataForRankingsUpdate(teamSeason);

            // Assert
            result.ShouldContainKey("TeamSeasonScheduleTotals");
            result.ShouldContainKey("TeamSeasonScheduleAverages");
            result.ShouldContainKey("LeagueSeason");

            result["TeamSeasonScheduleTotals"]["Col1"].ShouldBe("Value1");
            result["TeamSeasonScheduleTotals"]["Col2"].ShouldBe(42);
        }

        /// <summary>
        /// Testable subclass that overrides the protected EF Core call so it can be
        /// exercised without a real database or relational provider.
        /// </summary>
        private class TestableSeasonRankingsRepository : SeasonRankingsRepository
        {
            // Pass null for dbContext and connectionFactory — the override means they are never touched in tests.
            public TestableSeasonRankingsRepository() : base(null!, null!, null!) { }

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
