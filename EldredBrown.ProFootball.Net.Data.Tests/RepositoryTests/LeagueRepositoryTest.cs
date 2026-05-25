using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class LeagueRepositoryTest
    {
        public LeagueRepositoryTest() { }

        [Fact]
        public void GetLeagues_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();

            var leagues = new List<League>
            {
                new League { Id = 1, ShortName = "L1", LongName = "League 1", FirstSeasonId = 1920 },
                new League { Id = 2, ShortName = "L2", LongName = "League 2", FirstSeasonId = 1921 },
                new League { Id = 3, ShortName = "L3", LongName = "League 3", FirstSeasonId = 1922 },
            };

            var fakeDbSet = leagues.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagues();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<League>();
            }
        }

        [Fact]
        public void GetLeaguesAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();

            var leagues = new List<League>
            {
                new League { Id = 1, ShortName = "L1", LongName = "League 1", FirstSeasonId = 1920 },
                new League { Id = 2, ShortName = "L2", LongName = "League 2", FirstSeasonId = 1921 },
                new League { Id = 3, ShortName = "L3", LongName = "League 3", FirstSeasonId = 1922 },
            };

            var fakeDbSet = leagues.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagues();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetLeague_WhenLeaguesIsNotNull_ShouldSucceed()
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

            int id = 1;
            var league = new League
            {
                Id = id,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = testRepository.GetLeague(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(league);
        }

        [Fact]
        public void GetLeague_WhenLeaguesIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Leagues = null;
            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = testRepository.GetLeague(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueAsync_WhenLeaguesIsNotNull_ShouldSucceed()
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

            int id = 1;
            var league = new League
            {
                Id = id,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetLeagueAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(league);
        }

        [Fact]
        public async Task GetLeagueAsync_WhenLeaguesIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Leagues = null;
            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetLeagueAsync(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueByShortName_WhenLeaguesIsNotNull_ShouldSucceed()
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

            string shortName = "L1";
            var league = new League
            {
                Id = 1,
                ShortName = shortName,
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = testRepository.GetLeagueByShortName(shortName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(league);
        }

        [Fact]
        public void GetLeagueByShortName_WhenLeaguesIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Leagues = null;
            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = testRepository.GetLeagueByShortName("L1");

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueByShortNameAsync_WhenLeaguesIsNotNull_ShouldSucceed()
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

            string shortName = "L1";
            var league = new League
            {
                Id = 1,
                ShortName = shortName,
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetLeagueByShortNameAsync(shortName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(league);
        }

        [Fact]
        public async Task GetLeagueByShortNameAsync_WhenLeaguesIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Leagues = null;
            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetLeagueByShortNameAsync("L1");

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            var result = repository.Add(league);

            // Assert
            A.CallTo(() => fakeDbContext.Add(league)).MustHaveHappenedOnceExactly();
            result.ShouldBe(league);
        }

        [Fact]
        public void AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            var result = repository.AddAsync(league).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(league)).MustHaveHappenedOnceExactly();
            result.ShouldBe(league);
        }

        [Fact]
        public async Task Update_WhenLeaguesIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            league.LastSeasonId = 2026;
            testRepository.Update(league);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Leagues.FirstOrDefault(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenLeaguesIsNotNullAndSelectedLeagueIsNotNull_ShouldSucceed()
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
            var league = new League
            {
                Id = id,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueCountBeforeDelete = fakeDbContext.Leagues.Count();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(league.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Leagues.Count().ShouldBe(leagueCountBeforeDelete - 1);
            result.ShouldBe(league);
        }

        [Fact]
        public void Delete_WhenLeaguesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Leagues = null;

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            var result = testRepository.Delete(league.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenLeaguesIsNotNullAndSelectedLeagueIsNull_ShouldFailAndReturnNull()
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
            var league = new League
            {
                Id = id,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueCountBeforeDelete = fakeDbContext.Leagues.Count();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Leagues.Count().ShouldBe(leagueCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenLeaguesIsNotNullAndSelectedLeagueIsNotNull_ShouldSucceed()
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
            var league = new League
            {
                Id = id,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueCountBeforeDelete = fakeDbContext.Leagues.Count();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(league.Id);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Leagues.Count().ShouldBe(leagueCountBeforeDelete - 1);
            result.ShouldBe(league);
        }

        [Fact]
        public async Task DeleteAsync_WhenLeaguesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Leagues = null;

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            var result = await testRepository.DeleteAsync(league.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenLeaguesIsNotNullAndSelectedLeagueIsNull_ShouldFailAndReturnNull()
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
            var league = new League
            {
                Id = id,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueCountBeforeDelete = fakeDbContext.Leagues.Count();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(2);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Leagues.Count().ShouldBe(leagueCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void LeagueExists_WhenLeaguesIsNotNullAndSelectedLeagueExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<League> { league }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = testRepository.LeagueExists(league.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void LeagueExists_WhenLeaguesIsNotNullAndSelectedLeagueDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<League> { league }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = testRepository.LeagueExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueExistsAsync_WhenLeaguesIsNotNullAndSelectedLeagueExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<League> { league }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = await testRepository.LeagueExistsAsync(league.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task LeagueExistsAsync_WhenLeaguesIsNotNullAndSelectedLeagueDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<League> { league }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            var result = await testRepository.LeagueExistsAsync(2);

            // Assert
            result.ShouldBeFalse();
        }
    }
}
