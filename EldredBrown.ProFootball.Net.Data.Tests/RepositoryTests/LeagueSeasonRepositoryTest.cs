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
                new LeagueSeason { Id = 1, LeagueId = 1, SeasonId = 1920 },
                new LeagueSeason { Id = 2, LeagueId = 1, SeasonId = 1921 },
                new LeagueSeason { Id = 3, LeagueId = 1, SeasonId = 1922 },
            };

            var fakeDbSet = leagueSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.GetLeagueSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
            }
        }

        [Fact]
        public async Task GetLeagueSeasonsAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeasons = new List<LeagueSeason>
            {
                new LeagueSeason { Id = 1, LeagueId = 1, SeasonId = 1920 },
                new LeagueSeason { Id = 2, LeagueId = 1, SeasonId = 1921 },
                new LeagueSeason { Id = 3, LeagueId = 1, SeasonId = 1922 },
            };

            var fakeDbSet = leagueSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetLeagueSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetLeagueSeason_WhenLeagueSeasonsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            fakeDbContext.SaveChanges();

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                Id = id,
                LeagueId = 1,
                SeasonId = 1920
            };
            fakeDbContext.LeagueSeasons.Add(leagueSeason );
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void GetLeagueSeason_WhenLeagueSeasonsIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.LeagueSeasons = null!;
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.GetLeagueSeason(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenLeagueSeasonsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            fakeDbContext.SaveChanges();

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                Id = id,
                LeagueId = 1,
                SeasonId = 1920
            };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenLeagueSeasonsIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.LeagueSeasons = null!;
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetLeagueSeasonAsync(1);

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
                LeagueId = 1,
                SeasonId = 1920
            };

            // Act
            var result = repository.Add(leagueSeason);

            // Assert
            A.CallTo(() => fakeDbContext.Add(leagueSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            var repository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920
            };

            // Act
            var result = await repository.AddAsync(leagueSeason);

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
                LeagueId = 1,
                SeasonId = 1920,
                TotalGames = 1,
            };

            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var repository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            leagueSeason.TotalGames = 100;
            repository.Update(leagueSeason);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.LeagueSeasons.FirstOrDefault(ls => ls.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920,
                TotalGames = 1,
            };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var leagueSeasonCountBeforeDelete = fakeDbContext.LeagueSeasons.Count();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(leagueSeason.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.LeagueSeasons.Count().ShouldBe(leagueSeasonCountBeforeDelete - 1);
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Delete_WhenLeagueSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Leagues = null!;

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920,
                TotalGames = 1,
            };

            var result = testRepository.Delete(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920,
                TotalGames = 1,
            };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var leagueSeasonCountBeforeDelete = fakeDbContext.LeagueSeasons.Count();
    
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.LeagueSeasons.Count().ShouldBe(leagueSeasonCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920,
                TotalGames = 1,
            };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var leagueSeasonCountBeforeDelete = fakeDbContext.LeagueSeasons.Count();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(leagueSeason.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.LeagueSeasons.Count().ShouldBe(leagueSeasonCountBeforeDelete - 1);
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task DeleteAsync_WhenLeagueSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Leagues = null!;

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920,
                TotalGames = 1,
            };

            var result = await testRepository.DeleteAsync(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920,
                TotalGames = 1,
            };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var leagueSeasonCountBeforeDelete = fakeDbContext.LeagueSeasons.Count();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.LeagueSeasons.Count().ShouldBe(leagueSeasonCountBeforeDelete);
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
                LeagueId = 1,
                SeasonId = 1920
            };

            var fakeDbSet = new List<LeagueSeason> { leagueSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.LeagueSeasonExists(leagueSeason.Id);

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
                LeagueId = 1,
                SeasonId = 1920
            };

            var fakeDbSet = new List<LeagueSeason> { leagueSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.LeagueSeasonExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920
            };

            var fakeDbSet = new List<LeagueSeason> { leagueSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(leagueSeason.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenLeagueSeasonsIsNotNullAndSelectedLeagueSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();

            var leagueSeason = new LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920
            };

            var fakeDbSet = new List<LeagueSeason> { leagueSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(2);

            // Assert
            result.ShouldBeFalse();
        }
    }
}
