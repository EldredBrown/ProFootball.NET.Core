using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Decorators
{
    public class TeamSeasonDecorator : TeamSeason, ITeamSeasonDecorator
    {
        private const double _exponent = 2.37;

        private readonly TeamSeason _teamSeason;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonDecorator"/> class.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity that will be wrapped inside this object.</param>
        public TeamSeasonDecorator(TeamSeason teamSeason)
        {
            _teamSeason = teamSeason;
        }

        /// <summary>
        /// Gets or sets the Id of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        public int Id
        {
            get { return _teamSeason.Id; }
            set { _teamSeason.Id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's team.
        /// </summary>
        [Display(Name = "Team")]
        [Required(ErrorMessage = "Please enter a team name.")]
        public new string TeamName
        {
            get { return _teamSeason.TeamName; }
            set { _teamSeason.TeamName = value; }
        }

        /// <summary>
        /// Gets or sets the year of the wrapped <see cref="TeamSeason"/> entity's season.
        /// </summary>
        [Display(Name = "Season")]
        [Required(ErrorMessage = "Please enter a year.")]
        public new int SeasonYear
        {
            get { return _teamSeason.SeasonYear; }
            set { _teamSeason.SeasonYear = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's league.
        /// </summary>
        [Display(Name = "League")]
        [Required(ErrorMessage = "Please enter a league name.")]
        public new string LeagueName
        {
            get { return _teamSeason.LeagueName; }
            set { _teamSeason.LeagueName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's conference.
        /// </summary>
        [Display(Name = "Conference")]
        public new string? ConferenceName
        {
            get { return _teamSeason.ConferenceName; }
            set { _teamSeason.ConferenceName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's division.
        /// </summary>
        [Display(Name = "Division")]
        public new string? DivisionName
        {
            get { return _teamSeason.DivisionName; }
            set { _teamSeason.DivisionName = value; }
        }

        /// <summary>
        /// Gets or sets the number of games played by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DefaultValue(0)]
        public new int Games
        {
            get { return _teamSeason.Games; }
            set { _teamSeason.Games = value; }
        }

        /// <summary>
        /// Gets or sets the number of games won by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DefaultValue(0)]
        public new int Wins
        {
            get { return _teamSeason.Wins; }
            set { _teamSeason.Wins = value; }
        }

        /// <summary>
        /// Gets or sets the number of games lost by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DefaultValue(0)]
        public new int Losses
        {
            get { return _teamSeason.Losses; }
            set { _teamSeason.Losses = value; }
        }

        /// <summary>
        /// Gets or sets the number of games tied by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DefaultValue(0)]
        public new int Ties
        {
            get { return _teamSeason.Ties; }
            set { _teamSeason.Ties = value; }
        }

        /// <summary>
        /// Gets or sets the winning percentage of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "W%")]
        public decimal? WinningPercentage
        {
            get
            {
                return Divide(2 * _teamSeason.Wins + _teamSeason.Ties, 2 * _teamSeason.Games);
            }
        }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "Points For")]
        [DefaultValue(0)]
        public new int PointsFor
        {
            get { return _teamSeason.PointsFor; }
            set { _teamSeason.PointsFor = value; }
        }

        /// <summary>
        /// Gets or sets the points scored against the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "Points Against")]
        [DefaultValue(0)]
        public new int PointsAgainst
        {
            get { return _teamSeason.PointsAgainst; }
            set { _teamSeason.PointsAgainst = value; }
        }

        /// <summary>
        /// Gets or sets the expected wins for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Display(Name = "Expected Wins")]
        [DefaultValue(0)]
        public new decimal ExpectedWins
        {
            get { return _teamSeason.ExpectedWins; }
            set { _teamSeason.ExpectedWins = value; }
        }

        /// <summary>
        /// Gets or sets the expected losses for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Display(Name = "Expected Losses")]
        [DefaultValue(0)]
        public new decimal ExpectedLosses
        {
            get { return _teamSeason.ExpectedLosses; }
            set { _teamSeason.ExpectedLosses = value; }
        }

        /// <summary>
        /// Gets or sets the offensive average of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Offensive Average")]
        public new decimal? OffensiveAverage
        {
            get { return _teamSeason.OffensiveAverage; }
            set { _teamSeason.OffensiveAverage = value; }
        }

        /// <summary>
        /// Gets or sets the offensive factor of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Offensive Factor")]
        public new decimal? OffensiveFactor
        {
            get { return _teamSeason.OffensiveFactor; }
            set { _teamSeason.OffensiveFactor = value; }
        }

        /// <summary>
        /// Gets or sets the offensive index of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Offensive Index")]
        public new decimal? OffensiveIndex
        {
            get { return _teamSeason.OffensiveIndex; }
            set { _teamSeason.OffensiveIndex = value; }
        }

        /// <summary>
        /// Gets or sets the defensive average of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Defensive Average")]
        public new decimal? DefensiveAverage
        {
            get { return _teamSeason.DefensiveAverage; }
            set { _teamSeason.DefensiveAverage = value; }
        }

        /// <summary>
        /// Gets or sets the defensive factor of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Defensive Factor")]
        public new decimal? DefensiveFactor
        {
            get { return _teamSeason.DefensiveFactor; }
            set { _teamSeason.DefensiveFactor = value; }
        }

        /// <summary>
        /// Gets or sets the defensive index of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Defensive Index")]
        public new decimal? DefensiveIndex
        {
            get { return _teamSeason.DefensiveIndex; }
            set { _teamSeason.DefensiveIndex = value; }
        }

        /// <summary>
        /// Gets or sets the final Pythagorean winning percentage of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Final Exp. W%")]
        public new decimal? FinalExpectedWinningPercentage
        {
            get { return _teamSeason.FinalExpectedWinningPercentage; }
            set { _teamSeason.FinalExpectedWinningPercentage = value; }
        }

        /// <summary>
        /// Calculates and updates the wrapped <see cref="TeamSeason"/> entity's Pythagorean wins and losses.
        /// </summary>
        public void CalculateExpectedWinsAndLosses()
        {
            if (_teamSeason.Games == 0)
            {
                _teamSeason.ExpectedWins = 0;
                _teamSeason.ExpectedLosses = 0;
                return;
            }

            var expPct = CalculateExpectedWinningPercentage(_teamSeason.PointsFor, _teamSeason.PointsAgainst);

            if (expPct.HasValue)
            {
                _teamSeason.ExpectedWins = expPct.Value * _teamSeason.Games;
                _teamSeason.ExpectedLosses = (1m - expPct.Value) * _teamSeason.Games;
            }
            else
            {
                _teamSeason.ExpectedWins = 0;
                _teamSeason.ExpectedLosses = 0;
            }
        }

        /// <summary>
        /// Updates the offensive and defensive averages, factors, and indices for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        /// <param name="teamSeasonScheduleAveragePointsFor"></param>
        /// <param name="teamSeasonScheduleAveragePointsAgainst"></param>
        /// <param name="leagueSeasonAveragePoints"></param>
        public void UpdateRankings(
            decimal teamSeasonScheduleAveragePointsFor,
            decimal teamSeasonScheduleAveragePointsAgainst,
            decimal leagueSeasonAveragePoints)
        {
            var offense = UpdateRankings(
                _teamSeason.PointsFor, _teamSeason.Games, teamSeasonScheduleAveragePointsAgainst, leagueSeasonAveragePoints);
            OffensiveAverage = offense.Average;
            OffensiveFactor = offense.Factor;
            OffensiveIndex = offense.Index;

            var defense = UpdateRankings(
                _teamSeason.PointsAgainst, _teamSeason.Games, teamSeasonScheduleAveragePointsFor, leagueSeasonAveragePoints);
            DefensiveAverage = defense.Average;
            DefensiveFactor = defense.Factor;
            DefensiveIndex = defense.Index;

            CalculateFinalExpectedWinningPercentage();
        }

        private void CalculateFinalExpectedWinningPercentage()
        {
            if (_teamSeason.OffensiveIndex is null || _teamSeason.DefensiveIndex is null)
            {
                return;
            }

            _teamSeason.FinalExpectedWinningPercentage = CalculateExpectedWinningPercentage(
                _teamSeason.OffensiveIndex.Value, _teamSeason.DefensiveIndex.Value);
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

        private decimal? Divide(decimal numerator, decimal denominator)
        {
            if (denominator == 0)
            {
                return null;
            }

            return numerator / denominator;
        }

        private TeamSeasonRankingsData UpdateRankings(
            int points, int games, decimal teamSeasonScheduleAveragePoints, decimal leagueSeasonAveragePoints)
        {
            if (games == 0)
            {
                return new TeamSeasonRankingsData(null, null, null);
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
