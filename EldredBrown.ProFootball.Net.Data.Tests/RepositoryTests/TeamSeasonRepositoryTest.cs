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
        public TeamSeasonRepositoryTest() { }

        [Fact]
        public void GetTeamSeasons_WhenDbSetIsNotNull_ShouldReturnTeamSeasons()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
                new TeamSeason { Id = 4, TeamId = 1, SeasonId = 1921 },
                new TeamSeason { Id = 5, TeamId = 2, SeasonId = 1921 },
                new TeamSeason { Id = 6, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 7, TeamId = 1, SeasonId = 1922 },
                new TeamSeason { Id = 8, TeamId = 2, SeasonId = 1922 },
                new TeamSeason { Id = 9, TeamId = 3, SeasonId = 1922 },
            };

            DbSet<TeamSeason> fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

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
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.GetTeamSeasons();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonsAsync_WhenDbSetIsNotNull_ShouldReturnTeamSeasons()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
                new TeamSeason { Id = 4, TeamId = 1, SeasonId = 1921 },
                new TeamSeason { Id = 5, TeamId = 2, SeasonId = 1921 },
                new TeamSeason { Id = 6, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 7, TeamId = 1, SeasonId = 1922 },
                new TeamSeason { Id = 8, TeamId = 2, SeasonId = 1922 },
                new TeamSeason { Id = 9, TeamId = 3, SeasonId = 1922 },
            };

            DbSet<TeamSeason> fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

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
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.GetTeamSeasonsAsync();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonsBySeason_WhenDbSetIsNotNull_ShouldReturnTeamSeasonsForMatchingSeason()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
                new TeamSeason { Id = 4, TeamId = 1, SeasonId = 1921 },
                new TeamSeason { Id = 5, TeamId = 2, SeasonId = 1921 },
                new TeamSeason { Id = 6, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 7, TeamId = 1, SeasonId = 1922 },
                new TeamSeason { Id = 8, TeamId = 2, SeasonId = 1922 },
                new TeamSeason { Id = 9, TeamId = 3, SeasonId = 1922 },
            };

            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var seasonId = 1921;

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
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var seasonId = 1921;

            // Act
            var result = testRepository.GetTeamSeasonsBySeason(seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonsBySeasonAsync_WhenDbSetIsNotNull_ShouldReturnTeamSeasonsForMatchingSeason()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
                new TeamSeason { Id = 4, TeamId = 1, SeasonId = 1921 },
                new TeamSeason { Id = 5, TeamId = 2, SeasonId = 1921 },
                new TeamSeason { Id = 6, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 7, TeamId = 1, SeasonId = 1922 },
                new TeamSeason { Id = 8, TeamId = 2, SeasonId = 1922 },
                new TeamSeason { Id = 9, TeamId = 3, SeasonId = 1922 },
            };

            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var seasonId = 1921;

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
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var seasonId = 1921;

            // Act
            var result = await testRepository.GetTeamSeasonsBySeasonAsync(seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeason_WhenDbSetIsNotNull_ShouldReturnTeamSeasonWithMatchingId()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
                new TeamSeason { Id = 4, TeamId = 1, SeasonId = 1921 },
                new TeamSeason { Id = 5, TeamId = 2, SeasonId = 1921 },
                new TeamSeason { Id = 6, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 7, TeamId = 1, SeasonId = 1922 },
                new TeamSeason { Id = 8, TeamId = 2, SeasonId = 1922 },
                new TeamSeason { Id = 9, TeamId = 3, SeasonId = 1922 },
            };

            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var id = 3;

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
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var id = 3;

            // Act
            var result = testRepository.GetTeamSeason(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonAsync_WhenDbSetIsNotNull_ShouldReturnTeamSeasonWithMatchingId()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
                new TeamSeason { Id = 4, TeamId = 1, SeasonId = 1921 },
                new TeamSeason { Id = 5, TeamId = 2, SeasonId = 1921 },
                new TeamSeason { Id = 6, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 7, TeamId = 1, SeasonId = 1922 },
                new TeamSeason { Id = 8, TeamId = 2, SeasonId = 1922 },
                new TeamSeason { Id = 9, TeamId = 3, SeasonId = 1922 },
            };

            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var id = 3;

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
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var id = 3;

            // Act
            var result = await testRepository.GetTeamSeasonAsync(id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void GetTeamSeasonByTeamAndSeason_WhenDbSetIsNotNull_ShouldReturnTeamSeasonWithMatchingTeamAndSeasonIds()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
                new TeamSeason { Id = 4, TeamId = 1, SeasonId = 1921 },
                new TeamSeason { Id = 5, TeamId = 2, SeasonId = 1921 },
                new TeamSeason { Id = 6, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 7, TeamId = 1, SeasonId = 1922 },
                new TeamSeason { Id = 8, TeamId = 2, SeasonId = 1922 },
                new TeamSeason { Id = 9, TeamId = 3, SeasonId = 1922 },
            };

            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamId = 2;
            var seasonId = 1921;

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
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamId = 2;
            var seasonId = 1921;

            // Act
            var result = testRepository.GetTeamSeasonByTeamAndSeason(teamId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetTeamSeasonByTeamAndSeasonAsync_WhenDbSetIsNotNull_ShouldReturnTeamSeasonWithMatchingTeamAndSeasonIds()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
                new TeamSeason { Id = 4, TeamId = 1, SeasonId = 1921 },
                new TeamSeason { Id = 5, TeamId = 2, SeasonId = 1921 },
                new TeamSeason { Id = 6, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 7, TeamId = 1, SeasonId = 1922 },
                new TeamSeason { Id = 8, TeamId = 2, SeasonId = 1922 },
                new TeamSeason { Id = 9, TeamId = 3, SeasonId = 1922 },
            };

            var fakeDbSet = teamSeasons.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamId = 2;
            var seasonId = 1921;

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
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamId = 2;
            var seasonId = 1921;

            // Act
            var result = await testRepository.GetTeamSeasonByTeamAndSeasonAsync(teamId, seasonId);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Add_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920
            };

            // Act
            var result = testRepository.Add(teamSeason);

            // Assert
            A.CallTo(() => fakeDbContext.Add(teamSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task AddAsync_ShouldSucceed()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920
            };

            // Act
            var result = await testRepository.AddAsync(teamSeason);

            // Assert
            A.CallTo(() => fakeDbContext.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task Update_WhenArgIsNotNullAndDbSetIsNotNull_ShouldSucceed_WithInMemoryDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920,
                Games = 1
            };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            teamSeason.Games = 100;

            // Act
            testRepository.Update(teamSeason);
            fakeDbContext.SaveChanges();

            // Assert
            var updated = fakeDbContext.TeamSeasons.FirstOrDefault(ls => ls.Id == 1);
            updated.ShouldNotBeNull();
        }

        [Fact]
        public async Task Update_WhenArgIsNull_ShouldReturnNull()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();
            var testRepository = new TeamSeasonRepository(fakeDbContext);

            TeamSeason? teamSeason = null;

            // Act
            var updated = testRepository.Update(teamSeason);

            // Assert
            updated.ShouldBeNull();
        }

        [Fact]
        public async Task Update_WhenDbSetIsNull_ShouldReturnTeamSeason()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            DbSet<TeamSeason> fakeDbSet = null!;
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            TeamSeason? teamSeason = new TeamSeason { };

            // Act
            var updated = testRepository.Update(teamSeason);

            // Assert
            updated.ShouldNotBeNull();
            updated.ShouldBe(teamSeason);
        }

        [Fact]
        public void Delete_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var conferenceId = 1;
            var conference = new Conference { Id = conferenceId, LongName = "Conference", ShortName = "C" };
            fakeDbContext.Conferences.Add(conference);

            var divisionId = 1;
            var division = new Division { Id = divisionId, Name = "Division" };
            fakeDbContext.Divisions.Add(division);

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);

            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920,
                LeagueId = 1,
                ConferenceId = 1,
                DivisionId = 1,
            };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var teamSeasonCountBeforeDelete = fakeDbContext.TeamSeasons.Count();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(teamSeason.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.TeamSeasons.Count().ShouldBe(teamSeasonCountBeforeDelete - 1);
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public void Delete_WhenTeamSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Leagues = null!;

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920,
                Games = 1,
            };

            var result = testRepository.Delete(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void Delete_WhenSelectedTeamSeasonIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var conferenceId = 1;
            var conference = new Conference { Id = conferenceId, LongName = "Conference", ShortName = "C" };
            fakeDbContext.Conferences.Add(conference);

            var divisionId = 1;
            var division = new Division { Id = divisionId, Name = "Division" };
            fakeDbContext.Divisions.Add(division);

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);

            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920,
                Games = 1,
            };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var leagueSeasonCountBeforeDelete = fakeDbContext.TeamSeasons.Count();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.Delete(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.TeamSeasons.Count().ShouldBe(leagueSeasonCountBeforeDelete);
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonIsNotNull_ShouldSucceed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var conferenceId = 1;
            var conference = new Conference { Id = conferenceId, LongName = "Conference", ShortName = "C" };
            fakeDbContext.Conferences.Add(conference);

            var divisionId = 1;
            var division = new Division { Id = divisionId, Name = "Division" };
            fakeDbContext.Divisions.Add(division);

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);

            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920,
                LeagueId = 1,
                ConferenceId = 1,
                DivisionId = 1,
            };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var teamSeasonCountBeforeDelete = fakeDbContext.TeamSeasons.Count();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(teamSeason.Id);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.TeamSeasons.Count().ShouldBe(teamSeasonCountBeforeDelete - 1);
            result.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task DeleteAsync_WhenTeamSeasonsIsNull_ShouldFailAndReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);
            fakeDbContext.Leagues = null!;

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920,
                Games = 1,
            };

            var result = await testRepository.DeleteAsync(teamSeason.Id);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonIsNull_ShouldFailAndReturnNull()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var fakeDbContext = new ProFootballDbContext(options);

            var seasonId = 1920;
            var season = new Season { Id = seasonId };
            fakeDbContext.Seasons.Add(season);

            var leagueId = 1;
            var league = new League { Id = leagueId, LongName = "League", ShortName = "L" };
            fakeDbContext.Leagues.Add(league);

            var conferenceId = 1;
            var conference = new Conference { Id = conferenceId, LongName = "Conference", ShortName = "C" };
            fakeDbContext.Conferences.Add(conference);

            var divisionId = 1;
            var division = new Division { Id = divisionId, Name = "Division" };
            fakeDbContext.Divisions.Add(division);

            var teamId = 1;
            var team = new Team { Id = teamId, Name = "Team" };
            fakeDbContext.Teams.Add(team);

            fakeDbContext.SaveChanges();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920,
                Games = 1,
            };
            fakeDbContext.TeamSeasons.Add(teamSeason);
            fakeDbContext.SaveChanges();

            var leagueSeasonCountBeforeDelete = fakeDbContext.TeamSeasons.Count();

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.DeleteAsync(2);
            fakeDbContext.SaveChanges();

            // Assert
            fakeDbContext.TeamSeasons.Count().ShouldBe(leagueSeasonCountBeforeDelete);
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
                TeamId = 1,
                SeasonId = 1920
            };

            var fakeDbSet = new List<TeamSeason> { teamSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.TeamSeasonExists(teamSeason.Id);

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
                TeamId = 1,
                SeasonId = 1920
            };

            var fakeDbSet = new List<TeamSeason> { teamSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = testRepository.TeamSeasonExists(2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonExists_ShouldReturnTrue()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920
            };

            var fakeDbSet = new List<TeamSeason> { teamSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(teamSeason.Id);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task TeamSeasonExistsAsync_WhenTeamSeasonsIsNotNullAndSelectedTeamSeasonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var fakeDbContext = A.Fake<ProFootballDbContext>();
            fakeDbContext.TeamSeasons = A.Fake<DbSet<TeamSeason>>();

            var teamSeason = new TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920
            };

            var fakeDbSet = new List<TeamSeason> { teamSeason }.BuildMockDbSet();
            A.CallTo(() => fakeDbContext.TeamSeasons).Returns(fakeDbSet);

            var testRepository = new TeamSeasonRepository(fakeDbContext);

            // Act
            var result = await testRepository.TeamSeasonExistsAsync(2);

            // Assert
            result.ShouldBeFalse();
        }
    }
}
