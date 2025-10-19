using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conferences;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class ConferencesControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnConferencesIndexView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            var conferences = new List<Conference>();
            A.CallTo(() => conferenceRepository.GetConferencesAsync()).Returns(conferences);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => conferenceRepository.GetConferencesAsync()).MustHaveHappenedOnceExactly();
            conferencesIndexViewModel.Conferences.ShouldBe(conferences);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferencesIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndConferenceNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = null;
            A.CallTo(() => conferenceRepository.GetConferenceAsync(A<int>.Ignored)).Returns(conference);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => conferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndConferenceFound_ShouldReturnConferenceDetailsView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = new Conference();
            A.CallTo(() => conferenceRepository.GetConferenceAsync(A<int>.Ignored)).Returns(conference);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => conferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            conferencesDetailsViewModel.Conference.ShouldBe(conference);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conferencesDetailsViewModel);
        }

        [Fact]
        public void CreateGet_ShouldReturnConferenceCreateView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddConferenceToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            var conference = new Conference();

            // Act
            var result = await testController.Create(conference);

            // Assert
            A.CallTo(() => conferenceRepository.AddAsync(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnConferenceCreateView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            var conference = new Conference();

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Create(conference);

            // Assert
            A.CallTo(() => conferenceRepository.AddAsync(conference)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conference);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndConferenceNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = null;
            A.CallTo(() => conferenceRepository.GetConferenceAsync(A<int>.Ignored)).Returns(conference);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => conferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndConferenceFound_ShouldReturnConferenceEditView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = new Conference();
            A.CallTo(() => conferenceRepository.GetConferenceAsync(A<int>.Ignored)).Returns(conference);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => conferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conference);
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualConferenceId_ShouldReturnNotFound()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int id = 0;
            var conference = new Conference
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, conference);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsConferenceIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateConferenceInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int id = 1;
            var conference = new Conference
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, conference);

            // Assert
            A.CallTo(() => conferenceRepository.Update(conference)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsConferenceIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndConferenceWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            A.CallTo(() => conferenceRepository.Update(A<Conference>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => conferenceRepository.ConferenceExists(A<int>.Ignored)).Returns(false);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int id = 1;
            var conference = new Conference
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, conference);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsConferenceIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndConferenceWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            A.CallTo(() => conferenceRepository.Update(A<Conference>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => conferenceRepository.ConferenceExists(A<int>.Ignored)).Returns(true);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int id = 1;
            var conference = new Conference
            {
                ID = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, conference));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsConferenceIdAndModelStateIsNotValid_ShouldReturnConferenceEditView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int id = 1;
            var conference = new Conference
            {
                ID = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, conference);

            // Assert
            A.CallTo(() => conferenceRepository.Update(A<Conference>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conference);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndConferenceNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = null;
            A.CallTo(() => conferenceRepository.GetConferenceAsync(A<int>.Ignored)).Returns(conference);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => conferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndConferenceFound_ShouldReturnConferenceDeleteView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();

            var conferenceRepository = A.Fake<IConferenceRepository>();
            Conference? conference = new Conference();
            A.CallTo(() => conferenceRepository.GetConferenceAsync(A<int>.Ignored)).Returns(conference);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => conferenceRepository.GetConferenceAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(conference);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteConferenceFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var conferencesIndexViewModel = A.Fake<IConferencesIndexViewModel>();
            var conferencesDetailsViewModel = A.Fake<IConferencesDetailsViewModel>();
            var conferenceRepository = A.Fake<IConferenceRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new ConferencesController(conferencesIndexViewModel, conferencesDetailsViewModel,
                conferenceRepository, sharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => conferenceRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
