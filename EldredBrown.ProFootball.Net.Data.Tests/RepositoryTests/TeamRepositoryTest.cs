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
        public TeamRepositoryTest() { }

        [Fact]
        public void GetTeams_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var teams = new List<Team>
            {
                new Team{ Id = 1, Name = "Team 1" },
                new Team{ Id = 2, Name = "Team 2" },
                new Team{ Id = 3, Name = "Team 3" },
            };

            var fakeDbSet = teams.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.GetTeams();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<Team>();
            }
        }

        [Fact]
        public void GetTeamsAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var teams = new List<Team>
            {
                new Team{ Id = 1, Name = "Team 1" },
                new Team{ Id = 2, Name = "Team 2" },
                new Team{ Id = 3, Name = "Team 3" },
            };

            var fakeDbSet = teams.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.GetTeams();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetTeam_WhenTeamsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            int id = 1;
            var team = new Team
            {
                Id = id,
                Name = "Team"
            };

            A.CallTo(() => fakeDbContext.Teams.Find(An<int>.Ignored)).Returns(team);
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.GetTeam(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(team);
        }

        [Fact]
        public void GetTeam_WhenTeamsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = null;
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.GetTeam(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamAsync_WhenTeamsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            int id = 1;
            var team = new Team
            {
                Id = id,
                Name = "Team"
            };

            _ = A.CallTo(() => fakeDbContext.Teams.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Team?>(team));
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.GetTeamAsync(id).Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(team);
        }

        [Fact]
        public void GetTeamAsync_WhenTeamsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = null;
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.GetTeamAsync(1).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();
            var repository = new TeamRepository(fakeDbContext);

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            // Act
            var result = repository.Add(team);

            // Assert
            A.CallTo(() => fakeDbContext.Add(team)).MustHaveHappenedOnceExactly();
            result.ShouldBe(team);
        }

        [Fact]
        public void AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();
            var repository = new TeamRepository(fakeDbContext);

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            // Act
            var result = repository.AddAsync(team).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(team)).MustHaveHappenedOnceExactly();
            result.ShouldBe(team);
        }

        [Fact(Skip = "Unique constraints in Team entity do not permit changes to values.")]
        public async Task Update_WhenTeamsIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var fakeDbContext = new ProFootballDbContext(options);

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            fakeDbContext.Teams.Add(team);
            await fakeDbContext.SaveChangesAsync();

            var repository = new TeamRepository(fakeDbContext);

            // Act
            team.Name = "New Team";
            repository.Update(team);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            var updated = await fakeDbContext.Teams.FirstOrDefaultAsync(s => s.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenTeamsIsNotNullAndSelectedTeamIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            A.CallTo(() => fakeDbContext.Teams.Find(An<int>.Ignored)).Returns(team);
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.Delete(team.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Teams.Remove(team)).MustHaveHappenedOnceExactly();
            result.ShouldBe(team);
        }

        [Fact]
        public void Delete_WhenTeamsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = null;
            var repository = new TeamRepository(fakeDbContext);

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            // Act
            var result = repository.Delete(team.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenTeamsIsNotNullAndSelectedTeamIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();
            A.CallTo(() => fakeDbContext.Teams.Find(An<int>.Ignored)).Returns(null);
            var repository = new TeamRepository(fakeDbContext);

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            // Act
            var result = repository.Delete(team.Id);

            // Assert
            A.CallTo(() => fakeDbContext.Teams.Remove(team)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenTeamsIsNotNullAndSelectedTeamIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            _ = A.CallTo(() => fakeDbContext.Teams.FindAsync(An<int>.Ignored)).Returns(new ValueTask<Team?>(team));
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.DeleteAsync(team.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Teams.Remove(team)).MustHaveHappenedOnceExactly();
            result.ShouldBe(team);
        }

        [Fact]
        public void DeleteAsync_WhenTeamsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = null;
            var repository = new TeamRepository(fakeDbContext);

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            // Act
            var result = repository.DeleteAsync(team.Id).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenTeamsIsNotNullAndSelectedTeamIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();
            _ = A.CallTo(() => fakeDbContext.Teams.FindAsync(An<int>.Ignored)).Returns(null);
            var repository = new TeamRepository(fakeDbContext);

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            // Act
            var result = repository.DeleteAsync(team.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.Teams.Remove(team)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void TeamExists_WhenTeamsIsNotNullAndSelectedTeamExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            var fakeDbSet = new List<Team> { team }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.TeamExists(team.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void TeamExists_WhenTeamsIsNotNullAndSelectedTeamDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            var fakeDbSet = new List<Team> { team }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.TeamExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void TeamExistsAsync_WhenTeamsIsNotNullAndSelectedTeamExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            var fakeDbSet = new List<Team> { team }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.TeamExistsAsync(team.Id).Result;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void TeamExistsAsync_WhenTeamsIsNotNullAndSelectedTeamDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var team = new Team
            {
                Id = 1,
                Name = "Team"
            };

            var fakeDbSet = new List<Team> { team }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);
            var repository = new TeamRepository(fakeDbContext);

            // Act
            var result = repository.TeamExistsAsync(2).Result;

            // Assert
            result.ShouldBeFalse();
        }
    }
}
