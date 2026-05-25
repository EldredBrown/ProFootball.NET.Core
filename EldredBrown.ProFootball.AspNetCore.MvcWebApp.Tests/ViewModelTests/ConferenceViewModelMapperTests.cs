using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ViewModelTests
{
    public class ConferenceViewModelMapperTests
    {
        [Fact]
        public void MapConferenceToViewModel_ShouldSucceed()
        {
            // Arrange
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            var conference = new EldredBrown.ProFootball.Net.Data.Models.Conference
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueId = 1,
                FirstSeasonId = 1,
                LastSeasonId = 2
            };

            // Act
            var result = testMapper.MapConferenceToViewModel(conference);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<ConferenceViewModel>();
            result.Conference.ShouldBe(conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenParentLeagueIsNullAndLastSeasonYearIsNull_ShouldSucceed()
        {
            // Arrange
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(A<string>.Ignored)).Returns<League>(null);

            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "L1",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .Returns(firstSeason);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.LeagueId.ShouldBe(-1);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.FirstSeasonId.ShouldBe(firstSeason.Id);
            conferenceViewModel.Conference.LastSeasonId.ShouldBeNull();
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenParentLeagueShortNameIsNullAndLastSeasonYearIsNull_ShouldSucceed()
        {
            // Arrange
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var parentLeague = new League
            {
                Id = 1,
                ShortName = null
            };
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(A<string>.Ignored)).Returns<League>(parentLeague);

            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "L1",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .Returns(firstSeason);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.LeagueId.ShouldBe(-1);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.FirstSeasonId.ShouldBe(firstSeason.Id);
            conferenceViewModel.Conference.LastSeasonId.ShouldBeNull();
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenParentLeagueShortNameIsNotNullAndLastSeasonYearIsNull_ShouldSucceed()
        {
            // Arrange
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var parentLeague = new League
            {
                Id = 1,
                ShortName = "PL"
            };
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(A<string>.Ignored)).Returns<League>(parentLeague);

            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "L1",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .Returns(firstSeason);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.LeagueId.ShouldBe(parentLeague.Id);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.FirstSeasonId.ShouldBe(firstSeason.Id);
            conferenceViewModel.Conference.LastSeasonId.ShouldBeNull();
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenLastSeasonYearIsNotNull_ShouldSucceed()
        {
            // Arrange
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var parentLeague = new League
            {
                Id = 1,
                ShortName = "PL"
            };
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(A<string>.Ignored)).Returns<League>(parentLeague);

            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "L1",
                FirstSeasonYear = 1,
                LastSeasonYear = 2
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .Returns(firstSeason);
            var lastSeason = new Season { Id = 2 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.LastSeasonYear.Value))
                .Returns(lastSeason);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.LeagueId.ShouldBe(parentLeague.Id);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.FirstSeasonId.ShouldBe(firstSeason.Id);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.LastSeasonYear.Value))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.LastSeasonId.ShouldBe(lastSeason.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }
    }
}
