using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class LeagueSeasonRepositoryTest
    {
        public LeagueSeasonRepositoryTest() { }

        [Fact]
        public void GetLeagueSeasons_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeasons = new List<LeagueSeason>
            {
                new LeagueSeason { Id = 1, LeagueName = "NFL", SeasonYear = 1920 },
                new LeagueSeason { Id = 2, LeagueName = "NFL", SeasonYear = 1921 },
                new LeagueSeason { Id = 3, LeagueName = "NFL", SeasonYear = 1922 },
            };

            var fakeDbSet = leagueSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagueSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
            }
        }

        [Fact]
        public void GetLeagueSeasonsAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeasons = new List<LeagueSeason>
            {
                new LeagueSeason { Id = 1, LeagueName = "NFL", SeasonYear = 1920 },
                new LeagueSeason { Id = 2, LeagueName = "NFL", SeasonYear = 1921 },
                new LeagueSeason { Id = 3, LeagueName = "NFL", SeasonYear = 1922 },
            };

            var fakeDbSet = leagueSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagueSeasonsAsync().Result;

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetLeagueSeason_WhenLeagueSeasonsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                Id = id,
                LeagueName = "NFL",
                SeasonYear = 1920
            };

            A.CallTo(() => fakeDbContext.LeagueSeasons.Find(An<int>.Ignored)).Returns(leagueSeason);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagueSeason(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void GetLeagueSeason_WhenLeagueSeasonsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = null;
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagueSeason(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonAsync_WhenLeagueSeasonsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                Id = id,
                LeagueName = "NFL",
                SeasonYear = 1920
            };

            _ = A.CallTo(() => fakeDbContext.LeagueSeasons.FindAsync(An<int>.Ignored))
                .Returns(new ValueTask<LeagueSeason?>(leagueSeason));
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagueSeasonAsync(id).Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void GetLeagueSeasonAsync_WhenLeagueSeasonsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = null;
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetLeagueSeasonAsync(1).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            var repository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920
            };

            // Act
            var result = repository.Add(leagueSeason);

            // Assert
            A.CallTo(() => fakeDbContext.Add(leagueSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            var repository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920
            };

            // Act
            var result = repository.AddAsync(leagueSeason).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task Update_WhenLeagueSeasonsIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var fakeDbContext = new ProFootballDbContext(options);

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            await fakeDbContext.SaveChangesAsync();

            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            leagueSeason.TotalGames = 100;
            repository.Update(leagueSeason);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            var updated = await fakeDbContext.LeagueSeasons.FirstOrDefaultAsync(ls => ls.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            A.CallTo(() => fakeDbContext.LeagueSeasons.Find(An<int>.Ignored)).Returns(leagueSeason);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.Delete(leagueSeason.Id);

            // Assert
            A.CallTo(() => fakeDbContext.LeagueSeasons.Remove(leagueSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Delete_WhenLeagueSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = null;
            var repository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            // Act
            var result = repository.Delete(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            A.CallTo(() => fakeDbContext.LeagueSeasons.Find(An<int>.Ignored)).Returns(null);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            // Act
            var result = repository.Delete(leagueSeason.Id);

            // Assert
            A.CallTo(() => fakeDbContext.LeagueSeasons.Remove(leagueSeason)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            _ = A.CallTo(() => fakeDbContext.LeagueSeasons.FindAsync(An<int>.Ignored)).Returns(
                new ValueTask<LeagueSeason?>(leagueSeason));
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.DeleteAsync(leagueSeason.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.LeagueSeasons.Remove(leagueSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void DeleteAsync_WhenLeagueSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = null;
            var repository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            // Act
            var result = repository.DeleteAsync(leagueSeason.Id).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            _ = A.CallTo(() => fakeDbContext.LeagueSeasons.FindAsync(An<int>.Ignored)).Returns(null);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            // Act
            var result = repository.DeleteAsync(leagueSeason.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.LeagueSeasons.Remove(leagueSeason)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void LeagueSeasonExists_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            var fakeDbSet = new List<LeagueSeason> { leagueSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.LeagueSeasonExists(leagueSeason.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void LeagueSeasonExists_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            var fakeDbSet = new List<LeagueSeason> { leagueSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.LeagueSeasonExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void LeagueSeasonExistsAsync_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            var fakeDbSet = new List<LeagueSeason> { leagueSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.LeagueSeasonExistsAsync(leagueSeason.Id).Result;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void LeagueSeasonExistsAsync_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueName = "NFL",
                SeasonYear = 1920,
                TotalGames = 1,
            };

            var fakeDbSet = new List<LeagueSeason> { leagueSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = repository.LeagueSeasonExistsAsync(2).Result;

            // Assert
            result.ShouldBeFalse();
        }
    }
}
