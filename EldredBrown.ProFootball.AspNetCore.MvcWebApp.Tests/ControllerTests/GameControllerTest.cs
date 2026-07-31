using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Moq;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class GameControllerTest
    {
        [Fact]
        public async Task Index_WhenSelectedSeasonYearAndSelectedLeagueNameAndSelectedWeekAreNull_ShouldReturnGameIndexView()
        {
            var defaultSeasonYear = 1922;
            var defaultLeagueName = "NFL";
            int? defaultWeek = null;

            var gameViewModels = new List<GameViewModel>();
            for (int l = 1; l < 4; l++)
            {
                for (int w = 1; w < 4; w++)
                {
                    for (int t = 1; t < 4; t++)
                    {
                        gameViewModels.Add(
                            new GameViewModel
                            {
                                Id = defaultSeasonYear * 1000 + l * 100 + w * 10 + t,
                                SeasonYear = defaultSeasonYear,
                                LeagueName = $"League {l}",
                                Week = w,
                                GuestName = $"Guest {t}",
                                GuestScore = 0,
                                HostName = $"Host {t}",
                                HostScore = 0,
                                IsPlayoff = false,
                                Notes = "Notes"
                            }
                        );
                    }
                }
            }

            var games = new List<Game>();
            for (int l = 1; l < 4; l++)
            {
                for (int w = 1; w < 4; w++)
                {
                    for (int t = 1; t < 4; t++)
                    {
                        games.Add(
                            new Game
                            {
                                Id = defaultSeasonYear * 1000 + l * 100 + w * 10 + t,
                                SeasonYear = defaultSeasonYear,
                                LeagueId = l,
                                Week = w,
                                GuestName = $"Guest {t}",
                                GuestScore = 0,
                                HostName = $"Host {t}",
                                HostScore = 0,
                                IsPlayoff = false,
                                Notes = "Notes"
                            }
                        );
                    }
                }
            }

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(gameViewModels: gameViewModels, games: games);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(defaultSeasonYear);
            testController._gameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._gameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            testController._gameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            testController._gameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            testController._gameIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeasonYear);
            testController._gameIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeasonYear);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(defaultLeagueName);
            testController._gameIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._gameIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._gameIndexViewModel.Leagues.DataValueField.ShouldBe("ShortName");
            testController._gameIndexViewModel.Leagues.DataTextField.ShouldBe("ShortName");
            testController._gameIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeagueName);
            testController._gameIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeagueName);

            // Verify weeks.
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(defaultWeek);
            testController._gameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            testController._gameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            testController._gameIndexViewModel.Weeks.SelectedValue.ShouldBeNull();
            testController._gameIndexViewModel.SelectedWeek.ShouldBeNull();

            // Verify games.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesBySeasonLeagueAndWeekAsync(
                selectedSeason.Year, selectedLeague.Id, null
                )).MustHaveHappenedOnceExactly();
            games = [.. games.Where(g => g.SeasonYear == selectedSeason.Year)];
            foreach (var game in games)
            {
                A.CallTo(() => testController._gameViewModelMapper.MapGameToViewModel(game))
                    .MustHaveHappenedOnceExactly();
            }
            testController._gameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._gameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNotNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var selectedSeasonYear = 1922;
            var defaultLeagueName = "NFL";
            int? defaultWeek = null;

            var gameViewModels = new List<GameViewModel>();
            for (int l = 1; l < 4; l++)
            {
                for (int w = 1; w < 4; w++)
                {
                    for (int t = 1; t < 4; t++)
                    {
                        gameViewModels.Add(
                            new GameViewModel
                            {
                                Id = selectedSeasonYear * 1000 + l * 100 + w * 10 + t,
                                SeasonYear = selectedSeasonYear,
                                LeagueName = $"League {l}",
                                Week = w,
                                GuestName = $"Guest {t}",
                                GuestScore = 0,
                                HostName = $"Host {t}",
                                HostScore = 0,
                                IsPlayoff = false,
                                Notes = "Notes"
                            }
                        );
                    }
                }
            }

            var games = new List<Game>();
            for (int l = 1; l < 4; l++)
            {
                for (int w = 1; w < 4; w++)
                {
                    for (int t = 1; t < 4; t++)
                    {
                        games.Add(
                            new Game
                            {
                                Id = selectedSeasonYear * 1000 + l * 100 + w * 10 + t,
                                SeasonYear = selectedSeasonYear,
                                LeagueId = l,
                                Week = w,
                                GuestName = $"Guest {t}",
                                GuestScore = 0,
                                HostName = $"Host {t}",
                                HostScore = 0,
                                IsPlayoff = false,
                                Notes = "Notes"
                            }
                        );
                    }
                }
            }

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(seasonYear: selectedSeasonYear, gameViewModels: gameViewModels, games: games);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeason.Year);
            testController._gameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._gameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            testController._gameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            testController._gameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            testController._gameIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._gameIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(defaultLeagueName);
            testController._gameIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._gameIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._gameIndexViewModel.Leagues.DataValueField.ShouldBe("ShortName");
            testController._gameIndexViewModel.Leagues.DataTextField.ShouldBe("ShortName");
            testController._gameIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeagueName);
            testController._gameIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeagueName);

            // Verify weeks.
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(defaultWeek);
            testController._gameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            testController._gameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            testController._gameIndexViewModel.Weeks.SelectedValue.ShouldBeNull();
            testController._gameIndexViewModel.SelectedWeek.ShouldBeNull();

            // Verify games.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesBySeasonLeagueAndWeekAsync(
                selectedSeasonYear, selectedLeague.Id, null
                )).MustHaveHappenedOnceExactly();
            games = [.. games.Where(g => g.SeasonYear == selectedSeason.Year)];
            foreach (var game in games)
            {
                A.CallTo(() => testController._gameViewModelMapper.MapGameToViewModel(game))
                    .MustHaveHappenedOnceExactly();
            }
            testController._gameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._gameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedLeagueNameIsNotNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            int? defaultWeek = null;

            var gameViewModels = new List<GameViewModel>();
            for (int w = 1; w < 4; w++)
            {
                for (int t = 1; t < 4; t++)
                {
                    gameViewModels.Add(
                        new GameViewModel
                        {
                            Id = selectedSeasonYear * 1000 + 100 + w * 10 + t,
                            SeasonYear = selectedSeasonYear,
                            LeagueName = $"League {selectedLeagueName}",
                            Week = w,
                            GuestName = $"Guest {t}",
                            GuestScore = 0,
                            HostName = $"Host {t}",
                            HostScore = 0,
                            IsPlayoff = false,
                            Notes = "Notes"
                        }
                    );
                }
            }

            var games = new List<Game>();
            for (int w = 1; w < 4; w++)
            {
                for (int t = 1; t < 4; t++)
                {
                    games.Add(
                        new Game
                        {
                            Id = selectedSeasonYear * 1000 + 100 + w * 10 + t,
                            SeasonYear = selectedSeasonYear,
                            LeagueId = 1,
                            Week = w,
                            GuestName = $"Guest {t}",
                            GuestScore = 0,
                            HostName = $"Host {t}",
                            HostScore = 0,
                            IsPlayoff = false,
                            Notes = "Notes"
                        }
                    );
                }
            }

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName,
                gameViewModels: gameViewModels, games: games                
            );

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeasonYear);
            testController._gameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._gameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            testController._gameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            testController._gameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            testController._gameIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._gameIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(selectedLeagueName);
            testController._gameIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._gameIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._gameIndexViewModel.Leagues.DataValueField.ShouldBe("ShortName");
            testController._gameIndexViewModel.Leagues.DataTextField.ShouldBe("ShortName");
            testController._gameIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._gameIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(defaultWeek);
            testController._gameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            testController._gameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            testController._gameIndexViewModel.Weeks.SelectedValue.ShouldBeNull();
            testController._gameIndexViewModel.SelectedWeek.ShouldBeNull();

            // Verify games.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesBySeasonLeagueAndWeekAsync(
                selectedSeason.Year, selectedLeague.Id, null
                )).MustHaveHappenedOnceExactly();
            games = [.. games.Where(g => g.SeasonYear == selectedSeason.Year && g.LeagueId == selectedLeague.Id)];
            foreach (var game in games)
            {
                A.CallTo(() => testController._gameViewModelMapper.MapGameToViewModel(game))
                    .MustHaveHappenedOnceExactly();
            }
            testController._gameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._gameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedWeekIsNotNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var gameViewModels = new List<GameViewModel>();
            for (int t = 1; t < 4; t++)
            {
                gameViewModels.Add(
                    new GameViewModel
                    {
                        Id = selectedSeasonYear * 1000 + 100 + selectedWeek * 10 + t,
                        SeasonYear = selectedSeasonYear,
                        LeagueName = $"League 1",
                        Week = selectedWeek,
                        GuestName = $"Guest {t}",
                        GuestScore = 0,
                        HostName = $"Host {t}",
                        HostScore = 0,
                        IsPlayoff = false,
                        Notes = "Notes"
                    }
                );
            }

            var games = new List<Game>();
            for (int t = 1; t < 4; t++)
            {
                games.Add(
                    new Game
                    {
                        Id = selectedSeasonYear * 1000 + 100 + selectedWeek * 10 + t,
                        SeasonYear = selectedSeasonYear,
                        LeagueId = 1,
                        Week = selectedWeek,
                        GuestName = $"Guest {t}",
                        GuestScore = 0,
                        HostName = $"Host {t}",
                        HostScore = 0,
                        IsPlayoff = false,
                        Notes = "Notes"
                    }
                );
            }

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, selectedWeek: selectedWeek,
                gameViewModels: gameViewModels, games: games                
            );

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeasonYear);
            testController._gameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._gameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            testController._gameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            testController._gameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            testController._gameIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            testController._gameIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(selectedLeague.ShortName);
            testController._gameIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._gameIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._gameIndexViewModel.Leagues.DataValueField.ShouldBe("ShortName");
            testController._gameIndexViewModel.Leagues.DataTextField.ShouldBe("ShortName");
            testController._gameIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._gameIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);
            testController._gameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            testController._gameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            testController._gameIndexViewModel.Weeks.SelectedValue.ShouldBe(selectedWeek);
            testController._gameIndexViewModel.SelectedWeek.ShouldBe(selectedWeek);

            // Verify games.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesBySeasonLeagueAndWeekAsync(
                selectedSeason.Year, selectedLeague.Id, selectedWeek
                )).MustHaveHappenedOnceExactly();
            games = [.. games.Where(g => g.SeasonYear == selectedSeason.Year && g.LeagueId == selectedLeague.Id && g.Week == selectedWeek)];
            foreach (var game in games)
            {
                A.CallTo(() => testController._gameViewModelMapper.MapGameToViewModel(game))
                    .MustHaveHappenedOnceExactly();
            }
            testController._gameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._gameIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndGameFound_ShouldReturnGameDetailsView()
        {
            // Arrange
            var gameViewModel = new GameViewModel();
            var game = new Game();

            (GameController testController, _, _, _, _, _) = SetUp(gameViewModel: gameViewModel, game: game);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();
            testController._gameDetailsViewModel.Game.ShouldNotBeNull();
            testController._gameDetailsViewModel.Game.ShouldBeOfType<GameViewModel>();
            testController._gameDetailsViewModel.Game.ShouldBe(gameViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._gameDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            int? id = null;
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSelectedLeagueSeasonIsNotNull_ShouldShowGameCreateView()
        {
            // Arrange
            int? selectedLeagueId = 1;
            int? selectedSeasonYear = 1920;

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                int? selectedWeek
            ) = SetUpCreateGet(selectedLeagueId: selectedLeagueId, selectedSeasonYear: selectedSeasonYear);

            // Act
            var result = await testController.Create();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeason.Year);

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(selectedLeague.ShortName);

            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leagues = [.. leagues
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.ShortName)];
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSelectedLeagueSeasonIsNull_ShouldShowGameCreateView()
        {
            // Arrange
            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                int? selectedWeek
            ) = SetUpCreateGet();

            // Act
            var result = await testController.Create();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeason.Year);

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            seasonsSelectList.Items.ShouldBe(orderedSeasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(selectedLeague.ShortName);

            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?>();
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddGameToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var game = new Game();
            var gameViewModel = new GameViewModel { Game = game };
            (GameController testController, _, _, _, _, _) = SetUp(game: game, gameViewModel: gameViewModel);

            // Act
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.AddGameAsync(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int>("SelectedWeek").ShouldBe(game.Week);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionIsCaughtForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            int? selectedWeek = 1;

            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var ex = new DbUpdateException(
                "DbUpdateException",
                new Exception("Violation of PRIMARY KEY constraint 'PK_Game'.")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(selectedWeek: selectedWeek, game: game, games: games, ex: ex);

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.AddGameAsync(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A game with the same Id already exists.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionIsCaughtForGuestNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            int? selectedWeek = 1;

            var game = new Game
            {
                Id = 4,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Game', column 'guest_name'.")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(selectedWeek: selectedWeek, game: game, games: games, ex: ex);

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.AddGameAsync(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("GuestName");
            testController.ModelState["GuestName"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered GuestName is too long.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionIsCaughtForHostNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            int? selectedWeek = 1;

            var game = new Game
            {
                Id = 4,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Game', column 'host_name'.")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(selectedWeek: selectedWeek, game: game, games: games, ex: ex);

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.AddGameAsync(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("HostName");
            testController.ModelState["HostName"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered HostName is too long.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionIsCaughtForUniqueKeyConstraintViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            int? selectedWeek = 1;

            var game = new Game
            {
                Id = 4,
                SeasonYear = 1920,
                LeagueId = 1,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint UQ_Game_Season_League_Week_Teams")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(selectedWeek: selectedWeek, game: game, games: games, ex: ex);

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.AddGameAsync(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Violation of UNIQUE KEY constraint.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionIsCaughtForForeignKeyConstraintConflictOnSeasonYear_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            int? selectedWeek = 1;

            var game = new Game
            {
                Id = 4,
                SeasonYear = 1920,
                LeagueId = 1,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Game_Season_SeasonYear\".")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(selectedWeek: selectedWeek, game: game, games: games, ex: ex);

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.AddGameAsync(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on SeasonYear.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionIsCaughtForForeignKeyConstraintConflictOnLeagueId_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            int? selectedWeek = 1;

            var game = new Game
            {
                Id = 4,
                SeasonYear = 1920,
                LeagueId = 1,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Game_Association_LeagueId\".")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(selectedWeek: selectedWeek, game: game, games: games, ex: ex);

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.AddGameAsync(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueId.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            int? selectedWeek = 1;

            var game = new Game
            {
                Id = 4,
                SeasonYear = 1920,
                LeagueId = 1,
                Week = 1,
                GuestName = "Guest 1",
                GuestScore = 0,
                HostName = "Host 1",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Something else.")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(selectedWeek: selectedWeek, game: game, games: games, ex: ex);

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.AddGameAsync(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGamesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            int? selectedWeek = 1;

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(selectedWeek: selectedWeek);
            testController.ModelState.AddModelError("Name", "Please enter a name.");

            // Act
            var game = new Game();
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel)).MustNotHaveHappened();
            A.CallTo(() => testController._gameService.AddGameAsync(game)).MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            A.CallTo(() => testController._gameRepository.GetGamesAsync()).MustNotHaveHappened();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndGameFound_ShouldReturnGameEditView()
        {
            var game = new Game();
            var gameViewModel = new GameViewModel
            {
                Id = 1,
                SeasonYear = 1920,
                LeagueName = "APFA",
                Week = 1
            };

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> associations, _, Association selectedLeague
            ) = SetUp(game: game, gameViewModel: gameViewModel);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(gameViewModel.SeasonYear);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= gameViewModel.SeasonYear
                    && (l.LastSeasonYearNavigation is null || gameViewModel.SeasonYear <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.Id);
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(gameViewModel.LeagueName);

            // Verify weeks.
            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(gameViewModel.Week);

            var oldGame = testController.HttpContext.Session.GetObject<Game>("OldGame");
            oldGame.ShouldBeEquivalentTo(game);
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<GameViewModel>();
            resultModel.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnGameEditView()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            int? id = null;
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            (GameController testController, List<Season> seasons, _, List<Association> leagues, _, _) = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsGameIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateGameInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var id = 1;
            var game = new Game { Id = id };
            (GameController testController, _, _, _, _, _) = 
                SetUp(seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, game: game);

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.Update(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, A<Game>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualGameId_ShouldReturnNotFound()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            (GameController testController, _, _, _, _, _) = 
                SetUp(seasonYear: selectedSeasonYear, leagueName: selectedLeagueName);

            // Act
            int id = 0;
            var game = new Game { Id = 1 };
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndGameWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            int id = 1;
            var game = new Game { Id = id };

            var gameExists = false;

            var ex = new DbUpdateConcurrencyException();

            (GameController testController, _, _, _, _, _) = 
                SetUp(
                    seasonYear: selectedSeasonYear, leagueName: selectedLeagueName,
                    game: game, gameExists: gameExists, ex: ex
                );

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.Update(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndGameWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            int id = 1;
            var game = new Game { Id = id };

            var gameExists = true;

            var ex = new DbUpdateConcurrencyException();

            (GameController testController, _, _, _, _, _) = 
                SetUp(
                    seasonYear: selectedSeasonYear, leagueName: selectedLeagueName,
                    game: game, gameExists: gameExists, ex: ex
                );

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, gameViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForGuestNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Game', column 'guest_name'.")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> associations, _, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, selectedWeek: selectedWeek,
                games: games, game: game, ex: ex
            );

            // Act
            var id = 1;
            var guestName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                guestName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                Id = 1,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = guestName.ToString(),
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.Update(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, A<Game>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("GuestName");
            testController.ModelState["GuestName"]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. The entered GuestName is too long.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.Id)
                .ToList();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForHostNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Game', column 'host_name'.")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> associations, _, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, selectedWeek: selectedWeek,
                games: games, game: game, ex: ex
            );

            // Act
            var id = 1;
            var guestName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                guestName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                Id = 1,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = guestName.ToString(),
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.Update(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, A<Game>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("HostName");
            testController.ModelState["HostName"]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. The entered HostName is too long.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.Id)
                .ToList();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyConstraintViolation_ShouldRethrowException()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint UQ_Game_Season_League_Week_Teams")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> associations, _, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, selectedWeek: selectedWeek,
                games: games, game: game, ex: ex
            );

            // Act
            var id = 1;
            var guestName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                guestName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                Id = 1,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = guestName.ToString(),
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.Update(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, A<Game>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Violation of UNIQUE KEY constraint.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.Id)
                .ToList();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConstraintConflictOnSeasonYear_ShouldRethrowException()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Game_Season_SeasonYear\".")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> associations, _, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, selectedWeek: selectedWeek,
                games: games, game: game, ex: ex
            );

            // Act
            var id = 1;
            var guestName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                guestName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                Id = 1,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = guestName.ToString(),
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.Update(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, A<Game>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on SeasonYear.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.Id)
                .ToList();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConstraintConflictOnLeagueId_ShouldRethrowException()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Game_Association_LeagueId\".")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> associations, _, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, selectedWeek: selectedWeek,
                games: games, game: game, ex: ex
            );

            // Act
            var id = 1;
            var guestName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                guestName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                Id = 1,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = guestName.ToString(),
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.Update(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, A<Game>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueId.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.Id)
                .ToList();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldRethrowException()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Something else")
            );

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> associations, _, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, selectedWeek: selectedWeek,
                games: games, game: game, ex: ex
            );

            // Act
            var id = 1;
            var guestName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                guestName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                Id = 1,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = guestName.ToString(),
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.Update(game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, A<Game>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. An unexpected error occurred.");

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.Id)
                .ToList();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnGameEditView()
        {
            // Arrange
            var selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var selectedWeek = 1;

            var games = new List<Game>
            {
                new()
                {
                    Id = 1,
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 2,
                    SeasonYear = 1921,
                    LeagueId = 2,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new()
                {
                    Id = 3,
                    SeasonYear = 1922,
                    LeagueId = 3,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };

            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };

            (
                GameController testController,
                List<Season> seasons, Season selectedSeason,
                _, List<Association> leagues, Association selectedLeague
            ) = SetUp(
                seasonYear: selectedSeasonYear, leagueName: selectedLeagueName, selectedWeek: selectedWeek,
                games: games, game: game
            );

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            var id = 1;
            var gameViewModel = new GameViewModel
            {
                Game = game,
                Id = 1,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => testController._gameViewModelMapper.MapViewModelToGame(gameViewModel)).MustNotHaveHappened();
            A.CallTo(() => testController._gameRepository.Update(game)).MustNotHaveHappened();
            A.CallTo(() => testController._gameService.EditGameAsync(game, A<Game>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustNotHaveHappened();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();

            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(
                selectedLeague.Id, selectedSeason.Year
                )).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndGameFound_ShouldReturnGameDeleteView()
        {
            // Arrange
            var gameViewModel = new GameViewModel();
            var game = new Game();
            (GameController testController, _, _, _, _, _) = SetUp(gameViewModel: gameViewModel, game: game);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<GameViewModel>();
            resultModel.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            int? id = null;
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteGameFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => testController._gameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSessionVariablesAndRedirectToIndex()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            int? seasonYear = 1920;
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(seasonYear);
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBeNull();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            int? seasonYear = null;
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task SetSelectedLeagueName_WhenLeagueNameArgIsNeitherNullNorEmpty_ShouldSetSessionVariableAndRedirectToIndex()
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            string? leagueName = "APFA";
            var result = testController.SetSelectedLeagueName(leagueName);

            // Assert
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(leagueName);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task SetSelectedLeagueName_WhenLeagueNameArgIsNullOrEmpty_ShouldReturnBadRequest(
            string? selectedLeagueId
        )
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            var result = testController.SetSelectedLeagueName(selectedLeagueId);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task SetSelectedWeek_ShouldSetSessionVariableAndRedirectToIndex(int? selectedWeek)
        {
            // Arrange
            (GameController testController, _, _, _, _, _) = SetUp();

            // Act
            var result = testController.SetSelectedWeek(selectedWeek);

            // Assert
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        private static (GameController, List<Season>, Season, List<Association>, List<Association>, Association)
            SetUp(
                int? seasonYear = null, string? leagueName = null, int? selectedWeek = null, 
                List<GameViewModel>? gameViewModels = null, GameViewModel? gameViewModel = null,
                List<Game>? games = null, Game? game = null, bool? gameExists = null,
                Exception? ex = null
            )
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            IGameViewModelMapper fakeGameViewModelMapper = SetUpFakeGameViewModelMapper(gameViewModels, gameViewModel, game);
            var fakeGameService = A.Fake<IGameService>();
            (
                ISeasonRepository fakeSeasonRepository, List<Season> seasons, Season selectedSeason
            ) = SetUpFakeSeasonRepository(seasonYear);
            (
                IAssociationRepository fakeAssociationRepository,
                List<Association> associations, List<Association> leagues, Association selectedLeague
            ) = SetUpFakeAssociationRepository(seasons, selectedSeason);
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            IGameRepository fakeGameRepository = SetUpFakeGameRepository(games, game, gameExists);
            ILeagueSeasonRepository fakeLeagueSeasonRepository = SetUpFakeLeagueSeasonRepository(selectedSeason, selectedLeague);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(ex);
            Mock<HttpContext> fakeHttpContext = SetUpHttpContext(selectedSeason.Year, leagueName, selectedLeague, selectedWeek, game);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            return (testController, seasons, selectedSeason, associations, leagues, selectedLeague);
        }

        private static (GameController, List<Season>, Season, List<Association>, Association, int)
            SetUpCreateGet(int? selectedLeagueId = null, int? selectedSeasonYear = null)
        {
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            (ISeasonRepository fakeSeasonRepository, List<Season> seasons, Season selectedSeason) =
                SetUpFakeSeasonRepository();
            (IAssociationRepository fakeAssociationRepository, _, List<Association> leagues, Association selectedLeague) =
                SetUpFakeAssociationRepository(seasons, selectedSeason);
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            ILeagueSeasonRepository fakeLeagueSeasonRepository =
                SetUpFakeLeagueSeasonRepository(selectedLeagueId, selectedSeasonYear);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository();

            int selectedWeek = 2;
            Mock<HttpContext> httpContext = SetUpHttpContext(selectedSeason.Year, selectedLeague.ShortName, selectedWeek);

            var testController = new GameController(
                fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameViewModelMapper, fakeGameService,
                fakeSeasonRepository, fakeAssociationRepository, fakeTeamRepository, fakeGameRepository,
                fakeLeagueSeasonRepository, fakeSharedRepository
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            return (testController, seasons, selectedSeason, leagues, selectedLeague, selectedWeek);
        }

        private static IGameViewModelMapper SetUpFakeGameViewModelMapper(List<GameViewModel>? gameViewModels, GameViewModel? gameViewModel, Game? game)
        {
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            if (gameViewModels is not null)
            {
                A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                    .ReturnsNextFromSequence([.. gameViewModels]);
            }
            if (gameViewModel is not null)
            {
                A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(An<Game>.Ignored)).Returns(gameViewModel);
            }
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);
            return fakeGameViewModelMapper;
        }

        private static IGameRepository SetUpFakeGameRepository(List<Game>? games, Game? game, bool? gameExists)
        {
            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(An<int>.Ignored, An<int?>.Ignored, An<int?>.Ignored))
                .Returns(games);
            A.CallTo(() => fakeGameRepository.GetGamesAsync())
                .Returns(games);
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored))
                .Returns(game);
            if (gameExists.HasValue)
            {
                A.CallTo(() => fakeGameRepository.GameExistsAsync(An<int>.Ignored))
                    .Returns(gameExists.Value);
            }

            return fakeGameRepository;
        }

        private static ILeagueSeasonRepository SetUpFakeLeagueSeasonRepository(Season selectedSeason, Association selectedLeague)
        {
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>();
            for (int l = 1; l < 4; l++)
            {
                for (int y = 1920; y < 1923; y++)
                {
                    leagueSeasons.Add(
                        new LeagueSeason
                        {
                            LeagueId = l,
                            SeasonYear = y,
                            NumOfWeeksScheduled = 3,
                            NumOfWeeksCompleted = 3,
                        }
                    );
                }
            }
            var selectedLeagueSeason = leagueSeasons
                .First(ls => ls.LeagueId == selectedLeague.Id && ls.SeasonYear == selectedSeason.Year);
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(selectedLeagueSeason);
            return fakeLeagueSeasonRepository;
        }

        private static (ISeasonRepository, List<Season>, Season) SetUpFakeSeasonRepository(int? seasonYear = null)
        {
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            var selectedSeasonYear = seasonYear is null ? 1922 : seasonYear;
            var selectedSeason = seasons.First(s => s.Year == selectedSeasonYear);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            return (fakeSeasonRepository, seasons, selectedSeason);
        }

        private static (IAssociationRepository, List<Association>, List<Association>, Association) 
            SetUpFakeAssociationRepository(List<Season> seasons, Season selectedSeason)
        {
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    ParentId = null,
                    FirstSeasonYear = 1920,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1920),
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    ParentId = null,
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    ParentId = 2,
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
                    ParentId = 2,
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.ShortName)
                .ToList();
            var selectedLeague = leagues.First();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns(selectedLeague);

            return (fakeAssociationRepository, associations, leagues, selectedLeague);
        }

        private static ILeagueSeasonRepository SetUpFakeLeagueSeasonRepository(int? selectedLeagueId, int? selectedSeasonYear)
        {
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>();
            for (int l = 1; l < 4; l++)
            {
                for (int y = 1920; y < 1923; y++)
                {
                    leagueSeasons.Add(
                        new LeagueSeason
                        {
                            LeagueId = l,
                            SeasonYear = y,
                            NumOfWeeksScheduled = 3,
                            NumOfWeeksCompleted = 3,
                        }
                    );
                }
            }
            if (selectedLeagueId is not null && selectedSeasonYear is not null)
            {
                var selectedLeagueSeason =
                    leagueSeasons.First(ls => ls.LeagueId == selectedLeagueId && ls.SeasonYear == selectedSeasonYear);
                A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored, An<int>.Ignored))
                    .Returns(selectedLeagueSeason);
            }

            return fakeLeagueSeasonRepository;
        }

        private static ISharedRepository SetUpFakeSharedRepository(Exception? ex = null)
        {
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            if (ex is not null)
            {
                A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);
            }

            return fakeSharedRepository;
        }

        private static Mock<HttpContext> SetUpHttpContext(
            int? selectedSeasonYear, string? leagueName, Association selectedLeague, int? selectedWeek, Game? game = null
        )
        {
            var selectedLeagueName = leagueName is null ? selectedLeague.ShortName : leagueName;
            var httpContext = SetUpHttpContext(selectedSeasonYear, selectedLeagueName, selectedWeek, game);

            return httpContext;
        }

        private static Mock<HttpContext> SetUpHttpContext(
            int? selectedSeasonYear, string? leagueName, int? selectedWeek, Game? game = null
        )
        {
            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);
            fakeSession.SetObject("SelectedLeagueName", leagueName);
            fakeSession.SetObject("SelectedWeek", selectedWeek);
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            return httpContext;
        }
    }
}
