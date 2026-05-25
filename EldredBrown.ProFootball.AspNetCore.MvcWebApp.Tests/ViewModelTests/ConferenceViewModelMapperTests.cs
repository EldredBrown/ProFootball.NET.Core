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
        public async Task MapViewModelToConference_WhenParentLeagueNotFound_ShouldSetLeagueIdToMinusOne()
        {
            // Arrange
            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "TL",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .Returns<League>(null);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.LeagueId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenParentLeagueFoundAndShortNameIsNull_ShouldSetLeagueIdToMinusOne()
        {
            // Arrange
            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "TL",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var parentLeague = new League
            {
                Id = 1,
                ShortName = null
            };
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .Returns(parentLeague);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.LeagueId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenParentLeagueFoundAndShortNameIsNotNull_ShouldSetLeagueIdToParentLeagueId()
        {
            // Arrange
            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "TL",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var parentLeague = new League
            {
                Id = 1,
                ShortName = "PL"
            };
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .Returns(parentLeague);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(conferenceViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.LeagueId.ShouldBe(parentLeague.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenFirstSeasonNotFound_ShouldSetConferenceFirstSeasonIdToMinusOne()
        {
            // Arrange
            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "TL",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear)).Returns<Season>(null);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.FirstSeasonId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenFirstSeasonFound_ShouldSetConferenceFirstSeasonIdToFirstSeasonId()
        {
            // Arrange
            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "TL",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear)).Returns(firstSeason);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            conferenceViewModel.Conference.FirstSeasonId.ShouldBe(firstSeason.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenLastSeasonYearIsNull_ShouldSetConferenceLastSeasonIdToNull()
        {
            // Arrange
            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "TL",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear)).Returns(firstSeason);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            conferenceViewModel.Conference.LastSeasonId.ShouldBeNull();
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenLastSeasonYearIsNotNullAndLastSeasonNotFound_ShouldSetConferenceLastSeasonIdToMinusOne()
        {
            // Arrange
            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "TL",
                FirstSeasonYear = 1,
                LastSeasonYear = 2
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear)).Returns(firstSeason);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.LastSeasonYear.Value))
                .Returns<Season>(null);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            conferenceViewModel.Conference.LastSeasonId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }

        [Fact]
        public async Task MapViewModelToConference_WhenLastSeasonYearIsNotNullAndLastSeasonFound_ShouldSetConferenceLastSeasonIdToLastSeasonId()
        {
            // Arrange
            var conferenceViewModel = new ConferenceViewModel
            {
                Id = 1,
                ShortName = "TC",
                LongName = "Test Conference",
                LeagueName = "TL",
                FirstSeasonYear = 1,
                LastSeasonYear = 2
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.FirstSeasonYear)).Returns(firstSeason);
            var lastSeason = new Season { Id = 2 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(conferenceViewModel.LastSeasonYear.Value))
                .Returns(lastSeason);

            var testMapper = new ConferenceViewModelMapper(fakeLeagueRepository, fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToConference(conferenceViewModel);

            // Assert
            conferenceViewModel.Conference.LastSeasonId.ShouldBe(lastSeason.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Conference>();
            result.ShouldBe(conferenceViewModel.Conference);
        }
    }
}
