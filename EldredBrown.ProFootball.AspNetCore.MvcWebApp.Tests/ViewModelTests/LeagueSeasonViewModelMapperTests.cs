using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ViewModelTests
{
    public class LeagueSeasonViewModelMapperTests
    {
        [Fact]
        public void MapLeagueSeasonToViewModel_ShouldSucceed()
        {
            // Arrange
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var testMapper = new LeagueSeasonViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            var leagueSeason = new EldredBrown.ProFootball.Net.Data.Models.LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonId = 1920
            };

            // Act
            var result = testMapper.MapLeagueSeasonToViewModel(leagueSeason);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeasonViewModel>();
            result.LeagueSeason.ShouldBe(leagueSeason);
        }

        public static TheoryData<League, int> LeagueCases => new()
        {
            { new League { Id = 1, ShortName = "NFL" }, 1 },
            { null!, -1 },
        };

        [Theory]
        [MemberData(nameof(LeagueCases))]
        public async Task MapViewModelToLeagueSeason_ShouldSetLeagueSeasonLeagueIdToLeagueIdOrMinusOne(
            League league, int expectedLeagueId)
        {
            // Arrange
            var leagueName = "NFL";
            var leagueSeasonViewModel = new LeagueSeasonViewModel
            {
                LeagueName = leagueName
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(A<string>.Ignored)).Returns(league);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new LeagueSeasonViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(leagueName)).MustHaveHappenedOnceExactly();
            leagueSeasonViewModel.LeagueSeason.LeagueId.ShouldBe(expectedLeagueId);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.ShouldBe(leagueSeasonViewModel.LeagueSeason);
        }

        public static TheoryData<Season, int> SeasonCases => new()
        {
            { new Season { Id = 1920 }, 1920 },
            { null!, -1 },
        };

        [Theory]
        [MemberData(nameof(SeasonCases))]
        public async Task MapViewModelToLeagueSeason_ShouldSetLeagueSeasonSeasonIdToSeasonIdOrMinusOne(
            Season season, int expectedSeasonId)
        {
            // Arrange
            var seasonYear = 1920;
            var leagueSeasonViewModel = new LeagueSeasonViewModel
            {
                SeasonYear = seasonYear
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);

            var testMapper = new LeagueSeasonViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappenedOnceExactly();
            leagueSeasonViewModel.LeagueSeason.SeasonId.ShouldBe(expectedSeasonId);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.ShouldBe(leagueSeasonViewModel.LeagueSeason);
        }
    }
}
