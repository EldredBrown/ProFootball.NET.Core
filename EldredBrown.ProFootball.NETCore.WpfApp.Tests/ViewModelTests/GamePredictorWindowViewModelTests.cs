using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.GamePredictor;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Tests.ViewModelTests
{
    public class GamePredictorWindowViewModelTests
    {
        [Fact]
        public void GuestSeasonsSetter_WhenValueIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService);

            // Act
            Func<ReadOnlyCollection<int>> func = () => testObject.GuestSeasons = null!;

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testObject.GetType()}.{nameof(testObject.GuestSeasons)}");
        }

        [Fact]
        public void GuestSeasonsSetter_WhenValueIsNotNullAndDoesNotEqualGuestSeasons_ShouldAssignValueToGuestSeasons()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService);

            var guestSeasons = new ReadOnlyCollection<int>(new List<int>());

            // Act
            Func<ReadOnlyCollection<int>> func = () => testObject.GuestSeasons = guestSeasons;

            // Assert
            func.ShouldNotThrow();
            testObject.GuestSeasons.ShouldBeOfType<ReadOnlyCollection<int>>();
            testObject.GuestSeasons.ShouldBe(guestSeasons);
        }

        [Fact]
        public void GuestSelectedSeasonSetter_WhenValueDoesNotEqualGuestSelectedSeason_ShouldAssignValueToSelectedGuestSeason()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestSelectedSeason = 1920
            };

            var guestSelectedSeason = 1921;

            // Act
            testObject.GuestSelectedSeason = guestSelectedSeason;

            // Assert
            testObject.GuestSelectedSeason.ShouldBe(guestSelectedSeason);
        }

        [Fact]
        public void GuestNameSetter_WhenValueDoesNotEqualGuestName_ShouldAssignValueToGuestName()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = "Team"
            };

            var guestName = "Guest";

            // Act
            testObject.GuestName = guestName;

            // Assert
            testObject.GuestName.ShouldBe(guestName);
        }

        [Fact]
        public void GuestScoreSetter_WhenValueDoesNotEqualGuestScore_ShouldAssignValueToGuestScore()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestScore = 0
            };

            var guestScore = 7;

            // Act
            testObject.GuestScore = guestScore;

            // Assert
            testObject.GuestScore.ShouldBe(guestScore);
        }

        [Fact]
        public void HostSeasonsSetter_WhenValueIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService);

            // Act
            Func<ReadOnlyCollection<int>> func = () => testObject.HostSeasons = null!;

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testObject.GetType()}.{nameof(testObject.HostSeasons)}");
        }

        [Fact]
        public void HostSeasonsSetter_WhenValueIsNotNullAndDoesNotEqualHostSeasons_ShouldAssignValueToHostSeasons()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService);

            var hostSeasons = new ReadOnlyCollection<int>(new List<int>());

            // Act
            Func<ReadOnlyCollection<int>> func = () => testObject.HostSeasons = hostSeasons;

            // Assert
            func.ShouldNotThrow();
            testObject.HostSeasons.ShouldBeOfType<ReadOnlyCollection<int>>();
            testObject.HostSeasons.ShouldBe(hostSeasons);
        }

        [Fact]
        public void HostSelectedSeasonSetter_WhenValueDoesNotEqualHostSelectedSeason_ShouldAssignValueToSelectedHostSeason()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                HostSelectedSeason = 1920
            };

            var hostSelectedSeason = 1921;

            // Act
            testObject.HostSelectedSeason = hostSelectedSeason;

            // Assert
            testObject.HostSelectedSeason.ShouldBe(hostSelectedSeason);
        }

        [Fact]
        public void HostNameSetter_WhenValueDoesNotEqualHostName_ShouldAssignValueToHostName()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                HostName = "Team"
            };

            var hostName = "Host";

            // Act
            testObject.HostName = hostName;

            // Assert
            testObject.HostName.ShouldBe(hostName);
        }

        [Fact]
        public void HostScoreSetter_WhenValueDoesNotEqualHostScore_ShouldAssignValueToHostScore()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                HostScore = 0
            };

            var hostScore = 7;

            // Act
            testObject.HostScore = hostScore;

            // Assert
            testObject.HostScore.ShouldBe(hostScore);
        }

        [Fact]
        public void CalculatePredictionCommand_WhenGuestNameIsNull_ShouldShowBothTeamsNeededErrorMessage()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = null!,
                HostName = null!
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void CalculatePredictionCommand_WhenGuestNameIsEmpty_ShouldShowBothTeamsNeededErrorMessage()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = string.Empty,
                HostName = null!
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void CalculatePredictionCommand_WhenGuestNameIsWhiteSpace_ShouldShowBothTeamsNeededErrorMessage()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = " ",
                HostName = null!
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void CalculatePredictionCommand_WhenGuestNameIsValidAndHostNameIsNull_ShouldShowBothTeamsNeededErrorMessage()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = "Team",
                HostName = null!
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void CalculatePredictionCommand_WhenGuestNameIsValidAndHostNameIsEmpty_ShouldShowBothTeamsNeededErrorMessage()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = "Team",
                HostName = string.Empty
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void CalculatePredictionCommand_WhenGuestNameIsValidAndHostNameIsWhiteSpace_ShouldShowBothTeamsNeededErrorMessage()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = "Team",
                HostName = " "
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void CalculatePredictionCommand_WhenGuestSeasonNotFoundInDataStore_ShouldShowTeamNotInDatabaseMessageErrorMessage()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var guestName = "Guest";
            TeamSeason guestSeason = null!;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeason(guestName, A<int>.Ignored))
                .Returns(guestSeason);

            var hostName = "Host";
            TeamSeason hostSeason = null!;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeason(hostName, A<int>.Ignored))
                .Returns(hostSeason);

            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = guestName,
                HostName = hostName
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show(
                "Please make sure that both teams are in the league and that both team names are spelled correctly.",
                "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void CalculatePredictionCommand_WhenHostSeasonNotFoundInDataStore_ShouldShowTeamNotInDatabaseMessageErrorMessage()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var guestName = "Guest";
            TeamSeason guestSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeason(guestName, A<int>.Ignored))
                .Returns(guestSeason);

            var hostName = "Host";
            TeamSeason hostSeason = null!;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeason(hostName, A<int>.Ignored))
                .Returns(hostSeason);

            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = guestName,
                HostName = hostName
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show(
                "Please make sure that both teams are in the league and that both team names are spelled correctly.",
                "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void CalculatePredictionCommand_WhenDataEntryIsValid_ShouldCallGamePredictorServicePredictGameScore()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var guestName = "Guest";
            TeamSeason guestSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeason(guestName, A<int>.Ignored))
                .Returns(guestSeason);

            var hostName = "Host";
            TeamSeason hostSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeason(hostName, A<int>.Ignored))
                .Returns(hostSeason);

            var gamePredictorService = A.Fake<IGamePredictorService>();
            double guestScore = 7.00d;
            double hostScore = 14.00d;
            A.CallTo(() => gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .Returns((guestScore, hostScore));
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService)
            {
                GuestName = guestName,
                HostName = hostName
            };

            // Act
            testObject.CalculatePredictionCommand.Execute(null!);

            // Assert
            A.CallTo(() => messageBoxService.Show(A<string>.Ignored, "Invalid Data", MessageBoxButton.OK,
                MessageBoxImage.Error)).MustNotHaveHappened();
            A.CallTo(() => gamePredictorService.PredictGameScore(guestSeason, hostSeason))
                .MustHaveHappenedOnceExactly();
            testObject.GuestScore.ShouldBe((int)guestScore);
            testObject.HostScore.ShouldBe((int)hostScore);
        }

        [Fact]
        public void ViewSeasonsCommand_Should()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = 1920 },
                new Season { Year = 1921 },
                new Season { Year = 1922 }
            };
            A.CallTo(() => seasonRepository.GetSeasons()).Returns(seasons);

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var gamePredictorService = A.Fake<IGamePredictorService>();
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GamePredictorWindowViewModel(seasonRepository, teamSeasonRepository,
                gamePredictorService, messageBoxService);

            // Act
            testObject.ViewSeasonsCommand.Execute(null!);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasons()).MustHaveHappenedOnceExactly();
            testObject.GuestSeasons.ShouldBeOfType<ReadOnlyCollection<int>>();
            testObject.GuestSelectedSeason.ShouldBe(testObject.GuestSeasons.First());
            testObject.HostSeasons.ShouldBeOfType<ReadOnlyCollection<int>>();
            testObject.HostSelectedSeason.ShouldBe(testObject.HostSeasons.First());
        }
    }
}
