namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference
{
    public class ConferenceViewModel
    {
        private string _leagueName;
        private int _firstSeasonYear;
        private int? _lastSeasonYear;

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
            get
            {
                if (Conference.FirstSeasonIdNavigation is null)
                {
                    return _firstSeasonYear;
                }
                return Conference.FirstSeasonIdNavigation.Id;
            }
            set { _firstSeasonYear = value; }
        }

        public int? LastSeasonYear
        {
            get
            {
                if (Conference.LastSeasonIdNavigation is null)
                {
                    return _lastSeasonYear;
                }
                return Conference.LastSeasonIdNavigation.Id;
            }
            set { _lastSeasonYear = value; }
        }
    }
}
