using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services;
using EldredBrown.ProFootball.NETCore.WpfApp.Main;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.Rankings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.SeasonStandings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.GamePredictor;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.Games;
using EldredBrown.ProFootball.WpfApp;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Tests.ViewModelTests
{
    public class MainWindowViewModelTests
    {
        [Fact]
        public void SeasonsSetter_WhenValueIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var gamesWindowFactory = A.Fake<IGamesWindowFactory>();
            var gamePredictorWindowFactory = A.Fake<IGamePredictorWindowFactory>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            var testObject = new MainWindowViewModel(seasonRepository, gamesWindowFactory, gamePredictorWindowFactory,
                weeklyUpdateService);

            // Act
            Func<ReadOnlyCollection<int>> func = () => testObject.Seasons = null!;

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testObject.GetType()}.{nameof(testObject.Seasons)}");
        }

        [Fact]
        public void SeasonsSetter_WhenValueIsNotNullAndDoesNotEqualSeasons_ShouldAssignValueToSeasons()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var gamesWindowFactory = A.Fake<IGamesWindowFactory>();
            var gamePredictorWindowFactory = A.Fake<IGamePredictorWindowFactory>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            var testObject = new MainWindowViewModel(seasonRepository, gamesWindowFactory, gamePredictorWindowFactory,
                weeklyUpdateService);

            var seasons = new ReadOnlyCollection<int>(new List<int>());

            // Act
            Func<ReadOnlyCollection<int>> func = () => testObject.Seasons = seasons;

            // Assert
            func.ShouldNotThrow();
            testObject.Seasons.ShouldBeOfType<ReadOnlyCollection<int>>();
            testObject.Seasons.ShouldBe(seasons);
        }

        [Fact]
        public void SelectedSeasonsSetter_WhenValueDoesNotEqualSelectedSeason_ShouldAssignValueToSelectedSeason()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var gamesWindowFactory = A.Fake<IGamesWindowFactory>();
            var gamePredictorWindowFactory = A.Fake<IGamePredictorWindowFactory>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            var testObject = new MainWindowViewModel(seasonRepository, gamesWindowFactory, gamePredictorWindowFactory,
                weeklyUpdateService)
                {
                    TeamSeasonsControlViewModel = A.Fake<ITeamSeasonsControlViewModel>(),
                    SeasonStandingsControlViewModel = A.Fake<ISeasonStandingsControlViewModel>(),
                    RankingsControlViewModel = A.Fake<IRankingsControlViewModel>()
                };

            var season = 1921;

            // Act
            testObject.SelectedSeason = season;

            // Assert
            testObject.SelectedSeason.ShouldBe(season);
            A.CallTo(() => testObject.TeamSeasonsControlViewModel.Refresh()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testObject.SeasonStandingsControlViewModel.Refresh()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testObject.RankingsControlViewModel.Refresh()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void PredictGameScoreCommand_ShouldCallGamePredictorWindowFactoryCreateWindow()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var gamesWindowFactory = A.Fake<IGamesWindowFactory>();
            var gamePredictorWindowFactory = A.Fake<IGamePredictorWindowFactory>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            var testObject = new MainWindowViewModel(seasonRepository, gamesWindowFactory, gamePredictorWindowFactory,
                weeklyUpdateService);

            // Act
            testObject.PredictGameScoreCommand.Execute(null!);

            // Assert
            A.CallTo(() => gamePredictorWindowFactory.CreateWindow()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RunWeeklyUpdateCommand_ShouldCallWeeklyUpdateServiceRunWeeklyUpdate()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var gamesWindowFactory = A.Fake<IGamesWindowFactory>();
            var gamePredictorWindowFactory = A.Fake<IGamePredictorWindowFactory>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            var testObject = new MainWindowViewModel(seasonRepository, gamesWindowFactory, gamePredictorWindowFactory,
                weeklyUpdateService);

            // Act
            testObject.WeeklyUpdateCommand.Execute(null!);

            // Assert
            A.CallTo(() => weeklyUpdateService.RunWeeklyUpdate(WpfGlobals.SelectedSeason))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ShowGamesCommand_ShouldCallWeeklyUpdateServiceRunWeeklyUpdate()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var gamesWindowFactory = A.Fake<IGamesWindowFactory>();
            var gamePredictorWindowFactory = A.Fake<IGamePredictorWindowFactory>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            var testObject = new MainWindowViewModel(seasonRepository, gamesWindowFactory, gamePredictorWindowFactory,
                weeklyUpdateService)
                {
                    TeamSeasonsControlViewModel = A.Fake<ITeamSeasonsControlViewModel>()
                };

            // Act
            testObject.ShowGamesCommand.Execute(null!);

            // Assert
            A.CallTo(() => gamesWindowFactory.CreateWindow()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testObject.TeamSeasonsControlViewModel.Refresh()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ViewSeasonsCommand_ShouldShowSeasonAndSelectedSeasonFromDataStore()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var gamesWindowFactory = A.Fake<IGamesWindowFactory>();
            var years = new int[] { 1920, 1921, 1922 };
            var seasons = new List<Season>
            {
                new Season { Year = years[0] },
                new Season { Year = years[1] },
                new Season { Year = years[2] }
            };
            A.CallTo(() => seasonRepository.GetSeasons()).Returns(seasons);

            var gamePredictorWindowFactory = A.Fake<IGamePredictorWindowFactory>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testObject = new MainWindowViewModel(seasonRepository, gamesWindowFactory, gamePredictorWindowFactory,
                weeklyUpdateService)
                {
                    TeamSeasonsControlViewModel = A.Fake<ITeamSeasonsControlViewModel>(),
                    SeasonStandingsControlViewModel = A.Fake<ISeasonStandingsControlViewModel>(),
                    RankingsControlViewModel = A.Fake<IRankingsControlViewModel>()
                };

            // Act
            testObject.ViewSeasonsCommand.Execute(null!);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasons()).MustHaveHappenedOnceExactly();
            testObject.Seasons.ShouldBeOfType<ReadOnlyCollection<int>>();
            testObject.Seasons.ShouldBe(years);
            testObject.SelectedSeason.ShouldBe(years[0]);
        }
    }
}
