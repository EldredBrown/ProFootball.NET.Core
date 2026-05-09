using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    public class MockLeagueSeasonTotalsRepository : ILeagueSeasonTotalsRepository
    {
        private readonly LeagueSeasonTotals _leagueSeasonTotals;

        public MockLeagueSeasonTotalsRepository()
        {
            _leagueSeasonTotals = InitializeData();
        }

        public LeagueSeasonTotals GetLeagueSeasonTotals(string leagueName, int seasonYear)
        {
            return _leagueSeasonTotals;
        }

        private LeagueSeasonTotals InitializeData()
        {
            return new LeagueSeasonTotals
            {
                TotalGames = 256,
                TotalPoints = 5120
            };
        }
    }
}
