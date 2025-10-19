namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a pro football team.
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="TeamModel"/> object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="TeamModel"/> object.
        /// </summary>
        public string Name { get; set; }
    }
}
