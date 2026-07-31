using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class TeamSeasonRepositoryTest
    {
        [Fact]
        public void GetTeamSeasons_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnTeamSeasons()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = testRepository.GetTeamSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(9);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
            }
        }

        [Fact]
        public void GetTeamSeasons_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = testRepository.GetTeamSeasons();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasons_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = testRepository.GetTeamSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnTeamSeasons()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = await testRepository.GetTeamSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(9);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
            }
        }

        [Fact]
        public async Task GetTeamSeasonsAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = await testRepository.GetTeamSeasonsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonsAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = await testRepository.GetTeamSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeasonsByTeam_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonsAreFound_ShouldReturnTeamSeasons()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var result = testRepository.GetTeamSeasonsByTeam(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
                item.TeamId.ShouldBe(teamId);
            }
        }

        [Fact]
        public void GetTeamSeasonsByTeam_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var result = testRepository.GetTeamSeasonsByTeam(teamId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonsByTeam_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var result = testRepository.GetTeamSeasonsByTeam(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeasonsByTeam_WhenTeamSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = -1;
            var result = testRepository.GetTeamSeasonsByTeam(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsByTeamAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonsAreFound_ShouldReturnTeamSeasons()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var result = await testRepository.GetTeamSeasonsByTeamAsync(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
                item.TeamId.ShouldBe(teamId);
            }
        }

        [Fact]
        public async Task GetTeamSeasonsByTeamAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var result = await testRepository.GetTeamSeasonsByTeamAsync(teamId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonsByTeamAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var result = await testRepository.GetTeamSeasonsByTeamAsync(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsByTeamAsync_WhenTeamSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = -1;
            var result = await testRepository.GetTeamSeasonsByTeamAsync(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonsAreFound_ShouldReturnTeamSeasons()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetTeamSeasonsBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
                item.SeasonYear.ShouldBe(seasonYear);
            }
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetTeamSeasonsBySeason(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var seasonYear = 1920;
            var result = testRepository.GetTeamSeasonsBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenTeamSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var seasonYear = -1;
            var result = testRepository.GetTeamSeasonsBySeason(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonsAreFound_ShouldReturnTeamSeasons()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
                item.SeasonYear.ShouldBe(seasonYear);
            }
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var seasonYear = 1920;
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenTeamSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var seasonYear = -1;
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeason_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonIsFound_ShouldReturnTeamSeason()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var id = 1;
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetTeamSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var id = 1;
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var id = 1;
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeason_WhenTeamSeasonIsNotFound_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var id = -1;
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonIsFound_ShouldReturnTeamSeason()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var id = 1;
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var id = 1;
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var id = 1;
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenTeamSeasonIsNotFound_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var id = -1;
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonByTeamAndSeason_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonIsFound_ShouldReturnTeamSeason()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var seasonYear = 1920;
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.TeamId.ShouldBe(teamId);
            result.SeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public void GetTeamSeasonByTeamAndSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var seasonYear = 1920;
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonByTeamAndSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var seasonYear = 1920;
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public void GetTeamSeasonByTeamAndSeason_WhenTeamSeasonIsNotFound_ShouldReturnNull(int teamId, int seasonYear)
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonIsFound_ShouldReturnTeamSeason()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var seasonYear = 1920;
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonYear);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.TeamId.ShouldBe(teamId);
            result.SeasonYear.ShouldBe(seasonYear);
        }

        [Fact]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var seasonYear = 1920;
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamId = 1;
            var seasonYear = 1920;
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenTeamSeasonIsNotFound_ShouldReturnNull(int teamId, int seasonYear)
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonYear);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddTeamSeason()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<TeamSeason>>());
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var teamSeason = new TeamSeason { Id = 1 };
            var result = testRepository.Add(teamSeason);

            // Assert
            A.CallTo(() => fakeDbContext.Add(teamSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<TeamSeason>>());
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act & Assert
            TeamSeason? teamSeason = null!;
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(teamSeason));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnTeamSeasonWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var teamSeason = new TeamSeason { Id = 1 };
            var result = testRepository.Add(teamSeason);

            // Assert
            A.CallTo(() => fakeDbContext.Add(teamSeason)).MustNotHaveHappened();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddTeamSeason()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<TeamSeason>>());
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var teamSeason = new TeamSeason { Id = 1 };
            var result = await testRepository.AddAsync(teamSeason);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<TeamSeason>>());
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act & Assert
            TeamSeason? teamSeason = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(teamSeason));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnTeamSeasonWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var teamSeason = new TeamSeason { Id = 1 };
            var result = await testRepository.AddAsync(teamSeason);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(teamSeason)).MustNotHaveHappened();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonYear = seasonYear, LeagueId = leagueId };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            testRepository.Update(teamSeason);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.TeamSeasons.FirstOrDefault(ts => ts.Id == teamSeason.Id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act & Assert
            TeamSeason? teamSeason = null!;
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(teamSeason));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnTeamSeason()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            TeamSeason? teamSeason = new();
            var updated = testRepository.Update(teamSeason);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(teamSeason);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnTeamSeason()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            TeamSeason? teamSeason = new();
            var updated = testRepository.Update(teamSeason);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(teamSeason);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedTeamSeasonIsNotNull_ShouldSucceed()
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonYear = seasonYear, LeagueId = leagueId };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeasonCountBeforeDelete = fakeDbContext.TeamSeasons.Count();

            // Act
            var result = testRepository.Delete(teamSeason.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.TeamSeasons.Count().ShouldBe(teamSeasonCountBeforeDelete - 1);
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamSeason = new TeamSeason { Id = 1 };
            var result = testRepository.Delete(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamSeason = new TeamSeason { Id = 1 };
            var result = testRepository.Delete(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedTeamSeasonIsNull_ShouldFailAndReturnNull()
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonYear = seasonYear, LeagueId = leagueId };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeasonTeamSeasonCountBeforeDelete = fakeDbContext.TeamSeasons.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.TeamSeasons.Count().ShouldBe(teamSeasonTeamSeasonCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedTeamSeasonIsNotNull_ShouldSucceed()
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonYear = seasonYear, LeagueId = leagueId };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeasonCountBeforeDelete = fakeDbContext.TeamSeasons.Count();

            // Act
            var result = await testRepository.DeleteAsync(teamSeason.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.TeamSeasons.Count().ShouldBe(teamSeasonCountBeforeDelete - 1);
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamSeason = new TeamSeason { Id = 1 };
            var result = await testRepository.DeleteAsync(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var teamSeason = new TeamSeason { Id = 1 };
            var result = await testRepository.DeleteAsync(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedTeamSeasonIsNull_ShouldFailAndReturnNull()
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonYear = seasonYear, LeagueId = leagueId };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeasonTeamSeasonCountBeforeDelete = fakeDbContext.TeamSeasons.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.TeamSeasons.Count().ShouldBe(teamSeasonTeamSeasonCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void TeamSeasonExists_WhenDbSetIsNotNullAndSelectedTeamSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = testRepository.TeamSeasonExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void TeamSeasonExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = testRepository.TeamSeasonExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void TeamSeasonExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = testRepository.TeamSeasonExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void TeamSeasonExists_WhenSelectedTeamSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = testRepository.TeamSeasonExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenDbSetIsNotNullAndSelectedTeamSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            List<TeamSeason> teamSeasons = null!;
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenSelectedTeamSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            List<TeamSeason> teamSeasons = GetTeamSeasons();
            TeamSeasonRepository testRepository = CreateTestRepository(teamSeasons);

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private static ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<TeamSeason> teamSeasons)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = teamSeasons;
            return fakeDbContext;
        }

        private static TeamSeasonRepository CreateTestRepository(List<TeamSeason>? teamSeasons)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            DbSet<TeamSeason> fakeDbSet = teamSeasons is not null ? teamSeasons.BuildMockDbSet() : null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            return new TeamSeasonRepository(fakeDbContext);
        }

        private static List<TeamSeason> GetTeamSeasons()
        {
            var counter = 1;

            var teamSeasons = new List<TeamSeason>();
            for (int t = 1; t < 4; t++)
            {
                for (int y = 1920; y < 1923; y++)
                {
                    teamSeasons.Add(
                        new TeamSeason
                        {
                            Id = counter++,
                            TeamId = t,
                            SeasonYear = y
                        }
                    );
                }
            }

            return teamSeasons;
        }
    }
}
