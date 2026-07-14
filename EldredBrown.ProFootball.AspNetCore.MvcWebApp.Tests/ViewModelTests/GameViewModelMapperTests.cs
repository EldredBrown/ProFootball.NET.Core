using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ViewModelTests
{
    public class GameViewModelMapperTests
    {
        [Fact]
        public void MapGameToViewModel_ShouldSucceed()
        {
            // Arrange
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var testMapper = new GameViewModelMapper(fakeAssociationRepository);

            var game = new EldredBrown.ProFootball.Net.Data.Models.Game
            {
                Id = 1,
                SeasonYear = 1920,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            // Act
            var result = testMapper.MapGameToViewModel(game);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<GameViewModel>();
            result.Game.ShouldBe(game);
        }

        [Fact]
        public async Task MapViewModelToGame_WhenLeagueNameIsNeitherNullNorEmptyAndLeagueIsNull_ShouldSetGameLeagueIdToMinusOne()
        {
            // Arrange
            var gameViewModel = new GameViewModel
            {
                Id = 1,
                SeasonYear = 1920,
                LeagueName = "League",
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns<Association?>(null);

            var testMapper = new GameViewModelMapper(fakeAssociationRepository);

            // Act
            var result = await testMapper.MapViewModelToGame(gameViewModel);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(gameViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            gameViewModel.Game.LeagueId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.ShouldBe(gameViewModel.Game);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task MapViewModelToGame_WhenLeagueNameIsNullOrEmpty_ShouldSetGameLeagueIdToNull(string? leagueName)
        {
            // Arrange
            var gameViewModel = new GameViewModel
            {
                Id = 1,
                SeasonYear = 1920,
                LeagueName = leagueName,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns<Association?>(null);

            var testMapper = new GameViewModelMapper(fakeAssociationRepository);

            // Act
            var result = await testMapper.MapViewModelToGame(gameViewModel);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(gameViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            gameViewModel.Game.LeagueId.ShouldBeNull();
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.ShouldBe(gameViewModel.Game);
        }

        [Fact]
        public async Task MapViewModelToGame_WhenLeagueIsNotNull_ShouldSetGameLeagueIdToLeagueId()
        {
            // Arrange
            var gameViewModel = new GameViewModel
            {
                Id = 1,
                SeasonYear = 1920,
                LeagueName = "League",
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var league = new Association
            {
                Id = 1,
                LongName = "National Footbal League",
                ShortName = "NFL"
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored)).Returns(league);

            var testMapper = new GameViewModelMapper(fakeAssociationRepository);

            // Act
            var result = await testMapper.MapViewModelToGame(gameViewModel);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(gameViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            gameViewModel.Game.LeagueId.ShouldBe(league.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.ShouldBe(gameViewModel.Game);
        }
    }
}
