namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League
{
    public class LeagueViewModel
    {
        private int _firstSeasonYear;
        private int? _lastSeasonYear;

        public LeagueViewModel()
        {
            League = new EldredBrown.ProFootball.Net.Data.Models.League();
        }

        public EldredBrown.ProFootball.Net.Data.Models.League League { get; set; }

        public int Id
        {
            get { return League.Id; }
            set { League.Id = value; }
        }

        public string ShortName
        {
            get { return League.ShortName; }
            set { League.ShortName = value; }
        }

        public string LongName
        {
            get { return League.LongName; }
            set { League.LongName = value; }
        }

        public int FirstSeasonYear
        {
            get
            {
                if (League.FirstSeasonIdNavigation is null)
                {
                    return _firstSeasonYear;
                }
                return League.FirstSeasonIdNavigation.Id;
            }
            set { _firstSeasonYear = value; }
        }

        public int? LastSeasonYear
        {
            get
            {
                if (League.LastSeasonIdNavigation is null)
                {
                    return _lastSeasonYear;
                }
                return League.LastSeasonIdNavigation.Id;
            }
            set { _lastSeasonYear = value; }
        }
    }
}
