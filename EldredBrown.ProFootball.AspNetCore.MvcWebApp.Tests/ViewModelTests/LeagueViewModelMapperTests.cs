using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ViewModelTests
{
    public class LeagueViewModelMapperTests
    {
        [Fact]
        public void MapLeagueToViewModel_ShouldSucceed()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var testMapper = new LeagueViewModelMapper(fakeSeasonRepository);

            var league = new EldredBrown.ProFootball.Net.Data.Models.League
            {
                Id = 1,
                ShortName = "TL",
                LongName = "Test League",
                FirstSeasonId = 1,
                LastSeasonId = 2
            };

            // Act
            var result = testMapper.MapLeagueToViewModel(league);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueViewModel>();
            result.League.ShouldBe(league);
        }

        [Fact]
        public async Task MapViewModelToLeague_WhenFirstSeasonNotFound_ShouldSetLeagueFirstSeasonIdToMinusOne()
        {
            // Arrange
            var leagueViewModel = new LeagueViewModel
            {
                Id = 1,
                ShortName = "TL",
                LongName = "Test League",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.FirstSeasonYear)).Returns<Season>(null);

            var testMapper = new LeagueViewModelMapper(fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToLeague(leagueViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            leagueViewModel.League.FirstSeasonId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.ShouldBe(leagueViewModel.League);
        }

        [Fact]
        public async Task MapViewModelToLeague_WhenFirstSeasonFound_ShouldSetLeagueFirstSeasonIdToFirstSeasonId()
        {
            // Arrange
            var leagueViewModel = new LeagueViewModel
            {
                Id = 1,
                ShortName = "TL",
                LongName = "Test League",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.FirstSeasonYear)).Returns(firstSeason);

            var testMapper = new LeagueViewModelMapper(fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToLeague(leagueViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            leagueViewModel.League.FirstSeasonId.ShouldBe(firstSeason.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.ShouldBe(leagueViewModel.League);
        }

        [Fact]
        public async Task MapViewModelToLeague_WhenLastSeasonYearIsNull_ShouldSetLeagueLastSeasonIdToNull()
        {
            // Arrange
            var leagueViewModel = new LeagueViewModel
            {
                Id = 1,
                ShortName = "TL",
                LongName = "Test League",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.FirstSeasonYear)).Returns(firstSeason);

            var testMapper = new LeagueViewModelMapper(fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToLeague(leagueViewModel);

            // Assert
            leagueViewModel.League.LastSeasonId.ShouldBeNull();
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.ShouldBe(leagueViewModel.League);
        }

        [Fact]
        public async Task MapViewModelToLeague_WhenLastSeasonYearIsNotNullAndLastSeasonNotFound_ShouldSetLeagueLastSeasonIdToMinusOne()
        {
            // Arrange
            var leagueViewModel = new LeagueViewModel
            {
                Id = 1,
                ShortName = "TL",
                LongName = "Test League",
                FirstSeasonYear = 1,
                LastSeasonYear = 2
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.FirstSeasonYear)).Returns(firstSeason);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.LastSeasonYear.Value))
                .Returns<Season>(null);

            var testMapper = new LeagueViewModelMapper(fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToLeague(leagueViewModel);

            // Assert
            leagueViewModel.League.LastSeasonId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.ShouldBe(leagueViewModel.League);
        }

        [Fact]
        public async Task MapViewModelToLeague_WhenLastSeasonYearIsNotNullAndLastSeasonFound_ShouldSetLeagueLastSeasonIdToLastSeasonId()
        {
            // Arrange
            var leagueViewModel = new LeagueViewModel
            {
                Id = 1,
                ShortName = "TL",
                LongName = "Test League",
                FirstSeasonYear = 1,
                LastSeasonYear = 2
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.FirstSeasonYear)).Returns(firstSeason);
            var lastSeason = new Season { Id = 2 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(leagueViewModel.LastSeasonYear.Value))
                .Returns(lastSeason);

            var testMapper = new LeagueViewModelMapper(fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToLeague(leagueViewModel);

            // Assert
            leagueViewModel.League.LastSeasonId.ShouldBe(lastSeason.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<League>();
            result.ShouldBe(leagueViewModel.League);
        }
    }
}
