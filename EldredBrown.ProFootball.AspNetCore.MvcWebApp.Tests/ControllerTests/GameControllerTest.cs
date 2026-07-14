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
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var defaultSeasonYear = 1922;

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModels = new List<GameViewModel>();
            for (int l = 1; l < 4; l++)
            {
                for (int w = 1; w < 4; w++)
                {
                    gameViewModels.Add(
                        new GameViewModel
                        {
                            Id = defaultSeasonYear * 100 + l * 10 + w,
                            SeasonYear = defaultSeasonYear,
                            LeagueName = $"League {l}",
                            Week = w,
                            GuestName = $"Guest {w}",
                            GuestScore = 0,
                            HostName = $"Host {w}",
                            HostScore = 0,
                            IsPlayoff = false,
                            Notes = "Notes"
                        }
                    );
                }
            }
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .ReturnsNextFromSequence([.. gameViewModels]);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var defaultSeason = seasons.First(s => s.Year == defaultSeasonYear);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = 1920,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1920),
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= defaultSeason.Year
                    && (l.LastSeasonYearNavigation is null || defaultSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.ShortName)
                .ToList();
            Association defaultLeague = leagues.First();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns(defaultLeague);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            for (int s = 1920; s < 1923; s++)
            {
                for (int l = 1; l < 4; l++)
                {
                    for (int w = 1; w <= 3; w++)
                    {
                        games.Add(
                            new Game
                            {
                                Id = s * 100 + l * 10 + w,
                                SeasonYear = s,
                                LeagueId = l,
                                Week = w,
                                GuestName = $"Guest {w}",
                                GuestScore = 0,
                                HostName = $"Host {w}",
                                HostScore = 0,
                                IsPlayoff = false,
                                Notes = "Notes"
                            }
                        );
                    }
                }
            }
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(An<int>.Ignored, An<int?>.Ignored, An<int?>.Ignored))
                .Returns(games.Where(g => g.SeasonYear == defaultSeason.Year));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    Id = 1,
                    LeagueId = 1,
                    SeasonYear = 1920,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                },
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(leagueSeasons.First());

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", null);
            fakeSession.SetObject<string>("SelectedLeagueName", null!);
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(defaultSeasonYear);
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeasonYear);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(defaultLeague.ShortName);
            fakeGameIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeGameIndexViewModel.Leagues.DataValueField.ShouldBe("ShortName");
            fakeGameIndexViewModel.Leagues.DataTextField.ShouldBe("ShortName");
            fakeGameIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeague.ShortName);
            fakeGameIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(defaultLeague.Id, defaultSeasonYear))
                .MustHaveHappenedOnceExactly();
            int? selectedWeek = null;
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);
            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            fakeGameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBeNull();
            fakeGameIndexViewModel.SelectedWeek.ShouldBeNull();

            // Verify games.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(defaultLeague.ShortName))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(defaultSeasonYear, defaultLeague.Id, null))
                .MustHaveHappenedOnceExactly();
            games = [.. games.Where(g => g.SeasonYear == defaultSeasonYear)];
            foreach (var game in games)
            {
                A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game))
                    .MustHaveHappenedOnceExactly();
            }
            fakeGameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNotNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var selectedSeasonYear = 1920;

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModels = new List<GameViewModel>();
            for (int l = 1; l < 4; l++)
            {
                for (int w = 1; w < 4; w++)
                {
                    gameViewModels.Add(
                        new GameViewModel
                        {
                            Id = selectedSeasonYear * 100 + l * 10 + w,
                            SeasonYear = selectedSeasonYear,
                            LeagueName = $"League {l}",
                            Week = w,
                            GuestName = $"Guest {w}",
                            GuestScore = 0,
                            HostName = $"Host {w}",
                            HostScore = 0,
                            IsPlayoff = false,
                            Notes = "Notes" 
                        }
                    );
                }
            }
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .ReturnsNextFromSequence([.. gameViewModels]);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            for (int s = 1920; s < 1923; s++)
            {
                for (int l = 1; l < 4; l++)
                {
                    for (int w = 1; w <= 3; w++)
                    {
                        games.Add(
                            new Game
                            {
                                Id = s * 100 + l * 10 + w,
                                SeasonYear = s,
                                LeagueId = l,
                                Week = w,
                                GuestName = $"Guest {w}",
                                GuestScore = 0,
                                HostName = $"Host {w}",
                                HostScore = 0,
                                IsPlayoff = false,
                                Notes = "Notes"
                            }
                        );
                    }
                }
            }
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(An<int>.Ignored, An<int?>.Ignored, An<int?>.Ignored))
                .Returns(games.Where(g => g.SeasonYear == selectedSeasonYear));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    Id = 1,
                    LeagueId = 1,
                    SeasonYear = 1920,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                },
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(leagueSeasons.First());

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeasonYear);
            fakeSession.SetObject<string>("SelectedLeagueName", null!);
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeasonYear);
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(selectedLeague.ShortName);
            fakeGameIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeGameIndexViewModel.Leagues.DataValueField.ShouldBe("ShortName");
            fakeGameIndexViewModel.Leagues.DataTextField.ShouldBe("ShortName");
            fakeGameIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeGameIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeasonYear))
                .MustHaveHappenedOnceExactly();
            int? selectedWeek = null;
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);
            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            fakeGameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBeNull();
            fakeGameIndexViewModel.SelectedWeek.ShouldBeNull();

            // Verify games.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(selectedSeasonYear, selectedLeague.Id, null))
                .MustHaveHappenedOnceExactly();
            games = [.. games.Where(g => g.SeasonYear == selectedSeasonYear)];
            foreach (var game in games)
            {
                A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game))
                    .MustHaveHappenedOnceExactly();
            }
            fakeGameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedLeagueIdIsNotNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var selectedSeasonYear = 1920;
            var selectedLeagueId = 1;

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModels = new List<GameViewModel>();
            for (int w = 1; w < 4; w++)
            {
                gameViewModels.Add(
                    new GameViewModel
                    {
                        Id = selectedSeasonYear * 100 + selectedLeagueId * 10 + w,
                        SeasonYear = selectedSeasonYear,
                        LeagueName = $"League {selectedLeagueId}",
                        Week = w,
                        GuestName = $"Guest {w}",
                        GuestScore = 0,
                        HostName = $"Host {w}",
                        HostScore = 0,
                        IsPlayoff = false,
                        Notes = "Notes"
                    }
                );
            }
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .ReturnsNextFromSequence([.. gameViewModels]);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            for (int s = 1920; s < 1923; s++)
            {
                for (int l = 1; l < 4; l++)
                {
                    for (int w = 1; w <= 3; w++)
                    {
                        games.Add(
                            new Game
                            {
                                Id = s * 100 + l * 10 + w,
                                SeasonYear = s,
                                LeagueId = l,
                                Week = w,
                                GuestName = $"Guest {w}",
                                GuestScore = 0,
                                HostName = $"Host {w}",
                                HostScore = 0,
                                IsPlayoff = false,
                                Notes = "Notes"
                            }
                        );
                    }
                }
            }
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(An<int>.Ignored, An<int?>.Ignored, An<int?>.Ignored))
                .Returns(games.Where(g => g.SeasonYear == selectedSeasonYear && g.LeagueId == selectedLeagueId));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    Id = 1,
                    LeagueId = 1,
                    SeasonYear = 1920,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                },
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(leagueSeasons.First());

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeasonYear);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeasonYear);
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(selectedLeague.ShortName);
            fakeGameIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeGameIndexViewModel.Leagues.DataValueField.ShouldBe("ShortName");
            fakeGameIndexViewModel.Leagues.DataTextField.ShouldBe("ShortName");
            fakeGameIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeGameIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeagueId, selectedSeasonYear))
                .MustHaveHappenedOnceExactly();
            int? selectedWeek = null;
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);
            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            fakeGameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBeNull();
            fakeGameIndexViewModel.SelectedWeek.ShouldBeNull();

            // Verify games.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(selectedSeasonYear, selectedLeagueId, null))
                .MustHaveHappenedOnceExactly();
            games = [.. games.Where(g => g.SeasonYear == selectedSeasonYear && g.LeagueId == selectedLeagueId)];
            foreach (var game in games)
            {
                A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game))
                    .MustHaveHappenedOnceExactly();
            }
            fakeGameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedWeekIsNotNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var selectedSeasonYear = 1920;
            var selectedLeagueId = 1;
            var selectedWeek = 1;

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModels = new List<GameViewModel>
            {
                new()
                {
                    Id = selectedSeasonYear * 100 + selectedLeagueId * 10 + selectedWeek,
                    SeasonYear = selectedSeasonYear,
                    LeagueName = $"League {selectedLeagueId}",
                    Week = selectedWeek,
                    GuestName = $"Guest {selectedWeek}",
                    GuestScore = 0,
                    HostName = $"Host {selectedWeek}",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                }
            };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .ReturnsNextFromSequence([.. gameViewModels]);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            for (int s = 1920; s < 1923; s++)
            {
                for (int l = 1; l < 4; l++)
                {
                    for (int w = 1; w <= 3; w++)
                    {
                        games.Add(
                            new Game
                            {
                                Id = s * 100 + l * 10 + w,
                                SeasonYear = s,
                                LeagueId = l,
                                Week = w,
                                GuestName = $"Guest {w}",
                                GuestScore = 0,
                                HostName = $"Host {w}",
                                HostScore = 0,
                                IsPlayoff = false,
                                Notes = "Notes"
                            }
                        );
                    }
                }
            }
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(An<int>.Ignored, An<int?>.Ignored, An<int?>.Ignored))
                .Returns(games.Where(g => g.SeasonYear == selectedSeasonYear && g.LeagueId == selectedLeagueId && g.Week == selectedWeek));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    Id = 1,
                    LeagueId = 1,
                    SeasonYear = 1920,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                },
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(leagueSeasons.First());

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeasonYear);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            fakeSession.SetObject<int?>("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeasonYear);
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(selectedLeague.ShortName);
            fakeGameIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeGameIndexViewModel.Leagues.DataValueField.ShouldBe("ShortName");
            fakeGameIndexViewModel.Leagues.DataTextField.ShouldBe("ShortName");
            fakeGameIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeGameIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeagueId, selectedSeasonYear))
                .MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);
            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            fakeGameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBe(selectedWeek);
            fakeGameIndexViewModel.SelectedWeek.ShouldBe(selectedWeek);

            // Verify games.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesBySeasonLeagueAndWeekAsync(selectedSeasonYear, selectedLeagueId, selectedWeek))
                .MustHaveHappenedOnceExactly();
            games = [.. games.Where(g => g.SeasonYear == selectedSeasonYear && g.LeagueId == selectedLeagueId && g.Week == selectedWeek)];
            foreach (var game in games)
            {
                A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game))
                    .MustHaveHappenedOnceExactly();
            }
            fakeGameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndGameFound_ShouldReturnGameDetailsView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModel = new GameViewModel { };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(An<Game>.Ignored)).Returns(gameViewModel);

            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var game = new Game { };
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();
            fakeGameDetailsViewModel.Game.ShouldNotBeNull();
            fakeGameDetailsViewModel.Game.ShouldBeOfType<GameViewModel>();
            fakeGameDetailsViewModel.Game.ShouldBe(gameViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            var result = await testController.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSelectedLeagueSeasonIsNotNull_ShouldShowGameCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);

            var selectedWeek = 2;
            fakeSession.SetObject<int?>("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Create();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeason.Year);

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
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
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = null;
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);

            var selectedWeek = 2;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Create();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear").ShouldBe(selectedSeason.Year);

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            var orderedSeasons = seasons.OrderByDescending(s => s.Year).ToList();
            seasonsSelectList.Items.ShouldBe(orderedSeasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(selectedLeague.ShortName);

            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game { };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            fakeSession.GetObject<int>("SelectedWeek").ShouldBe(game.Week);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionIsCaughtForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                },
            };

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A game with the same Id already exists.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Game', column 'guest_name'.")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var guestName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                guestName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = guestName.ToString(),
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("GuestName");
            testController.ModelState["GuestName"].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. The entered GuestName is too long.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Game', column 'host_name'.")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var hostName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                hostName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = "Guest",
                HostName = hostName.ToString()
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("HostName");
            testController.ModelState["HostName"].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. The entered HostName is too long.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
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
                    SeasonYear = 1920,
                    LeagueId = 1,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint UQ_Game_Season_League_Week_Teams")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Violation of UNIQUE KEY constraint.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game
            {
                Id = 4,
                SeasonYear = -1,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Game_Season_SeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on SeasonYear.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game
            {
                Id = 4,
                SeasonYear = 1920,
                LeagueId = -1,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Game_Association_LeagueId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = "Host",
                HostScore = 0
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueId.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game
            {
                Id = 4,
                SeasonYear = 1923,
                LeagueId = 2,
                Week = 2,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var hostName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                hostName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = "Guest",
                HostName = hostName.ToString()
            };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustNotHaveHappened();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustNotHaveHappened();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModel = new GameViewModel
            {
                Id = 1,
                SeasonYear = 1920,
                LeagueName = "APFA",
                Week = 1
            };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored)).Returns(gameViewModel);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason,
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = 2,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = 2,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = new();
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason
            {
                LeagueId = selectedLeague.Id,
                SeasonYear = selectedSeason.Year,
                NumOfWeeksScheduled = 3,
                NumOfWeeksCompleted = 3,
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(gameViewModel.SeasonYear);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
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

            var oldGame = fakeSession.GetObject<Game>("OldGame");
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsGameIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateGameInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 1;
            var game = new Game { Id = id };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.EditGameAsync(game, A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualGameId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", null);
            fakeSession.SetObject("SelectedLeagueName", string.Empty);
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 1;
            var game = new Game { Id = id };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GameExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndGameWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 1;
            var game = new Game { Id = id };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GameExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason,
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = 2,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = 2,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Game', column 'guest_name'.")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.EditGameAsync(game, A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("GuestName");
            testController.ModelState["GuestName"].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. The entered GuestName is too long.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason,
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = 2,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = 2,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Game', column 'host_name'.")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var id = 1;
            var hostName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                hostName.Append('Z');
            }
            var gameViewModel = new GameViewModel
            {
                Game = game,
                Id = 1,
                SeasonYear = selectedSeason.Year,
                LeagueName = selectedLeague.ShortName,
                GuestName = "Guest",
                GuestScore = 0,
                HostName = hostName.ToString(),
                HostScore = 0
            };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.EditGameAsync(game, A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("HostName");
            testController.ModelState["HostName"].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. The entered HostName is too long.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game
            {
                Id = 2,
                SeasonYear = 1922,
                LeagueId = 3,
                Week = 3,
                GuestName = "Guest 3",
                GuestScore = 0,
                HostName = "Host 3",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason,
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = 2,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = 2,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint UQ_Game_Season_League_Week_Teams")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.EditGameAsync(game, A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Violation of UNIQUE KEY constraint.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game
            {
                Id = 2,
                SeasonYear = -1,
                LeagueId = 4,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason,
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = 2,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = 2,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Game_Season_SeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.EditGameAsync(game, A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on SeasonYear.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game
            {
                Id = 2,
                SeasonYear = 1923,
                LeagueId = -1,
                Week = 4,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason,
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = 2,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = 2,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Game_Association_LeagueId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.EditGameAsync(game, A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueId.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason,
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = 2,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = 2,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.EditGameAsync(game, A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. An unexpected error occurred.");

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored)).Returns(game);

            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = selectedSeason.Year,
                    FirstSeasonYearNavigation = selectedSeason,
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = 2,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = 2,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
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
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new()
                {
                    LeagueId = selectedLeague.Id,
                    SeasonYear = selectedSeason.Year,
                    NumOfWeeksScheduled = 3,
                    NumOfWeeksCompleted = 3,
                }
            };
            LeagueSeason? selectedLeagueSeason = leagueSeasons.First();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .Returns(selectedLeagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            int? selectedWeek = null;
            fakeSession.SetObject("SelectedWeek", selectedWeek);
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

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
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.Update(game)).MustNotHaveHappened();
            A.CallTo(() => fakeGameService.EditGameAsync(game, A<Game>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();

            // Verify model state.
            testController.ModelState.IsValid.ShouldBeFalse();

            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons);
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Leagues);
            var leaguesSelectList = (SelectList)testController.ViewBag.Leagues;
            leaguesSelectList.Items.ShouldBe(leagues);
            leaguesSelectList.DataValueField.ShouldBe<string>("ShortName");
            leaguesSelectList.DataTextField.ShouldBe<string>("ShortName");
            leaguesSelectList.SelectedValue.ShouldBe(selectedLeague.ShortName);

            // Verify weeks.
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(selectedLeague.Id, selectedSeason.Year))
                .MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModel = new GameViewModel { };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .Returns(gameViewModel);

            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = new();
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();
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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteGameFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeGameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSessionVariablesAndRedirectToIndex()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var seasonYear = 1920;
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            fakeSession.GetObject<int?>("SelectedSeasonYear").ShouldBe(seasonYear);
            fakeSession.GetObject<int?>("SelectedWeek").ShouldBeNull();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            var result = testController.SetSelectedSeasonYear(null);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task SetSelectedLeagueName_WhenLeagueNameArgIsNeitherNullNorEmpty_ShouldSetSessionVariableAndRedirectToIndex()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var leagueName = "NFL";
            var result = testController.SetSelectedLeagueName(leagueName);

            // Assert
            fakeSession.GetObject<string>("SelectedLeagueName").ShouldBe(leagueName);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SetSelectedLeagueName_WhenLeagueNameArgIsNullOrEmpty_ShouldReturnBadRequest(
            string? selectedLeagueName
        )
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            var result = testController.SetSelectedLeagueName(selectedLeagueName);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public async Task SetSelectedWeek_ShouldSetSessionVariableAndRedirectToIndex(int? selectedWeek)
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeSeasonRepository, fakeAssociationRepository,
                fakeTeamRepository, fakeGameRepository, fakeLeagueSeasonRepository, fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = testController.SetSelectedWeek(selectedWeek);

            // Assert
            fakeSession.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
