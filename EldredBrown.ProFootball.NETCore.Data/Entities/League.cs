using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a pro football league.
    /// </summary>
    public class League
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="League"/> entity.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the long name of the current <see cref="League"/> entity.
        /// </summary>
        [Display(Name = "Long Name")]
        [StringLength(50)]
        [Required(ErrorMessage = "Please enter a long name.")]
        public string LongName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the short name of the current <see cref="League"/> entity.
        /// </summary>
        [Display(Name = "Short Name")]
        [StringLength(5)]
        [Required(ErrorMessage = "Please enter a short name.")]
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the year of the current <see cref="League"/> entity's first season.
        /// </summary>
        [Display(Name = "First Season")]
        [Required(ErrorMessage = "Please enter a first season.")]
        public int FirstSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="League"/> entity's last season.
        /// </summary>
        [Display(Name = "Last Season")]
        public int? LastSeasonYear { get; set; }
    }
}
