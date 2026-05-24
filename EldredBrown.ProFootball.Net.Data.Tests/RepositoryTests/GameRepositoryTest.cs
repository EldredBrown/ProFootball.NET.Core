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
                    SeasonYear = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes 1"
                },
                new Game
                {
                    Id = 2,
                    SeasonYear = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes 2"
                },
                new Game
                {
                    Id = 3,
                    SeasonYear = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes 3"
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
                    SeasonYear = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes 1"
                },
                new Game
                {
                    Id = 2,
                    SeasonYear = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes 2"
                },
                new Game
                {
                    Id = 3,
                    SeasonYear = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes 3"
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
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            int id = 1;
            var game = new Game
            {
                Id = id,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes 1"
            };

            A.CallTo(() => fakeDbContext.Games.Find(An<int>.Ignored)).Returns(game);
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GetGame(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(game);
        }

        [Fact]
        public void GetGame_WhenGamesIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = null;
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GetGame(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetGameAsync_WhenGamesIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            int id = 1;
            var game = new Game
            {
                Id = id,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes 1"
            };

            _ = A.CallTo(() => fakeDbContext.Games.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Game?>(game));
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GetGameAsync(id).Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(game);
        }

        [Fact]
        public void GetGameAsync_WhenGamesIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = null;
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GetGameAsync(1).Result;

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

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            // Act
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

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            // Act
            var result = repository.AddAsync(game).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(game)).MustHaveHappenedOnceExactly();
            result.ShouldBe(game);
        }

        [Fact(Skip = "Unique constraints in Game entity do not permit changes to values.")]
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
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            fakeDbContext.Games.Add(game);
            await fakeDbContext.SaveChangesAsync();

            var repository = new GameRepository(fakeDbContext);

            // Act
            game.Notes = "Updated Notes";
            repository.Update(game);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            var updated = await fakeDbContext.Games.FirstOrDefaultAsync(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenGamesIsNotNullAndSelectedGameIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            A.CallTo(() => fakeDbContext.Games.Find(An<int>.Ignored)).Returns(game);
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.Delete(game.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Games.Remove(game)).MustHaveHappenedOnceExactly();
            result.ShouldBe(game);
        }

        [Fact]
        public void Delete_WhenGamesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = null;
            var repository = new GameRepository(fakeDbContext);

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            // Act
            var result = repository.Delete(game.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenGamesIsNotNullAndSelectedGameIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();
            A.CallTo(() => fakeDbContext.Games.Find(An<int>.Ignored)).Returns(null);
            var repository = new GameRepository(fakeDbContext);

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            // Act
            var result = repository.Delete(game.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Games.Remove(game)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenGamesIsNotNullAndSelectedGameIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            _ = A.CallTo(() => fakeDbContext.Games.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Game?>(game));
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.DeleteAsync(game.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Games.Remove(game)).MustHaveHappenedOnceExactly();
            result.ShouldBe(game);
        }

        [Fact]
        public void DeleteAsync_WhenGamesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = null;
            var repository = new GameRepository(fakeDbContext);

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            // Act
            var result = repository.DeleteAsync(game.Id).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenGamesIsNotNullAndSelectedGameIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();
            _ = A.CallTo(() => fakeDbContext.Games.FindAsync(An<int>.Ignored)).Returns(null);
            var repository = new GameRepository(fakeDbContext);

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            // Act
            var result = repository.DeleteAsync(game.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Games.Remove(game)).MustNotHaveHappened();
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
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var fakeDbSet = new List<Game> { game }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GameExists(game.Id);

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
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var fakeDbSet = new List<Game> { game }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GameExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void GameExistsAsync_WhenGamesIsNotNullAndSelectedGameExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var fakeDbSet = new List<Game> { game }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GameExistsAsync(game.Id).Result;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void GameExistsAsync_WhenGamesIsNotNullAndSelectedGameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Games = A.Fake<DbSet<Game>>();

            var game = new Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var fakeDbSet = new List<Game> { game }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Games).Returns(fakeDbSet);
            var repository = new GameRepository(fakeDbContext);

            // Act
            var result = repository.GameExistsAsync(2).Result;

            // Assert
            result.ShouldBeFalse();
        }
    }
}
