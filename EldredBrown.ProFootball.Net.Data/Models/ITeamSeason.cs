namespace EldredBrown.ProFootball.Net.Data.Models
{
    public interface ITeamSeason
    {
        int Id { get; set; }

        int TeamId { get; set; }

        int SeasonId { get; set; }

        int LeagueId { get; set; }

        int? ConferenceId { get; set; }

        int? DivisionId { get; set; }

        int Games { get; set; }

        int Wins { get; set; }

        int Losses { get; set; }

        int Ties { get; set; }

        int PointsFor { get; set; }

        int PointsAgainst { get; set; }

        decimal ExpectedWins { get; set; }

        decimal ExpectedLosses { get; set; }

        decimal? OffensiveAverage { get; set; }

        decimal? OffensiveFactor { get; set; }

        decimal? OffensiveIndex { get; set; }

        decimal? DefensiveAverage { get; set; }

        decimal? DefensiveFactor { get; set; }

        decimal? DefensiveIndex { get; set; }

        decimal? FinalExpectedWinningPercentage { get; set; }

        Team TeamIdNavigation { get; set; }

        Season SeasonIdNavigation { get; set; }

        League LeagueIdNavigation { get; set; }

        Conference? ConferenceIdNavigation { get; set; }

        Division? DivisionIdNavigation { get; set; }

        decimal? WinningPercentage { get; }

        /// <summary>
        /// Calculates and updates this <see cref="TeamSeason"/> model's expected wins and losses.
        /// </summary>
        void CalculateExpectedWinsAndLosses();

        /// <summary>
        /// Updates the offensive and defensive averages, factors, and indices for this <see cref="TeamSeason"/> object.
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
