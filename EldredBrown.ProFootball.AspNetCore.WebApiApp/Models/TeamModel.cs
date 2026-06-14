using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a pro football team.
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// Gets or sets the Id of the current <see cref="TeamModel"/> object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="TeamModel"/> object.
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
