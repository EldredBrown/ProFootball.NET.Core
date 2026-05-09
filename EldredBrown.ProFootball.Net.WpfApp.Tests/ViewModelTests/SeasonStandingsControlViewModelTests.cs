using System.Collections.Generic;
using System.Collections.ObjectModel;
using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.SeasonStandings;
using EldredBrown.ProFootball.WpfApp;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Tests.ViewModelTests
{
    public class SeasonStandingsControlViewModelTests
    {
        [Fact]
        public void StandingsSetter_WhenValueDoesNotEqualStandings_ShouldAssignValueToStandings()
        {
            // Arrange
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var testObject = new SeasonStandingsControlViewModel(seasonStandingsRepository);

            // Act
            var standings = new ReadOnlyCollection<SeasonTeamStanding>(new List<SeasonTeamStanding>());
            testObject.Standings = standings;

            // Assert
            testObject.Standings.ShouldBeOfType<ReadOnlyCollection<SeasonTeamStanding>>();
            testObject.Standings.ShouldBe(standings);
        }

        [Fact]
        public void ViewStandingsCommand_ShouldLoadStandings()
        {
            // Arrange
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var testObject = new SeasonStandingsControlViewModel(seasonStandingsRepository);

            var seasonTeamStandings = new List<SeasonTeamStanding>();
            A.CallTo(() => seasonStandingsRepository.GetSeasonStandings(A<int>.Ignored)).Returns(seasonTeamStandings);

            // Act
            testObject.ViewStandingsCommand.Execute(null!);

            // Assert
            A.CallTo(() => seasonStandingsRepository.GetSeasonStandings(WpfGlobals.SelectedSeason))
                .MustHaveHappenedOnceExactly();
            testObject.Standings.ShouldBeOfType<ReadOnlyCollection<SeasonTeamStanding>>();
            testObject.Standings.ShouldBe(seasonTeamStandings);
        }

        [Fact]
        public void Refresh_ShouldLoadStandings()
        {
            // Arrange
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var testObject = new SeasonStandingsControlViewModel(seasonStandingsRepository);

            var seasonTeamStandings = new List<SeasonTeamStanding>();
            A.CallTo(() => seasonStandingsRepository.GetSeasonStandings(A<int>.Ignored)).Returns(seasonTeamStandings);

            // Act
            testObject.ViewStandingsCommand.Execute(null!);

            // Assert
            A.CallTo(() => seasonStandingsRepository.GetSeasonStandings(WpfGlobals.SelectedSeason))
                .MustHaveHappenedOnceExactly();
            testObject.Standings.ShouldBeOfType<ReadOnlyCollection<SeasonTeamStanding>>();
            testObject.Standings.ShouldBe(seasonTeamStandings);
        }
    }
}
