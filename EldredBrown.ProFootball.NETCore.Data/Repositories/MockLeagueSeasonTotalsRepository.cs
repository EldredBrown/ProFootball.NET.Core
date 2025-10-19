using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
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
