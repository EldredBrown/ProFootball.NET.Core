using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason
{
    public class LeagueSeasonViewModel
    {
        private string _leagueName;
        private int _seasonYear;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeagueSeasonViewModel"/> class.
        /// </summary>
        /// <param name="LeagueSeason">The <see cref="LeagueSeason"/> entity that will be wrapped inside this object.</param>
        public LeagueSeasonViewModel()
        {
            LeagueSeason = new EldredBrown.ProFootball.Net.Data.Models.LeagueSeason();
        }

        public EldredBrown.ProFootball.Net.Data.Models.LeagueSeason LeagueSeason { get; set; }

        /// <summary>
        /// Gets or sets the Id of the wrapped <see cref="LeagueSeason"/> entity.
        /// </summary>
        public int Id
        {
            get { return LeagueSeason.Id; }
            set { LeagueSeason.Id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="LeagueSeason"/> entity's League.
        /// </summary>
        [Display(Name = "League")]
        [Required(ErrorMessage = "Please enter a league name.")]
        public string LeagueName
        {
            get
            {
                if (LeagueSeason.LeagueIdNavigation is null)
                {
                    return _leagueName;
                }
                return LeagueSeason.LeagueIdNavigation.ShortName;
            }
            set { _leagueName = value; }
        }

        /// <summary>
        /// Gets or sets the year of the wrapped <see cref="LeagueSeason"/> entity's season.
        /// </summary>
        [Display(Name = "Season")]
        [Required(ErrorMessage = "Please enter a year.")]
        public int SeasonYear
        {
            get
            {
                if (LeagueSeason.SeasonIdNavigation is null)
                {
                    return _seasonYear;
                }
                return LeagueSeason.SeasonIdNavigation.Id;
            }
            set { _seasonYear = value; }
        }

        /// <summary>
        /// Gets or sets the number of games played by the wrapped <see cref="LeagueSeason"/> entity.
        /// </summary>
        [DefaultValue(0)]
        public int TotalGames
        {
            get { return LeagueSeason.TotalGames; }
            set { LeagueSeason.TotalGames = value; }
        }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="LeagueSeason"/> entity.
        /// </summary>
        [DefaultValue(0)]
        public int TotalPoints
        {
            get { return LeagueSeason.TotalPoints; }
            set { LeagueSeason.TotalPoints = value; }
        }

        /// <summary>
        /// Gets or sets the offensive average of the wrapped <see cref="LeagueSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? AveragePoints
        {
            get { return LeagueSeason.AveragePoints; }
            set { LeagueSeason.AveragePoints = value; }
        }
    }
}
