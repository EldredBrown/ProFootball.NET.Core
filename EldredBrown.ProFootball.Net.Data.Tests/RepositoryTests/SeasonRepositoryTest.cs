using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class SeasonRepositoryTest
    {
        [Fact]
        public void GetSeasons_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnSeasons()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = testRepository.GetSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(seasons.Count);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Season>();
            }
        }

        [Fact]
        public void GetSeasons_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = testRepository.GetSeasons();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetSeasons_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var seasons = new List<Season>();
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = testRepository.GetSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetSeasonsAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnSeasons()
        {
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = await testRepository.GetSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(seasons.Count);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Season>();
            }
        }

        [Fact]
        public async Task GetSeasonsAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = await testRepository.GetSeasonsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetSeasonsAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var seasons = new List<Season>();
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = await testRepository.GetSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public void GetSeason_WhenDbSetIsNeitherNullNorEmptyAndSeasonIsFound_ShouldReturnSeason()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var year = 1920;
            var result = testRepository.GetSeason(year);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Season>();
            result.Year.ShouldBe(year);
        }

        [Fact]
        public void GetSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var year = 1920;
            var result = testRepository.GetSeason(year);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var seasons = new List<Season>();
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var year = 1920;
            var result = testRepository.GetSeason(year);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetSeason_WhenSeasonIsNotFound_ShouldReturnNull()
        {
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var year = -1;
            var result = testRepository.GetSeason(year);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndSeasonIsFound_ShouldReturnSeason()
        {
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var year = 1920;
            var result = await testRepository.GetSeasonAsync(year);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Season>();
            result.Year.ShouldBe(year);
        }

        [Fact]
        public async Task GetSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var year = 1920;
            var result = await testRepository.GetSeasonAsync(year);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var seasons = new List<Season>();
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var year = 1920;
            var result = await testRepository.GetSeasonAsync(year);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetSeasonAsync_WhenSeasonIsNotFound_ShouldReturnNull()
        {
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var year = -1;
            var result = await testRepository.GetSeasonAsync(year);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddSeason()
        {
            // Arrange
            var (testRepository, fakeDbContext) = CreateFakeDbContextForAddOperations();

            // Act
            var season = new Season { Year = 1920 };
            var result = testRepository.Add(season);

            // Assert
            A.CallTo(() => fakeDbContext.Add(season)).MustHaveHappenedOnceExactly();
            result.ShouldBe(season);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            List<Season>? seasons = [];
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act & Assert
            Season? season = null!;
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(season));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnSeasonWithoutAddingIt()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, ProFootballDbContext fakeDbContext) = CreateTestRepository(seasons);

            // Act
            var season = new Season { Year = 1920 };
            var result = testRepository.Add(season);

            // Assert
            A.CallTo(() => fakeDbContext.Add(season)).MustNotHaveHappened();
            result.ShouldBe(season);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddSeason()
        {
            // Arrange
            var (testRepository, fakeDbContext) = CreateFakeDbContextForAddOperations();

            // Act
            var season = new Season { Year = 1920 };
            var result = await testRepository.AddAsync(season);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(season)).MustHaveHappenedOnceExactly();
            result.ShouldBe(season);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            List<Season> seasons = [];
            (SeasonRepository testRepository, ProFootballDbContext fakeDbContext) = CreateTestRepository(seasons);

            Season? season = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(season));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnSeasonWithoutAddingIt()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, ProFootballDbContext fakeDbContext) = 
                CreateTestRepository(seasons!);

            // Act
            var season = new Season { Year = 1920 };
            var result = await testRepository.AddAsync(season);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(season)).MustNotHaveHappened();
            result.ShouldBe(season);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var season = new Season { Year = 1920 };
            fakeDbContext.Seasons.Add(season);
            fakeDbContext.SaveChanges();

            var testRepository = new SeasonRepository(fakeDbContext);

            var seasonCountBeforeDelete = fakeDbContext.Seasons.Count();

            // Act
            var result = testRepository.Delete(season.Year);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Seasons.Count().ShouldBe(seasonCountBeforeDelete - 1);
            result.ShouldBe(season);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var seasonYear = 1920;
            var result = testRepository.Delete(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var seasons = new List<Season>();
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var seasonYear = 1920;
            var result = testRepository.Delete(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var season = new Season { Year = 1920 };
            fakeDbContext.Seasons.Add(season);
            fakeDbContext.SaveChanges();

            var testRepository = new SeasonRepository(fakeDbContext);

            var leagueSeasonCountBeforeDelete = fakeDbContext.Seasons.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Seasons.Count().ShouldBe(leagueSeasonCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var season = new Season { Year = 1920 };
            fakeDbContext.Seasons.Add(season);
            fakeDbContext.SaveChanges();

            var testRepository = new SeasonRepository(fakeDbContext);

            var seasonCountBeforeDelete = fakeDbContext.Seasons.Count();

            // Act
            var result = await testRepository.DeleteAsync(season.Year);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Seasons.Count().ShouldBe(seasonCountBeforeDelete - 1);
            result.ShouldBe(season);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.DeleteAsync(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var seasons = new List<Season>();
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.DeleteAsync(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var season = new Season { Year = 1920 };
            fakeDbContext.Seasons.Add(season);
            fakeDbContext.SaveChanges();

            var testRepository = new SeasonRepository(fakeDbContext);

            var leagueSeasonCountBeforeDelete = fakeDbContext.Seasons.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Seasons.Count().ShouldBe(leagueSeasonCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void SeasonExists_WhenDbSetIsNotNullAndSelectedSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = testRepository.SeasonExists(1920);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void SeasonExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = testRepository.SeasonExists(1920);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void SeasonExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var seasons = new List<Season>();
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = testRepository.SeasonExists(1920);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void SeasonExists_WhenSelectedSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = testRepository.SeasonExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task SeasonExistsAsync_WhenDbSetIsNotNullAndSelectedSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = await testRepository.SeasonExistsAsync(1920);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task SeasonExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<Season> seasons = null!;
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = await testRepository.SeasonExistsAsync(1920);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task SeasonExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var seasons = new List<Season>();
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = await testRepository.SeasonExistsAsync(1920);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task SeasonExistsAsync_WhenSelectedSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            (SeasonRepository testRepository, _) = CreateTestRepository(seasons);

            // Act
            var result = await testRepository.SeasonExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private static (SeasonRepository, ProFootballDbContext) CreateFakeDbContextForAddOperations()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();
            var testRepository = new SeasonRepository(fakeDbContext);

            return (testRepository, fakeDbContext);
        }

        private static (SeasonRepository, ProFootballDbContext) CreateTestRepository(List<Season>? seasons)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();
            DbSet<Season> fakeDbSet = seasons is not null ? seasons.BuildMockDbSet() : null!;
            A.CallTo(() => fakeDbContext.Seasons).Returns(fakeDbSet);

            return (new SeasonRepository(fakeDbContext), fakeDbContext);
        }
    }
}
