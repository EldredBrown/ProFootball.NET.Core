namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    public class ConferenceViewModel
    {
        private string _leagueName;

        public ConferenceViewModel()
        {
            Conference = new EldredBrown.ProFootball.Net.Data.Models.Conference();
        }

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
                if (Conference.LeagueIdNavigation is null)
                {
                    return _leagueName;
                }
                return Conference.LeagueIdNavigation.ShortName;
            }
            set { _leagueName = value; }
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
