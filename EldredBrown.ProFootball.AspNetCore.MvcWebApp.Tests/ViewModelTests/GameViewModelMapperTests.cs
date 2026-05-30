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
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var testMapper = new GameViewModelMapper(fakeSeasonRepository);

            var game = new EldredBrown.ProFootball.Net.Data.Models.Game
            {
                Id = 1,
                SeasonId = 1920,
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
        public async Task MapViewModelToGame_WhenFirstSeasonNotFound_ShouldSetGameFirstSeasonIdToMinusOne()
        {
            // Arrange
            var gameViewModel = new GameViewModel
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

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(gameViewModel.SeasonYear)).Returns<Season>(null);

            var testMapper = new GameViewModelMapper(fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToGame(gameViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(gameViewModel.SeasonYear))
                .MustHaveHappenedOnceExactly();
            gameViewModel.Game.SeasonId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.ShouldBe(gameViewModel.Game);
        }

        [Fact]
        public async Task MapViewModelToGame_WhenFirstSeasonFound_ShouldSetGameFirstSeasonIdToFirstSeasonId()
        {
            // Arrange
            var gameViewModel = new GameViewModel
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

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(gameViewModel.SeasonYear)).Returns(firstSeason);

            var testMapper = new GameViewModelMapper(fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToGame(gameViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(gameViewModel.SeasonYear))
                .MustHaveHappenedOnceExactly();
            gameViewModel.Game.SeasonId.ShouldBe(firstSeason.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Game>();
            result.ShouldBe(gameViewModel.Game);
        }
    }
}
