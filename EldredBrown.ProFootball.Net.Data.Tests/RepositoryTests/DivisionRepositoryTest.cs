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
        [Fact]
        public void GetDivisions_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnDivisions()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetDivisions();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Division>();
            }
        }

        [Fact]
        public void GetDivisions_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.GetDivisions();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetDivisions_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.GetDivisions();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetDivisionsAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnDivisions()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetDivisionsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Division>();
            }
        }

        [Fact]
        public async Task GetDivisionsAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.GetDivisionsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetDivisionsAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.GetDivisionsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetDivision_WhenDbSetIsNeitherNullNorEmptyAndDivisionIsFound_ShouldReturnDivision()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetDivision(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetDivision_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetDivision(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetDivision_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetDivision(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetDivision_WhenDivisionIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = testRepository.GetDivision(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetDivisionAsync_WhenDbSetIsNeitherNullNorEmptyAndDivisionIsFound_ShouldReturnDivision()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetDivisionAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetDivisionAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetDivisionAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetDivisionAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetDivisionAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetDivisionAsync_WhenDivisionIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = await testRepository.GetDivisionAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetDivisionByName_WhenDbSetIsNeitherNullNorEmptyAndDivisionIsFound_ShouldReturnDivision()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var name = "Division 1";

            // Act
            var result = testRepository.GetDivisionByName(name);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.Name.ShouldBe(name);
        }

        [Fact]
        public void GetDivisionByName_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var name = "Division 1";

            // Act
            var result = testRepository.GetDivisionByName(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetDivisionByName_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var name = "Division 1";

            // Act
            var result = testRepository.GetDivisionByName(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetDivisionByName_WhenDivisionIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var name = "Division 99";

            // Act
            var result = testRepository.GetDivisionByName(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetDivisionByNameAsync_WhenDbSetIsNeitherNullNorEmptyAndDivisionIsFound_ShouldReturnDivision()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var name = "Division 1";

            // Act
            var result = await testRepository.GetDivisionByNameAsync(name);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.Name.ShouldBe(name);
        }

        [Fact]
        public async Task GetDivisionByNameAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var name = "Division 1";

            // Act
            var result = await testRepository.GetDivisionByNameAsync(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetDivisionByNameAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var name = "Division 1";

            // Act
            var result = await testRepository.GetDivisionByNameAsync(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetDivisionByNameAsync_WhenDivisionIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var name = "Division 99";

            // Act
            var result = await testRepository.GetDivisionByNameAsync(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddDivision()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Division>>());
            var testRepository = new DivisionRepository(fakeDbContext);

            var division = new Division { Id = 1 };

            // Act
            var result = testRepository.Add(division);

            // Assert
            A.CallTo(() => fakeDbContext.Add(division)).MustHaveHappenedOnceExactly();
            result.ShouldBe(division);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Division>>());
            var testRepository = new DivisionRepository(fakeDbContext);

            Division? division = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(division));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnDivisionWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new DivisionRepository(fakeDbContext);

            var division = new Division { Id = 1 };

            // Act
            var result = testRepository.Add(division);

            // Assert
            A.CallTo(() => fakeDbContext.Add(division)).MustNotHaveHappened();
            result.ShouldBe(division);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddDivision()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Division>>());
            var testRepository = new DivisionRepository(fakeDbContext);

            var division = new Division { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(division);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(division)).MustHaveHappenedOnceExactly();
            result.ShouldBe(division);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Division>>());
            var testRepository = new DivisionRepository(fakeDbContext);

            Division? division = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(division));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnDivisionWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new DivisionRepository(fakeDbContext);

            var division = new Division { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(division);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(division)).MustNotHaveHappened();
            result.ShouldBe(division);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var division = new Division
            {
                Id = 1,
                Name = "Divsion 1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var testRepository = new DivisionRepository(fakeDbContext);

            // Act
            testRepository.Update(division);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Divisions.FirstOrDefault(d => d.Id == division.Id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            Division? division = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(division));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnDivision()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            Division? division = new();

            // Act
            var updated = testRepository.Update(division);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(division);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnDivision()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            Division? division = new();

            // Act
            var updated = testRepository.Update(division);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(division);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedDivisionIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var division = new Division
            {
                Id = 1,
                Name = "Divsion 1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var testRepository = new DivisionRepository(fakeDbContext);

            var divisionCountBeforeDelete = fakeDbContext.Divisions.Count();

            // Act
            var result = testRepository.Delete(division.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Divisions.Count().ShouldBe(divisionCountBeforeDelete - 1);
            result.ShouldBe(division);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var division = new Division { Id = 1 };

            // Act
            var result = testRepository.Delete(division.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var division = new Division { Id = 1 };

            // Act
            var result = testRepository.Delete(division.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedDivisionIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var division = new Division
            {
                Id = 1,
                Name = "Divsion 1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var testRepository = new DivisionRepository(fakeDbContext);

            var divisionDivisionCountBeforeDelete = fakeDbContext.Divisions.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Divisions.Count().ShouldBe(divisionDivisionCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedDivisionIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var division = new Division
            {
                Id = 1,
                Name = "Divsion 1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var testRepository = new DivisionRepository(fakeDbContext);

            var divisionCountBeforeDelete = fakeDbContext.Divisions.Count();

            // Act
            var result = await testRepository.DeleteAsync(division.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Divisions.Count().ShouldBe(divisionCountBeforeDelete - 1);
            result.ShouldBe(division);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var division = new Division { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(division.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var division = new Division { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(division.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedDivisionIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League 1",
                ShortName = "L1",
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var division = new Division
            {
                Id = 1,
                Name = "Divsion 1",
                LeagueId = leagueId,
                FirstSeasonId = firstSeasonId
            };
            fakeDbContext.Divisions.Add(division);
            fakeDbContext.SaveChanges();

            var testRepository = new DivisionRepository(fakeDbContext);

            var divisionDivisionCountBeforeDelete = fakeDbContext.Divisions.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Divisions.Count().ShouldBe(divisionDivisionCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void DivisionExists_WhenDbSetIsNotNullAndSelectedDivisionExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.DivisionExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void DivisionExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.DivisionExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void DivisionExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.DivisionExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void DivisionExists_WhenSelectedDivisionDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.DivisionExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task DivisionExistsAsync_WhenDbSetIsNotNullAndSelectedDivisionExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.DivisionExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task DivisionExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.DivisionExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task DivisionExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.DivisionExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task DivisionExistsAsync_WhenSelectedDivisionDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.DivisionExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<Division> divisions)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = divisions;
            return fakeDbContext;
        }

        private IDivisionRepository CreateTestRepositoryWithEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();
            var divisions = new List<Division>();
            var fakeDbSet = divisions.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);

            var testRepository = new DivisionRepository(fakeDbContext);
            return testRepository;
        }

        private IDivisionRepository CreateTestRepositoryWithNotEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();
            var divisions = new List<Division>
            {
                new() { Id = 1, Name = "Division 1", LeagueId = 1, FirstSeasonId = 1920 },
                new() { Id = 2, Name = "Division 2", LeagueId = 1, FirstSeasonId = 1920 },
                new() { Id = 3, Name = "Division 3", LeagueId = 1, FirstSeasonId = 1920 },
            };
            var fakeDbSet = divisions.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);

            var testRepository = new DivisionRepository(fakeDbContext);
            return testRepository;
        }

        private IDivisionRepository CreateTestRepositoryWithNullDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();
            DbSet<Division> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);

            var testRepository = new DivisionRepository(fakeDbContext);
            return testRepository;
        }
    }
}
