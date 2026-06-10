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
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

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
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.GetTeamSeasons();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasons_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.GetTeamSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnTeamSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

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
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.GetTeamSeasonsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonsAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.GetTeamSeasonsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeasonsByTeam_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonsAreFound_ShouldReturnTeamSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var teamId = 1;

            // Act
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
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var teamId = 1;

            // Act
            var result = testRepository.GetTeamSeasonsByTeam(teamId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonsByTeam_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var teamId = 1;

            // Act
            var result = testRepository.GetTeamSeasonsByTeam(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeasonsByTeam_WhenTeamSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var teamId = -1;

            // Act
            var result = testRepository.GetTeamSeasonsByTeam(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsByTeamAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonsAreFound_ShouldReturnTeamSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var teamId = 1;

            // Act
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
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var teamId = 1;

            // Act
            var result = await testRepository.GetTeamSeasonsByTeamAsync(teamId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonsByTeamAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var teamId = 1;

            // Act
            var result = await testRepository.GetTeamSeasonsByTeamAsync(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsByTeamAsync_WhenTeamSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var teamId = -1;

            // Act
            var result = await testRepository.GetTeamSeasonsByTeamAsync(teamId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonsAreFound_ShouldReturnTeamSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetTeamSeasonsBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
                item.SeasonId.ShouldBe(seasonId);
            }
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetTeamSeasonsBySeason(seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = testRepository.GetTeamSeasonsBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenTeamSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = -1;

            // Act
            var result = testRepository.GetTeamSeasonsBySeason(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonsAreFound_ShouldReturnTeamSeasons()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
                item.SeasonId.ShouldBe(seasonId);
            }
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var seasonId = 1920;

            // Act
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenTeamSeasonsAreNotFound_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var seasonId = -1;

            // Act
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeamSeason_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonIsFound_ShouldReturnTeamSeason()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetTeamSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeason_WhenTeamSeasonIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonIsFound_ShouldReturnTeamSeason()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenTeamSeasonIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonByTeamAndSeason_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonIsFound_ShouldReturnTeamSeason()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var teamId = 1;
            var seasonId = 1920;

            // Act
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.TeamId.ShouldBe(teamId);
            result.SeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public void GetTeamSeasonByTeamAndSeason_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var teamId = 1;
            var seasonId = 1920;

            // Act
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonByTeamAndSeason_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var teamId = 1;
            var seasonId = 1920;

            // Act
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        [InlineData(-1, -1)]
        public void GetTeamSeasonByTeamAndSeason_WhenTeamSeasonIsNotFound_ShouldReturnNull(int teamId, int seasonId)
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamSeasonIsFound_ShouldReturnTeamSeason()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var teamId = 1;
            var seasonId = 1920;

            // Act
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonId);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.TeamId.ShouldBe(teamId);
            result.SeasonId.ShouldBe(seasonId);
        }

        [Fact]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var teamId = 1;
            var seasonId = 1920;

            // Act
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var teamId = 1;
            var seasonId = 1920;

            // Act
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(-1, 1)]
        [InlineData(-1, -1)]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenTeamSeasonIsNotFound_ShouldReturnNull(int teamId, int seasonId)
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddTeamSeason()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<TeamSeason>>());
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason { Id = 1 };

            // Act
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

            TeamSeason? teamSeason = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(teamSeason));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnTeamSeasonWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason { Id = 1 };

            // Act
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

            var teamSeason = new TeamSeason { Id = 1 };

            // Act
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

            TeamSeason? teamSeason = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(teamSeason));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnTeamSeasonWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(teamSeason);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(teamSeason)).MustNotHaveHappened();
            result.ShouldBe(teamSeason);
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonId = seasonId, LeagueId = leagueId };
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
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            TeamSeason? teamSeason = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(teamSeason));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnTeamSeason()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            TeamSeason? teamSeason = new TeamSeason { };

            // Act
            var updated = testRepository.Update(teamSeason);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(teamSeason);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnTeamSeason()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            TeamSeason? teamSeason = new TeamSeason { };

            // Act
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonId = seasonId, LeagueId = leagueId };
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
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var teamSeason = new TeamSeason { Id = 1 };

            // Act
            var result = testRepository.Delete(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var teamSeason = new TeamSeason { Id = 1 };

            // Act
            var result = testRepository.Delete(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedTeamSeasonIsNull_ShouldFailAndReturnNull()
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonId = seasonId, LeagueId = leagueId };
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonId = seasonId, LeagueId = leagueId };
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
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var teamSeason = new TeamSeason { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var teamSeason = new TeamSeason { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedTeamSeasonIsNull_ShouldFailAndReturnNull()
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

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason { Id = 1, TeamId = teamId, SeasonId = seasonId, LeagueId = leagueId };
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
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.TeamSeasonExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void TeamSeasonExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.TeamSeasonExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void TeamSeasonExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.TeamSeasonExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void TeamSeasonExists_WhenSelectedTeamSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.TeamSeasonExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenDbSetIsNotNullAndSelectedTeamSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenSelectedTeamSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<TeamSeason> teamSeasons)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = teamSeasons;
            return fakeDbContext;
        }

        private ITeamSeasonRepository CreateTestRepositoryWithEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            var teamSeasons = new List<TeamSeason>();
            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);
            return testRepository;
        }

        private ITeamSeasonRepository CreateTestRepositoryWithNotEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            var teamSeasons = new List<TeamSeason>
            {
                new() { Id = 1, TeamId = 1, SeasonId = 1920, LeagueId = 1 },
                new() { Id = 2, TeamId = 1, SeasonId = 1921, LeagueId = 1 },
                new() { Id = 3, TeamId = 1, SeasonId = 1922, LeagueId = 1 },
                new() { Id = 4, TeamId = 2, SeasonId = 1920, LeagueId = 1 },
                new() { Id = 5, TeamId = 2, SeasonId = 1921, LeagueId = 1 },
                new() { Id = 6, TeamId = 2, SeasonId = 1922, LeagueId = 1 },
                new() { Id = 7, TeamId = 3, SeasonId = 1920, LeagueId = 1 },
                new() { Id = 8, TeamId = 3, SeasonId = 1921, LeagueId = 1 },
                new() { Id = 9, TeamId = 3, SeasonId = 1922, LeagueId = 1 },
            };
            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);
            return testRepository;
        }

        private ITeamSeasonRepository CreateTestRepositoryWithNullDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);
            return testRepository;
        }
    }
}
