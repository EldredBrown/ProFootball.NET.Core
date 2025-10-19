using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a pro football season.
    /// </summary>
    public class Season
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="Season"/> entity.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="Season"/> entity.
        /// </summary>
        [Required(ErrorMessage = "Please enter a year.")]
        [Range(0, int.MaxValue)]
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the number of weeks scheduled in the current <see cref="Season"/> entity.
        /// </summary>
        [DisplayName("Weeks Scheduled")]
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int NumOfWeeksScheduled { get; set; }

        /// <summary>
        /// Gets or sets the number of weeks completed in the current <see cref="Season"/> entity.
        /// </summary>
        [DisplayName("Weeks Completed")]
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int NumOfWeeksCompleted { get; set; }
    }
}
