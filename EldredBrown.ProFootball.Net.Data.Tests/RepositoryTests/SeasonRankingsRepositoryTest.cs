using System.Data;
using System.Data.Common;

using FakeItEasy;
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
        public void GetOffensiveRankings_ShouldReturnOffensiveRankings()
        {
            // Arrange
            List<RankingsOffensiveTeamSeason> expected = SetUpOffensiveRankings();

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var result = _testRepository.GetOffensiveRankings(seasonYear, leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedseasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetOffensiveRankingsAsync_ShouldReturnOffensiveRankings()
        {
            // Arrange
            List<RankingsOffensiveTeamSeason> expected = SetUpOffensiveRankings();

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var result = await _testRepository.GetOffensiveRankingsAsync(seasonYear, leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedseasonYear.ShouldBe(seasonYear);
        }

        private List<RankingsOffensiveTeamSeason> SetUpOffensiveRankings()
        {
            var expected = new List<RankingsOffensiveTeamSeason>();
            _testRepository.OffensiveRankingsToReturn = expected;
            return expected;
        }

        [Fact]
        public void GetDefensiveRankings_ShouldReturnDefensiveRankings()
        {
            // Arrange
            List<RankingsDefensiveTeamSeason> expected = SetUpDefensiveRankings();

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var result = _testRepository.GetDefensiveRankings(seasonYear, leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedseasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetDefensiveRankingsAsync_ShouldReturnDefensiveRankings()
        {
            // Arrange
            List<RankingsDefensiveTeamSeason> expected = SetUpDefensiveRankings();

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var result = await _testRepository.GetDefensiveRankingsAsync(seasonYear, leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedseasonYear.ShouldBe(seasonYear);
        }

        private List<RankingsDefensiveTeamSeason> SetUpDefensiveRankings()
        {
            var expected = new List<RankingsDefensiveTeamSeason>();
            _testRepository.DefensiveRankingsToReturn = expected;
            return expected;
        }

        [Fact]
        public void GetTotalRankings_ShouldReturnTotalRankings()
        {
            // Arrange
            List<RankingsTotalTeamSeason> expected = SetUpTotalRankings();

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var result = _testRepository.GetTotalRankings(seasonYear, leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedseasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTotalRankingsAsync_ShouldReturnTotalRankings()
        {
            // Arrange
            List<RankingsTotalTeamSeason> expected = SetUpTotalRankings();

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var result = await _testRepository.GetTotalRankingsAsync(seasonYear, leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(expected);
            _testRepository.CapturedseasonYear.ShouldBe(seasonYear);
        }

        private List<RankingsTotalTeamSeason> SetUpTotalRankings()
        {
            var expected = new List<RankingsTotalTeamSeason>();
            _testRepository.TotalRankingsToReturn = expected;
            return expected;
        }

        [Fact]
        public void GetDataForRankingsUpdate_ReturnsMappedResultSets()
        {
            // Arrange
            SeasonRankingsRepository testRepository = CreateTestRepositoryForRankingsUpdate();

            // Act
            var teamSeason = new TeamSeason { TeamId = 1, SeasonYear = 1, LeagueId = 1 };
            var result = testRepository.GetDataForRankingsUpdate(teamSeason);

            // Assert
            result.ShouldContainKey("TeamSeasonScheduleTotals");
            result.ShouldContainKey("TeamSeasonScheduleAverages");
            result.ShouldContainKey("LeagueSeason");

            result["TeamSeasonScheduleTotals"]["Col1"].ShouldBe("Value1");
            result["TeamSeasonScheduleTotals"]["Col2"].ShouldBe(42);
        }

        private static SeasonRankingsRepository CreateTestRepositoryForRankingsUpdate()
        {
            IConnectionStringProvider fakeConnectionStringProvider = FakeConnectionStringProvider();
            IDbConnectionFactory fakeFactory = FakeConnectionFactory();

            return new SeasonRankingsRepository(null!, fakeConnectionStringProvider, fakeFactory);
        }

        private static IConnectionStringProvider FakeConnectionStringProvider()
        {
            FakeConnectionString();

            var fakeConnectionStringProvider = A.Fake<IConnectionStringProvider>();
            A.CallTo(() => fakeConnectionStringProvider.GetConnectionString())
                .Returns("Server=fake;Database=test;");

            return fakeConnectionStringProvider;
        }

        private static void FakeConnectionString()
        {
            var fakeDbConnection = A.Fake<DbConnection>();
            A.CallTo(() => fakeDbConnection.ConnectionString).Returns("Server=fake;");
        }

        private static IDbConnectionFactory FakeConnectionFactory()
        {
            IDbConnection fakeConnection = FakeConnection();

            var fakeFactory = A.Fake<IDbConnectionFactory>();
            A.CallTo(() => fakeFactory.CreateConnection(A<string>.Ignored)).Returns(fakeConnection);

            return fakeFactory;
        }

        private static IDbConnection FakeConnection()
        {
            IDbCommand fakeCommand = FakeCommand();

            var fakeConnection = A.Fake<IDbConnection>();
            A.CallTo(() => fakeConnection.CreateCommand()).Returns(fakeCommand);

            return fakeConnection;
        }

        private static IDbCommand FakeCommand()
        {
            DbDataReader fakeReader = FakeReader();

            var fakeCommand = A.Fake<IDbCommand>();
            A.CallTo(() => fakeCommand.ExecuteReader()).Returns(fakeReader);
            A.CallTo(() => fakeCommand.Parameters).Returns(A.Fake<DbParameterCollection>());

            return fakeCommand;
        }

        private static DbDataReader FakeReader()
        {
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

            return fakeReader;
        }

        /// <summary>
        /// Testable subclass that overrides the protected EF Core call so it can be
        /// exercised without a real database or relational provider.
        /// </summary>
        private class TestableSeasonRankingsRepository : SeasonRankingsRepository
        {
            // Pass null for dbContext and connectionFactory — the override means they are never touched in tests.
            public TestableSeasonRankingsRepository() : base(null!, null!, null!) { }

            public int CapturedseasonYear { get; private set; }

            public IEnumerable<RankingsOffensiveTeamSeason> OffensiveRankingsToReturn { get; set; }
                = new List<RankingsOffensiveTeamSeason>();

            public IEnumerable<RankingsDefensiveTeamSeason> DefensiveRankingsToReturn { get; set; }
                = new List<RankingsDefensiveTeamSeason>();

            public IEnumerable<RankingsTotalTeamSeason> TotalRankingsToReturn { get; set; }
                = new List<RankingsTotalTeamSeason>();

            protected override IEnumerable<RankingsOffensiveTeamSeason> 
                ExecuteGetOffensiveRankings(int seasonYear, int leagueId)
            {
                CapturedseasonYear = seasonYear;
                return OffensiveRankingsToReturn;
            }

            protected override async Task<IEnumerable<RankingsOffensiveTeamSeason>>
                ExecuteGetOffensiveRankingsAsync(int seasonYear, int leagueId)
            {
                CapturedseasonYear = seasonYear;
                return await Task.FromResult(OffensiveRankingsToReturn);
            }

            protected override IEnumerable<RankingsDefensiveTeamSeason>
                ExecuteGetDefensiveRankings(int seasonYear, int leagueId)
            {
                CapturedseasonYear = seasonYear;
                return DefensiveRankingsToReturn;
            }

            protected override async Task<IEnumerable<RankingsDefensiveTeamSeason>>
                ExecuteGetDefensiveRankingsAsync(int seasonYear, int leagueId)
            {
                CapturedseasonYear = seasonYear;
                return await Task.FromResult(DefensiveRankingsToReturn);
            }

            protected override IEnumerable<RankingsTotalTeamSeason>
                ExecuteGetTotalRankings(int seasonYear, int leagueId)
            {
                CapturedseasonYear = seasonYear;
                return TotalRankingsToReturn;
            }

            protected override async Task<IEnumerable<RankingsTotalTeamSeason>>
                ExecuteGetTotalRankingsAsync(int seasonYear, int leagueId)
            {
                CapturedseasonYear = seasonYear;
                return await Task.FromResult(TotalRankingsToReturn);
            }
        }
    }
}
