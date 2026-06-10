using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class TeamSeasonAdminControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnTeamSeasonIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeasonViewModels = new List<TeamSeasonViewModel>
            {
                new TeamSeasonViewModel { Id = 1 },
                new TeamSeasonViewModel { Id = 2 },
                new TeamSeasonViewModel { Id = 3 },
            };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(A<TeamSeason>.Ignored))
                .ReturnsNextFromSequence(teamSeasonViewModels.ToArray());

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1 },
                new TeamSeason { Id = 2 },
                new TeamSeason { Id = 3 },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
            foreach (var teamSeason in teamSeasons)
            {
                A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason))
                    .MustHaveHappenedOnceExactly();
            }
            fakeTeamSeasonIndexViewModel.TeamSeasons.ShouldBe(teamSeasonViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonDetailsView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeasonViewModel = new TeamSeasonViewModel { };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(An<TeamSeason>.Ignored))
                .Returns(teamSeasonViewModel);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeason = new TeamSeason { };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason))
                .MustHaveHappenedOnceExactly();
            fakeTeamSeasonDetailsViewModel.TeamSeason.ShouldNotBeNull();
            fakeTeamSeasonDetailsViewModel.TeamSeason.ShouldBeOfType<TeamSeasonViewModel>();
            fakeTeamSeasonDetailsViewModel.TeamSeason.ShouldBe(teamSeasonViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnTeamSeasonCreateView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddTeamSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeason = new TeamSeason { };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeason = new TeamSeason { Id = 2 };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1 },
                new TeamSeason { Id = 2 },
                new TeamSeason { Id = 3 },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A teamSeason with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeason = new TeamSeason { Id = 4, TeamId = 2, SeasonId = 1920 };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 2, SeasonId = 1920 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1920 },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A teamSeason with the same team name and season year already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Theory]
        [InlineData("FK_TeamSeason_League_LeagueId", "LeagueId")]
        [InlineData("FK_TeamSeason_Season_SeasonId", "SeasonId")]
        [InlineData("FK_TeamSeason_Team_TeamId", "TeamId")]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView(
            string foreignKeyConstraintName, string modelStateKey
        )
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeason = new TeamSeason { };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception($"The INSERT statement conflicted with the FOREIGN KEY constraint \"{foreignKeyConstraintName}\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on {modelStateKey}.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeason = new TeamSeason { };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnTeamSeasonCreateView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            var teamSeason = new TeamSeason { };
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.AddAsync(teamSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonEditView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason { };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<TeamSeasonViewModel>();
            ((TeamSeasonViewModel)resultModel).TeamSeason.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamSeasonIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateTeamSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            int id = 1;
            var teamSeason = new TeamSeason { Id = id };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualTeamSeasonId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int id = 0;
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            int id = 1;
            var teamSeason = new TeamSeason { Id = id };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => fakeTeamSeasonRepository.TeamSeasonExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            int id = 1;
            var teamSeason = new TeamSeason { Id = id };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => fakeTeamSeasonRepository.TeamSeasonExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, teamSeasonViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            int id = 2;
            var teamSeason = new TeamSeason { Id = id, TeamId = 3, SeasonId = 1921 };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1, TeamId = 1, SeasonId = 1920 },
                new TeamSeason { Id = 2, TeamId = 3, SeasonId = 1921 },
                new TeamSeason { Id = 3, TeamId = 3, SeasonId = 1921 },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A teamSeason with the same team name and season year already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Theory]
        [InlineData("FK_TeamSeason_League_LeagueId", "LeagueId")]
        [InlineData("FK_TeamSeason_Season_SeasonId", "SeasonId")]
        [InlineData("FK_TeamSeason_Team_TeamId", "TeamId")]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConflict_ShouldHandleExceptionAndReturnViewForSeason(
            string foreignKeyConstraintName, string modelStateKey
        )
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            int id = 2;
            var teamSeason = new TeamSeason { Id = id };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception($"The UPDATE statement conflicted with the FOREIGN KEY constraint \"{foreignKeyConstraintName}\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on {modelStateKey}.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            int id = 2;
            var teamSeason = new TeamSeason { Id = id };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(teamSeason));

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonRepository.Update(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnTeamSeasonEditView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            int id = 1;
            var teamSeason = new TeamSeason { Id = id };
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };

            // Act
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeTeamSeasonRepository.Update(teamSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonDeleteView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeasonViewModel = new TeamSeasonViewModel { };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(A<TeamSeason>.Ignored))
                .Returns(teamSeasonViewModel);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason { };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<TeamSeasonViewModel>();
            resultModel.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteTeamSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper, fakeTeamSeasonRepository,
                fakeSharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
