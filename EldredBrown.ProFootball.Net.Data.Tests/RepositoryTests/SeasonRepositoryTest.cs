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
        public void GetSeasons_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            var seasons = new List<Season>
            {
                new Season { Id = 1920 },
                new Season { Id = 1921 },
                new Season { Id = 1922 },
            };

            var fakeDbSet = seasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Seasons).Returns(fakeDbSet);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Season>();
            }
        }

        [Fact]
        public void GetSeasonsAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            var seasons = new List<Season>
            {
                new Season { Id = 1920 },
                new Season { Id = 1921 },
                new Season { Id = 1922 },
            };

            var fakeDbSet = seasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Seasons).Returns(fakeDbSet);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetSeason_WhenSeasonsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            int id = 1920;
            var season = new Season { Id = id };
            A.CallTo(() => fakeDbContext.Seasons.Find(An<int>.Ignored)).Returns(season);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetSeason(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(season);
        }

        [Fact]
        public void GetSeason_WhenSeasonsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = null;
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetSeason(1920);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetSeasonAsync_WhenSeasonsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            int id = 1920;
            var season = new Season { Id = id };
            _ = A.CallTo(() => fakeDbContext.Seasons.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Season?>(season));
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetSeasonAsync(id).Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(season);
        }

        [Fact]
        public void GetSeasonAsync_WhenSeasonsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = null;
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetSeasonAsync(1920).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var season = new Season { Id = 1920 };
            var result = repository.Add(season);

            // Assert
            A.CallTo(() => fakeDbContext.Add(season)).MustHaveHappenedOnceExactly();
            result.ShouldBe(season);
        }

        [Fact]
        public void AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var season = new Season { Id = 1920 };
            var result = repository.AddAsync(season).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(season)).MustHaveHappenedOnceExactly();
            result.ShouldBe(season);
        }

        [Fact]
        public async Task Update_WhenSeasonsIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var fakeDbContext = new ProFootballDbContext(options);

            int id = 1920;
            var season = new Season { Id = id, NumOfWeeksCompleted = 0 };
            fakeDbContext.Seasons.Add(season);
            await fakeDbContext.SaveChangesAsync();

            var repository = new SeasonRepository(fakeDbContext);

            // Act
            season.NumOfWeeksCompleted = 13;
            repository.Update(season);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            var updated = await fakeDbContext.Seasons.FirstOrDefaultAsync(s => s.Id == id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenSeasonsIsNotNullAndSelectedSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            var season = new Season { Id = 1920 };
            A.CallTo(() => fakeDbContext.Seasons.Find(An<int>.Ignored)).Returns(season);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.Delete(season.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Seasons.Remove(season)).MustHaveHappenedOnceExactly();
            result.ShouldBe(season);
        }

        [Fact]
        public void Delete_WhenSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = null;
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var season = new Season { Id = 1920 };
            var result = repository.Delete(season.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSeasonsIsNotNullAndSelectedSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();
            A.CallTo(() => fakeDbContext.Seasons.Find(An<int>.Ignored)).Returns(null);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var season = new Season { Id = 1920 };
            var result = repository.Delete(season.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Seasons.Remove(season)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenSeasonsIsNotNullAndSelectedSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            var season = new Season { Id = 1920 };
            _ = A.CallTo(() => fakeDbContext.Seasons.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Season?>(season));
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.DeleteAsync(season.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Seasons.Remove(season)).MustHaveHappenedOnceExactly();
            result.ShouldBe(season);
        }

        [Fact]
        public void DeleteAsync_WhenSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = null;
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var season = new Season { Id = 1920 };
            var result = repository.DeleteAsync(season.Id).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenSeasonsIsNotNullAndSelectedSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();
            _ = A.CallTo(() => fakeDbContext.Seasons.FindAsync(An<int>.Ignored)).Returns(null);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var season = new Season { Id = 1920 };
            var result = repository.DeleteAsync(season.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Seasons.Remove(season)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void SeasonExists_WhenSeasonsIsNotNullAndSelectedSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            var season = new Season { Id = 1920 };
            var fakeDbSet = new List<Season> { season }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Seasons).Returns(fakeDbSet);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.SeasonExists(season.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void SeasonExists_WhenSeasonsIsNotNullAndSelectedSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            var season = new Season { Id = 1920 };
            var fakeDbSet = new List<Season> { season }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Seasons).Returns(fakeDbSet);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.SeasonExists(1921);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void SeasonExistsAsync_WhenSeasonsIsNotNullAndSelectedSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            var season = new Season { Id = 1920 };
            var fakeDbSet = new List<Season> { season }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Seasons).Returns(fakeDbSet);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.SeasonExistsAsync(season.Id).Result;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void SeasonExistsAsync_WhenSeasonsIsNotNullAndSelectedSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Seasons = A.Fake<DbSet<Season>>();

            var season = new Season { Id = 1920 };
            var fakeDbSet = new List<Season> { season }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Seasons).Returns(fakeDbSet);
            var repository = new SeasonRepository(fakeDbContext);

            // Act
            var result = repository.SeasonExistsAsync(1921).Result;

            // Assert
            result.ShouldBeFalse();
        }
    }
}
