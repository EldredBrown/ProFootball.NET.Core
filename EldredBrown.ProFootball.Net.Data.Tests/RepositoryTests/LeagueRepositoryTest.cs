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
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();

            int id = 1;
            var league = new League
            {
                Id = id,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };

            A.CallTo(() => fakeDbContext.Leagues.Find(An<int>.Ignored)).Returns(league);
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.GetLeague(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(league);
        }

        [Fact]
        public void GetLeague_WhenLeaguesIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = null;
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.GetLeague(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueAsync_WhenLeaguesIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();

            int id = 1;
            var league = new League
            {
                Id = id,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };

            _ = A.CallTo(() => fakeDbContext.Leagues.FindAsync(An<int>.Ignored)).Returns(new ValueTask<League?>(league));
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagueAsync(id).Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(league);
        }

        [Fact]
        public void GetLeagueAsync_WhenLeaguesIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = null;
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagueAsync(1).Result;

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

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };

            // Act
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

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };

            // Act
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
            await fakeDbContext.SaveChangesAsync();

            var repository = new LeagueRepository(fakeDbContext);

            // Act
            league.LastSeasonId = 2026;
            repository.Update(league);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            var updated = await fakeDbContext.Leagues.FirstOrDefaultAsync(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenLeaguesIsNotNullAndSelectedLeagueIsNotNull_ShouldSucceed()
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

            A.CallTo(() => fakeDbContext.Leagues.Find(An<int>.Ignored)).Returns(league);
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.Delete(league.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Leagues.Remove(league)).MustHaveHappenedOnceExactly();
            result.ShouldBe(league);
        }

        [Fact]
        public void Delete_WhenLeaguesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = null;
            var repository = new LeagueRepository(fakeDbContext);

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };

            // Act
            var result = repository.Delete(league.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenLeaguesIsNotNullAndSelectedLeagueIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();
            A.CallTo(() => fakeDbContext.Leagues.Find(An<int>.Ignored)).Returns(null);
            var repository = new LeagueRepository(fakeDbContext);

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };

            // Act
            var result = repository.Delete(league.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Leagues.Remove(league)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenLeaguesIsNotNullAndSelectedLeagueIsNotNull_ShouldSucceed()
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

            _ = A.CallTo(() => fakeDbContext.Leagues.FindAsync(An<int>.Ignored)).Returns(new ValueTask<League?>(league));
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.DeleteAsync(league.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Leagues.Remove(league)).MustHaveHappenedOnceExactly();
            result.ShouldBe(league);
        }

        [Fact]
        public void DeleteAsync_WhenLeaguesIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = null;
            var repository = new LeagueRepository(fakeDbContext);

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };

            // Act
            var result = repository.DeleteAsync(league.Id).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenLeaguesIsNotNullAndSelectedLeagueIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = A.Fake<DbSet<League>>();
            _ = A.CallTo(() => fakeDbContext.Leagues.FindAsync(An<int>.Ignored)).Returns(null);
            var repository = new LeagueRepository(fakeDbContext);

            var league = new League
            {
                Id = 1,
                ShortName = "L1",
                LongName = "League 1",
                FirstSeasonId = 1920
            };

            // Act
            var result = repository.DeleteAsync(league.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Leagues.Remove(league)).MustNotHaveHappened();
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
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.LeagueExists(league.Id);

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
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.LeagueExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void LeagueExistsAsync_WhenLeaguesIsNotNullAndSelectedLeagueExists_ShouldReturnTrue()
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
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.LeagueExistsAsync(league.Id).Result;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void LeagueExistsAsync_WhenLeaguesIsNotNullAndSelectedLeagueDoesNotExist_ShouldReturnFalse()
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
            var repository = new LeagueRepository(fakeDbContext);

            // Act
            var result = repository.LeagueExistsAsync(2).Result;

            // Assert
            result.ShouldBeFalse();
        }
    }
}
