using System.Threading.Tasks;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EldredBrown.ProFootball.AspNetCore.WebApp.Pages.Leagues
{
    public class DetailsModel : PageModel
    {
        private readonly IAssociationRepository _associationRepository;

        public DetailsModel(IAssociationRepository associationRepository)
        {
            _associationRepository = associationRepository;
        }

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
    }
}
