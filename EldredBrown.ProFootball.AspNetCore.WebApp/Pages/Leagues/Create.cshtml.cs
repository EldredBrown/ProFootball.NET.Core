using System.Threading.Tasks;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EldredBrown.ProFootball.AspNetCore.WebApp.Pages.Leagues
{
    public class CreateModel : PageModel
    {
        private readonly IAssociationRepository _associationRepository;
        private readonly ISharedRepository _sharedRepository;

        public CreateModel(IAssociationRepository associationRepository, ISharedRepository sharedRepository)
        {
            _associationRepository = associationRepository;
            _sharedRepository = sharedRepository;
        }

        [BindProperty]
        public Association League { get; set; }

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

            await _associationRepository.AddAsync(League);
            await _sharedRepository.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
