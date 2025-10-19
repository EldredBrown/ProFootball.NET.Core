using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons;
using EldredBrown.ProFootball.WpfApp;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Tests.ViewModelTests
{
    public class TeamSeasonsControlViewModelTest
    {
        [Fact]
        public void TeamsSetter_WhenValueIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var testObject = new TeamSeasonsControlViewModel(teamSeasonRepository, teamSeasonScheduleRepository);

            // Act
            Func<ReadOnlyCollection<TeamSeason>> func = () => testObject.Teams = null!;

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testObject.GetType()}.{nameof(testObject.Teams)}");
        }

        [Fact]
        public void TeamsSetter_WhenValueDoesNotEqualTeams_ShouldAssignValueToTeams()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var testObject = new TeamSeasonsControlViewModel(teamSeasonRepository, teamSeasonScheduleRepository);

            var teams = new ReadOnlyCollection<TeamSeason>(new List<TeamSeason>());

            // Act
            Func<ReadOnlyCollection<TeamSeason>> func = () => testObject.Teams = teams;

            // Assert
            func.ShouldNotThrow();
            testObject.Teams.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.Teams.ShouldBe(teams);
        }

        [Fact]
        public void SelectedTeamSetter_WhenValueDoesNotEqualSelectedTeam_ShouldAssignValueToSelectedTeam()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var teamSeasonOpponentProfiles = new List<TeamSeasonOpponentProfile>();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfile(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonOpponentProfiles);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotals(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleAverages(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleAverages);

            var testObject = new TeamSeasonsControlViewModel(teamSeasonRepository, teamSeasonScheduleRepository)
            {
                SelectedTeam = null
            };

            var teamSeason = new TeamSeason();

            // Act
            testObject.SelectedTeam = teamSeason;

            // Assert
            testObject.SelectedTeam.ShouldBe(teamSeason);

            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfile(A<string>.Ignored,
                A<int>.Ignored)).MustHaveHappenedOnceExactly();
            testObject.TeamSeasonScheduleProfile.ShouldBeOfType<ReadOnlyCollection<TeamSeasonOpponentProfile>>();
            testObject.TeamSeasonScheduleProfile.ShouldBe(teamSeasonOpponentProfiles.ToList());

            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotals(A<string>.Ignored,
                A<int>.Ignored)).MustHaveHappenedOnceExactly();
            testObject.TeamSeasonScheduleTotals.ShouldBeOfType<ReadOnlyCollection<TeamSeasonScheduleTotals>>();

            var teamSeasonScheduleTotalsList = new List<TeamSeasonScheduleTotals> { teamSeasonScheduleTotals };
            testObject.TeamSeasonScheduleTotals.ShouldBe(teamSeasonScheduleTotalsList);

            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleAverages(A<string>.Ignored,
                A<int>.Ignored)).MustHaveHappenedOnceExactly();
            testObject.TeamSeasonScheduleAverages.ShouldBeOfType<ReadOnlyCollection<TeamSeasonScheduleAverages>>();

            var teamSeasonScheduleAveragesList = new List<TeamSeasonScheduleAverages> { teamSeasonScheduleAverages };
            testObject.TeamSeasonScheduleAverages.ShouldBe(teamSeasonScheduleAveragesList);
        }

        [Fact]
        public void ViewTeamsCommand_ShouldLoadTeams()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeason(A<int>.Ignored)).Returns(teamSeasons);

            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();

            var testObject = new TeamSeasonsControlViewModel(teamSeasonRepository, teamSeasonScheduleRepository);

            // Act
            testObject.ViewTeamsCommand.Execute(null!);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeason(WpfGlobals.SelectedSeason))
                .MustHaveHappenedOnceExactly();
            testObject.Teams.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.Teams.ShouldBe(teamSeasons);
        }

        [Fact]
        public void ViewTeamScheduleCommand_WhenSelectedTeamIsNotNull_ShouldLoadTeamSeasonScheduleProfileTotalsAndAverages()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var teamSeasonOpponentProfiles = new List<TeamSeasonOpponentProfile>();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfile(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonOpponentProfiles);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotals(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleAverages(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleAverages);

            var testObject = new TeamSeasonsControlViewModel(teamSeasonRepository, teamSeasonScheduleRepository)
            {
                SelectedTeam = new TeamSeason()
            };

            // Act
            testObject.ViewTeamScheduleCommand.Execute(null!);

            // Assert
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfile(A<string>.Ignored,
                A<int>.Ignored)).MustHaveHappened();
            testObject.TeamSeasonScheduleProfile.ShouldBeOfType<ReadOnlyCollection<TeamSeasonOpponentProfile>>();
            testObject.TeamSeasonScheduleProfile.ShouldBe(teamSeasonOpponentProfiles.ToList());

            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotals(A<string>.Ignored,
                A<int>.Ignored)).MustHaveHappened();
            testObject.TeamSeasonScheduleTotals.ShouldBeOfType<ReadOnlyCollection<TeamSeasonScheduleTotals>>();

            var teamSeasonScheduleTotalsList = new List<TeamSeasonScheduleTotals> { teamSeasonScheduleTotals };
            testObject.TeamSeasonScheduleTotals.ShouldBe(teamSeasonScheduleTotalsList);

            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleAverages(A<string>.Ignored,
                A<int>.Ignored)).MustHaveHappened();
            testObject.TeamSeasonScheduleAverages.ShouldBeOfType<ReadOnlyCollection<TeamSeasonScheduleAverages>>();

            var teamSeasonScheduleAveragesList = new List<TeamSeasonScheduleAverages> { teamSeasonScheduleAverages };
            testObject.TeamSeasonScheduleAverages.ShouldBe(teamSeasonScheduleAveragesList);
        }

        [Fact]
        public void ViewTeamScheduleCommand_WhenSelectedTeamIsNull_ShouldNotLoadTeamSeasonScheduleProfileTotalsAndAverages()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var testObject = new TeamSeasonsControlViewModel(teamSeasonRepository, teamSeasonScheduleRepository);

            var teamSeasonOpponentProfiles = new List<TeamSeasonOpponentProfile>();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfile(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonOpponentProfiles);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotals(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleAverages(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleAverages);

            // Act
            testObject.ViewTeamScheduleCommand.Execute(null!);

            // Assert
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfile(A<string>.Ignored,
                A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotals(A<string>.Ignored,
                A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleAverages(A<string>.Ignored,
                A<int>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Refresh_ShouldLoadTeams()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeason(A<int>.Ignored)).Returns(teamSeasons);

            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var testObject = new TeamSeasonsControlViewModel(teamSeasonRepository, teamSeasonScheduleRepository);

            // Act
            testObject.ViewTeamsCommand.Execute(null!);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeason(WpfGlobals.SelectedSeason))
                .MustHaveHappenedOnceExactly();
            testObject.Teams.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.Teams.ShouldBe(teamSeasons);
        }
    }
}
