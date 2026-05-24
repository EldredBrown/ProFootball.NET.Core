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
        public void GetTeamSeasons_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamName = "Team", SeasonYear = 1920 },
                new TeamSeason { Id = 2, TeamName = "Team", SeasonYear = 1921 },
                new TeamSeason { Id = 3, TeamName = "Team", SeasonYear = 1922 },
            };

            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetTeamSeasons();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            foreach (var item in result)
            {
                item.ShouldBeOfType<TeamSeason>();
            }
        }

        [Fact]
        public void GetTeamSeasonsAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamName = "Team", SeasonYear = 1920 },
                new TeamSeason { Id = 2, TeamName = "Team", SeasonYear = 1921 },
                new TeamSeason { Id = 3, TeamName = "Team", SeasonYear = 1922 },
            };

            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetTeamSeasonsAsync().Result;

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void GetTeamSeason_WhenTeamSeasonsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            int id = 1;
            var teamSeason = new TeamSeason
            {
                Id = id,
                TeamName = "Team",
                SeasonYear = 1920
            };

            A.CallTo(() => fakeDbContext.TeamSeasons.Find(An<int>.Ignored)).Returns(teamSeason);
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetTeamSeason(id);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void GetTeamSeason_WhenTeamSeasonsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = null;
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetTeamSeason(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonAsync_WhenTeamSeasonsIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            int id = 1;
            var teamSeason = new TeamSeason
            {
                Id = id,
                TeamName = "Team",
                SeasonYear = 1920
            };

            _ = A.CallTo(() => fakeDbContext.TeamSeasons.FindAsync(An<int>.Ignored))
                .Returns(new ValueTask<TeamSeason?>(teamSeason));
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetTeamSeasonAsync(id).Result;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void GetTeamSeasonAsync_WhenTeamSeasonsIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = null;
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.GetTeamSeasonAsync(1).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            var repository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            // Act
            var result = repository.Add(teamSeason);

            // Assert
            A.CallTo(() => fakeDbContext.Add(teamSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            var repository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            // Act
            var result = repository.AddAsync(teamSeason).Result;

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task Update_WhenTeamSeasonsIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var fakeDbContext = new ProFootballDbContext(options);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920,
                LeagueName = "League",
                Games = 1
            };

            fakeDbContext.TeamSeasons.Add(teamSeason);
            await fakeDbContext.SaveChangesAsync();

            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            teamSeason.Games = 2;
            repository.Update(teamSeason);
            await fakeDbContext.SaveChangesAsync();

            // Assert
            var updated = await fakeDbContext.TeamSeasons.FirstOrDefaultAsync(ts => ts.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            A.CallTo(() => fakeDbContext.TeamSeasons.Find(An<int>.Ignored)).Returns(teamSeason);
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.Delete(teamSeason.Id);

            // Assert
            A.CallTo(() => fakeDbContext.TeamSeasons.Remove(teamSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void Delete_WhenTeamSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = null;
            var repository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            // Act
            var result = repository.Delete(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            A.CallTo(() => fakeDbContext.TeamSeasons.Find(An<int>.Ignored)).Returns(null);
            var repository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            // Act
            var result = repository.Delete(teamSeason.Id);

            // Assert
            A.CallTo(() => fakeDbContext.TeamSeasons.Remove(teamSeason)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            _ = A.CallTo(() => fakeDbContext.TeamSeasons.FindAsync(An<int>.Ignored)).Returns(
                new ValueTask<TeamSeason?>(teamSeason));
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.DeleteAsync(teamSeason.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.TeamSeasons.Remove(teamSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void DeleteAsync_WhenTeamSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = null;
            var repository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            // Act
            var result = repository.DeleteAsync(teamSeason.Id).Result;

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void DeleteAsync_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            _ = A.CallTo(() => fakeDbContext.TeamSeasons.FindAsync(An<int>.Ignored)).Returns(null);
            var repository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            // Act
            var result = repository.DeleteAsync(teamSeason.Id).Result;

            // Assert
            A.CallTo(() => fakeDbContext.TeamSeasons.Remove(teamSeason)).MustNotHaveHappened();
            result.ShouldBeNull();
        }

        [Fact]
        public void TeamSeasonExists_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            var fakeDbSet = new List<TeamSeason> { teamSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.TeamSeasonExists(teamSeason.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void TeamSeasonExists_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            var fakeDbSet = new List<TeamSeason> { teamSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.TeamSeasonExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void TeamSeasonExistsAsync_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            var fakeDbSet = new List<TeamSeason> { teamSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.TeamSeasonExistsAsync(teamSeason.Id).Result;

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void TeamSeasonExistsAsync_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamName = "Team",
                SeasonYear = 1920
            };

            var fakeDbSet = new List<TeamSeason> { teamSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);
            var repository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = repository.TeamSeasonExistsAsync(2).Result;

            // Assert
            result.ShouldBeFalse();
        }
    }
}
