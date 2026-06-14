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
        [Fact]
        public void GetLeagueSeasons_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnLeagueSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetLeagueSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(9);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
            }
        }

        [Fact]
        public void GetLeagueSeasons_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.GetLeagueSeasons();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasons_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.GetLeagueSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnLeagueSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetLeagueSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(9);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
            }
        }

        [Fact]
        public async Task GetLeagueSeasonsAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.GetLeagueSeasonsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonsAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.GetLeagueSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeasonsByLeague_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonsAreFound_ShouldReturnLeagueSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var leagueId = 1;

            // Act
            var result = testRepository.GetLeagueSeasonsByLeague(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
                item.LeagueId.ShouldBe(leagueId);
            }
        }

        [Fact]
        public void GetLeagueSeasonsByLeague_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var leagueId = 1;

            // Act
            var result = testRepository.GetLeagueSeasonsByLeague(leagueId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonsByLeague_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var leagueId = 1;

            // Act
            var result = testRepository.GetLeagueSeasonsByLeague(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeasonsByLeague_WhenLeagueSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var leagueId = -1;

            // Act
            var result = testRepository.GetLeagueSeasonsByLeague(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsByLeagueAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonsAreFound_ShouldReturnLeagueSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var leagueId = 1;

            // Act
            var result = await testRepository.GetLeagueSeasonsByLeagueAsync(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
                item.LeagueId.ShouldBe(leagueId);
            }
        }

        [Fact]
        public async Task GetLeagueSeasonsByLeagueAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var leagueId = 1;

            // Act
            var result = await testRepository.GetLeagueSeasonsByLeagueAsync(leagueId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonsByLeagueAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var leagueId = 1;

            // Act
            var result = await testRepository.GetLeagueSeasonsByLeagueAsync(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsByLeagueAsync_WhenLeagueSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var leagueId = -1;

            // Act
            var result = await testRepository.GetLeagueSeasonsByLeagueAsync(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeasonsBySeason_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonsAreFound_ShouldReturnLeagueSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetLeagueSeasonsBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
                item.SeasonId.ShouldBe(seasonId);
            }
        }

        [Fact]
        public void GetLeagueSeasonsBySeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetLeagueSeasonsBySeason(seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonsBySeason_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetLeagueSeasonsBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeasonsBySeason_WhenLeagueSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = -1;

            // Act
            var result = testRepository.GetLeagueSeasonsBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsBySeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonsAreFound_ShouldReturnLeagueSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetLeagueSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
                item.SeasonId.ShouldBe(seasonId);
            }
        }

        [Fact]
        public async Task GetLeagueSeasonsBySeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetLeagueSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonsBySeasonAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetLeagueSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsBySeasonAsync_WhenLeagueSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = -1;

            // Act
            var result = await testRepository.GetLeagueSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeason_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonIsFound_ShouldReturnLeagueSeason()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetLeagueSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeason_WhenLeagueSeasonIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonIsFound_ShouldReturnLeagueSeason()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenLeagueSeasonIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonByLeagueAndSeason_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonIsFound_ShouldReturnLeagueSeason()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var leagueId = 1;
            var seasonId = 1920;

            // Act
            var result = testRepository.GetLeagueSeasonByLeagueAndSeason(leagueId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.LeagueId.ShouldBe(leagueId);
            result.SeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public void GetLeagueSeasonByLeagueAndSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var leagueId = 1;
            var seasonId = 1920;

            // Act
            var result = testRepository.GetLeagueSeasonByLeagueAndSeason(leagueId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonByLeagueAndSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var leagueId = 1;
            var seasonId = 1920;

            // Act
            var result = testRepository.GetLeagueSeasonByLeagueAndSeason(leagueId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        [InlineData(-1, -1)]
        public void GetLeagueSeasonByLeagueAndSeason_WhenLeagueSeasonIsNotFound_ShouldReturnNull(int leagueId, int seasonId)
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetLeagueSeasonByLeagueAndSeason(leagueId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonByLeagueAndSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonIsFound_ShouldReturnLeagueSeason()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var leagueId = 1;
            var seasonId = 1920;

            // Act
            var result = await testRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.LeagueId.ShouldBe(leagueId);
            result.SeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public async Task GetLeagueSeasonByLeagueAndSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var leagueId = 1;
            var seasonId = 1920;

            // Act
            var result = await testRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonByLeagueAndSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var leagueId = 1;
            var seasonId = 1920;

            // Act
            var result = await testRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        [InlineData(-1, -1)]
        public async Task GetLeagueSeasonByLeagueAndSeasonAsync_WhenLeagueSeasonIsNotFound_ShouldReturnNull(int leagueId, int seasonId)
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddLeagueSeason()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<LeagueSeason>>());
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason { Id = 1 };

            // Act
            var result = testRepository.Add(leagueSeason);

            // Assert
            A.CallTo(() => fakeDbContext.Add(leagueSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<LeagueSeason>>());
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            LeagueSeason? leagueSeason = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(leagueSeason));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnLeagueSeasonWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason { Id = 1 };

            // Act
            var result = testRepository.Add(leagueSeason);

            // Assert
            A.CallTo(() => fakeDbContext.Add(leagueSeason)).MustNotHaveHappened();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddLeagueSeason()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<LeagueSeason>>());
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(leagueSeason);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<LeagueSeason>>());
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            LeagueSeason? leagueSeason = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(leagueSeason));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnLeagueSeasonWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeason = new LeagueSeason { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(leagueSeason);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(leagueSeason)).MustNotHaveHappened();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonId = seasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonId = seasonId };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            testRepository.Update(leagueSeason);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.LeagueSeasons.FirstOrDefault(ts => ts.Id == leagueSeason.Id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            LeagueSeason? leagueSeason = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(leagueSeason));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnLeagueSeason()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            LeagueSeason? leagueSeason = new() { };

            // Act
            var updated = testRepository.Update(leagueSeason);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnLeagueSeason()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            LeagueSeason? leagueSeason = new() { };

            // Act
            var updated = testRepository.Update(leagueSeason);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedLeagueSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonId = seasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonId = seasonId };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeasonCountBeforeDelete = fakeDbContext.LeagueSeasons.Count();

            // Act
            var result = testRepository.Delete(leagueSeason.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.LeagueSeasons.Count().ShouldBe(leagueSeasonCountBeforeDelete - 1);
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var leagueSeason = new LeagueSeason { Id = 1 };

            // Act
            var result = testRepository.Delete(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var leagueSeason = new LeagueSeason { Id = 1 };

            // Act
            var result = testRepository.Delete(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedLeagueSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonId = seasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonId = seasonId };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeasonLeagueSeasonCountBeforeDelete = fakeDbContext.LeagueSeasons.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.LeagueSeasons.Count().ShouldBe(leagueSeasonLeagueSeasonCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedLeagueSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonId = seasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonId = seasonId };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeasonCountBeforeDelete = fakeDbContext.LeagueSeasons.Count();

            // Act
            var result = await testRepository.DeleteAsync(leagueSeason.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.LeagueSeasons.Count().ShouldBe(leagueSeasonCountBeforeDelete - 1);
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var leagueSeason = new LeagueSeason { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var leagueSeason = new LeagueSeason { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedLeagueSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonId = 1920;
            var firstSeason = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new League
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonId = seasonId
            };
            fakeDbContext.Leagues.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonId = seasonId };
            fakeDbContext.LeagueSeasons.Add(leagueSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            var leagueSeasonLeagueSeasonCountBeforeDelete = fakeDbContext.LeagueSeasons.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.LeagueSeasons.Count().ShouldBe(leagueSeasonLeagueSeasonCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void LeagueSeasonExists_WhenDbSetIsNotNullAndSelectedLeagueSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.LeagueSeasonExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void LeagueSeasonExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.LeagueSeasonExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void LeagueSeasonExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.LeagueSeasonExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void LeagueSeasonExists_WhenSelectedLeagueSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.LeagueSeasonExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenDbSetIsNotNullAndSelectedLeagueSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenSelectedLeagueSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<LeagueSeason> leagueSeasons)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = leagueSeasons;
            return fakeDbContext;
        }

        private ILeagueSeasonRepository CreateTestRepositoryWithEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            var leagueSeasons = new List<LeagueSeason>();
            var fakeDbSet = leagueSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);

            var testRepository = new LeagueSeasonRepository(fakeDbContext);
            return testRepository;
        }

        private ILeagueSeasonRepository CreateTestRepositoryWithNotEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1, LeagueId = 1, SeasonId = 1920 },
                new() { Id = 2, LeagueId = 1, SeasonId = 1921 },
                new() { Id = 3, LeagueId = 1, SeasonId = 1922 },
                new() { Id = 4, LeagueId = 2, SeasonId = 1920 },
                new() { Id = 5, LeagueId = 2, SeasonId = 1921 },
                new() { Id = 6, LeagueId = 2, SeasonId = 1922 },
                new() { Id = 7, LeagueId = 3, SeasonId = 1920 },
                new() { Id = 8, LeagueId = 3, SeasonId = 1921 },
                new() { Id = 9, LeagueId = 3, SeasonId = 1922 },
            };
            var fakeDbSet = leagueSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);

            var testRepository = new LeagueSeasonRepository(fakeDbContext);
            return testRepository;
        }

        private ILeagueSeasonRepository CreateTestRepositoryWithNullDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            DbSet<LeagueSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);

            var testRepository = new LeagueSeasonRepository(fakeDbContext);
            return testRepository;
        }
    }
}
