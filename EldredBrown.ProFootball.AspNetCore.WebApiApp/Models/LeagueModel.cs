namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a pro football league.
    /// </summary>
    public class LeagueModel
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="LeagueModel"/> object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the long name of the current <see cref="LeagueModel"/> object.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// Gets or sets the short name of the current <see cref="LeagueModel"/> object.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="LeagueModel"/> object's first season.
        /// </summary>
        public int FirstSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="LeagueModel"/> object's last season.
        /// </summary>
        public int? LastSeasonYear { get; set; }
    }
}
