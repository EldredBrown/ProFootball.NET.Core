using EldredBrown.ProFootball.Net.Data.Models;
using System.Threading.Tasks;

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

        public async Task<LeagueSeasonTotals> GetLeagueSeasonTotalsAsync(string leagueName, int seasonYear)
        {
            return await Task.FromResult(_leagueSeasonTotals);
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
