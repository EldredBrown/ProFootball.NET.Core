using System;
using System.Linq;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Exceptions;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.Utilities;

namespace EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy
{
    public class ProcessGameStrategyBase
    {
        private const double _exponent = 2.37;

        protected readonly ITeamRepository _teamRepository;
        protected readonly ITeamSeasonRepository _teamSeasonRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessGameStrategyBase"/> class.
        /// </summary>
        /// <param name="teamRepository">The repository by which team data will be accessed.</param>
        /// <param name="teamSeasonRepository">The repository by which team season data will be accessed.</param>
        public ProcessGameStrategyBase(ITeamRepository teamRepository, ITeamSeasonRepository teamSeasonRepository)
        {
            _teamRepository = teamRepository;
            _teamSeasonRepository = teamSeasonRepository;
        }

        /// <summary>
        /// Processes a <see cref="Game"/> entity into the Teams data store.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public virtual void ProcessGame(Game game)
        {
            Guard.ThrowIfNull(game, $"{GetType()}.{nameof(ProcessGame)}: {nameof(game)}");

            var seasonId = game.SeasonId;
            var teamSeasons = _teamSeasonRepository.GetTeamSeasonsBySeason(seasonId);

            // I am starting this app with the 1920 season, in which the inaugural APFA allowed member teams to play 
            // opponents from outside the association. Eventually, after the APFA became the NFL, the league would 
            // restrict games only to two member teams. I anticipate that, at that time, this app will need to verify 
            // that both teams in a game are members of the NFL, and I will need to uncomment the following code 
            // blocks. Until then, I need to keep the following blocks of code commented out so that non-member 
            // opponents are permitted.

            var guestSeason = teamSeasons.FirstOrDefault(
                ts => _teamRepository.GetTeam(ts.TeamId)?.Name == game.GuestName);
            //if (guestSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.GuestName}' and season year {seasonId}.");
            //}

            var hostSeason = teamSeasons.FirstOrDefault(
                ts => _teamRepository.GetTeam(ts.TeamId)?.Name == game.HostName);
            //if (hostSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.HostName}' and season year {seasonId}.");
            //}

            EditWinLossData(guestSeason, hostSeason, game);
            EditScoringData(guestSeason, hostSeason, game.GuestScore, game.HostScore);

            _teamSeasonRepository.Update(guestSeason);
            _teamSeasonRepository.Update(hostSeason);
        }

        /// <summary>
        /// Processes a <see cref="Game"/> entity into the Teams data store asynchronously.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public virtual async Task ProcessGameAsync(Game game)
        {
            Guard.ThrowIfNull(game, $"{GetType()}.{nameof(ProcessGameAsync)}: {nameof(game)}");

            var seasonId = game.SeasonId;
            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId);

            // I am starting this app with the 1920 season, in which the inaugural APFA allowed member teams to play 
            // opponents from outside the association. Eventually, after the APFA became the NFL, the league would 
            // restrict games only to two member teams. I anticipate that, at that time, this app will need to verify 
            // that both teams in a game are members of the NFL, and I will need to uncomment the following code 
            // blocks. Until then, I need to keep the following blocks of code commented out so that non-member 
            // opponents are permitted.

            var guestSeason = teamSeasons.FirstOrDefault(
                ts => _teamRepository.GetTeamAsync(ts.TeamId).Result?.Name == game.GuestName);
            //if (guestSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.GuestName}' and season year {seasonId}.");
            //}

            var hostSeason = teamSeasons.FirstOrDefault(
                ts => _teamRepository.GetTeamAsync(ts.TeamId).Result?.Name == game.HostName);
            //if (hostSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.HostName}' and season year {seasonId}.");
            //}

            EditWinLossData(guestSeason, hostSeason, game);
            EditScoringData(guestSeason, hostSeason, game.GuestScore, game.HostScore);

            _teamSeasonRepository.Update(guestSeason);
            _teamSeasonRepository.Update(hostSeason);
        }

        /// <summary>
        /// Updates the offensive and defensive averages, factors, and indices for this <see cref="TeamSeason"/> object.
        /// </summary>
        /// <param name="teamSeason"/>
        /// <param name="teamSeasonScheduleAveragePointsFor"/>
        /// <param name="teamSeasonScheduleAveragePointsAgainst"/>
        /// <param name="leagueSeasonAveragePoints"/>
        public void UpdateRankings(
            TeamSeason teamSeason,
            decimal teamSeasonScheduleAveragePointsFor,
            decimal teamSeasonScheduleAveragePointsAgainst,
            decimal leagueSeasonAveragePoints
        )
        {
            var offense = UpdateRankings(teamSeason.PointsFor, teamSeason.Games, teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);
            teamSeason.OffensiveAverage = offense.Average;
            teamSeason.OffensiveFactor = offense.Factor;
            teamSeason.OffensiveIndex = offense.Index;

            var defense = UpdateRankings(teamSeason.PointsAgainst, teamSeason.Games, teamSeasonScheduleAveragePointsFor,
                leagueSeasonAveragePoints);
            teamSeason.DefensiveAverage = defense.Average;
            teamSeason.DefensiveFactor = defense.Factor;
            teamSeason.DefensiveIndex = defense.Index;

            CalculateFinalExpectedWinningPercentage(teamSeason);
        }

        /// <summary>
        /// Calculates and updates this <see cref="TeamSeason"/> model's expected wins and losses.
        /// <param name="teamSeason"/>
        /// </summary>
        protected void CalculateExpectedWinsAndLosses(TeamSeason teamSeason)
        {
            if (teamSeason.Games == 0)
            {
                teamSeason.ExpectedWins = 0m;
                teamSeason.ExpectedLosses = 0m;
                return;
            }

            var expPct = CalculateExpectedWinningPercentage(teamSeason.PointsFor, teamSeason.PointsAgainst);

            if (expPct.HasValue)
            {
                teamSeason.ExpectedWins = expPct.Value * teamSeason.Games;
                teamSeason.ExpectedLosses = (1m - expPct.Value) * teamSeason.Games;
            }
            else
            {
                teamSeason.ExpectedWins = 0;
                teamSeason.ExpectedLosses = 0;
            }
        }

        protected void EditWinLossData(TeamSeason? guestSeason, TeamSeason? hostSeason, Game game)
        {
            UpdateGamesForTeamSeasons(guestSeason, hostSeason);
            UpdateWinsLossesAndTiesForTeamSeasons(guestSeason, hostSeason, game);
        }

        protected virtual void UpdateGamesForTeamSeasons(TeamSeason? guestSeason, TeamSeason? hostSeason)
        {
            throw new NotImplementedException(
                nameof(UpdateGamesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected virtual void UpdateWinsLossesAndTiesForTeamSeasons(
            TeamSeason? guestSeason, TeamSeason? hostSeason, Game game)
        {
            throw new NotImplementedException(
                nameof(UpdateWinsLossesAndTiesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected void EditScoringData(TeamSeason? guestSeason, TeamSeason? hostSeason, int guestScore, int hostScore)
        {
            EditScoringDataForTeamSeason(guestSeason, guestScore, hostScore);
            EditScoringDataForTeamSeason(hostSeason, hostScore, guestScore);
        }

        protected virtual void EditScoringDataForTeamSeason(TeamSeason? teamSeason, int teamScore, int opponentScore)
        {
            throw new NotImplementedException(
                nameof(EditScoringDataForTeamSeason) + " must be implemented in a subclass.");
        }

        private decimal? CalculateExpectedWinningPercentage(decimal pointsFor, decimal pointsAgainst)
        {
            if (pointsFor < 0 || pointsAgainst < 0)
            {
                throw new ArgumentOutOfRangeException($"Points values must be non-negative; got {pointsFor},  {pointsAgainst}.");
            }

            var o = Math.Pow((double)pointsFor, _exponent);
            var d = Math.Pow((double)pointsAgainst, _exponent);
            decimal? result = Divide((decimal)o, (decimal)(o + d));
            return result;
        }

        private void CalculateFinalExpectedWinningPercentage(TeamSeason teamSeason)
        {
            if (teamSeason.OffensiveIndex is null || teamSeason.DefensiveIndex is null)
            {
                teamSeason.FinalExpectedWinningPercentage = null;
                return;
            }

            teamSeason.FinalExpectedWinningPercentage = CalculateExpectedWinningPercentage(
                teamSeason.OffensiveIndex.Value, teamSeason.DefensiveIndex.Value);
        }

        private decimal? Divide(decimal numerator, decimal denominator)
        {
            if (denominator == 0)
            {
                return null;
            }

            return numerator / denominator;
        }

        private TeamSeasonRankingsData UpdateRankings(int points, int games,
            decimal teamSeasonScheduleAveragePoints, decimal leagueSeasonAveragePoints)
        {
            if (games == 0)
            {
                return new TeamSeasonRankingsData(average: null, factor: null, index: null);
            }

            decimal? average = Divide(points, games);
            decimal? factor = Divide(average!.Value, teamSeasonScheduleAveragePoints);
            decimal? index = factor.HasValue
                ? (average.Value + factor.Value * leagueSeasonAveragePoints) / 2m
                : null;

            return new TeamSeasonRankingsData(average, factor, index);
        }

        private class TeamSeasonRankingsData
        {
            public TeamSeasonRankingsData(decimal? average, decimal? factor, decimal? index)
            {
                Average = average;
                Factor = factor;
                Index = index;
            }

            public decimal? Average { get; set; }
            public decimal? Factor { get; set; }
            public decimal? Index { get; set; }
        }
    }
}
