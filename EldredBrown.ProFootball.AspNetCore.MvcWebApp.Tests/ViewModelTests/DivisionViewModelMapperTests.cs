using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ViewModelTests
{
    public class DivisionViewModelMapperTests
    {
        [Fact]
        public void MapDivisionToViewModel_ShouldSucceed()
        {
            // Arrange
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            var division = new EldredBrown.ProFootball.Net.Data.Models.Division
            {
                Id = 1,
                Name = "Test Division",
                LeagueId = 1,
                FirstSeasonId = 1,
                LastSeasonId = 2
            };

            // Act
            var result = testMapper.MapDivisionToViewModel(division);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<DivisionViewModel>();
            result.Division.ShouldBe(division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenParentLeagueNotFound_ShouldSetLeagueIdToMinusOne()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = null,
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(divisionViewModel.LeagueName))
                .Returns<League>(null);

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(divisionViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            divisionViewModel.Division.LeagueId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenParentLeagueFoundAndShortNameIsNull_ShouldSetLeagueIdToMinusOne()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = null,
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var parentLeague = new League
            {
                Id = 1,
                ShortName = null
            };
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(divisionViewModel.LeagueName))
                .Returns(parentLeague);

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(divisionViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            divisionViewModel.Division.LeagueId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenParentLeagueFoundAndShortNameIsNotNull_ShouldSetLeagueIdToParentLeagueId()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = null,
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var parentLeague = new League
            {
                Id = 1,
                ShortName = "PL"
            };
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(divisionViewModel.LeagueName))
                .Returns(parentLeague);

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(divisionViewModel.LeagueName))
                .MustHaveHappenedOnceExactly();
            divisionViewModel.Division.LeagueId.ShouldBe(parentLeague.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenConferenceNameIsNull_ShouldNotSetConferenceId()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = null,
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            A.CallTo(() => fakeConferenceRepository.GetConferenceByShortNameAsync(divisionViewModel.ConferenceName))
                .Returns<Conference>(null);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceByShortNameAsync(divisionViewModel.ConferenceName))
                .MustNotHaveHappened();
            divisionViewModel.Division.ConferenceId.ShouldBeNull();
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenParentConferenceNotFound_ShouldSetConferenceIdToMinusOne()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = "TC",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            A.CallTo(() => fakeConferenceRepository.GetConferenceByShortNameAsync(divisionViewModel.ConferenceName))
                .Returns<Conference>(null);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceByShortNameAsync(divisionViewModel.ConferenceName))
                .MustHaveHappenedOnceExactly();
            divisionViewModel.Division.ConferenceId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenParentConferenceFoundAndShortNameIsNull_ShouldSetConferenceIdToMinusOne()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = "TC",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var parentConference = new Conference
            {
                Id = 1,
                ShortName = null
            };
            A.CallTo(() => fakeConferenceRepository.GetConferenceByShortNameAsync(divisionViewModel.ConferenceName))
                .Returns(parentConference);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceByShortNameAsync(divisionViewModel.ConferenceName))
                .MustHaveHappenedOnceExactly();
            divisionViewModel.Division.ConferenceId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenParentConferenceFoundAndShortNameIsNotNull_ShouldSetConferenceIdToParentConferenceId()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = "TC",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var parentConference = new Conference
            {
                Id = 1,
                ShortName = "PC"
            };
            A.CallTo(() => fakeConferenceRepository.GetConferenceByShortNameAsync(divisionViewModel.ConferenceName))
                .Returns(parentConference);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceByShortNameAsync(divisionViewModel.ConferenceName))
                .MustHaveHappenedOnceExactly();
            divisionViewModel.Division.ConferenceId.ShouldBe(parentConference.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenFirstSeasonNotFound_ShouldSetDivisionFirstSeasonIdToMinusOne()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = "TC",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.FirstSeasonYear)).Returns<Season>(null);

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            divisionViewModel.Division.FirstSeasonId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenFirstSeasonFound_ShouldSetDivisionFirstSeasonIdToFirstSeasonId()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = "TC",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.FirstSeasonYear)).Returns(firstSeason);

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.FirstSeasonYear))
                .MustHaveHappenedOnceExactly();
            divisionViewModel.Division.FirstSeasonId.ShouldBe(firstSeason.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenLastSeasonYearIsNull_ShouldNotSetDivisionLastSeasonId()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = "TC",
                FirstSeasonYear = 1,
                LastSeasonYear = null
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.FirstSeasonYear)).Returns(firstSeason);

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            divisionViewModel.Division.LastSeasonId.ShouldBeNull();
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenLastSeasonYearIsNotNullAndLastSeasonNotFound_ShouldSetDivisionLastSeasonIdToMinusOne()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = "TC",
                FirstSeasonYear = 1,
                LastSeasonYear = 2
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.FirstSeasonYear)).Returns(firstSeason);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.LastSeasonYear.Value))
                .Returns<Season>(null);

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            divisionViewModel.Division.LastSeasonId.ShouldBe(-1);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }

        [Fact]
        public async Task MapViewModelToDivision_WhenLastSeasonYearIsNotNullAndLastSeasonFound_ShouldSetDivisionLastSeasonIdToLastSeasonId()
        {
            // Arrange
            var divisionViewModel = new DivisionViewModel
            {
                Id = 1,
                Name = "Test Division",
                LeagueName = "TL",
                ConferenceName = "TC",
                FirstSeasonYear = 1,
                LastSeasonYear = 2
            };

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var firstSeason = new Season { Id = 1 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.FirstSeasonYear)).Returns(firstSeason);
            var lastSeason = new Season { Id = 2 };
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(divisionViewModel.LastSeasonYear.Value))
                .Returns(lastSeason);

            var testMapper = new DivisionViewModelMapper(fakeLeagueRepository, fakeConferenceRepository,
                fakeSeasonRepository);

            // Act
            var result = await testMapper.MapViewModelToDivision(divisionViewModel);

            // Assert
            divisionViewModel.Division.LastSeasonId.ShouldBe(lastSeason.Id);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Division>();
            result.ShouldBe(divisionViewModel.Division);
        }
    }
}
