using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a pro football team.
    /// </summary>
    public class Team
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="Team"/> entity.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="Team"/> entity.
        /// </summary>
        [Required(ErrorMessage = "Please enter a name.")]
        public string Name { get; set; } = "";
    }
}
