using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EldredBrown.ProFootball.AspNetCore.WebApp.Pages.Leagues
{
    public class CreateModel : PageModel
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly ISharedRepository _sharedRepository;

        public CreateModel(ILeagueRepository leagueRepository, ISharedRepository sharedRepository)
        {
            _leagueRepository = leagueRepository;
            _sharedRepository = sharedRepository;
        }

        [BindProperty]
        public League League { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _leagueRepository.AddAsync(League);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
