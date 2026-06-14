using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using MockQueryable.FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    public class TeamRepositoryTest
    {
        [Fact]
        public void GetTeams_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnTeams()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.GetTeams();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Team>();
            }
        }

        [Fact]
        public void GetTeams_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.GetTeams();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeams_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.GetTeams();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public async Task GetTeamsAsync_WhenDbSetIsNeitherNullNorEmpty_ShouldReturnTeams()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.GetTeamsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Team>();
            }
        }

        [Fact]
        public async Task GetTeamsAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.GetTeamsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamsAsync_WhenDbSetIsEmpty_ShouldReturnEmptyCollection()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.GetTeamsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetTeam_WhenDbSetIsNeitherNullNorEmptyAndTeamIsFound_ShouldReturnTeam()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetTeam(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Team>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public void GetTeam_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetTeam(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeam_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = testRepository.GetTeam(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeam_WhenTeamIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = testRepository.GetTeam(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamIsFound_ShouldReturnTeam()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetTeamAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Team>();
            result.Id.ShouldBe(id);
        }

        [Fact]
        public async Task GetTeamAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetTeamAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var id = 1;

            // Act
            var result = await testRepository.GetTeamAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamAsync_WhenTeamIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var id = -1;

            // Act
            var result = await testRepository.GetTeamAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamByName_WhenDbSetIsNeitherNullNorEmptyAndTeamIsFound_ShouldReturnTeam()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var name = "Team 1";

            // Act
            var result = testRepository.GetTeamByName(name);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Team>();
            result.Name.ShouldBe(name);
        }

        [Fact]
        public void GetTeamByName_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var name = "Team 1";

            // Act
            var result = testRepository.GetTeamByName(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamByName_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var name = "Team 1";

            // Act
            var result = testRepository.GetTeamByName(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamByName_WhenTeamIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var name = "Team 99";

            // Act
            var result = testRepository.GetTeamByName(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamByNameAsync_WhenDbSetIsNeitherNullNorEmptyAndTeamIsFound_ShouldReturnTeam()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var name = "Team 1";

            // Act
            var result = await testRepository.GetTeamByNameAsync(name);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Team>();
            result.Name.ShouldBe(name);
        }

        [Fact]
        public async Task GetTeamByNameAsync_WhenDbSetIsNull_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var name = "Team 1";

            // Act
            var result = await testRepository.GetTeamByNameAsync(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamByNameAsync_WhenDbSetIsEmpty_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var name = "Team 1";

            // Act
            var result = await testRepository.GetTeamByNameAsync(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamByNameAsync_WhenTeamIsNotFound_ShouldReturnNull()
        {
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            var name = "Team 99";

            // Act
            var result = await testRepository.GetTeamByNameAsync(name);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddTeam()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Team>>());
            var testRepository = new TeamRepository(fakeDbContext);

            var team = new Team { Id = 1 };

            // Act
            var result = testRepository.Add(team);

            // Assert
            A.CallTo(() => fakeDbContext.Add(team)).MustHaveHappenedOnceExactly();
            result.ShouldBe(team);
        }

        [Fact]
        public void Add_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Team>>());
            var testRepository = new TeamRepository(fakeDbContext);

            Team? team = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Add(team));
        }

        [Fact]
        public void Add_WhenDbSetIsNull_ShouldReturnTeamWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new TeamRepository(fakeDbContext);

            var team = new Team { Id = 1 };

            // Act
            var result = testRepository.Add(team);

            // Assert
            A.CallTo(() => fakeDbContext.Add(team)).MustNotHaveHappened();
            result.ShouldBe(team);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNotNullAndDbSetIsNotNull_ShouldAddTeam()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Team>>());
            var testRepository = new TeamRepository(fakeDbContext);

            var team = new Team { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(team);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(team)).MustHaveHappenedOnceExactly();
            result.ShouldBe(team);
        }

        [Fact]
        public async Task AddAsync_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(A.Fake<DbSet<Team>>());
            var testRepository = new TeamRepository(fakeDbContext);

            Team? team = null!;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await testRepository.AddAsync(team));
        }

        [Fact]
        public async Task AddAsync_WhenDbSetIsNull_ShouldReturnTeamWithoutAddingIt()
        {
            // Arrange
            var fakeDbContext = CreateFakeDbContextForAddOperations(null!);
            var testRepository = new TeamRepository(fakeDbContext);

            var team = new Team { Id = 1 };

            // Act
            var result = await testRepository.AddAsync(team);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(team)).MustNotHaveHappened();
            result.ShouldBe(team);
        }

        [Fact]
        public void Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var team = new Team { Id = 1, Name = "Team 1" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            testRepository.Update(team);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Teams.FirstOrDefault(t => t.Id == team.Id);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenArgIsNull_ShouldThrowException()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            Team? team = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => testRepository.Update(team));
        }

        [Fact]
        public void Update_WhenDbSetIsNull_ShouldReturnTeam()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            Team? team = new();

            // Act
            var updated = testRepository.Update(team);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(team);
        }

        [Fact]
        public void Update_WhenDbSetIsEmpty_ShouldReturnTeam()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            Team? team = new();

            // Act
            var updated = testRepository.Update(team);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(team);
        }

        [Fact]
        public void Delete_WhenDbSetIsNotNullAndSelectedTeamIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var team = new Team { Id = 1, Name = "Team 1" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamRepository(fakeDbContext);

            var teamCountBeforeDelete = fakeDbContext.Teams.Count();

            // Act
            var result = testRepository.Delete(team.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Teams.Count().ShouldBe(teamCountBeforeDelete - 1);
            result.ShouldBe(team);
        }

        [Fact]
        public void Delete_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var team = new Team { Id = 1 };

            // Act
            var result = testRepository.Delete(team.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var team = new Team { Id = 1 };

            // Act
            var result = testRepository.Delete(team.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedTeamIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var team = new Team { Id = 1, Name = "Team 1" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamRepository(fakeDbContext);

            var teamTeamCountBeforeDelete = fakeDbContext.Teams.Count();

            // Act
            var result = testRepository.Delete(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Teams.Count().ShouldBe(teamTeamCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNotNullAndSelectedTeamIsNotNull_ShouldSucceed()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var team = new Team { Id = 1, Name = "Team 1" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamRepository(fakeDbContext);

            var teamCountBeforeDelete = fakeDbContext.Teams.Count();

            // Act
            var result = await testRepository.DeleteAsync(team.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Teams.Count().ShouldBe(teamCountBeforeDelete - 1);
            result.ShouldBe(team);
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            var team = new Team { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(team.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenDbSetIsEmpty_ShouldFailAndReturnNull()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            var team = new Team { Id = 1 };

            // Act
            var result = await testRepository.DeleteAsync(team.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenSelectedTeamIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            using var fakeDbContext = TestDbContext.CreateFakeDbContextWithInMemoryDb();

            var team = new Team { Id = 1, Name = "Team 1" };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamRepository(fakeDbContext);

            var teamTeamCountBeforeDelete = fakeDbContext.Teams.Count();

            // Act
            var result = await testRepository.DeleteAsync(-1);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Teams.Count().ShouldBe(teamTeamCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public void TeamExists_WhenDbSetIsNotNullAndSelectedTeamExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.TeamExists(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void TeamExists_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = testRepository.TeamExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void TeamExists_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = testRepository.TeamExists(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void TeamExists_WhenSelectedTeamDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = testRepository.TeamExists(-1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamExistsAsync_WhenDbSetIsNotNullAndSelectedTeamExists_ShouldReturnTrue()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.TeamExistsAsync(1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task TeamExistsAsync_WhenDbSetIsNull_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNullDbSet();

            // Act
            var result = await testRepository.TeamExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamExistsAsync_WhenDbSetIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithEmptyDbSet();

            // Act
            var result = await testRepository.TeamExistsAsync(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamExistsAsync_WhenSelectedTeamDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var testRepository = CreateTestRepositoryWithNotEmptyDbSet();

            // Act
            var result = await testRepository.TeamExistsAsync(-1);

            // Assert
            result.ShouldBeFalse();
        }

        private ProFootballDbContext CreateFakeDbContextForAddOperations(DbSet<Team> teams)
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = teams;
            return fakeDbContext;
        }

        private ITeamRepository CreateTestRepositoryWithEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Teams = A.Fake<DbSet<Team>>();
            var teams = new List<Team>();
            var fakeDbSet = teams.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);

            var testRepository = new TeamRepository(fakeDbContext);
            return testRepository;
        }

        private ITeamRepository CreateTestRepositoryWithNotEmptyDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Teams = A.Fake<DbSet<Team>>();
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Team 1" },
                new() { Id = 2, Name = "Team 2" },
                new() { Id = 3, Name = "Team 3" },
            };
            var fakeDbSet = teams.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);

            var testRepository = new TeamRepository(fakeDbContext);
            return testRepository;
        }

        private ITeamRepository CreateTestRepositoryWithNullDbSet()
        {
            var fakeDbContext = A.Fake<ProFootballDbContext>();

            fakeDbContext.Teams = A.Fake<DbSet<Team>>();
            DbSet<Team> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);

            var testRepository = new TeamRepository(fakeDbContext);
            return testRepository;
        }
    }
}
