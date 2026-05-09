using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Decorators
{
    public interface ITeamSeasonDecorator
    {
        /// <summary>
        /// Gets or sets the Id of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's team.
        /// </summary>
        string TeamName { get; set; }

        /// <summary>
        /// Gets or sets the year of the wrapped <see cref="TeamSeason"/> entity's season.
        /// </summary>
        int SeasonYear {  get; set; }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's league.
        /// </summary>
        string LeagueName {  get; set; }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's conference.
        /// </summary>
        string? ConferenceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's division.
        /// </summary>
        string? DivisionName { get; set; }

        /// <summary>
        /// Gets or sets the number of games played by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        int Games {  get; set; }

        /// <summary>
        /// Gets or sets the number of games won by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        int Wins {  get; set; }

        /// <summary>
        /// Gets or sets the number of games lost by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        int Losses {  get; set; }

        /// <summary>
        /// Gets or sets the number of games tied by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        int Ties {  get; set; }

        /// <summary>
        /// Gets or sets the winning percentage of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal? WinningPercentage { get; }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        int PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the points scored against the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        int PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the expected wins for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal ExpectedWins { get; set; }

        /// <summary>
        /// Gets or sets the expected losses for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal ExpectedLosses { get; set; }

        /// <summary>
        /// Gets or sets the offensive average of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal? OffensiveAverage { get; set; }

        /// <summary>
        /// Gets or sets the offensive factor of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal? OffensiveFactor { get; set; }

        /// <summary>
        /// Gets or sets the offensive index of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal? OffensiveIndex { get; set; }

        /// <summary>
        /// Gets or sets the defensive average of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal? DefensiveAverage { get; set; }

        /// <summary>
        /// Gets or sets the defensive factor of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal? DefensiveFactor { get; set; }

        /// <summary>
        /// Gets or sets the defensive index of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal? DefensiveIndex { get; set; }

        /// <summary>
        /// Gets or sets the final Pythagorean winning percentage of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        decimal? FinalExpectedWinningPercentage { get; set; }

        /// <summary>
        /// Calculates and updates the wrapped <see cref="TeamSeason"/> entity's Pythagorean wins and losses.
        /// </summary>
        void CalculateExpectedWinsAndLosses();

        /// <summary>
        /// Updates the offensive and defensive averages, factors, and indices for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        /// <param name="teamSeasonScheduleAveragePointsFor"></param>
        /// <param name="teamSeasonScheduleAveragePointsAgainst"></param>
        /// <param name="leagueSeasonAveragePoints"></param>
        void UpdateRankings(
            decimal teamSeasonScheduleAveragePointsFor,
            decimal teamSeasonScheduleAveragePointsAgainst,
            decimal leagueSeasonAveragePoints);
    }
}
