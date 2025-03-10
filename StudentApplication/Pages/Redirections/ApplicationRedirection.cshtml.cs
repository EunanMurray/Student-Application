using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Data;
using System.Threading.Tasks;

namespace StudentApplication.Pages.Redirections
{
    [Authorize]
    public class ApplicationRedirectionModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PrimaryContext _context;
        private readonly ILogger<ApplicationRedirectionModel> _logger;

        public ApplicationRedirectionModel(
            UserManager<IdentityUser> userManager,
            PrimaryContext context,
            ILogger<ApplicationRedirectionModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public bool HasExistingApplication { get; set; }
        public bool IsReturningApplicant { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var roles = await _userManager.GetRolesAsync(user);
            IsReturningApplicant = roles.Contains("ReturningApplicant");

            // Check if the user has an applicantion already in the db 
            var applicant = await _context.Applicants
                .Include(a => a.ContactDetail)
                .FirstOrDefaultAsync(a => a.ContactDetail.Email == user.Email);

            HasExistingApplication = applicant != null;

            _logger.LogInformation($"User roles - Returning Applicant: {IsReturningApplicant}, Has Existing Application: {HasExistingApplication}");

            return Page();
        }
    }
}