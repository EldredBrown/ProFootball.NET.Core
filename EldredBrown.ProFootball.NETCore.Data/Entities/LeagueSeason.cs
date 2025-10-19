using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a season for a pro football league.
    /// </summary>
    public class LeagueSeason
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="LeagueSeason"/> entity.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="LeagueSeason"/> entity's league.
        /// </summary>
        [DisplayName("League")]
        [StringLength(50)]
        [Required(ErrorMessage = "Please enter a league name.")]
        public string LeagueName { get; set; } = "";

        /// <summary>
        /// Gets or sets the year of the current <see cref="LeagueSeason"/> entity's season.
        /// </summary>
        [DisplayName("Season")]
        [Required(ErrorMessage = "Please enter a year.")]
        [Range(0, int.MaxValue)]
        public int SeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the total games of the current <see cref="LeagueSeason"/> entity.
        /// </summary>
        [DisplayName("Total Games")]
        [DefaultValue(0)]
        public int TotalGames { get; set; }

        /// <summary>
        /// Gets or sets the total points of the current <see cref="LeagueSeason"/> entity.
        /// </summary>
        [DisplayName("Total Points")]
        [DefaultValue(0)]
        public int TotalPoints { get; set; }

        /// <summary>
        /// Gets or sets the average points of the current <see cref="LeagueSeason"/> entity.
        /// </summary>
        [DisplayName("Average Points")]
        public double? AveragePoints { get; set; }
    }
}
