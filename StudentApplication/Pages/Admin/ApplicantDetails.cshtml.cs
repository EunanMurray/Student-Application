using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplication.ViewModels;
using StudentApplicationModel.Data;
using Microsoft.Extensions.Logging;

namespace StudentApplication.Pages.Admin
{
    public class ApplicantDetailsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PrimaryContext _primaryContext;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<ApplicantDetailsModel> _logger;

        public ApplicantDetailsModel(
            UserManager<IdentityUser> userManager,
            PrimaryContext primaryContext,
            ApplicationDbContext applicationDbContext,
            ILogger<ApplicantDetailsModel> logger)
        {
            _userManager = userManager;
            _primaryContext = primaryContext;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public ApplicantViewModel Applicant { get; set; }

        public async Task<IActionResult> OnGetAsync(string name)
        {
            _logger.LogInformation("OnGetAsync started.");

            try
            {
                // Validate the name parameter
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Name parameter is null or empty. Redirecting to error page.");
                    TempData["ErrorMessage"] = "Invalid applicant name.";
                    return RedirectToPage("/Error");
                }

                // Log the name parameter
                _logger.LogInformation($"Fetching details for applicant with name: {name}");

                // Retrieve the applicant from the database
                var applicant = await _primaryContext.Applicants
                    .Include(a => a.Referees)
                    .FirstOrDefaultAsync(a => a.Name == name);

                // Check if the applicant exists
                if (applicant == null)
                {
                    _logger.LogWarning($"No applicant found with name: {name}");
                    TempData["ErrorMessage"] = "Applicant not found.";
                    return RedirectToPage("/Error");
                }

                // Map the applicant to the view model
                _logger.LogInformation($"Mapping applicant {name} to ApplicantViewModel.");
                Applicant = new ApplicantViewModel
                {
                    Name = applicant.Name,
                    DateOfBirth = applicant.DateOfBirth,
                    SportingDetails = applicant.SportPositionOrCategory,
                    SportingAchievements = applicant.SportingAchievements,
                    SportingGoals = applicant.SportingGoals,
                    PastClubs = applicant.PastClubs,
                    HighestCompetitionLevel = applicant.HighestCompetitionLevel,
                    Referees = applicant.Referees.Select(r => new RefereeViewModel
                    {
                        Name = r.Name,
                        TitleOrRole = r.TitleOrRole,
                        PhoneNumber = r.PhoneNumber,
                        Email = r.Email
                    }).ToList()
                };

                _logger.LogInformation($"Applicant details successfully retrieved for {name}.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching details for applicant with name: {name}");
                TempData["ErrorMessage"] = "An error occurred while loading the applicant details.";
                return RedirectToPage("/Error");
            }
        }
    }
}
