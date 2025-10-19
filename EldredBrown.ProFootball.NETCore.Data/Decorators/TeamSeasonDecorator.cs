using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Decorators
{
    public class TeamSeasonDecorator : TeamSeason
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
        /// Gets or sets the ID of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        public new int ID
        {
            get { return _teamSeason.ID; }
            set { _teamSeason.ID = value; }
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
        [Display(Name = "Winning Pct.")]
        public new double? WinningPercentage
        {
            get { return _teamSeason.WinningPercentage; }
            set { _teamSeason.WinningPercentage = value; }
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
        /// Gets or sets the Pythagorean wins for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Display(Name = "Expected Wins")]
        [DefaultValue(0)]
        public new double PythagoreanWins
        {
            get { return _teamSeason.PythagoreanWins; }
            set { _teamSeason.PythagoreanWins = value; }
        }

        /// <summary>
        /// Gets or sets the Pythagorean losses for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Display(Name = "Expected Losses")]
        [DefaultValue(0)]
        public new double PythagoreanLosses
        {
            get { return _teamSeason.PythagoreanLosses; }
            set { _teamSeason.PythagoreanLosses = value; }
        }

        /// <summary>
        /// Gets or sets the offensive average of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Offensive Average")]
        public new double? OffensiveAverage
        {
            get { return _teamSeason.OffensiveAverage; }
            set { _teamSeason.OffensiveAverage = value; }
        }

        /// <summary>
        /// Gets or sets the offensive factor of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Offensive Factor")]
        public new double? OffensiveFactor
        {
            get { return _teamSeason.OffensiveFactor; }
            set { _teamSeason.OffensiveFactor = value; }
        }

        /// <summary>
        /// Gets or sets the offensive index of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Offensive Index")]
        public new double? OffensiveIndex
        {
            get { return _teamSeason.OffensiveIndex; }
            set { _teamSeason.OffensiveIndex = value; }
        }

        /// <summary>
        /// Gets or sets the defensive average of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Defensive Average")]
        public new double? DefensiveAverage
        {
            get { return _teamSeason.DefensiveAverage; }
            set { _teamSeason.DefensiveAverage = value; }
        }

        /// <summary>
        /// Gets or sets the defensive factor of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Defensive Factor")]
        public new double? DefensiveFactor
        {
            get { return _teamSeason.DefensiveFactor; }
            set { _teamSeason.DefensiveFactor = value; }
        }

        /// <summary>
        /// Gets or sets the defensive index of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Defensive Index")]
        public new double? DefensiveIndex
        {
            get { return _teamSeason.DefensiveIndex; }
            set { _teamSeason.DefensiveIndex = value; }
        }

        /// <summary>
        /// Gets or sets the final Pythagorean winning percentage of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Final Exp. Win Pct.")]
        public new double? FinalPythagoreanWinningPercentage
        {
            get { return _teamSeason.FinalPythagoreanWinningPercentage; }
            set { _teamSeason.FinalPythagoreanWinningPercentage = value; }
        }

        /// <summary>
        /// Calculates and updates the wrapped <see cref="TeamSeason"/> entity's Pythagorean wins and losses.
        /// </summary>
        public void CalculatePythagoreanWinsAndLosses()
        {
            var pythPct = CalculatePythagoreanWinningPercentage(_teamSeason.PointsFor, _teamSeason.PointsAgainst);

            if (pythPct.HasValue)
            {
                _teamSeason.PythagoreanWins = pythPct.Value * _teamSeason.Games;
                _teamSeason.PythagoreanLosses = (1d - pythPct.Value) * _teamSeason.Games;
            }
            else
            {
                _teamSeason.PythagoreanWins = 0;
                _teamSeason.PythagoreanLosses = 0;
            }
        }

        /// <summary>
        /// Calculates and updates the wrapped <see cref="TeamSeason"/> entity's winning percentage.
        /// </summary>
        public void CalculateWinningPercentage()
        {
            _teamSeason.WinningPercentage = Divide(2 * _teamSeason.Wins + _teamSeason.Ties, 2 * _teamSeason.Games);
        }

        /// <summary>
        /// Updates the offensive and defensive averages, factors, and indices for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        /// <param name="teamSeasonScheduleAveragePointsFor"></param>
        /// <param name="teamSeasonScheduleAveragePointsAgainst"></param>
        /// <param name="leagueSeasonAveragePoints"></param>
        public void UpdateRankings(double teamSeasonScheduleAveragePointsFor,
            double teamSeasonScheduleAveragePointsAgainst, double leagueSeasonAveragePoints)
        {
            (_teamSeason.OffensiveAverage, _teamSeason.OffensiveFactor, _teamSeason.OffensiveIndex) =
                UpdateRankings(_teamSeason.PointsFor, _teamSeason.Games, teamSeasonScheduleAveragePointsAgainst,
                leagueSeasonAveragePoints);

            (_teamSeason.DefensiveAverage, _teamSeason.DefensiveFactor, _teamSeason.DefensiveIndex) =
                UpdateRankings(_teamSeason.PointsAgainst, _teamSeason.Games, teamSeasonScheduleAveragePointsFor,
                leagueSeasonAveragePoints);

            CalculateFinalPythagoreanWinningPercentage();
        }

        private void CalculateFinalPythagoreanWinningPercentage()
        {
            if (_teamSeason.OffensiveIndex is null || _teamSeason.DefensiveIndex is null)
            {
                return;
            }

            _teamSeason.FinalPythagoreanWinningPercentage = CalculatePythagoreanWinningPercentage(
                _teamSeason.OffensiveIndex.Value, _teamSeason.DefensiveIndex.Value);
        }

        private double? CalculatePythagoreanWinningPercentage(double pointsFor, double pointsAgainst)
        {
            var o = Math.Pow(pointsFor, _exponent);
            var d = Math.Pow(pointsAgainst, _exponent);

            double? result = Divide(o, o + d);

            return result;
        }

        private double? Divide(double numerator, double denominator)
        {
            if (denominator == 0)
            {
                return null;
            }

            return numerator / denominator;
        }

        private (double?, double?, double?) UpdateRankings(
            int points, int games, double teamSeasonScheduleAveragePoints, double leagueSeasonAveragePoints)
        {
            if (games == 0)
            {
                return (null, null, null);
            }

            var average = Divide(points, games);
            var factor = Divide(average!.Value, teamSeasonScheduleAveragePoints);

            double? index = null;
            if (factor.HasValue)
            {
                index = (average.Value + factor.Value * leagueSeasonAveragePoints) / 2d;
            }

            return (average, factor, index);
        }
    }
}
