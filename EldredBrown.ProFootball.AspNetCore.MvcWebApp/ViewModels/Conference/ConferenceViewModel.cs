using EldredBrown.ProFootball.Net.Data;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    public class ConferenceViewModel
    {
        private readonly ILeagueRepository _leagueRepository = new LeagueRepository(new ProFootballDbContext());

        public ConferenceViewModel()
        {
            Conference = new EldredBrown.ProFootball.Net.Data.Models.Conference();
        }

        /// <summary>
        /// Gets or sets the conference of the current <see cref="ConferenceDetailsViewModel"/> object.
        /// </summary>
        public EldredBrown.ProFootball.Net.Data.Models.Conference Conference { get; set; }

        public int Id
        {
            get { return Conference.Id; }
            set { Conference.Id = value; }
        }

        public string ShortName
        {
            get { return Conference.ShortName; }
            set { Conference.ShortName = value; }
        }

        public string LongName
        {
            get { return Conference.LongName; }
            set { Conference.LongName = value; }
        }

        public string LeagueName
        {
            get
            {
                var parentLeague = _leagueRepository.GetLeague(Conference.LeagueId);
                return parentLeague?.ShortName;
            }
            set
            {
                var parentLeague = _leagueRepository.GetLeagueByShortName(value);
                if (parentLeague is null)
                {
                    Conference.LeagueId = -1;
                }
                else
                {
                    Conference.LeagueId = parentLeague.Id;
                }
            }
        }

        public int FirstSeasonYear
        {
            get { return Conference.FirstSeasonId; }
            set { Conference.FirstSeasonId = value; }
        }


        public int? LastSeasonYear
        {
            get { return Conference.LastSeasonId; }
            set { Conference.LastSeasonId = value; }
        }
    }
}
