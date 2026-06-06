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
        [Fact]
        public void GetLeagues_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnLeagues()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetLeagues();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<League>();
            }
        }

        [Fact]
        public void GetLeagues_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.GetLeagues();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagues_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.GetLeagues();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeaguesAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnLeagues()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetLeaguesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<League>();
            }
        }

        [Fact]
        public async Task GetLeaguesAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.GetLeaguesAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeaguesAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.GetLeaguesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeague_WhenDbSetIsNeitherNullNorEmptyAndLeagueIsFound_ShouldReturnLeague()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetLeague(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetLeague_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetLeague(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeague_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetLeague(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeague_WhenLeagueIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = testRepository.GetLeague(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueIsFound_ShouldReturnLeague()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetLeagueAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetLeagueAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetLeagueAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetLeagueAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueAsync_WhenLeagueIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = await testRepository.GetLeagueAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueByShortName_WhenDbSetIsNeitherNullNorEmptyAndLeagueIsFound_ShouldReturnLeague()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var shortName = "L1";

            // Act
            var result = testRepository.GetLeagueByShortName(shortName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.ShortName.ShouldBe(shortName);
        }

        [Fact]
        public void GetLeagueByShortName_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var shortName = "L1";

            // Act
            var result = testRepository.GetLeagueByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueByShortName_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var shortName = "L1";

            // Act
            var result = testRepository.GetLeagueByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueByShortName_WhenLeagueIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var shortName = "L99";

            // Act
            var result = testRepository.GetLeagueByShortName(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueByShortNameAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueIsFound_ShouldReturnLeague()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var shortName = "L1";

            // Act
            var result = await testRepository.GetLeagueByShortNameAsync(shortName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.ShortName.ShouldBe(shortName);
        }

        [Fact]
        public async Task GetLeagueByShortNameAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var shortName = "L1";

            // Act
            var result = await testRepository.GetLeagueByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueByShortNameAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var shortName = "L1";

            // Act
            var result = await testRepository.GetLeagueByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueByShortNameAsync_WhenLeagueIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var shortName = "L99";

            // Act
            var result = await testRepository.GetLeagueByShortNameAsync(shortName);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddLeague()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<League>>());
            var testRepository = new LeagueRepository(fakeDbContext);

            var league = new League { Id = 1 };

            // Act
            var result = testRepository.Add(league);

            // Assert
            A.CallTo(() => fakeDbContext.Add(league)).MustHaveHappenedOnceExactly();
            result.ShouldBe(league);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<League>>());
            var testRepository = new LeagueRepository(fakeDbContext);

            League? league = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(league));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnLeagueWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new LeagueRepository(fakeDbContext);

            var league = new League { Id = 1 };

            // Act
            var result = testRepository.Add(league);

            // Assert
            A.CallTo(() => fakeDbContext.Add(league)).MustNotHaveHappened();
            result.ShouldBe(league);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddLeague()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<League>>());
            var testRepository = new LeagueRepository(fakeDbContext);

            var league = new League { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(league);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(league)).MustHaveHappenedOnceExactly();
            result.ShouldBe(league);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<League>>());
            var testRepository = new LeagueRepository(fakeDbContext);

            League? league = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(league));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnLeagueWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new LeagueRepository(fakeDbContext);

            var league = new League { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(league);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(league)).MustNotHaveHappened();
            result.ShouldBe(league);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var league = new League { Id = 1, LongName = "League 1", ShortName = "L1", FirstSeasonId = firstSeasonId };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            // Act
            testRepository.Update(league);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Leagues.FirstOrDefault(l => l.Id == league.Id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            League? league = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(league));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnLeague()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            League? league = new League { };

            // Act
            var updated = testRepository.Update(league);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(league);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnLeague()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            League? league = new League { };

            // Act
            var updated = testRepository.Update(league);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(league);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedLeagueIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var league = new League { Id = 1, LongName = "League 1", ShortName = "L1", FirstSeasonId = firstSeasonId };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            var leagueCountBeforeDelete = fakeDbContext.Leagues.Count();

            // Act
            var result = testRepository.Delete(league.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Leagues.Count().ShouldBe(leagueCountBeforeDelete - 1);
            result.ShouldBe(league);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var league = new League { Id = 1 };

            // Act
            var result = testRepository.Delete(league.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var league = new League { Id = 1 };

            // Act
            var result = testRepository.Delete(league.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedLeagueIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var league = new League { Id = 1, LongName = "League 1", ShortName = "L1", FirstSeasonId = 1920 };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            var leagueLeagueCountBeforeDelete = fakeDbContext.Leagues.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Leagues.Count().ShouldBe(leagueLeagueCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedLeagueIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var firstSeasonId = 1920;
            var firstSeason = new Season { Id = firstSeasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var league = new League { Id = 1, LongName = "League 1", ShortName = "L1", FirstSeasonId = firstSeasonId };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            var leagueCountBeforeDelete = fakeDbContext.Leagues.Count();

            // Act
            var result = await testRepository.DeleteAsync(league.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Leagues.Count().ShouldBe(leagueCountBeforeDelete - 1);
            result.ShouldBe(league);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var league = new League { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(league.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var league = new League { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(league.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedLeagueIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var league = new League { Id = 1, LongName = "League 1", ShortName = "L1", FirstSeasonId = 1920 };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueRepository(fakeDbContext);

            var leagueLeagueCountBeforeDelete = fakeDbContext.Leagues.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Leagues.Count().ShouldBe(leagueLeagueCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void LeagueExists_WhenDbSetIsNotNullAndSelectedLeagueExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.LeagueExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void LeagueExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.LeagueExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void LeagueExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.LeagueExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void LeagueExists_WhenSelectedLeagueDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.LeagueExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueExistsAsync_WhenDbSetIsNotNullAndSelectedLeagueExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.LeagueExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task LeagueExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.LeagueExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.LeagueExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueExistsAsync_WhenSelectedLeagueDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.LeagueExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<League> leagues)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Leagues = leagues;
            return fakeDbContext;
        }

        private ILeagueRepository CreateTestRepositoryWithEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Leagues = A.Fake<DbSet<League>>();
            var leagues = new List<League>();
            var fakeDbSet = leagues.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);

            var testRepository = new LeagueRepository(fakeDbContext);
            return testRepository;
        }

        private ILeagueRepository CreateTestRepositoryWithNotEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Leagues = A.Fake<DbSet<League>>();
            var leagues = new List<League>
            {
                new League { Id = 1, LongName = "League 1", ShortName = "L1", FirstSeasonId = 1920 },
                new League { Id = 2, LongName = "League 2", ShortName = "L2", FirstSeasonId = 1920 },
                new League { Id = 3, LongName = "League 3", ShortName = "L3", FirstSeasonId = 1920 },
            };
            var fakeDbSet = leagues.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);

            var testRepository = new LeagueRepository(fakeDbContext);
            return testRepository;
        }

        private ILeagueRepository CreateTestRepositoryWithNullDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Leagues = A.Fake<DbSet<League>>();
            DbSet<League> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.Leagues).Returns(fakeDbSet);

            var testRepository = new LeagueRepository(fakeDbContext);
            return testRepository;
        }
    }
}
