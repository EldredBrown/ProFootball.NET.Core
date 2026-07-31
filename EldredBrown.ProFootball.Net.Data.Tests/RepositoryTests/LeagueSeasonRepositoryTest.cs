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
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = testRepository.GetLeagueSeasons();

            // Assert
            result.ShouldNotBeNull();

            var leagueSeasonCount = leagueSeasons.Count;
            result.Count().ShouldBe(leagueSeasonCount);

            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
            }
        }

        [Fact]
        public void GetLeagueSeasons_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = testRepository.GetLeagueSeasons();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasons_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = testRepository.GetLeagueSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnLeagueSeasons()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = await testRepository.GetLeagueSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();

            var leagueSeasonCount = leagueSeasons.Count;
            result.Count().ShouldBe(leagueSeasonCount);

            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
            }
        }

        [Fact]
        public async Task GetLeagueSeasonsAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = await testRepository.GetLeagueSeasonsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonsAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = await testRepository.GetLeagueSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeasonsByLeague_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonsAreFound_ShouldReturnLeagueSeasons()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

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
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var result = testRepository.GetLeagueSeasonsByLeague(leagueId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonsByLeague_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var result = testRepository.GetLeagueSeasonsByLeague(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeasonsByLeague_WhenLeagueSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = -1;
            var result = testRepository.GetLeagueSeasonsByLeague(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsByLeagueAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonsAreFound_ShouldReturnLeagueSeasons()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
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
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var result = await testRepository.GetLeagueSeasonsByLeagueAsync(leagueId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonsByLeagueAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var result = await testRepository.GetLeagueSeasonsByLeagueAsync(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsByLeagueAsync_WhenLeagueSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = -1;
            var result = await testRepository.GetLeagueSeasonsByLeagueAsync(leagueId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeasonsBySeason_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonsAreFound_ShouldReturnLeagueSeasons()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetLeagueSeasonsBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
                item.SeasonYear.ShouldBe(seasonYear);
            }
        }

        [Fact]
        public void GetLeagueSeasonsBySeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetLeagueSeasonsBySeason(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonsBySeason_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetLeagueSeasonsBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeasonsBySeason_WhenLeagueSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var seasonYear = -1;
            var result = testRepository.GetLeagueSeasonsBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsBySeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonsAreFound_ShouldReturnLeagueSeasons()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetLeagueSeasonsBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<LeagueSeason>();
                item.SeasonYear.ShouldBe(seasonYear);
            }
        }

        [Fact]
        public async Task GetLeagueSeasonsBySeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetLeagueSeasonsBySeasonAsync(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonsBySeasonAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetLeagueSeasonsBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetLeagueSeasonsBySeasonAsync_WhenLeagueSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var seasonYear = -1;
            var result = await testRepository.GetLeagueSeasonsBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetLeagueSeason_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonIsFound_ShouldReturnLeagueSeason()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var id = 1;
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetLeagueSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var id = 1;
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var id = 1;
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeason_WhenLeagueSeasonIsNotFound_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var id = -1;
            var result = testRepository.GetLeagueSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonIsFound_ShouldReturnLeagueSeason()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var id = 1;
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var id = 1;
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var id = 1;
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonAsync_WhenLeagueSeasonIsNotFound_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var id = -1;
            var result = await testRepository.GetLeagueSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonByLeagueAndSeason_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonIsFound_ShouldReturnLeagueSeason()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var seasonYear = 1920;
            var result = testRepository.GetLeagueSeasonByLeagueAndSeason(leagueId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.LeagueId.ShouldBe(leagueId);
            result.SeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public void GetLeagueSeasonByLeagueAndSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var seasonYear = 1920;
            var result = testRepository.GetLeagueSeasonByLeagueAndSeason(leagueId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetLeagueSeasonByLeagueAndSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var seasonYear = 1920;
            var result = testRepository.GetLeagueSeasonByLeagueAndSeason(leagueId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public void GetLeagueSeasonByLeagueAndSeason_WhenLeagueSeasonIsNotFound_ShouldReturnNull(int leagueId, int seasonYear)
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = testRepository.GetLeagueSeasonByLeagueAndSeason(leagueId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonByLeagueAndSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndLeagueSeasonIsFound_ShouldReturnLeagueSeason()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var seasonYear = 1920;
            var result = await testRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.LeagueId.ShouldBe(leagueId);
            result.SeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetLeagueSeasonByLeagueAndSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var seasonYear = 1920;
            var result = await testRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLeagueSeasonByLeagueAndSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueId = 1;
            var seasonYear = 1920;
            var result = await testRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public async Task GetLeagueSeasonByLeagueAndSeasonAsync_WhenLeagueSeasonIsNotFound_ShouldReturnNull(int leagueId, int seasonYear)
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = await testRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddLeagueSeason()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<LeagueSeason>>());
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var leagueSeason = new LeagueSeason { Id = 1 };
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

            // Act & Assert
            LeagueSeason? leagueSeason = null!;
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(leagueSeason));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnLeagueSeasonWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var leagueSeason = new LeagueSeason { Id = 1 };
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

            // Act
            var leagueSeason = new LeagueSeason { Id = 1 };
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

            // Act & Assert
            LeagueSeason? leagueSeason = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(leagueSeason));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnLeagueSeasonWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new LeagueSeasonRepository(fakeDbContext);

            // Act
            var leagueSeason = new LeagueSeason { Id = 1 };
            var result = await testRepository.AddAsync(leagueSeason);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(leagueSeason)).MustNotHaveHappened();
            result.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new Association
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonYear = seasonYear
            };
            fakeDbContext.Associations.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason {
                Id = 1, LeagueId = leagueId, SeasonYear = seasonYear, NumOfWeeksScheduled = 8
            };
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
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act & Assert
            LeagueSeason? leagueSeason = null!;
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(leagueSeason));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnLeagueSeason()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            LeagueSeason? leagueSeason = new();
            var updated = testRepository.Update(leagueSeason);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnLeagueSeason()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            LeagueSeason? leagueSeason = new();
            var updated = testRepository.Update(leagueSeason);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(leagueSeason);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedLeagueSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new Association
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonYear = seasonYear
            };
            fakeDbContext.Associations.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonYear = seasonYear };
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
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueSeason = new LeagueSeason { Id = 1 };
            var result = testRepository.Delete(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueSeason = new LeagueSeason { Id = 1 };
            var result = testRepository.Delete(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedLeagueSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new Association
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonYear = seasonYear
            };
            fakeDbContext.Associations.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonYear = seasonYear };
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
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new Association
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonYear = seasonYear
            };
            fakeDbContext.Associations.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonYear = seasonYear };
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
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueSeason = new LeagueSeason { Id = 1 };
            var result = await testRepository.DeleteAsync(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var leagueSeason = new LeagueSeason { Id = 1 };
            var result = await testRepository.DeleteAsync(leagueSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedLeagueSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var seasonYear = 1920;
            var firstSeason = new Season { Year = seasonYear };
            fakeDbContext.Seasons.Add(firstSeason);
            fakeDbContext.SaveChanges();

            var leagueId = 1;
            var league = new Association
            {
                Id = leagueId,
                LongName = "League",
                ShortName = "L",
                FirstSeasonYear = seasonYear
            };
            fakeDbContext.Associations.Add(league);
            fakeDbContext.SaveChanges();

            var leagueSeason = new LeagueSeason { Id = 1, LeagueId = leagueId, SeasonYear = seasonYear };
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
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = testRepository.LeagueSeasonExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void LeagueSeasonExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = testRepository.LeagueSeasonExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void LeagueSeasonExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = testRepository.LeagueSeasonExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void LeagueSeasonExists_WhenSelectedLeagueSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = testRepository.LeagueSeasonExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenDbSetIsNotNullAndSelectedLeagueSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = null!;
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task LeagueSeasonExistsAsync_WhenSelectedLeagueSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = GetLeagueSeasons();
            LeagueSeasonRepository testRepository = CreateTestRepository(leagueSeasons);

            // Act
            var result = await testRepository.LeagueSeasonExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private static ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<LeagueSeason> leagueSeasons)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = leagueSeasons;
            return fakeDbContext;
        }

        private static LeagueSeasonRepository CreateTestRepository(List<LeagueSeason>? leagueSeasons = null)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.LeagueSeasons = A.Fake<DbSet<LeagueSeason>>();
            DbSet<LeagueSeason> fakeDbSet = leagueSeasons is not null ? leagueSeasons.BuildMockDbSet() : null!;
            A.CallTo(() => fakeDbContext.LeagueSeasons).Returns(fakeDbSet);

            return new LeagueSeasonRepository(fakeDbContext);
        }

        private static List<LeagueSeason> GetLeagueSeasons()
        {
            var counter = 1;

            var leagueSeasons = new List<LeagueSeason>();
            for (int l = 1; l < 4; l++)
            {
                for (int y = 1920; y < 1923; y++)
                {
                    leagueSeasons.Add(
                        new LeagueSeason
                        {
                            Id = counter++,
                            LeagueId = l,
                            SeasonYear = y,
                            NumOfWeeksScheduled = 16,
                            NumOfWeeksCompleted = 0
                        }
                    );
                }
            }

            return leagueSeasons;
        }
    }
}
