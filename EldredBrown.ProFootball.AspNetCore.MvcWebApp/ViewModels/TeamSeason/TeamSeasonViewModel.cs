using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason
{
    public class TeamSeasonViewModel
    {
        private readonly EldredBrown.ProFootball.Net.Data.Models.TeamSeason _teamSeason;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonDecorator"/> class.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity that will be wrapped inside this object.</param>
        public TeamSeasonViewModel(EldredBrown.ProFootball.Net.Data.Models.TeamSeason teamSeason)
        {
            _teamSeason = teamSeason;
        }

        /// <summary>
        /// Gets or sets the Id of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        public new int Id
        {
            get { return _teamSeason.Id; }
            set { _teamSeason.Id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's team.
        /// </summary>
        [Display(Name = "Team")]
        [Required(ErrorMessage = "Please enter a team name.")]
        public new int TeamId
        {
            get { return _teamSeason.TeamId; }
            set { _teamSeason.TeamId = value; }
        }

        /// <summary>
        /// Gets or sets the year of the wrapped <see cref="TeamSeason"/> entity's season.
        /// </summary>
        [Display(Name = "Season")]
        [Required(ErrorMessage = "Please enter a year.")]
        public new int SeasonYear
        {
            get { return _teamSeason.SeasonId; }
            set { _teamSeason.SeasonId = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's league.
        /// </summary>
        [Display(Name = "League")]
        [Required(ErrorMessage = "Please enter a league name.")]
        public new int LeagueId
        {
            get { return _teamSeason.LeagueId; }
            set { _teamSeason.LeagueId = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's conference.
        /// </summary>
        [Display(Name = "Conference")]
        public new int? ConferenceId
        {
            get { return _teamSeason.ConferenceId; }
            set { _teamSeason.ConferenceId = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's division.
        /// </summary>
        [Display(Name = "Division")]
        public new int? DivisionId
        {
            get { return _teamSeason.DivisionId; }
            set { _teamSeason.DivisionId = value; }
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
        public new decimal? WinningPercentage
        {
            get { return _teamSeason.WinningPercentage; }
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

    }
}
