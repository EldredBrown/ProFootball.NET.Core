using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class DivisionRepositoryTest
    {
        public DivisionRepositoryTest() { }

        [Fact]
        public void GetDivisions_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var divisions = new List<Division>
            {
                new Division { Id = 1, Name = "Division 1" },
                new Division { Id = 2, Name = "Division 2" },
                new Division { Id = 3, Name = "Division 3" },
            };

            var fakeDbSet = divisions.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.GetDivisions();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Division>();
            }
        }

        [Fact]
        public void GetDivisionsAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var divisions = new List<Division>
            {
                new Division { Id = 1, Name = "Division 1" },
                new Division { Id = 2, Name = "Division 2" },
                new Division { Id = 3, Name = "Division 3" },
            };

            var fakeDbSet = divisions.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.GetDivisions();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetDivision_WhenDivisionsIsNotNull_ShouldSucceed()
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
            var division = new Division
            {
                Id = id,
                Name = "Division 1",
                LeagueId = 1,
                ConferenceId = 1,
                FirstSeasonId = 1920
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = testRepository.GetDivision(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(division);
        }

        [Fact]
        public void GetDivision_WhenDivisionsIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Divisions = null!;
            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = testRepository.GetDivision(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetDivisionAsync_WhenDivisionsIsNotNull_ShouldSucceed()
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
            var division = new Division
            {
                Id = id,
                Name = "Division 1",
                LeagueId = 1,
                ConferenceId = 1,
                FirstSeasonId = 1920
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetDivisionAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(division);
        }

        [Fact]
        public async Task GetDivisionAsync_WhenDivisionsIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Divisions = null!;
            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetDivisionAsync(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            var result = repository.Add(division);

            // Assert
            A.CallTo(() => fakeDbContext.Add(division)).MustHaveHappenedOnceExactly();
            result.ShouldBe(division);
        }

        [Fact]
        public async Task AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            var result = await repository.AddAsync(division);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(division)).MustHaveHappenedOnceExactly();
            result.ShouldBe(division);
        }

        [Fact]
        public async Task Update_WhenDivisionsIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            division.LastSeasonId = 2026;
            testRepository.Update(division);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Divisions.FirstOrDefault(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenDivisionsIsNotNullAndSelectedDivisionIsNotNull_ShouldSucceed()
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
            var division = new Division
            {
                Id = id,
                Name = "Division 1",
                LeagueId = 1,
                FirstSeasonId = 1920
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var divisionCountBeforeDelete = fakeDbContext.Divisions.Count();

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(division.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Divisions.Count().ShouldBe(divisionCountBeforeDelete - 1);
            result.ShouldBe(division);
        }

        [Fact]
        public void Delete_WhenDivisionsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Divisions = null!;

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            var result = testRepository.Delete(division.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDivisionsIsNotNullAndSelectedDivisionIsNull_ShouldFailAndReturnNull()
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
            var division = new Division
            {
                Id = id,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var divisionCountBeforeDelete = fakeDbContext.Divisions.Count();

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Divisions.Count().ShouldBe(divisionCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDivisionsIsNotNullAndSelectedDivisionIsNotNull_ShouldSucceed()
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
            var division = new Division
            {
                Id = id,
                Name = "Division 1",
                LeagueId = 1,
                FirstSeasonId = 1920
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var divisionCountBeforeDelete = fakeDbContext.Divisions.Count();

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(division.Id);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Divisions.Count().ShouldBe(divisionCountBeforeDelete - 1);
            result.ShouldBe(division);
        }

        [Fact]
        public async Task DeleteAsync_WhenDivisionsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Divisions = null!;

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            var result = await testRepository.DeleteAsync(division.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDivisionsIsNotNullAndSelectedDivisionIsNull_ShouldFailAndReturnNull()
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
            var division = new Division
            {
                Id = id,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var divisionCountBeforeDelete = fakeDbContext.Divisions.Count();

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(2);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Divisions.Count().ShouldBe(divisionCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void DivisionExists_WhenDivisionsIsNotNullAndSelectedDivisionExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<Division> { division }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = testRepository.DivisionExists(division.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void DivisionExists_WhenDivisionsIsNotNullAndSelectedDivisionDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<Division> { division }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = testRepository.DivisionExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task DivisionExistsAsync_WhenDivisionsIsNotNullAndSelectedDivisionExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<Division> { division }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = await testRepository.DivisionExistsAsync(division.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task DivisionExistsAsync_WhenDivisionsIsNotNullAndSelectedDivisionDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var division = new Division
            {
                Id = 1,
                Name = "Division 1",
                FirstSeasonId = 1920
            };
            var fakeDbSet = new List<Division> { division }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            var result = await testRepository.DivisionExistsAsync(2);

            // Assert
            result.ShouldBeFalse();
        }
    }
}
