using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a season for a pro football team.
    /// </summary>
    public class TeamSeason
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="TeamSeason"/> entity's team.
        /// </summary>
        [Display(Name = "Team")]
        [Required(ErrorMessage = "Please enter a team name.")]
        public string TeamName { get; set; } = "";

        /// <summary>
        /// Gets or sets the year of the current <see cref="TeamSeason"/> entity's season.
        /// </summary>
        [Display(Name = "Season")]
        [Required(ErrorMessage = "Please enter a year.")]
        [Range(0, int.MaxValue)]
        public int SeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="TeamSeason"/> entity's league.
        /// </summary>
        [Display(Name = "League")]
        [Required(ErrorMessage = "Please enter a league name.")]
        public string LeagueName { get; set; } = "";

        /// <summary>
        /// Gets or sets the name of the current <see cref="TeamSeason"/> entity's conference.
        /// </summary>
        [Display(Name = "Conference")]
        public string? ConferenceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="TeamSeason"/> entity's division.
        /// </summary>
        [Display(Name = "Division")]
        public string? DivisionName { get; set; }

        /// <summary>
        /// Gets or sets the number of games played by the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int Games { get; set; }

        /// <summary>
        /// Gets or sets the number of games won by the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets the number of games lost by the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int Losses { get; set; }

        /// <summary>
        /// Gets or sets the number of games tied by the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int Ties { get; set; }

        /// <summary>
        /// Gets or sets the winning percentage of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Winning Pct.")]
        public double? WinningPercentage { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "Points For")]
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the points scored against the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [Display(Name = "Points Against")]
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the Pythagorean wins for the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Display(Name = "Expected Wins")]
        [Range(0d, double.MaxValue)]
        [DefaultValue(0)]
        public double PythagoreanWins { get; set; }

        /// <summary>
        /// Gets or sets the Pythagorean losses for the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        [Display(Name = "Expected Losses")]
        [Range(0d, double.MaxValue)]
        [DefaultValue(0)]
        public double PythagoreanLosses { get; set; }

        /// <summary>
        /// Gets or sets the offensive average of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Offensive Average")]
        [Range(0d, double.MaxValue)]
        public double? OffensiveAverage { get; set; }

        /// <summary>
        /// Gets or sets the offensive factor of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Offensive Factor")]
        [Range(0d, double.MaxValue)]
        public double? OffensiveFactor { get; set; }

        /// <summary>
        /// Gets or sets the offensive index of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Offensive Index")]
        [Range(0d, double.MaxValue)]
        public double? OffensiveIndex { get; set; }

        /// <summary>
        /// Gets or sets the defensive average of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Defensive Average")]
        [Range(0d, double.MaxValue)]
        public double? DefensiveAverage { get; set; }

        /// <summary>
        /// Gets or sets the defensive factor of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Defensive Factor")]
        [Range(0d, double.MaxValue)]
        public double? DefensiveFactor { get; set; }

        /// <summary>
        /// Gets or sets the defensive index of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Defensive Index")]
        [Range(0d, double.MaxValue)]
        public double? DefensiveIndex { get; set; }

        /// <summary>
        /// Gets or sets the final Pythagorean winning percentage of the current <see cref="TeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [Display(Name = "Final Exp. Win Pct.")]
        [Range(0d, double.MaxValue)]
        public double? FinalPythagoreanWinningPercentage { get; set; }
    }
}
