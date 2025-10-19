using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.NETCore.Services.Tests
{
    public class WeeklyUpdateServiceTest
    {
        private readonly ISeasonRepository _seasonRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILeagueSeasonRepository _leagueSeasonRepository;
        private readonly ILeagueSeasonTotalsRepository _leagueSeasonTotalsRepository;
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ITeamSeasonScheduleRepository _teamSeasonScheduleRepository;
        private readonly ISharedRepository _sharedRepository;

        private readonly WeeklyUpdateService _testService;

        public WeeklyUpdateServiceTest()
        {
            _seasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => _seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns<Season?>(null);

            _gameRepository = A.Fake<IGameRepository>();
            _leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            _leagueSeasonTotalsRepository = A.Fake<ILeagueSeasonTotalsRepository>();
            _teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            _teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            _sharedRepository = A.Fake<ISharedRepository>();

            _testService = new WeeklyUpdateService(_seasonRepository, _gameRepository, _leagueSeasonRepository,
                _leagueSeasonTotalsRepository, _teamSeasonRepository, _teamSeasonScheduleRepository, _sharedRepository);
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsTotalGamesIsNull_ShouldNotUpdateLeagueSeasonGamesAndPoints()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 0
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored,
                A<int>.Ignored)).Returns<LeagueSeason?>(null);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = null,
                TotalPoints = null
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();

            var leagueName = "APFA";
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsTotalPointsIsNull_ShouldNotUpdateLeagueSeasonGamesAndPoints()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 0
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns<LeagueSeason?>(null);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = null
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();

            var leagueName = "APFA";
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsNull_ShouldNotUpdateLeagueSeasonGamesAndPoints()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 0
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns<LeagueSeason?>(null);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();

            var leagueName = "APFA";
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonTotalsTotalGamesAndTotalPointsAreNotNull_ShouldUpdateLeagueSeasonGamesAndPoints()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 0
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            var leagueSeason = new LeagueSeason();
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeason);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();

            var leagueName = "APFA";
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenWeekCountLessThanThree_ShouldNotUpdateRankings()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 2
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            var leagueSeason = new LeagueSeason();
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeason);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();

            var leagueName = "APFA";
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                A<int>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleTotalsScheduleGamesIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 3
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            var leagueSeason = new LeagueSeason
            {
                AveragePoints = null
            };
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeason);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            var teamName = "Team";
            var leagueName = "APFA";
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason
                {
                    TeamName = teamName,
                    SeasonYear = seasonYear,
                    LeagueName = leagueName
                }
            };
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).Returns(teamSeasons);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals
            {
                ScheduleGames = null
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages
            {
                PointsFor = null,
                PointsAgainst = null
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                seasonYear)).Returns<TeamSeasonScheduleAverages>(teamSeasonScheduleAverages);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear))
                .MustNotHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleAveragesPointsForIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 3
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            var leagueSeason = new LeagueSeason
            {
                AveragePoints = null
            };
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeason);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            var teamName = "Team";
            var leagueName = "APFA";
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason
                {
                    TeamName = teamName,
                    SeasonYear = seasonYear,
                    LeagueName = leagueName
                }
            };
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).Returns(teamSeasons);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals
            {
                ScheduleGames = 0
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages
            {
                PointsFor = null,
                PointsAgainst = null
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleAverages);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleAveragesPointsAgainstIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 3
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            var leagueSeason = new LeagueSeason
            {
                AveragePoints = null
            };
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeason);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            var teamName = "Team";
            var leagueName = "APFA";
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason
                {
                    TeamName = teamName,
                    SeasonYear = seasonYear,
                    LeagueName = leagueName
                }
            };
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).Returns(teamSeasons);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals
            {
                ScheduleGames = 0
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages
            {
                PointsFor = 0,
                PointsAgainst = null
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleAverages);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 3
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns<LeagueSeason?>(null);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            var teamName = "Team";
            var leagueName = "APFA";
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason
                {
                    TeamName = teamName,
                    SeasonYear = seasonYear,
                    LeagueName = leagueName
                }
            };
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).Returns(teamSeasons);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals
            {
                ScheduleGames = 0
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages
            {
                PointsFor = 0,
                PointsAgainst = 0
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleAverages);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenLeagueSeasonAveragePointsIsNull_ShouldNotUpdateRankingsForTeamSeason()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 3
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            var leagueSeason = new LeagueSeason
            {
                AveragePoints = null
            };
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeason);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 0,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            var teamName = "Team";
            var leagueName = "APFA";
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason
                {
                    TeamName = teamName,
                    SeasonYear = seasonYear,
                    LeagueName = leagueName
                }
            };
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).Returns(teamSeasons);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals
            {
                ScheduleGames = 0
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages
            {
                PointsFor = 0,
                PointsAgainst = 0
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleAverages);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenTeamSeasonScheduleTotalsScheduleGamesIsNotNullAndWhenTeamSeasonScheduleAveragesPointsForAndPointsAgainstAreNotNullAndLeagueSeasonAveragePointsIsNotNull_ShouldUpdateRankingsForTeamSeason()
        {
            // Arrange
            var seasonYear = 1920;
            var games = new List<Game>
            {
                new Game
                {
                    SeasonYear = seasonYear,
                    Week = 3
                }
            };
            A.CallTo(() => _gameRepository.GetGamesAsync()).Returns(games);

            var leagueSeason = new LeagueSeason
            {
                AveragePoints = 0
            };
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeason);

            var leagueSeasonTotals = new LeagueSeasonTotals
            {
                TotalGames = 1,
                TotalPoints = 0
            };
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(A<string>.Ignored, A<int>.Ignored))
                .Returns(leagueSeasonTotals);

            var teamName = "Team";
            var leagueName = "APFA";
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason
                {
                    TeamName = teamName,
                    SeasonYear = seasonYear,
                    LeagueName = leagueName
                }
            };
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).Returns(teamSeasons);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals
            {
                ScheduleGames = 0
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages
            {
                PointsFor = 0,
                PointsAgainst = 0
            };
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                seasonYear)).Returns(teamSeasonScheduleAverages);

            // Act
            await _testService.RunWeeklyUpdate(seasonYear);

            // Assert
            A.CallTo(() => _seasonRepository.GetSeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _gameRepository.GetGamesAsync()).MustHaveHappened();
            A.CallTo(() => _leagueSeasonRepository.GetLeagueSeasonByLeagueAndSeasonAsync(leagueName, seasonYear))
                .MustHaveHappenedTwiceExactly();
            A.CallTo(() => _leagueSeasonTotalsRepository.GetLeagueSeasonTotals(leagueName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear)).MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear))
                .MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedTwiceExactly();
        }
    }
}
