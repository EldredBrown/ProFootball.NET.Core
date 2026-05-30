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
        public GameRepositoryTest() { }

        [Fact]
        public void GetGames_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var fakeDbSet = games.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GetGames();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Game>();
            }
        }

        [Fact]
        public void GetGamesAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var fakeDbSet = games.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GetGames();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetGame_WhenGamesIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = testRepository.GetGame(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(game);
        }

        [Fact]
        public void GetGame_WhenGamesIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Games = null;
            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = testRepository.GetGame(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetGameAsync_WhenGamesIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetGameAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(game);
        }

        [Fact]
        public async Task GetGameAsync_WhenGamesIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Games = null;
            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetGameAsync(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();
            var repository = new GameRepository(fakeDbContext);

            // Act
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var result = repository.Add(game);

            // Assert
            A.CallTo(() => fakeDbContext.Add(game)).MustHaveHappenedOnceExactly();
            result.ShouldBe(game);
        }

        [Fact]
        public void AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();
            var repository = new GameRepository(fakeDbContext);

            // Act
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var result = repository.AddAsync(game).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(game)).MustHaveHappenedOnceExactly();
            result.ShouldBe(game);
        }

        [Fact]
        public async Task Update_WhenGamesIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            game.SeasonId = 2026;
            testRepository.Update(game);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Games.FirstOrDefault(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenGamesIsNotNullAndSelectedGameIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var gameCountBeforeDelete = fakeDbContext.Games.Count();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(game.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Games.Count().ShouldBe(gameCountBeforeDelete - 1);
            result.ShouldBe(game);
        }

        [Fact]
        public void Delete_WhenGamesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Games = null;

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var result = testRepository.Delete(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenGamesIsNotNullAndSelectedGameIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };

            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            int id = 1;
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var gameCountBeforeDelete = fakeDbContext.Games.Count();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Games.Count().ShouldBe(gameCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenGamesIsNotNullAndSelectedGameIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var gameCountBeforeDelete = fakeDbContext.Games.Count();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(game.Id);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Games.Count().ShouldBe(gameCountBeforeDelete - 1);
            result.ShouldBe(game);
        }

        [Fact]
        public async Task DeleteAsync_WhenGamesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Games = null;

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var result = await testRepository.DeleteAsync(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenGamesIsNotNullAndSelectedGameIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };

            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            int id = 1;
            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            fakeDbContext.Games.Add(game);
            fakeDbContext.SaveChanges();

            var gameCountBeforeDelete = fakeDbContext.Games.Count();

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(2);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Games.Count().ShouldBe(gameCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void GameExists_WhenGamesIsNotNullAndSelectedGameExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var fakeDbSet = new List<Game> { game }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = testRepository.GameExists(game.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void GameExists_WhenGamesIsNotNullAndSelectedGameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var fakeDbSet = new List<Game> { game }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = testRepository.GameExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GameExistsAsync_WhenGamesIsNotNullAndSelectedGameExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var fakeDbSet = new List<Game> { game }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = await testRepository.GameExistsAsync(game.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task GameExistsAsync_WhenGamesIsNotNullAndSelectedGameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var game = new Game
            {
                Id = 1,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var fakeDbSet = new List<Game> { game }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);

            var testRepository = new GameRepository(fakeDbContext);

            // Act
            var result = await testRepository.GameExistsAsync(2);

            // Assert
            result.ShouldBeFalse();
        }
    }
}
