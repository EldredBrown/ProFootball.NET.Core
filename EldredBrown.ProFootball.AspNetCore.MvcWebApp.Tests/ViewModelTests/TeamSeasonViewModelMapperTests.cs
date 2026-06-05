using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ViewModelTests
{
    public class TeamSeasonViewModelMapperTests
    {
        [Fact]
        public void MapTeamSeasonToViewModel_ShouldSucceed()
        {
            // Arrange
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var testMapper = new TeamSeasonViewModelMapper(fakeTeamRepository, fakeSeasonRepository,
                fakeLeagueRepository);

            var teamSeason = new EldredBrown.ProFootball.Net.Data.Models.TeamSeason
            {
                Id = 1,
                TeamId = 1,
                SeasonId = 1920,
                LeagueId = 1
            };

            // Act
            var result = testMapper.MapTeamSeasonToViewModel(teamSeason);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeasonViewModel>();
            result.TeamSeason.ShouldBe(teamSeason);
        }

        public static TheoryData<Team, int> TeamCases => new()
        {
            { new Team { Id = 1, Name = "Team" }, 1 },
            { null!, -1 },
        };

        [Theory]
        [MemberData(nameof(TeamCases))]
        public async Task MapViewModelToTeamSeason_ShouldSetTeamSeasonTeamIdToTeamIdOrMinusOne(
            Team team, int expectedTeamId
        )
        {
            // Arrange
            var teamName = "Team";
            var teamSeasonViewModel = new TeamSeasonViewModel
            {
                TeamName = teamName
            };

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => fakeTeamRepository.GetTeamByNameAsync(A<string>.Ignored)).Returns(team);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var testMapper = new TeamSeasonViewModelMapper(fakeTeamRepository, fakeSeasonRepository,
                fakeLeagueRepository);

            // Act
            var result = await testMapper.MapViewModelToTeamSeason(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamRepository.GetTeamByNameAsync(teamName)).MustHaveHappenedOnceExactly();
            teamSeasonViewModel.TeamSeason.TeamId.ShouldBe(expectedTeamId);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.ShouldBe(teamSeasonViewModel.TeamSeason);
        }

        public static TheoryData<Season, int> SeasonCases => new()
        {
            { new Season { Id = 1920 }, 1920 },
            { null!, -1 },
        };

        [Theory]
        [MemberData(nameof(SeasonCases))]
        public async Task MapViewModelToTeamSeason_ShouldSetTeamSeasonSeasonIdToSeasonIdOrMinusOne(
            Season season, int expectedSeasonId
        )
        {
            // Arrange
            var seasonYear = 1920;
            var teamSeasonViewModel = new TeamSeasonViewModel
            {
                SeasonYear = seasonYear
            };

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var testMapper = new TeamSeasonViewModelMapper(fakeTeamRepository, fakeSeasonRepository,
                fakeLeagueRepository);

            // Act
            var result = await testMapper.MapViewModelToTeamSeason(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappenedOnceExactly();
            teamSeasonViewModel.TeamSeason.SeasonId.ShouldBe(expectedSeasonId);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.ShouldBe(teamSeasonViewModel.TeamSeason);
        }

        public static TheoryData<League, int> LeagueCases => new()
        {
            { new League { Id = 1, ShortName = "NFL" }, 1 },
            { null!, -1 },
        };

        [Theory]
        [MemberData(nameof(LeagueCases))]
        public async Task MapViewModelToTeamSeason_ShouldSetTeamSeasonLeagueIdToLeagueIdOrMinusOne(
            League league, int expectedLeagueId
        )
        {
            // Arrange
            var leagueName = "NFL";
            var teamSeasonViewModel = new TeamSeasonViewModel
            {
                LeagueName = leagueName
            };

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(A<string>.Ignored)).Returns(league);

            var testMapper = new TeamSeasonViewModelMapper(fakeTeamRepository, fakeSeasonRepository,
                fakeLeagueRepository);

            // Act
            var result = await testMapper.MapViewModelToTeamSeason(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(leagueName)).MustHaveHappenedOnceExactly();
            teamSeasonViewModel.TeamSeason.LeagueId.ShouldBe(expectedLeagueId);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<TeamSeason>();
            result.ShouldBe(teamSeasonViewModel.TeamSeason);
        }
    }
}
