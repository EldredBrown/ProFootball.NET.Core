using System.Collections.Generic;
using System.Threading.Tasks;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EldredBrown.ProFootball.AspNetCore.WebApp.Pages.Leagues
{
    public class IndexModel : PageModel
    {
        private readonly ILeagueRepository _leagueRepository;

        public IndexModel(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public IEnumerable<League> Leagues { get;set; }

        public async Task OnGetAsync()
        {
            Leagues = await _leagueRepository.GetLeaguesAsync();
        }
    }
}
