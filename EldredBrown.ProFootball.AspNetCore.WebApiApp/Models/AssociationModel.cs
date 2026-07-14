using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a pro football association.
    /// </summary>
    public class AssociationModel
    {
        /// <summary>
        /// Gets or sets the Id of the current <see cref="AssociationModel"/> object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the short name of the current <see cref="AssociationModel"/> object.
        /// </summary>
        [Required]
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the long name of the current <see cref="AssociationModel"/> object.
        /// </summary>
        [Required]
        public string LongName { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="AssociationModel"/> object's first season.
        /// </summary>
        [Required]
        public int FirstSeasonId { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="AssociationModel"/> object's last season.
        /// </summary>
        public int? LastSeasonId { get; set; }
    }
}
