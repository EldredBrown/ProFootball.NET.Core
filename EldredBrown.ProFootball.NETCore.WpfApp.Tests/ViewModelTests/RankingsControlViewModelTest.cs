using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.Rankings;
using EldredBrown.ProFootball.WpfApp;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Tests.ViewModelTests
{
    public class RankingsControlViewModelTest
    {
        [Fact]
        public void TotalRankingsSetter_WhenValueIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testObject = new RankingsControlViewModel(teamSeasonRepository);

            // Act
            Func<ReadOnlyCollection<TeamSeason>> func = () => testObject.TotalRankings = null!;

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testObject.GetType()}.{nameof(testObject.TotalRankings)}");
        }

        [Fact]
        public void TotalRankingsSetter_WhenValueIsNotNullAndDoesNotEqualTotalRankings_ShouldAssignValueToTotalRankings()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testObject = new RankingsControlViewModel(teamSeasonRepository);

            // Act
            var teamSeasons = new ReadOnlyCollection<TeamSeason>(new List<TeamSeason>());
            Func<ReadOnlyCollection<TeamSeason>> func = () => testObject.TotalRankings = teamSeasons;

            // Assert
            func.ShouldNotThrow();
            testObject.TotalRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.TotalRankings.ShouldBe(teamSeasons);
        }

        [Fact]
        public void OffensiveRankingsSetter_WhenValueIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testObject = new RankingsControlViewModel(teamSeasonRepository);

            // Act
            Func<ReadOnlyCollection<TeamSeason>> func = () => testObject.OffensiveRankings = null!;

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testObject.GetType()}.{nameof(testObject.OffensiveRankings)}");
        }

        [Fact]
        public void OffensiveRankingsSetter_WhenValueDoesNotEqualOffensiveRankings_ShouldAssignValueToOffensiveRankings()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testObject = new RankingsControlViewModel(teamSeasonRepository);

            // Act
            var teamSeasons = new ReadOnlyCollection<TeamSeason>(new List<TeamSeason>());
            Func<ReadOnlyCollection<TeamSeason>> func = () => testObject.OffensiveRankings = teamSeasons;

            // Assert
            func.ShouldNotThrow();
            testObject.OffensiveRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.OffensiveRankings.ShouldBe(teamSeasons);
        }

        [Fact]
        public void DefensiveRankingsSetter_WhenValueIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testObject = new RankingsControlViewModel(teamSeasonRepository);

            // Act
            Func<ReadOnlyCollection<TeamSeason>> func = () => testObject.DefensiveRankings = null!;

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testObject.GetType()}.{nameof(testObject.DefensiveRankings)}");
        }

        [Fact]
        public void DefensiveRankingsSetter_WhenValueDoesNotEqualDefensiveRankings_ShouldAssignValueToDefensiveRankings()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testObject = new RankingsControlViewModel(teamSeasonRepository);

            // Act
            var teamSeasons = new ReadOnlyCollection<TeamSeason>(new List<TeamSeason>());
            Func<ReadOnlyCollection<TeamSeason>> func = () => testObject.DefensiveRankings = teamSeasons;

            // Assert
            func.ShouldNotThrow();
            testObject.DefensiveRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.DefensiveRankings.ShouldBe(teamSeasons);
        }

        [Fact]
        public void ViewRankingsCommand_ShouldLoadRankings()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testObject = new RankingsControlViewModel(teamSeasonRepository);

            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeason(A<int>.Ignored)).Returns(teamSeasons);

            // Act
            testObject.ViewRankingsCommand.Execute(null!);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeason(WpfGlobals.SelectedSeason))
                .MustHaveHappenedOnceExactly();
            testObject.TotalRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.TotalRankings.ShouldBe(teamSeasons);
            testObject.OffensiveRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.OffensiveRankings.ShouldBe(teamSeasons);
            testObject.DefensiveRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.DefensiveRankings.ShouldBe(teamSeasons);
        }

        [Fact]
        public void Refresh_ShouldLoadRankings()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testObject = new RankingsControlViewModel(teamSeasonRepository);

            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeason(A<int>.Ignored)).Returns(teamSeasons);

            // Act
            testObject.ViewRankingsCommand.Execute(null!);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeason(WpfGlobals.SelectedSeason))
                .MustHaveHappenedOnceExactly();
            testObject.TotalRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.TotalRankings.ShouldBe(teamSeasons);
            testObject.OffensiveRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.OffensiveRankings.ShouldBe(teamSeasons);
            testObject.DefensiveRankings.ShouldBeOfType<ReadOnlyCollection<TeamSeason>>();
            testObject.DefensiveRankings.ShouldBe(teamSeasons);
        }
    }
}
