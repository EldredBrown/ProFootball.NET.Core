using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class TeamSeasonScheduleControllerTest
    {
        [Fact]
        public async Task GetTeamSeasonScheduleProfile_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(
                A<string>.Ignored, A<int>.Ignored)).Throws<Exception>();

            var mapper = A.Fake<IMapper>();

            var testController = new TeamSeasonScheduleController(teamSeasonScheduleRepository, mapper);

            string teamName = "Team";
            int seasonYear = 1920;

            // Act
            var result = await testController.GetTeamSeasonScheduleProfile(teamName, seasonYear);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeamSeasonScheduleProfile_WhenProfileIsEmpty_ShouldReturnNotFoundResult()
        {
            // Arrange
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var teamSeasonScheduleProfile = new List<TeamSeasonOpponentProfile>();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(
                A<string>.Ignored, A<int>.Ignored)).Returns(teamSeasonScheduleProfile);

            var mapper = A.Fake<IMapper>();

            var testController = new TeamSeasonScheduleController(teamSeasonScheduleRepository, mapper);

            string teamName = "Team";
            int seasonYear = 1920;

            // Act
            var result = await testController.GetTeamSeasonScheduleProfile(teamName, seasonYear);

            // Assert
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamName, seasonYear))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetTeamSeasonScheduleProfile_WhenProfileIsNotEmpty_ShouldReturnTeamSeasonOpponentProfileModelArray()
        {
            // Arrange
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var teamSeasonScheduleProfile = new List<TeamSeasonOpponentProfile>
            {
                new TeamSeasonOpponentProfile()
            };
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(
                A<string>.Ignored, A<int>.Ignored)).Returns(teamSeasonScheduleProfile);

            var mapper = A.Fake<IMapper>();
            TeamSeasonOpponentProfileModel[] teamSeasonScheduleProfileModels = {
                new TeamSeasonOpponentProfileModel()
            };
            A.CallTo(() => mapper.Map<TeamSeasonOpponentProfileModel[]>(A<TeamSeasonOpponentProfile>.Ignored))
                .Returns(teamSeasonScheduleProfileModels);

            var testController = new TeamSeasonScheduleController(teamSeasonScheduleRepository, mapper);

            string teamName = "Team";
            int seasonYear = 1920;

            // Act
            var result = await testController.GetTeamSeasonScheduleProfile(teamName, seasonYear);

            // Assert
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamSeasonOpponentProfileModel[]>(teamSeasonScheduleProfile))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamSeasonOpponentProfileModel[]>();
        }

        [Fact]
        public async Task GetTeamSeasonScheduleTotals_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(
                A<string>.Ignored, A<int>.Ignored)).Throws<Exception>();

            var mapper = A.Fake<IMapper>();

            var testController = new TeamSeasonScheduleController(teamSeasonScheduleRepository, mapper);

            string teamName = "Team";
            int seasonYear = 1920;

            // Act
            var result = await testController.GetTeamSeasonScheduleTotals(teamName, seasonYear);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeamSeasonScheduleTotals_WhenExceptionIsNotCaught_ShouldReturnTeamSeasonOpponentTotalsModel()
        {
            // Arrange
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            TeamSeasonScheduleTotals? teamSeasonScheduleTotals = new TeamSeasonScheduleTotals();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(
                A<string>.Ignored, A<int>.Ignored)).Returns(teamSeasonScheduleTotals);

            var mapper = A.Fake<IMapper>();
            var teamSeasonScheduleTotalsModel = new TeamSeasonScheduleTotalsModel();
            A.CallTo(() => mapper.Map<TeamSeasonScheduleTotalsModel>(A<TeamSeasonScheduleTotals>.Ignored))
                .Returns(teamSeasonScheduleTotalsModel);

            var testController = new TeamSeasonScheduleController(teamSeasonScheduleRepository, mapper);

            string teamName = "Team";
            int seasonYear = 1920;

            // Act
            var result = await testController.GetTeamSeasonScheduleTotals(teamName, seasonYear);

            // Assert
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamSeasonScheduleTotalsModel>(teamSeasonScheduleTotals))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamSeasonScheduleTotalsModel>();
        }

        [Fact]
        public async Task GetTeamSeasonScheduleAverages_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamSeasonScheduleAveragesRepository = A.Fake<ITeamSeasonScheduleRepository>();
            A.CallTo(() => teamSeasonScheduleAveragesRepository.GetTeamSeasonScheduleAveragesAsync(
                A<string>.Ignored, A<int>.Ignored)).Throws<Exception>();

            var mapper = A.Fake<IMapper>();

            var testController = new TeamSeasonScheduleController(teamSeasonScheduleAveragesRepository, mapper);

            string teamName = "Team";
            int seasonYear = 1920;

            // Act
            var result = await testController.GetTeamSeasonScheduleAverages(teamName, seasonYear);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeamSeasonScheduleAverages_WhenExceptionIsNotCaught_ShouldReturnTeamSeasonOpponentAveragesModel()
        {
            // Arrange
            var teamSeasonScheduleAveragesRepository = A.Fake<ITeamSeasonScheduleRepository>();
            TeamSeasonScheduleAverages? teamSeasonScheduleAverages = new TeamSeasonScheduleAverages();
            A.CallTo(() => teamSeasonScheduleAveragesRepository.GetTeamSeasonScheduleAveragesAsync(
                A<string>.Ignored, A<int>.Ignored)).Returns(teamSeasonScheduleAverages);

            var mapper = A.Fake<IMapper>();
            var teamSeasonScheduleAveragesModel = new TeamSeasonScheduleAveragesModel();
            A.CallTo(() => mapper.Map<TeamSeasonScheduleAveragesModel>(A<TeamSeasonScheduleAverages>.Ignored))
                .Returns(teamSeasonScheduleAveragesModel);

            var testController = new TeamSeasonScheduleController(teamSeasonScheduleAveragesRepository, mapper);

            string teamName = "Team";
            int seasonYear = 1920;

            // Act
            var result = await testController.GetTeamSeasonScheduleAverages(teamName, seasonYear);

            // Assert
            A.CallTo(() => teamSeasonScheduleAveragesRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamSeasonScheduleAveragesModel>(teamSeasonScheduleAverages))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamSeasonScheduleAveragesModel>();
        }
    }
}
