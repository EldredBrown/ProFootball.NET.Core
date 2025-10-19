using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Decorators
{
    public class LeagueSeasonDecorator : LeagueSeason
    {
        private readonly LeagueSeason _leagueSeason;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeagueSeasonDecorator"/> class.
        /// </summary>
        /// <param name="leagueSeason">The <see cref="LeagueSeason"/> entity that will be wrapped inside this object.</param>
        public LeagueSeasonDecorator(LeagueSeason leagueSeason)
        {
            _leagueSeason = leagueSeason;
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="LeagueSeason"/> entity's league.
        /// </summary>
        public new string LeagueName
        {
            get { return _leagueSeason.LeagueName; }
            set { _leagueSeason.LeagueName = value; }
        }

        /// <summary>
        /// Gets or sets the year of the wrapped <see cref="LeagueSeason"/> entity's season.
        /// </summary>
        public new int SeasonYear
        {
            get { return _leagueSeason.SeasonYear; }
            set { _leagueSeason.SeasonYear = value; }
        }

        /// <summary>
        /// Gets or sets the total games of the wrapped <see cref="LeagueSeason"/> entity.
        /// </summary>
        public new int TotalGames
        {
            get { return _leagueSeason.TotalGames; }
            set { _leagueSeason.TotalGames = value; }
        }

        /// <summary>
        /// Gets or sets the total points of the wrapped <see cref="LeagueSeason"/> entity.
        /// </summary>
        public new int TotalPoints
        {
            get { return _leagueSeason.TotalPoints; }
            set { _leagueSeason.TotalPoints = value; }
        }

        /// <summary>
        /// Gets or sets the average points of the wrapped <see cref="LeagueSeason"/> entity.
        /// </summary>
        public new double? AveragePoints
        {
            get { return _leagueSeason.AveragePoints; }
            set { _leagueSeason.AveragePoints = value; }
        }

        /// <summary>
        /// Updates the games and points totals of the wrapped <see cref="LeagueSeason"/> entity."
        /// </summary>
        /// <param name="totalGames">The value to be updated to the specified <see cref="LeagueSeason"/> entity's total games.</param>
        /// <param name="totalPoints">The value to be updated to the specified <see cref="LeagueSeason"/> entity's total points.</param>
        public void UpdateGamesAndPoints(int totalGames, int totalPoints)
        {
            _leagueSeason.TotalGames = totalGames;
            _leagueSeason.TotalPoints = totalPoints;

            double? avgPoints = null;
            if (totalGames != 0)
            {
                avgPoints = totalPoints / totalGames;
            }
            _leagueSeason.AveragePoints = avgPoints;
        }
    }
}
