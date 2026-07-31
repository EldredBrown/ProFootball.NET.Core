using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class GameRepositoryTest
    {
        [Fact]
        public void GetGames_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnGames()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GetGames();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(games.Count);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
            }
        }

        [Fact]
        public void GetGames_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GetGames();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGames_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GetGames();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGamesAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnGames()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GetGamesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(games.Count);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
            }
        }

        [Fact]
        public async Task GetGamesAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GetGamesAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGamesAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GetGamesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetGamesBySeason_WhenDbSetIsNeitherNullNorEmptyAndGamesAreFound_ShouldReturnGames()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetGamesBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();

            var gameCount = games.Count(g => g.SeasonYear == seasonYear);
            result.Count().ShouldBe(gameCount);

            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
                item.SeasonYear.ShouldBe(seasonYear);
            }
        }

        [Fact]
        public void GetGamesBySeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetGamesBySeason(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGamesBySeason_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetGamesBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetGamesBySeason_WhenGamesAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = -1;
            var result = testRepository.GetGamesBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGamesBySeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndGamesAreFound_ShouldReturnGames()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetGamesBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();

            var gameCount = games.Count(g => g.SeasonYear == seasonYear);
            result.Count().ShouldBe(gameCount);

            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
                item.SeasonYear.ShouldBe(seasonYear);
            }
        }

        [Fact]
        public async Task GetGamesBySeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetGamesBySeasonAsync(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGamesBySeasonAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetGamesBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGamesBySeasonAsync_WhenGamesAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = -1;
            var result = await testRepository.GetGamesBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetGamesBySeasonLeagueAndWeek_WhenDbSetIsNeitherNullNorEmptyAndGamesAreFound_ShouldReturnGames()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var week = 1;
            var result = testRepository.GetGamesBySeasonLeagueAndWeek(seasonYear, leagueId, week);

            // Assert
            result.ShouldNotBeNull();

            var gameCount = games.Count(g => g.SeasonYear == seasonYear && g.LeagueId == leagueId && g.Week == week);
            result.Count().ShouldBe(gameCount);

            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
                item.SeasonYear.ShouldBe(seasonYear);
                item.LeagueId.ShouldBe(leagueId);
                item.Week.ShouldBe(week);
            }
        }

        [Fact]
        public void GetGamesBySeasonLeagueAndWeek_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var week = 1;
            var result = testRepository.GetGamesBySeasonLeagueAndWeek(seasonYear, leagueId, week);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGamesBySeasonLeagueAndWeek_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var week = 1;
            var result = testRepository.GetGamesBySeasonLeagueAndWeek(seasonYear, leagueId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Theory]
        [InlineData(-1, -1, -1)]
        [InlineData(-1, -1, 1)]
        [InlineData(-1, 1, -1)]
        [InlineData(1920, -1, -1)]
        public void GetGamesBySeasonLeagueAndWeek_WhenGamesAreNotFound_ShouldReturnEmptyCollection(
            int seasonYear, int leagueId, int week
        )
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GetGamesBySeasonLeagueAndWeek(seasonYear, leagueId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGamesBySeasonLeagueAndWeekAsync_WhenDbSetIsNeitherNullNorEmptyAndGamesAreFound_ShouldReturnGames()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var week = 1;
            var result = await testRepository.GetGamesBySeasonLeagueAndWeekAsync(seasonYear, leagueId, week);

            // Assert
            result.ShouldNotBeNull();

            var gameCount = games.Count(g => g.SeasonYear == seasonYear && g.LeagueId == leagueId && g.Week == week);
            result.Count().ShouldBe(gameCount);

            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
                item.SeasonYear.ShouldBe(seasonYear);
                item.LeagueId.ShouldBe(leagueId);
                item.Week.ShouldBe(week);
            }
        }

        [Fact]
        public async Task GetGamesBySeasonLeagueAndWeekAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var week = 1;
            var result = await testRepository.GetGamesBySeasonLeagueAndWeekAsync(seasonYear, leagueId, week);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGamesBySeasonLeagueAndWeekAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var leagueId = 1;
            var week = 1;
            var result = await testRepository.GetGamesBySeasonLeagueAndWeekAsync(seasonYear, leagueId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Theory]
        [InlineData(-1, -1, -1)]
        [InlineData(-1, -1, 1)]
        [InlineData(-1, 1, -1)]
        [InlineData(1920, -1, -1)]
        public async Task GetGamesBySeasonLeagueAndWeekAsync_WhenGamesAreNotFound_ShouldReturnEmptyCollection(
            int seasonYear, int leagueId, int week
        )
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GetGamesBySeasonLeagueAndWeekAsync(seasonYear, leagueId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetGame_WhenDbSetIsNeitherNullNorEmptyAndGameIsFound_ShouldReturnGame()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var id = 1;
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetGame_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var id = 1;
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGame_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var id = 1;
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGame_WhenGameIsNotFound_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var id = -1;
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameAsync_WhenDbSetIsNeitherNullNorEmptyAndGameIsFound_ShouldReturnGame()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var id = 1;
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetGameAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var id = 1;
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var id = 1;
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameAsync_WhenGameIsNotFound_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var id = -1;
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGameBySeasonWeekGuestAndHost_WhenDbSetIsNeitherNullNorEmptyAndGameIsFound_ShouldReturnGame()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var week = 1;
            var guestName = "Guest 1";
            var hostName = "Host 1";
            var result = testRepository.GetGameBySeasonWeekGuestAndHost(seasonYear, week, guestName, hostName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.SeasonYear.ShouldBe(seasonYear);
            result.Week.ShouldBe(week);
            result.GuestName.ShouldBe(guestName);
            result.HostName.ShouldBe(hostName);
        }

        [Fact]
        public void GetGameBySeasonWeekGuestAndHost_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var week = 1;
            var guestName = "Guest";
            var hostName = "Host";
            var result = testRepository.GetGameBySeasonWeekGuestAndHost(seasonYear, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGameBySeasonWeekGuestAndHost_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var week = 1;
            var guestName = "Guest";
            var hostName = "Host";
            var result = testRepository.GetGameBySeasonWeekGuestAndHost(seasonYear, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(-1, -1, "", "")]
        [InlineData(-1, 1, "Guest 1", "Host 1")]
        [InlineData(1920, -1, "Guest 1", "Host 1")]
        [InlineData(1920, 1, "", "Host 1")]
        [InlineData(1920, 1, "Guest 1", "")]
        public void GetGameBySeasonWeekGuestAndHost_WhenGameIsNotFound_ShouldReturnNull(
            int seasonYear, int week, string guestName, string hostName
        )
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GetGameBySeasonWeekGuestAndHost(seasonYear, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameBySeasonWeekGuestAndHostAsync_WhenDbSetIsNeitherNullNorEmptyAndGameIsFound_ShouldReturnGame()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var week = 1;
            var guestName = "Guest 1";
            var hostName = "Host 1";
            var result = await testRepository.GetGameBySeasonWeekGuestAndHostAsync(seasonYear, week, guestName, hostName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.SeasonYear.ShouldBe(seasonYear);
            result.Week.ShouldBe(week);
            result.GuestName.ShouldBe(guestName);
            result.HostName.ShouldBe(hostName);
        }

        [Fact]
        public async Task GetGameBySeasonWeekGuestAndHostAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var week = 1;
            var guestName = "Guest";
            var hostName = "Host";
            var result = await testRepository.GetGameBySeasonWeekGuestAndHostAsync(seasonYear, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameBySeasonWeekGuestAndHostAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var seasonYear = 1920;
            var week = 1;
            var guestName = "Guest";
            var hostName = "Host";
            var result = await testRepository.GetGameBySeasonWeekGuestAndHostAsync(seasonYear, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(-1, -1, "", "")]
        [InlineData(-1, 1, "Guest 1", "Host 1")]
        [InlineData(1920, -1, "Guest 1", "Host 1")]
        [InlineData(1920, 1, "", "Host 1")]
        [InlineData(1920, 1, "Guest 1", "")]
        public async Task GetGameBySeasonWeekGuestAndHostAsync_WhenGameIsNotFound_ShouldReturnNull(
            int seasonYear, int week, string guestName, string hostName)
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GetGameBySeasonWeekGuestAndHostAsync(seasonYear, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddGame()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Game>>());
            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var game = new Game { Id = 1 };
            var result = testRepository.Add(game);

            // Assert
            A.CallTo(() => fakeDbContext.Add(game)).MustHaveHappenedOnceExactly();
            result.ShouldBe(game);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Game>>());
            var testRepository = new GameRepository(fakeDbContext);

            // Act & Assert
            Game? game = null!;
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(game));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnGameWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var game = new Game { Id = 1 };
            var result = testRepository.Add(game);

            // Assert
            A.CallTo(() => fakeDbContext.Add(game)).MustNotHaveHappened();
            result.ShouldBe(game);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddGame()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Game>>());
            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var game = new Game { Id = 1 };
            var result = await testRepository.AddAsync(game);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(game)).MustHaveHappenedOnceExactly();
            result.ShouldBe(game);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Game>>());
            var testRepository = new GameRepository(fakeDbContext);

            // Act & Assert
            Game? game = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(game));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnGameWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var game = new Game { Id = 1 };
            var result = await testRepository.AddAsync(game);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(game)).MustNotHaveHappened();
            result.ShouldBe(game);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                HostName = "Host"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            testRepository.Update(game);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Games.FirstOrDefault(s => s.Id == game.Id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act & Assert
            Game? game = null!;
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(game));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnGame()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            Game? game = new();
            var updated = testRepository.Update(game);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(game);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnGame()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            Game? game = new();
            var updated = testRepository.Update(game);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(game);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedGameIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                HostName = "Host"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            var gameCountBeforeDelete = fakeDbContext.Games.Count();

            // Act
            var result = testRepository.Delete(game.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Games.Count().ShouldBe(gameCountBeforeDelete - 1);
            result.ShouldBe(game);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var game = new Game { Id = 1 };
            var result = testRepository.Delete(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var game = new Game { Id = 1 };
            var result = testRepository.Delete(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedGameIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                HostName = "Host"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            var gameGameCountBeforeDelete = fakeDbContext.Games.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Games.Count().ShouldBe(gameGameCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedGameIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                HostName = "Host"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            var gameCountBeforeDelete = fakeDbContext.Games.Count();

            // Act
            var result = await testRepository.DeleteAsync(game.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Games.Count().ShouldBe(gameCountBeforeDelete - 1);
            result.ShouldBe(game);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var game = new Game { Id = 1 };
            var result = await testRepository.DeleteAsync(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var game = new Game { Id = 1 };
            var result = await testRepository.DeleteAsync(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedGameIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                HostName = "Host"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            var gameGameCountBeforeDelete = fakeDbContext.Games.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Games.Count().ShouldBe(gameGameCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void GameExists_WhenDbSetIsNotNullAndSelectedGameExists_ShouldReturnTrue()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GameExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void GameExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GameExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void GameExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GameExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void GameExists_WhenSelectedGameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = testRepository.GameExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GameExistsAsync_WhenDbSetIsNotNullAndSelectedGameExists_ShouldReturnTrue()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GameExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task GameExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<Game> games = null!;
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GameExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GameExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var games = new List<Game>();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GameExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GameExistsAsync_WhenSelectedGameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            List<Game> games = GetGames();
            GameRepository testRepository = CreateTestRepository(games);

            // Act
            var result = await testRepository.GameExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GetMaxWeekForSeasonAsync_WhenWeeksExistForSeason_ShouldReturnMaxWeek()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new() { Id = 1, SeasonYear = seasonYear, Week = 1, GuestName = "Guest", HostName = "Host" },
                new() { Id = 2, SeasonYear = seasonYear, Week = 2, GuestName = "Guest", HostName = "Host" },
                new() { Id = 3, SeasonYear = seasonYear, Week = 3, GuestName = "Guest", HostName = "Host" },
            };
            GameRepository testRepository = SetUpGetMaxWeeksForSeason(seasonYear, games);

            // Act
            var result = await testRepository.GetMaxWeekForSeasonAsync(seasonYear);

            // Assert
            result.ShouldBe(3);
        }

        [Fact]
        public async Task GetMaxWeekForSeasonAsync_WhenWeeksDoNotExistForSeason_ShouldReturnZero()
        {
            // Arrange
            var seasonYear = 1920;
            GameRepository testRepository = SetUpGetMaxWeeksForSeason(seasonYear);

            // Act
            var result = await testRepository.GetMaxWeekForSeasonAsync(seasonYear);

            // Assert
            result.ShouldBe(0);
        }

        private static ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<Game> games)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = games;
            return fakeDbContext;
        }

        private static GameRepository CreateTestRepository(List<Game>? games)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();
            DbSet<Game> fakeDbSet = games is not null ? games.BuildMockDbSet() : null!;
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);

            return new GameRepository(fakeDbContext);
        }

        private static List<Game> GetGames()
        {
            var counter = 1;

            var games = new List<Game>();
            for (int y = 1920; y < 1923; y++)
            {
                for (int l = 1; l < 4; l++)
                {
                    for (int w = 1; w < 4; w++)
                    {
                        for (int t = 1; t < 4; t++)
                        {
                            games.Add(
                                new Game
                                {
                                    Id = counter++,
                                    SeasonYear = y,
                                    LeagueId = l,
                                    Week = w,
                                    GuestName = $"Guest {t}",
                                    GuestScore = 0,
                                    HostName = $"Host {t}",
                                    HostScore = 0,
                                    IsPlayoff = false,
                                    Notes = string.Empty
                                }
                            );
                        }
                    }
                }
            }

            return games;
        }

        private static GameRepository SetUpGetMaxWeeksForSeason(int seasonYear, List<Game>? games = null)
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var fakeDbContext = new ProFootballDbContext(options);
            var season = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(season);

            if (games is not null)
            {
                fakeDbContext.Games.AddRange(games);
                fakeDbContext.SaveChanges();
            }

            return new GameRepository(fakeDbContext);
        }
    }
}
