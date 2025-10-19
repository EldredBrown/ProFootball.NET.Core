using System.Windows;
using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.GameFinder;

namespace EldredBrown.ProFootball.NETCore.WpfApp.Tests.ViewModelTests
{
    public class GameFinderWindowViewModelTests
    {
        [Fact]
        public void GuestNameSetter_WhenValueDoesNotEqualGuestName_ShouldAssignValueToGuestName()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService);

            var guestName = "Guest";

            // Act
            testObject.GuestName = guestName;

            // Assert
            testObject.GuestName.ShouldBe(guestName);
        }

        [Fact]
        public void HostNameSetter_WhenValueDoesNotEqualGuestName_ShouldAssignValueToHostName()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService);

            var hostName = "Host";

            // Act
            testObject.HostName = hostName;

            // Assert
            testObject.HostName.ShouldBe(hostName);
        }

        [Fact]
        public void OK_WhenGuestNameIsNull_ShouldShowBothTeamsNeededErrorMessageAndReturnFalse()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService)
            {
                GuestName = null!,
                HostName = null!
            };

            // Act
            bool valid = testObject.OK();

            // Assert
            valid.ShouldBeFalse();
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OK_WhenGuestNameIsEmpty_ShouldShowBothTeamsNeededErrorMessageAndReturnFalse()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService)
            {
                GuestName = string.Empty,
                HostName = null!
            };

            // Act
            bool valid = testObject.OK();

            // Assert
            valid.ShouldBeFalse();
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OK_WhenGuestNameIsWhiteSpace_ShouldShowBothTeamsNeededErrorMessageAndReturnFalse()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService)
            {
                GuestName = " ",
                HostName = null!
            };

            // Act
            bool valid = testObject.OK();

            // Assert
            valid.ShouldBeFalse();
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OK_WhenHostNameIsNull_ShouldShowBothTeamsNeededErrorMessageAndReturnFalse()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService)
            {
                GuestName = "Team",
                HostName = null!
            };

            // Act
            bool valid = testObject.OK();

            // Assert
            valid.ShouldBeFalse();
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OK_WhenHostNameIsEmpty_ShouldShowBothTeamsNeededErrorMessageAndReturnFalse()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService)
            {
                GuestName = "Team",
                HostName = string.Empty
            };

            // Act
            bool valid = testObject.OK();

            // Assert
            valid.ShouldBeFalse();
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OK_WhenHostNameIsWhiteSpace_ShouldShowBothTeamsNeededErrorMessageAndReturnFalse()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService)
            {
                GuestName = "Team",
                HostName = " "
            };

            // Act
            bool valid = testObject.OK();

            // Assert
            valid.ShouldBeFalse();
            A.CallTo(() => messageBoxService.Show("Please enter names for both teams.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OK_WhenGuestNameAndHostNameAreSame_ShouldShowDifferentTeamsNeededErrorMessageAndReturnFalse()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService)
            {
                GuestName = "Team",
                HostName = "Team"
            };

            // Act
            bool valid = testObject.OK();

            // Assert
            valid.ShouldBeFalse();
            A.CallTo(() => messageBoxService.Show("Please enter a different name for each team.", "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OK_WhenGuestNameAndHostNameAreDifferent_ShouldNotShowMessageAndReturnTrue()
        {
            // Arrange
            var messageBoxService = A.Fake<IMessageBoxService>();
            var testObject = new GameFinderWindowViewModel(messageBoxService)
            {
                GuestName = "Guest",
                HostName = "Host"
            };

            // Act
            bool valid = testObject.OK();

            // Assert
            valid.ShouldBeTrue();
            A.CallTo(() => messageBoxService.Show(A<string>.Ignored, "Invalid Data",
                MessageBoxButton.OK, MessageBoxImage.Error)).MustNotHaveHappened();
        }
    }
}
