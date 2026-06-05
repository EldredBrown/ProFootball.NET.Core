using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class LeagueControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnLeagueIndexView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var leagueViewModels = new List<LeagueViewModel>
            {
                new LeagueViewModel { Id = 1 },
                new LeagueViewModel { Id = 2 },
                new LeagueViewModel { Id = 3 },
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapLeagueToViewModel(A<League>.Ignored))
                .ReturnsNextFromSequence(leagueViewModels.ToArray());

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League { Id = 1 },
                new League { Id = 2 },
                new League { Id = 3 },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            foreach (var league in leagues)
            {
                A.CallTo(() => fakeLeagueViewModelMapper.MapLeagueToViewModel(league))
                    .MustHaveHappenedOnceExactly();
            }
            fakeLeagueIndexViewModel.Leagues.ShouldBe(leagueViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeLeagueIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndLeagueFound_ShouldReturnLeagueDetailsView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var leagueViewModel = new LeagueViewModel { };
            A.CallTo(() => fakeLeagueViewModelMapper.MapLeagueToViewModel(An<League>.Ignored))
                .Returns(leagueViewModel);

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var league = new League { };
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(An<int>.Ignored)).Returns(league);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueViewModelMapper.MapLeagueToViewModel(league))
                .MustHaveHappenedOnceExactly();
            fakeLeagueDetailsViewModel.League.ShouldNotBeNull();
            fakeLeagueDetailsViewModel.League.ShouldBeOfType<LeagueViewModel>();
            fakeLeagueDetailsViewModel.League.ShouldBe(leagueViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeLeagueDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var result = await testController.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenLeagueNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(An<int>.Ignored)).Returns(league);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnLeagueCreateView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddLeagueToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var league = new League { };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Create(leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.AddAsync(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var league = new League
            {
                Id = 2,
                ShortName = "L4",
                LongName = "League 4",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Create(leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.AddAsync(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A league with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyShortNameViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var league = new League
            {
                Id = 4,
                ShortName = "L2",
                LongName = "League 4",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Create(leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.AddAsync(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ShortName");
            testController.ModelState["ShortName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A league with the same short name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyLongNameViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var league = new League
            {
                Id = 4,
                ShortName = "L4",
                LongName = "League 2",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Create(leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.AddAsync(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LongName");
            testController.ModelState["LongName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A league with the same long name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyFirstSeasonIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var league = new League { };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_League_Season_FirstSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Create(leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.AddAsync(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyLastSeasonIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var league = new League { };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_League_Season_LastSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Create(leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.AddAsync(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var league = new League { };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Create(leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.AddAsync(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnLeagueCreateView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var league = new League { };
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Create(leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueRepository.AddAsync(league)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndLeagueFound_ShouldReturnLeagueEditView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League { };
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(An<int>.Ignored)).Returns(league);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<LeagueViewModel>();
            ((LeagueViewModel)resultModel).League.ShouldBe(league);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var result = await testController.Edit(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenLeagueNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(An<int>.Ignored)).Returns(league);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateLeagueInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            int id = 1;
            var league = new League
            {
                Id = id
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.Update(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualLeagueId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            int id = 0;
            var league = new League
            {
                Id = 1
            };
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndLeagueWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            int id = 1;
            var league = new League
            {
                Id = id
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => fakeLeagueRepository.LeagueExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.Update(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndLeagueWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            int id = 1;
            var league = new League
            {
                Id = id
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => fakeLeagueRepository.LeagueExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, leagueViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyShortNameViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            int id = 2;
            var league = new League
            {
                Id = id,
                ShortName = "L3",
                LongName = "League 2",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L3",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.Update(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ShortName");
            testController.ModelState["ShortName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A league with the same short name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyLongNameViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            int id = 2;
            var league = new League
            {
                Id = id,
                ShortName = "L2",
                LongName = "League 3",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.Update(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LongName");
            testController.ModelState["LongName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A league with the same long name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyFirstSeasonIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            int id = 2;
            var league = new League { Id = id };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_League_Season_FirstSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.Update(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyLastSeasonIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            int id = 2;
            var league = new League { Id = id };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_League_Season_LastSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.Update(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            int id = 2;
            var league = new League { Id = id };
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(A<LeagueViewModel>.Ignored))
                .Returns(Task.FromResult(league));

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new League
                {
                    Id = 1,
                    ShortName = "L1",
                    LongName = "League 1",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 2,
                    ShortName = "L2",
                    LongName = "League 2",
                    FirstSeasonId = 1920
                },
                new League
                {
                    Id = 3,
                    ShortName = "L3",
                    LongName = "League 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueRepository.Update(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnLeagueEditView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            int id = 1;
            var league = new League
            {
                Id = 1
            };
            var leagueViewModel = new LeagueViewModel { League = league };
            var result = await testController.Edit(id, leagueViewModel);

            // Assert
            A.CallTo(() => fakeLeagueViewModelMapper.MapViewModelToLeague(leagueViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueRepository.Update(league)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndLeagueFound_ShouldReturnLeagueDeleteView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();

            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var leagueViewModel = new LeagueViewModel { };
            A.CallTo(() => fakeLeagueViewModelMapper.MapLeagueToViewModel(A<League>.Ignored))
                .Returns(leagueViewModel);

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League { };
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(An<int>.Ignored)).Returns(league);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueViewModelMapper.MapLeagueToViewModel(league))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<LeagueViewModel>();
            resultModel.ShouldBe(leagueViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            var result = await testController.Delete(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenLeagueNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();

            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(An<int>.Ignored)).Returns(league);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeLeagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteLeagueFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeLeagueIndexViewModel = A.Fake<ILeagueIndexViewModel>();
            var fakeLeagueDetailsViewModel = A.Fake<ILeagueDetailsViewModel>();
            var fakeLeagueViewModelMapper = A.Fake<ILeagueViewModelMapper>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueController(fakeLeagueIndexViewModel, fakeLeagueDetailsViewModel,
                fakeLeagueViewModelMapper, fakeLeagueRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeLeagueRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
