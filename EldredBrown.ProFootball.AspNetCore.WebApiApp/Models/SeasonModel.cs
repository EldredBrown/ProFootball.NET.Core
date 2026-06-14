namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a pro football season.
    /// </summary>
    public class SeasonModel
    {
        /// <summary>
        /// Gets or sets the Id of the current <see cref="SeasonModel"/> object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the number of weeks in the current <see cref="SeasonModel"/> object.
        /// </summary>
        public int NumOfWeeksScheduled { get; set; }

        /// <summary>
        /// Gets or sets the number of weeks completed in the current <see cref="SeasonModel"/> object.
        /// </summary>
        public int NumOfWeeksCompleted { get; set; }
    }
}
