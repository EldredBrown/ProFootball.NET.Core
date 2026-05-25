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
                new Team { Id = 1, Name = "Team 1" },
                new Team { Id = 2, Name = "Team 2" },
                new Team { Id = 3, Name = "Team 3" },
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
                new Team { Id = 1, Name = "Team 1" },
                new Team { Id = 2, Name = "Team 2" },
                new Team { Id = 3, Name = "Team 3" },
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
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var team = new Team
            {
                Id = id,
                Name = "Team 1"
            };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = testRepository.GetTeam(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(team);
        }

        [Fact]
        public void GetTeam_WhenTeamsIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Teams = null;
            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = testRepository.GetTeam(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamAsync_WhenTeamsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var team = new Team
            {
                Id = id,
                Name = "Team 1"
            };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetTeamAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(team);
        }

        [Fact]
        public async Task GetTeamAsync_WhenTeamsIsNull_ShouldReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            fakeDbContext.Teams = null;
            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetTeamAsync(1);

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

            // Act
            var team = new Team
            {
                Id = 1,
                Name = "Team 1"
            };
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

            // Act
            var team = new Team
            {
                Id = 1,
                Name = "Team 1"
            };
            var result = repository.AddAsync(team).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(team)).MustHaveHappenedOnceExactly();
            result.ShouldBe(team);
        }

        [Fact]
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
                Name = "Team 1"
            };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            team.Name = "Team 2";
            testRepository.Update(team);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.Teams.Find(1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenTeamsIsNotNullAndSelectedTeamIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var team = new Team
            {
                Id = id,
                Name = "Team 1"
            };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamCountBeforeDelete = fakeDbContext.Teams.Count();

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(team.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Teams.Count().ShouldBe(teamCountBeforeDelete - 1);
            result.ShouldBe(team);
        }

        [Fact]
        public void Delete_WhenTeamsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Teams = null;

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var team = new Team
            {
                Id = 1,
                Name = "Team 1"
            };
            var result = testRepository.Delete(team.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenTeamsIsNotNullAndSelectedTeamIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };

            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            int id = 1;
            var team = new Team
            {
                Id = id,
                Name = "Team 1"
            };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamCountBeforeDelete = fakeDbContext.Teams.Count();

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.Teams.Count().ShouldBe(teamCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenTeamsIsNotNullAndSelectedTeamIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };
            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            var parentLeague = new League { Id = 1, ShortName = "L1", LongName = "League 1" };
            fakeDbContext.Leagues.Add(parentLeague);
            fakeDbContext.SaveChanges();

            int id = 1;
            var team = new Team
            {
                Id = id,
                Name = "Team 1"
            };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamCountBeforeDelete = fakeDbContext.Teams.Count();

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(team.Id);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Teams.Count().ShouldBe(teamCountBeforeDelete - 1);
            result.ShouldBe(team);
        }

        [Fact]
        public async Task DeleteAsync_WhenTeamsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Teams = null;

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var team = new Team
            {
                Id = 1,
                Name = "Team 1"
            };
            var result = await testRepository.DeleteAsync(team.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenTeamsIsNotNullAndSelectedTeamIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var firstSeason = new Season { Id = 1920 };
            var lastSeason = new Season { Id = 1921 };

            fakeDbContext.Seasons.AddRange(firstSeason, lastSeason);
            fakeDbContext.SaveChanges();

            int id = 1;
            var team = new Team
            {
                Id = id,
                Name = "Team 1"
            };
            fakeDbContext.Teams.Add(team);
            fakeDbContext.SaveChanges();

            var teamCountBeforeDelete = fakeDbContext.Teams.Count();

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(2);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            fakeDbContext.Teams.Count().ShouldBe(teamCountBeforeDelete);
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
                Name = "Team 1"
            };
            var fakeDbSet = new List<Team> { team }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = testRepository.TeamExists(team.Id);

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
                Name = "Team 1"
            };
            var fakeDbSet = new List<Team> { team }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = testRepository.TeamExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamExistsAsync_WhenTeamsIsNotNullAndSelectedTeamExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var team = new Team
            {
                Id = 1,
                Name = "Team 1"
            };
            var fakeDbSet = new List<Team> { team }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = await testRepository.TeamExistsAsync(team.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task TeamExistsAsync_WhenTeamsIsNotNullAndSelectedTeamDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.Teams = A.Fake<DbSet<Team>>();

            var team = new Team
            {
                Id = 1,
                Name = "Team 1"
            };
            var fakeDbSet = new List<Team> { team }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.Teams).Returns(fakeDbSet);

            var testRepository = new TeamRepository(fakeDbContext);

            // Act
            var result = await testRepository.TeamExistsAsync(2);

            // Assert
            result.ShouldBeFalse();
        }
    }
}
