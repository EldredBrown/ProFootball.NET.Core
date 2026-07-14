using System.Threading.Tasks;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EldredBrown.ProFootball.AspNetCore.WebApp.Pages.Leagues
{
    public class DeleteModel : PageModel
    {
        private readonly IAssociationRepository _associationRepository;
        private readonly ISharedRepository _sharedRepository;

        public DeleteModel(IAssociationRepository leagueRepository, ISharedRepository sharedRepository)
        {
            _associationRepository = leagueRepository;
            _sharedRepository = sharedRepository;
        }

        [BindProperty]
        public Association League { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            League = await _associationRepository.GetAssociationAsync(id.Value);

            if (League is null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            League = await _associationRepository.GetAssociationAsync(id.Value);

            if (!(League is null))
            {
                await _associationRepository.DeleteAsync(League.Id);
                await _sharedRepository.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
