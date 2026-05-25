namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division
{
    public class DivisionViewModel
    {
        private string _leagueName;
        private string _conferenceName;
        private int _firstSeasonYear;
        private int? _lastSeasonYear;

        public DivisionViewModel()
        {
            Division = new EldredBrown.ProFootball.Net.Data.Models.Division();
        }

        public EldredBrown.ProFootball.Net.Data.Models.Division Division { get; set; }

        public int Id
        {
            get { return Division.Id; }
            set { Division.Id = value; }
        }

        public string Name
        {
            get { return Division.Name; }
            set { Division.Name = value; }
        }

        public string LeagueName
        {
            get
            {
                if (Division.LeagueIdNavigation is null)
                {
                    return _leagueName;
                }
                return Division.LeagueIdNavigation.ShortName;
            }
            set { _leagueName = value; }
        }

        public string ConferenceName
        {
            get
            {
                if (Division.ConferenceIdNavigation is null)
                {
                    return _conferenceName;
                }
                return Division.ConferenceIdNavigation.ShortName;
            }
            set { _conferenceName = value; }
        }

        public int FirstSeasonYear
        {
            get
            {
                if (Division.FirstSeasonIdNavigation is null)
                {
                    return _firstSeasonYear;
                }
                return Division.FirstSeasonIdNavigation.Id;
            }
            set { _firstSeasonYear = value; }
        }

        public int? LastSeasonYear
        {
            get
            {
                if (Division.LastSeasonIdNavigation is null)
                {
                    return _lastSeasonYear;
                }
                return Division.LastSeasonIdNavigation.Id;
            }
            set { _lastSeasonYear = value; }
        }
    }
}
