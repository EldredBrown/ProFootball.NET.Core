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
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetGames();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(27);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
            }
        }

        [Fact]
        public void GetGames_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.GetGames();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGames_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.GetGames();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGamesAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnGames()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetGamesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(27);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
            }
        }

        [Fact]
        public async Task GetGamesAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.GetGamesAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGamesAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.GetGamesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetGamesBySeason_WhenDbSetIsNeitherNullNorEmptyAndGamesAreFound_ShouldReturnGames()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetGamesBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(9);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
                item.SeasonId.ShouldBe(seasonId);
            }
        }

        [Fact]
        public void GetGamesBySeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetGamesBySeason(seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGamesBySeason_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetGamesBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetGamesBySeason_WhenGamesAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = -1;

            // Act
            var result = testRepository.GetGamesBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGamesBySeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndGamesAreFound_ShouldReturnGames()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetGamesBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(9);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
                item.SeasonId.ShouldBe(seasonId);
            }
        }

        [Fact]
        public async Task GetGamesBySeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetGamesBySeasonAsync(seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGamesBySeasonAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetGamesBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGamesBySeasonAsync_WhenGamesAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = -1;

            // Act
            var result = await testRepository.GetGamesBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetGamesBySeasonAndWeek_WhenDbSetIsNeitherNullNorEmptyAndGamesAreFound_ShouldReturnGames()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;
            var week = 1;

            // Act
            var result = testRepository.GetGamesBySeasonAndWeek(seasonId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
                item.SeasonId.ShouldBe(seasonId);
                item.Week.ShouldBe(week);
            }
        }

        [Fact]
        public void GetGamesBySeasonAndWeek_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;
            var week = 1;

            // Act
            var result = testRepository.GetGamesBySeasonAndWeek(seasonId, week);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGamesBySeasonAndWeek_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;
            var week = 1;

            // Act
            var result = testRepository.GetGamesBySeasonAndWeek(seasonId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Theory]
        [InlineData(1920, -1)]
        [InlineData(-1, 1)]
        [InlineData(-1, -1)]
        public void GetGamesBySeasonAndWeek_WhenGamesAreNotFound_ShouldReturnEmptyCollection(
            int seasonId, int week
        )
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetGamesBySeasonAndWeek(seasonId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetGamesBySeasonAndWeekAsync_WhenDbSetIsNeitherNullNorEmptyAndGamesAreFound_ShouldReturnGames()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;
            var week = 1;

            // Act
            var result = testRepository.GetGamesBySeasonAndWeek(seasonId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
                item.SeasonId.ShouldBe(seasonId);
                item.Week.ShouldBe(week);
            }
        }

        [Fact]
        public async Task GetGamesBySeasonAndWeekAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;
            var week = 1;

            // Act
            var result = await testRepository.GetGamesBySeasonAndWeekAsync(seasonId, week);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGamesBySeasonAndWeekAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;
            var week = 1;

            // Act
            var result = await testRepository.GetGamesBySeasonAndWeekAsync(seasonId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Theory]
        [InlineData(1920, -1)]
        [InlineData(-1, 1)]
        [InlineData(-1, -1)]
        public async Task GetGamesBySeasonAndWeekAsync_WhenGamesAreNotFound_ShouldReturnEmptyCollection(
            int seasonId, int week
        )
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetGamesBySeasonAndWeekAsync(seasonId, week);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetGame_WhenDbSetIsNeitherNullNorEmptyAndGameIsFound_ShouldReturnGame()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetGame_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGame_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGame_WhenGameIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameAsync_WhenDbSetIsNeitherNullNorEmptyAndGameIsFound_ShouldReturnGame()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetGameAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameAsync_WhenGameIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGameBySeasonWeekGuestAndHost_WhenDbSetIsNeitherNullNorEmptyAndGameIsFound_ShouldReturnGame()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;
            var week = 1;
            var guestName = "Guest 1";
            var hostName = "Host 1";

            // Act
            var result = testRepository.GetGameBySeasonWeekGuestAndHost(seasonId, week, guestName, hostName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.SeasonId.ShouldBe(seasonId);
            result.Week.ShouldBe(week);
            result.GuestName.ShouldBe(guestName);
            result.HostName.ShouldBe(hostName);
        }

        [Fact]
        public void GetGameBySeasonWeekGuestAndHost_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;
            var week = 1;
            var guestName = "Guest";
            var hostName = "Host";

            // Act
            var result = testRepository.GetGameBySeasonWeekGuestAndHost(seasonId, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGameBySeasonWeekGuestAndHost_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;
            var week = 1;
            var guestName = "Guest";
            var hostName = "Host";

            // Act
            var result = testRepository.GetGameBySeasonWeekGuestAndHost(seasonId, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(1920, 1, "Guest 1", "")]
        [InlineData(1920, 1, "", "Host 1")]
        [InlineData(1920, -1, "Guest 1", "Host 1")]
        [InlineData(-1, 1, "Guest 1", "Host 1")]
        [InlineData(-1, -1, "", "")]
        public void GetGameBySeasonWeekGuestAndHost_WhenGameIsNotFound_ShouldReturnNull(
            int seasonId, int week, string guestName, string hostName)
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetGameBySeasonWeekGuestAndHost(seasonId, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameBySeasonWeekGuestAndHostAsync_WhenDbSetIsNeitherNullNorEmptyAndGameIsFound_ShouldReturnGame()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;
            var week = 1;
            var guestName = "Guest 1";
            var hostName = "Host 1";

            // Act
            var result = await testRepository.GetGameBySeasonWeekGuestAndHostAsync(seasonId, week, guestName, hostName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.SeasonId.ShouldBe(seasonId);
            result.Week.ShouldBe(week);
            result.GuestName.ShouldBe(guestName);
            result.HostName.ShouldBe(hostName);
        }

        [Fact]
        public async Task GetGameBySeasonWeekGuestAndHostAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;
            var week = 1;
            var guestName = "Guest";
            var hostName = "Host";

            // Act
            var result = await testRepository.GetGameBySeasonWeekGuestAndHostAsync(seasonId, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameBySeasonWeekGuestAndHostAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;
            var week = 1;
            var guestName = "Guest";
            var hostName = "Host";

            // Act
            var result = await testRepository.GetGameBySeasonWeekGuestAndHostAsync(seasonId, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(1920, 1, "Guest 1", "")]
        [InlineData(1920, 1, "", "Host 1")]
        [InlineData(1920, -1, "Guest 1", "Host 1")]
        [InlineData(-1, 1, "Guest 1", "Host 1")]
        [InlineData(-1, -1, "", "")]
        public async Task GetGameBySeasonWeekGuestAndHostAsync_WhenGameIsNotFound_ShouldReturnNull(
            int seasonId, int week, string guestName, string hostName)
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetGameBySeasonWeekGuestAndHostAsync(seasonId, week, guestName, hostName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddGame()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Game>>());
            var testRepository = new GameRepository(fakeDbContext);

            var game = new Game { Id = 1 };

            // Act
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

            Game? game = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(game));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnGameWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new GameRepository(fakeDbContext);

            var game = new Game { Id = 1 };

            // Act
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

            var game = new Game { Id = 1 };

            // Act
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

            Game? game = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(game));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnGameWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new GameRepository(fakeDbContext);

            var game = new Game { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(game);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(game)).MustNotHaveHappened();
            result.ShouldBe(game);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
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
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            Game? game = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(game));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnGame()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            Game? game = new Game { };

            // Act
            var updated = testRepository.Update(game);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(game);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnGame()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            Game? game = new Game { };

            // Act
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

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
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
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var game = new Game { Id = 1 };

            // Act
            var result = testRepository.Delete(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var game = new Game { Id = 1 };

            // Act
            var result = testRepository.Delete(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedGameIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
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

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
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
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var game = new Game { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var game = new Game { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedGameIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
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
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GameExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void GameExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.GameExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void GameExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.GameExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void GameExists_WhenSelectedGameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GameExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GameExistsAsync_WhenDbSetIsNotNullAndSelectedGameExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GameExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task GameExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.GameExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GameExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.GameExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GameExistsAsync_WhenSelectedGameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GameExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<Game> games)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = games;
            return fakeDbContext;
        }

        private IGameRepository CreateTestRepositoryWithEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Games = A.Fake<DbSet<Game>>();
            var games = new List<Game>();
            var fakeDbSet = games.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);

            var testRepository = new GameRepository(fakeDbContext);
            return testRepository;
        }

        private IGameRepository CreateTestRepositoryWithNotEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Games = A.Fake<DbSet<Game>>();
            var games = new List<Game>
            {
                new Game { Id = 1, SeasonId = 1920, Week = 1, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 2, SeasonId = 1920, Week = 1, GuestName = "Guest 2", HostName = "Host 2" },
                new Game { Id = 3, SeasonId = 1920, Week = 1, GuestName = "Guest 3", HostName = "Host 3" },
                new Game { Id = 4, SeasonId = 1920, Week = 2, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 5, SeasonId = 1920, Week = 2, GuestName = "Guest 2", HostName = "Host 2" },
                new Game { Id = 6, SeasonId = 1920, Week = 2, GuestName = "Guest 3", HostName = "Host 3" },
                new Game { Id = 7, SeasonId = 1920, Week = 3, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 8, SeasonId = 1920, Week = 3, GuestName = "Guest 2", HostName = "Host 2" },
                new Game { Id = 9, SeasonId = 1920, Week = 3, GuestName = "Guest 3", HostName = "Host 3" },
                new Game { Id = 10, SeasonId = 1921, Week = 1, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 11, SeasonId = 1921, Week = 1, GuestName = "Guest 2", HostName = "Host 2" },
                new Game { Id = 12, SeasonId = 1921, Week = 1, GuestName = "Guest 3", HostName = "Host 3" },
                new Game { Id = 13, SeasonId = 1921, Week = 2, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 14, SeasonId = 1921, Week = 2, GuestName = "Guest 2", HostName = "Host 2" },
                new Game { Id = 15, SeasonId = 1921, Week = 2, GuestName = "Guest 3", HostName = "Host 3" },
                new Game { Id = 16, SeasonId = 1921, Week = 3, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 17, SeasonId = 1921, Week = 3, GuestName = "Guest 2", HostName = "Host 2" },
                new Game { Id = 18, SeasonId = 1921, Week = 3, GuestName = "Guest 3", HostName = "Host 3" },
                new Game { Id = 19, SeasonId = 1922, Week = 1, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 20, SeasonId = 1922, Week = 1, GuestName = "Guest 2", HostName = "Host 2" },
                new Game { Id = 21, SeasonId = 1922, Week = 1, GuestName = "Guest 3", HostName = "Host 3" },
                new Game { Id = 22, SeasonId = 1922, Week = 2, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 23, SeasonId = 1922, Week = 2, GuestName = "Guest_2", HostName = "Host_2" },
                new Game { Id = 24, SeasonId = 1922, Week = 2, GuestName = "Guest_3", HostName = "Host_3" },
                new Game { Id = 25, SeasonId = 1922, Week = 3, GuestName = "Guest 1", HostName = "Host 1" },
                new Game { Id = 26, SeasonId = 1922, Week = 3, GuestName = "Guest 2", HostName = "Host 2" },
                new Game { Id = 27, SeasonId = 1922, Week = 3, GuestName = "Guest 3", HostName = "Host 3" },
            };
            var fakeDbSet = games.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);

            var testRepository = new GameRepository(fakeDbContext);
            return testRepository;
        }

        private IGameRepository CreateTestRepositoryWithNullDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Games = A.Fake<DbSet<Game>>();
            DbSet<Game> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);

            var testRepository = new GameRepository(fakeDbContext);
            return testRepository;
        }

        [Fact]
        public async Task GetMaxWeekForSeasonAsync_WhenWeeksExistForSeason_ShouldReturnMaxWeek()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            var game1 = new Game { Id = 1, SeasonId = seasonId, Week = 1, GuestName = "Guest", HostName = "Host" };
            var game2 = new Game { Id = 2, SeasonId = seasonId, Week = 2, GuestName = "Guest", HostName = "Host" };
            var game3 = new Game { Id = 3, SeasonId = seasonId, Week = 3, GuestName = "Guest", HostName = "Host" };
            fakeDbContext.Games.AddRange(game1, game2, game3);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetMaxWeekForSeasonAsync(1920);

            // Assert
            result.ShouldBe(3);
        }

        [Fact]
        public async Task GetMaxWeekForSeasonAsync_WhenWeeksDoNotExistForSeason_ShouldReturnZero()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetMaxWeekForSeasonAsync(1920);

            // Assert
            result.ShouldBe(0);
        }
    }
}
