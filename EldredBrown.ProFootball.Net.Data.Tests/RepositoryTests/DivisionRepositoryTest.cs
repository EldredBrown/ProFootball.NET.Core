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
                new Division
                {
                    Id = 1,
                    Name = "East",
                    LeagueName = "L1",
                    ConferenceName = "C1",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Central",
                    LeagueName = "L1",
                    ConferenceName = "C1",
                    FirstSeasonYear = 1921
                },
                new Division 
                {
                    Id = 3,
                    Name = "West",
                    LeagueName = "L1",
                    ConferenceName = "C1",
                    FirstSeasonYear = 1922
                },
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
                new Division
                {
                    Id = 1,
                    Name = "East",
                    LeagueName = "L1",
                    ConferenceName = "C1",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Central",
                    LeagueName = "L1",
                    ConferenceName = "C1",
                    FirstSeasonYear = 1921
                },
                new Division
                {
                    Id = 3,
                    Name = "West",
                    LeagueName = "L1",
                    ConferenceName = "C1",
                    FirstSeasonYear = 1922
                },
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
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            int id = 1;
            var division = new Division
            {
                Id = id,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            A.CallTo(() => fakeDbContext.Divisions.Find(An<int>.Ignored)).Returns(division);
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.GetDivision(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(division);
        }

        [Fact]
        public void GetDivision_WhenDivisionsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = null;
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.GetDivision(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetDivisionAsync_WhenDivisionsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            int id = 1;
            var division = new Division
            {
                Id = id,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            _ = A.CallTo(() => fakeDbContext.Divisions.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Division?>(division));
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.GetDivisionAsync(id).Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(division);
        }

        [Fact]
        public void GetDivisionAsync_WhenDivisionsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = null;
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.GetDivisionAsync(1).Result;

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

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            // Act
            var result = repository.Add(division);

            // Assert
            A.CallTo(() => fakeDbContext.Add(division)).MustHaveHappenedOnceExactly();
            result.ShouldBe(division);
        }

        [Fact]
        public void AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();
            var repository = new DivisionRepository(fakeDbContext);

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            // Act
            var result = repository.AddAsync(division).Result;

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
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            fakeDbContext.Divisions.Add(division);
            await fakeDbContext.SaveChangesAsync();

            var repository = new DivisionRepository(fakeDbContext);

            // Act
            division.LastSeasonYear = 2026;
            repository.Update(division);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            var updated = await fakeDbContext.Divisions.FirstOrDefaultAsync(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenDivisionsIsNotNullAndSelectedDivisionIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            A.CallTo(() => fakeDbContext.Divisions.Find(An<int>.Ignored)).Returns(division);
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.Delete(division.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Divisions.Remove(division)).MustHaveHappenedOnceExactly();
            result.ShouldBe(division);
        }

        [Fact]
        public void Delete_WhenDivisionsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = null;
            var repository = new DivisionRepository(fakeDbContext);

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            // Act
            var result = repository.Delete(division.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDivisionsIsNotNullAndSelectedDivisionIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();
            A.CallTo(() => fakeDbContext.Divisions.Find(An<int>.Ignored)).Returns(null);
            var repository = new DivisionRepository(fakeDbContext);

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            // Act
            var result = repository.Delete(division.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Divisions.Remove(division)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenDivisionsIsNotNullAndSelectedDivisionIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            _ = A.CallTo(() => fakeDbContext.Divisions.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Division?>(division));
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.DeleteAsync(division.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Divisions.Remove(division)).MustHaveHappenedOnceExactly();
            result.ShouldBe(division);
        }

        [Fact]
        public void DeleteAsync_WhenDivisionsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = null;
            var repository = new DivisionRepository(fakeDbContext);

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            // Act
            var result = repository.DeleteAsync(division.Id).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenDivisionsIsNotNullAndSelectedDivisionIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();
            _ = A.CallTo(() => fakeDbContext.Divisions.FindAsync(An<int>.Ignored)).Returns(null);
            var repository = new DivisionRepository(fakeDbContext);

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            // Act
            var result = repository.DeleteAsync(division.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Divisions.Remove(division)).MustNotHaveHappened();
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
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            var fakeDbSet = new List<Division> { division }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.DivisionExists(division.Id);

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
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            var fakeDbSet = new List<Division> { division }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.DivisionExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void DivisionExistsAsync_WhenDivisionsIsNotNullAndSelectedDivisionExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            var fakeDbSet = new List<Division> { division }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.DivisionExistsAsync(division.Id).Result;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void DivisionExistsAsync_WhenDivisionsIsNotNullAndSelectedDivisionDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Divisions = A.Fake<DbSet<Division>>();

            var division = new Division
            {
                Id = 1,
                Name = "Division",
                LeagueName = "L1",
                ConferenceName = "C1",
                FirstSeasonYear = 1920
            };

            var fakeDbSet = new List<Division> { division }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Divisions).Returns(fakeDbSet);
            var repository = new DivisionRepository(fakeDbContext);

            // Act
            var result = repository.DivisionExistsAsync(2).Result;

            // Assert
            result.ShouldBeFalse();
        }
    }
}
