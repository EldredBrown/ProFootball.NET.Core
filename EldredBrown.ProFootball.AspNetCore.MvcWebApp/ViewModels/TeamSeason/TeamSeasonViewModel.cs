using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason
{
    public class TeamSeasonViewModel
    {
        private string _teamName;
        private int _seasonYear;
        private string _leagueName;
        private string _conferenceName;
        private string _divisionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonViewModel"/> class.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity that will be wrapped inside this object.</param>
        public TeamSeasonViewModel()
        {
            TeamSeason = new EldredBrown.ProFootball.Net.Data.Models.TeamSeason();
        }

        public EldredBrown.ProFootball.Net.Data.Models.TeamSeason TeamSeason { get; set; }

        /// <summary>
        /// Gets or sets the Id of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        public int Id
        {
            get { return TeamSeason.Id; }
            set { TeamSeason.Id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's team.
        /// </summary>
        [Display(Name = "Team")]
        [Required(ErrorMessage = "Please enter a team name.")]
        public string TeamName
        {
            get
            {
                if (TeamSeason.TeamIdNavigation is null)
                {
                    return _teamName;
                }
                return TeamSeason.TeamIdNavigation.Name;
            }
            set { _teamName = value; }
        }

        /// <summary>
        /// Gets or sets the year of the wrapped <see cref="TeamSeason"/> entity's season.
        /// </summary>
        [Display(Name = "Season")]
        [Required(ErrorMessage = "Please enter a year.")]
        public int SeasonYear
        {
            get
            {
                if (TeamSeason.SeasonYearNavigation is null)
                {
                    return _seasonYear;
                }
                return TeamSeason.SeasonYearNavigation.Year;
            }
            set { _seasonYear = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's league.
        /// </summary>
        [Display(Name = "League")]
        [Required(ErrorMessage = "Please enter the name of a league.")]
        public string LeagueName
        {
            get
            {
                if (TeamSeason.LeagueIdNavigation is null)
                {
                    return _leagueName;
                }
                return TeamSeason.LeagueIdNavigation.ShortName;
            }
            set { _leagueName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's conference.
        /// </summary>
        [Display(Name = "Conference")]
        public string ConferenceName
        {
            get
            {
                if (TeamSeason.ConferenceIdNavigation is null)
                {
                    return _conferenceName;
                }
                return TeamSeason.ConferenceIdNavigation.ShortName;
            }
            set { _conferenceName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="TeamSeason"/> entity's division.
        /// </summary>
        [Display(Name = "Division")]
        public string DivisionName
        {
            get
            {
                if (TeamSeason.DivisionIdNavigation is null)
                {
                    return _divisionName;
                }
                return TeamSeason.DivisionIdNavigation.ShortName;
            }
            set { _divisionName = value; }
        }

        /// <summary>
        /// Gets or sets the number of games played by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "G")]
        [DefaultValue(0)]
        public int Games
        {
            get { return TeamSeason.Games; }
            set { TeamSeason.Games = value; }
        }

        /// <summary>
        /// Gets or sets the number of games won by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "W")]
        [DefaultValue(0)]
        public int Wins
        {
            get { return TeamSeason.Wins; }
            set { TeamSeason.Wins = value; }
        }

        /// <summary>
        /// Gets or sets the number of games lost by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "L")]
        [DefaultValue(0)]
        public int Losses
        {
            get { return TeamSeason.Losses; }
            set { TeamSeason.Losses = value; }
        }

        /// <summary>
        /// Gets or sets the number of games tied by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DefaultValue(0)]
        [Display(Name = "T")]
        public int Ties
        {
            get { return TeamSeason.Ties; }
            set { TeamSeason.Ties = value; }
        }

        /// <summary>
        /// Gets or sets the winning percentage of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "W%")]
        public decimal? WinningPercentage
        {
            get { return TeamSeason.WinningPercentage; }
        }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "PF")]
        [DefaultValue(0)]
        public int PointsFor
        {
            get { return TeamSeason.PointsFor; }
            set { TeamSeason.PointsFor = value; }
        }

        /// <summary>
        /// Gets or sets the points scored against the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "PA")]
        [DefaultValue(0)]
        public int PointsAgainst
        {
            get { return TeamSeason.PointsAgainst; }
            set { TeamSeason.PointsAgainst = value; }
        }

        /// <summary>
        /// Gets or sets the expected wins for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Display(Name = "ExpW")]
        [DefaultValue(0)]
        public decimal ExpectedWins
        {
            get { return TeamSeason.ExpectedWins; }
            set { TeamSeason.ExpectedWins = value; }
        }

        /// <summary>
        /// Gets or sets the expected losses for the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Display(Name = "ExpL")]
        [DefaultValue(0)]
        public decimal ExpectedLosses
        {
            get { return TeamSeason.ExpectedLosses; }
            set { TeamSeason.ExpectedLosses = value; }
        }

        /// <summary>
        /// Gets or sets the offensive average of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "OA")]
        public decimal? OffensiveAverage
        {
            get { return TeamSeason.OffensiveAverage; }
            set { TeamSeason.OffensiveAverage = value; }
        }

        /// <summary>
        /// Gets or sets the offensive factor of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "OF")]
        public decimal? OffensiveFactor
        {
            get { return TeamSeason.OffensiveFactor; }
            set { TeamSeason.OffensiveFactor = value; }
        }

        /// <summary>
        /// Gets or sets the offensive index of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "OI")]
        public decimal? OffensiveIndex
        {
            get { return TeamSeason.OffensiveIndex; }
            set { TeamSeason.OffensiveIndex = value; }
        }

        /// <summary>
        /// Gets or sets the defensive average of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "DA")]
        public decimal? DefensiveAverage
        {
            get { return TeamSeason.DefensiveAverage; }
            set { TeamSeason.DefensiveAverage = value; }
        }

        /// <summary>
        /// Gets or sets the defensive factor of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "DF")]
        public decimal? DefensiveFactor
        {
            get { return TeamSeason.DefensiveFactor; }
            set { TeamSeason.DefensiveFactor = value; }
        }

        /// <summary>
        /// Gets or sets the defensive index of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "DI")]
        public decimal? DefensiveIndex
        {
            get { return TeamSeason.DefensiveIndex; }
            set { TeamSeason.DefensiveIndex = value; }
        }

        /// <summary>
        /// Gets or sets the final Pythagorean winning percentage of the wrapped <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "FinExpW%")]
        public decimal? FinalExpectedWinningPercentage
        {
            get { return TeamSeason.FinalExpectedWinningPercentage; }
            set { TeamSeason.FinalExpectedWinningPercentage = value; }
        }
    }
}
