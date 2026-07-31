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
            LeagueSeasonViewModelMapper testMapper = SetUp();

            // Act
            var leagueSeason = new EldredBrown.ProFootball.Net.Data.Models.LeagueSeason
            {
                Id = 1,
                LeagueId = 1,
                SeasonYear = 1
            };
            var result = testMapper.MapLeagueSeasonToViewModel(leagueSeason);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeasonViewModel>();
            result.LeagueSeason.ShouldBe(leagueSeason);
        }

        public static TheoryData<Association, int> LeagueCases => new()
        {
            { new Association { Id = 1, LongName="National Football League", ShortName = "NFL" }, 1 },
            { null!, -1 },
        };

        [Theory]
        [MemberData(nameof(LeagueCases))]
        public async Task MapViewModelToLeagueSeason_ShouldSetLeagueSeasonLeagueIdToLeagueIdOrMinusOne(
            Association league, int expectedLeagueId)
        {
            // Arrange
            LeagueSeasonViewModelMapper testMapper = SetUp(league);

            // Act
            var leagueName = "NFL";
            var leagueSeasonViewModel = new LeagueSeasonViewModel
            {
                LeagueName = leagueName
            };

            var result = await testMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testMapper._leagueRepository.GetAssociationByShortNameAsync(leagueName))
                .MustHaveHappenedOnceExactly();
            leagueSeasonViewModel.LeagueSeason.LeagueId.ShouldBe(expectedLeagueId);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<LeagueSeason>();
            result.ShouldBe(leagueSeasonViewModel.LeagueSeason);
        }

        private static LeagueSeasonViewModelMapper SetUp(Association? league = null)
        {
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored)).Returns(league);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            return new LeagueSeasonViewModelMapper(fakeAssociationRepository, fakeSeasonRepository);
        }
    }
}
