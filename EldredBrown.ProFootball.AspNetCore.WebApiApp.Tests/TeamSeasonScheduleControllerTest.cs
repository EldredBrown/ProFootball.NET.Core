using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class TeamSeasonScheduleControllerTest
    {
        [Fact]
        public async Task GetTeamSeasonScheduleProfile_WhenProfileIsEmpty_ShouldReturnNotFoundResult()
        {
            // Arrange
            List<TeamSeasonOpponentProfile> teamSeasonScheduleProfile = [];
            TeamSeasonScheduleController testController = SetUp(teamSeasonScheduleProfile: teamSeasonScheduleProfile);

            // Act
            var teamId = 1;
            int seasonId = 1920;
            var result = await testController.GetTeamSeasonScheduleProfile(teamId, seasonId);

            // Assert
            A.CallTo(() => testController._teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamId, seasonId))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetTeamSeasonScheduleProfile_WhenProfileIsNotEmpty_ShouldReturnTeamSeasonOpponentProfileModelArray()
        {
            // Arrange
            var teamSeasonScheduleProfile = new List<TeamSeasonOpponentProfile>
            {
                new()
            };
            var teamSeasonScheduleProfileModels = new List<TeamSeasonOpponentProfileModel>
            {
                new()
            };
            TeamSeasonScheduleController testController = SetUp(
                teamSeasonScheduleProfile: teamSeasonScheduleProfile,
                teamSeasonScheduleProfileModels: teamSeasonScheduleProfileModels
            );

            // Act
            var teamId = 1;
            int seasonId = 1920;
            var result = await testController.GetTeamSeasonScheduleProfile(teamId, seasonId);

            // Assert
            A.CallTo(() => testController._teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamId, seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<List<TeamSeasonOpponentProfileModel>>(teamSeasonScheduleProfile))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<List<TeamSeasonOpponentProfileModel>>();
        }

        [Fact]
        public async Task GetTeamSeasonScheduleProfile_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamSeasonScheduleController testController = SetUp(ex: ex);

            // Act
            var teamId = 1;
            int seasonId = 1920;
            var result = await testController.GetTeamSeasonScheduleProfile(teamId, seasonId);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeamSeasonScheduleTotals_WhenNoExceptionIsCaught_ShouldReturnTeamSeasonOpponentTotalsModel()
        {
            // Arrange
            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals();
            var teamSeasonScheduleTotalsModel = new TeamSeasonScheduleTotalsModel();
            TeamSeasonScheduleController testController = SetUp(
                teamSeasonScheduleTotals: teamSeasonScheduleTotals,
                teamSeasonScheduleTotalsModel: teamSeasonScheduleTotalsModel
            );

            // Act
            var teamId = 1;
            int seasonId = 1920;
            var result = await testController.GetTeamSeasonScheduleTotals(teamId, seasonId);

            // Assert
            A.CallTo(() => testController._teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamId, seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamSeasonScheduleTotalsModel>(teamSeasonScheduleTotals))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamSeasonScheduleTotalsModel>();
        }

        [Fact]
        public async Task GetTeamSeasonScheduleTotals_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamSeasonScheduleController testController = SetUp(ex: ex);

            // Act
            var teamId = 1;
            int seasonId = 1920;
            var result = await testController.GetTeamSeasonScheduleTotals(teamId, seasonId);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeamSeasonScheduleAverages_WhenNoExceptionIsCaught_ShouldReturnTeamSeasonOpponentAveragesModel()
        {
            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages();
            var teamSeasonScheduleAveragesModel = new TeamSeasonScheduleAveragesModel();
            TeamSeasonScheduleController testController = SetUp(
                teamSeasonScheduleAverages: teamSeasonScheduleAverages,
                teamSeasonScheduleAveragesModel: teamSeasonScheduleAveragesModel
            );

            // Act
            var teamId = 1;
            int seasonId = 1920;
            var result = await testController.GetTeamSeasonScheduleAverages(teamId, seasonId);

            // Assert
            A.CallTo(() => testController._teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamId, seasonId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamSeasonScheduleAveragesModel>(teamSeasonScheduleAverages))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamSeasonScheduleAveragesModel>();
        }

        [Fact]
        public async Task GetTeamSeasonScheduleAverages_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamSeasonScheduleController testController = SetUp(ex: ex);

            // Act
            var teamId = 1;
            int seasonId = 1920;
            var result = await testController.GetTeamSeasonScheduleAverages(teamId, seasonId);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        private static TeamSeasonScheduleController SetUp(
            List<TeamSeasonOpponentProfile>? teamSeasonScheduleProfile = null,
            TeamSeasonScheduleTotals? teamSeasonScheduleTotals = null,
            TeamSeasonScheduleAverages? teamSeasonScheduleAverages = null,
            List<TeamSeasonOpponentProfileModel>? teamSeasonScheduleProfileModels = null,
            TeamSeasonScheduleTotalsModel? teamSeasonScheduleTotalsModel = null,
            TeamSeasonScheduleAveragesModel? teamSeasonScheduleAveragesModel = null,
            Exception? ex = null
        )
        {
            ITeamSeasonScheduleRepository fakeTeamSeasonScheduleRepository = 
                SetUpFakeTeamSeasonScheduleRepository(
                    teamSeasonScheduleProfile, teamSeasonScheduleTotals, teamSeasonScheduleAverages, ex
                );
            IMapper fakeMapper = 
                SetUpFakeMapper(
                    teamSeasonScheduleProfileModels, teamSeasonScheduleTotalsModel, teamSeasonScheduleAveragesModel
                );
            return new TeamSeasonScheduleController(fakeTeamSeasonScheduleRepository, fakeMapper);
        }

        private static ITeamSeasonScheduleRepository SetUpFakeTeamSeasonScheduleRepository(
            List<TeamSeasonOpponentProfile>? teamSeasonScheduleProfile,
            TeamSeasonScheduleTotals? teamSeasonScheduleTotals,
            TeamSeasonScheduleAverages? teamSeasonScheduleAverages,
            Exception? ex
        )
        {
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            if (ex is null)
            {
                A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(An<int>.Ignored, An<int>.Ignored))
                    .Returns(teamSeasonScheduleProfile!);
                A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                    .Returns(teamSeasonScheduleTotals!);
                A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(An<int>.Ignored, An<int>.Ignored))
                    .Returns(teamSeasonScheduleAverages!);
            }
            else
            {
                A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(An<int>.Ignored, An<int>.Ignored))
                    .Throws(ex);
                A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(An<int>.Ignored, An<int>.Ignored))
                    .Throws(ex);
                A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(An<int>.Ignored, An<int>.Ignored))
                    .Throws(ex);
            }

            return fakeTeamSeasonScheduleRepository;
        }

        private static IMapper SetUpFakeMapper(
            List<TeamSeasonOpponentProfileModel>? teamSeasonScheduleProfileModels,
            TeamSeasonScheduleTotalsModel? teamSeasonScheduleTotalsModel,
            TeamSeasonScheduleAveragesModel? teamSeasonScheduleAveragesModel
        )
        {
            var fakeMapper = A.Fake<IMapper>();
            if (teamSeasonScheduleProfileModels is not null)
            {
                A.CallTo(() => fakeMapper.Map<List<TeamSeasonOpponentProfileModel>>(A<List<TeamSeasonOpponentProfile>>.Ignored))
                    .Returns(teamSeasonScheduleProfileModels);
            }
            if (teamSeasonScheduleTotalsModel is not null)
            {
                A.CallTo(() => fakeMapper.Map<TeamSeasonScheduleTotalsModel>(A<TeamSeasonScheduleTotals>.Ignored))
                    .Returns(teamSeasonScheduleTotalsModel);
            }
            if (teamSeasonScheduleAveragesModel is not null)
            {
                A.CallTo(() => fakeMapper.Map<TeamSeasonScheduleAveragesModel>(A<TeamSeasonScheduleAverages>.Ignored))
                    .Returns(teamSeasonScheduleAveragesModel);
            }

            return fakeMapper;
        }
    }
}
