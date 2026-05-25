using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class ConferenceControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnConferenceIndexView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference> { };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeConferenceIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndConferenceFound_ShouldReturnConferenceDetailsView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conferenceViewModel = new ConferenceViewModel { };
            A.CallTo(() => fakeConferenceViewModelMapper.MapConferenceToViewModel(An<Conference>.Ignored))
                .Returns(conferenceViewModel);

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conference = new Conference { };
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(An<int>.Ignored)).Returns(conference);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceViewModelMapper.MapConferenceToViewModel(conference))
                .MustHaveHappenedOnceExactly();
            fakeConferenceDetailsViewModel.Conference.ShouldNotBeNull();
            fakeConferenceDetailsViewModel.Conference.ShouldBeOfType<ConferenceViewModel>();
            fakeConferenceDetailsViewModel.Conference.ShouldBe(conferenceViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeConferenceDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var result = await testController.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenConferenceNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = null;
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(An<int>.Ignored)).Returns(conference);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnConferenceCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddConferenceToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conference = new Conference { };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conference = new Conference
            {
                Id = 2,
                ShortName = "C4",
                LongName = "Conference 4",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A conference with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyShortNameViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conference = new Conference
            {
                Id = 4,
                ShortName = "C2",
                LongName = "Conference 4",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ShortName");
            testController.ModelState["ShortName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A conference with the same short name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyLongNameViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conference = new Conference
            {
                Id = 4,
                ShortName = "C4",
                LongName = "Conference 2",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LongName");
            testController.ModelState["LongName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A conference with the same long name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyLeagueIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conference = new Conference { };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Conference_League_LeagueId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyFirstSeasonIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conference = new Conference { };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Conference_Season_FirstSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyLastSeasonIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conference = new Conference { };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Conference_Season_LastSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conference = new Conference { };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnConferenceCreateView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var conference = new Conference { };
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Create(conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeConferenceRepository.AddAsync(conference)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndConferenceFound_ShouldReturnConferenceEditView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = new Conference { };
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(An<int>.Ignored)).Returns(conference);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<ConferenceViewModel>();
            ((ConferenceViewModel)resultModel).Conference.ShouldBe(conference);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var result = await testController.Edit(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenConferenceNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = null;
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(An<int>.Ignored)).Returns(conference);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsConferenceIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateConferenceInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 1;
            var conference = new Conference
            {
                Id = id
            };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualConferenceId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            int id = 0;
            var conference = new Conference
            {
                Id = 1
            };
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndConferenceWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 1;
            var conference = new Conference
            {
                Id = id
            };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            A.CallTo(() => fakeConferenceRepository.ConferenceExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndConferenceWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 1;
            var conference = new Conference
            {
                Id = id
            };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            A.CallTo(() => fakeConferenceRepository.ConferenceExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, conferenceViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyShortNameViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 2;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C3",
                LongName = "Conference 2",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C3",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ShortName");
            testController.ModelState["ShortName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A conference with the same short name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyLongNameViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 2;
            var conference = new Conference
            {
                Id = id,
                ShortName = "C2",
                LongName = "Conference 3",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LongName");
            testController.ModelState["LongName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A conference with the same long name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyLeagueIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 2;
            var conference = new Conference { Id = id };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Conference_League_LeagueId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyFirstSeasonIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 2;
            var conference = new Conference { Id = id };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Conference_Season_FirstSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyLastSeasonIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 2;
            var conference = new Conference { Id = id };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Conference_Season_LastSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            int id = 2;
            var conference = new Conference { Id = id };
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(A<ConferenceViewModel>.Ignored))
                .Returns(Task.FromResult(conference));

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>
            {
                new Conference
                {
                    Id = 1,
                    ShortName = "C1",
                    LongName = "Conference 1",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 2,
                    ShortName = "C2",
                    LongName = "Conference 2",
                    FirstSeasonId = 1920
                },
                new Conference
                {
                    Id = 3,
                    ShortName = "C3",
                    LongName = "Conference 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeConferenceRepository.GetConferencesAsync()).Returns(conferences);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnConferenceEditView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            int id = 1;
            var conference = new Conference
            {
                Id = 1
            };
            var conferenceViewModel = new ConferenceViewModel { Conference = conference };
            var result = await testController.Edit(id, conferenceViewModel);

            // Assert
            A.CallTo(() => fakeConferenceViewModelMapper.MapViewModelToConference(conferenceViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeConferenceRepository.Update(conference)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndConferenceFound_ShouldReturnConferenceDeleteView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();

            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var conferenceViewModel = new ConferenceViewModel { };
            A.CallTo(() => fakeConferenceViewModelMapper.MapConferenceToViewModel(A<Conference>.Ignored))
                .Returns(conferenceViewModel);

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = new Conference { };
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(An<int>.Ignored)).Returns(conference);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConferenceViewModelMapper.MapConferenceToViewModel(conference))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<ConferenceViewModel>();
            resultModel.ShouldBe(conferenceViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            var result = await testController.Delete(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenConferenceNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();

            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = null;
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(An<int>.Ignored)).Returns(conference);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeConferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteConferenceFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeConferenceIndexViewModel = A.Fake<IConferenceIndexViewModel>();
            var fakeConferenceDetailsViewModel = A.Fake<IConferenceDetailsViewModel>();
            var fakeConferenceViewModelMapper = A.Fake<IConferenceViewModelMapper>();
            var fakeConferenceRepository = A.Fake<IConferenceRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferenceController(fakeConferenceIndexViewModel, fakeConferenceDetailsViewModel,
                fakeConferenceViewModelMapper, fakeConferenceRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeConferenceRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
