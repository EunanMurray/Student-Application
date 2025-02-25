using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentApplication.Pages.Applicants
{
    [AllowAnonymous]
    public class ScholarshipConfirmationModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}