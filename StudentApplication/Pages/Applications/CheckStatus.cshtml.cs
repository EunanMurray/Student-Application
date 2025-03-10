using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Data;
using System.Threading.Tasks;

namespace StudentApplication.Pages.Applications
{
    [Authorize(Roles = "Applicant,ReturningApplicant")]
    public class CheckStatusModel : PageModel
    {
        private readonly PrimaryContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CheckStatusModel> _logger;

        public CheckStatusModel(
            PrimaryContext context,
            UserManager<IdentityUser> userManager,
            ILogger<CheckStatusModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public string ApplicationStatus { get; set; }
        public string ScholarshipStatus { get; set; }
        public string ApplicantName { get; set; }
        public bool HasApplication { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("/Account/Login");
                }

                // Check if the user has an application which can be used to show em the status
                var applicant = await _context.Applicants
                    .Include(a => a.ContactDetail)
                    .FirstOrDefaultAsync(a => a.ContactDetail.Email == user.Email);

                if (applicant == null)
                {
                    HasApplication = false;
                    return Page();
                }

                HasApplication = true;
                ApplicantName = $"{applicant.FirstName} {applicant.LastName}";
                ApplicationStatus = applicant.ApplicationStatus ?? "Not Reviewed";

                // Check to see if they have been offered any scholarships yet
                var scholarshipOffer = await _context.ScholarshipOfferHistories
                    .FirstOrDefaultAsync(s => s.ApplicantID == applicant.ApplicantID);

                ScholarshipStatus = scholarshipOffer?.ResponseStatus ?? "No Offer";

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking application status");
                return RedirectToPage("/Error");
            }
        }
    }
}