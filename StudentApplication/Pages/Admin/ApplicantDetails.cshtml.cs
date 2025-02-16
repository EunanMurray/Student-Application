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
using System.ComponentModel.DataAnnotations;
using StudentApplicationModel.Models;

namespace StudentApplication.Pages.Admin
{
    [Authorize(Roles = "Committee Member")]
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

        [BindProperty]
        public ScholarshipDecisionModel ScholarshipDecision { get; set; }

        public class ScholarshipDecisionModel
        {
            [Required]
            public string ScholarshipLevel { get; set; }
            public string Notes { get; set; }
            [Display(Name = "Supporting Documentation")]
            public string SupportingDocs { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string name)
        {
            _logger.LogInformation("OnGetAsync started.");

            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Name parameter is null or empty.");
                    TempData["ErrorMessage"] = "Invalid applicant name.";
                    return RedirectToPage("/Error");
                }

                _logger.LogInformation($"Fetching details for applicant with name: {name}");

                var applicant = await _primaryContext.Applicants
                    .Include(a => a.Referees)
                    .Include(a => a.ApplicantSports)
                        .ThenInclude(a => a.Sport)
                    .FirstOrDefaultAsync(a => a.Name == name);

                if (applicant == null)
                {
                    _logger.LogWarning($"No applicant found with name: {name}");
                    TempData["ErrorMessage"] = "Applicant not found.";
                    return RedirectToPage("/Error");
                }

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

        public async Task<IActionResult> OnPostScholarshipDecisionAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadApplicantData();
                    return Page();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    await LoadApplicantData();
                    return Page();
                }

                var applicantName = RouteData.Values["name"]?.ToString();
                var applicant = await _primaryContext.Applicants
                    .Include(a => a.ApplicantSports)
                    .Include(a => a.Referees)
                    .FirstOrDefaultAsync(a => a.Name == applicantName);

                if (applicant == null)
                {
                    TempData["ErrorMessage"] = "Applicant not found.";
                    return RedirectToPage("/Error");
                }

                if (ScholarshipDecision.ScholarshipLevel == "Reject")
                {
                    applicant.ApplicationStatus = "rejected";
                    await _primaryContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Application has been rejected.";
                    return RedirectToPage("/Admin/ViewApplicantsByCommitteeSport");
                }

                var scholarshipType = await _primaryContext.ScholarshipTypes
                    .FirstOrDefaultAsync(st => st.ScholarshipLevelName == ScholarshipDecision.ScholarshipLevel);

                if (scholarshipType == null)
                {
                    TempData["ErrorMessage"] = "Invalid scholarship level selected.";
                    await LoadApplicantData();
                    return Page();
                }

                var scholarship = new Scholarship
                {
                    ScholarshipTypeID = scholarshipType.ScholarshipTypeID,
                    OtherDetails = $"Notes: {ScholarshipDecision.Notes}\nSupporting Docs: {ScholarshipDecision.SupportingDocs}",
                    hasAccepted = false
                };

                _primaryContext.Scholarships.Add(scholarship);
                await _primaryContext.SaveChangesAsync();

                var offerHistory = new ScholarshipOfferHistory
                {
                    ApplicantID = applicant.ApplicantID,
                    SportID = applicant.ApplicantSports.FirstOrDefault()?.SportID ?? 0,
                    CampusID = applicant.CampusID,
                    ScholarshipID = scholarship.ScholarshipID,
                    OfferDate = DateTime.UtcNow,
                    Stage = "Initial Offer",
                    ResponseStatus = "Pending"
                };

                _primaryContext.ScholarshipOfferHistories.Add(offerHistory);

                applicant.ApplicationStatus = "reviewed";

                await _primaryContext.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{ScholarshipDecision.ScholarshipLevel} scholarship offer has been created successfully.";
                return RedirectToPage("/Admin/ViewApplicantsByCommitteeSport");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scholarship decision");
                TempData["ErrorMessage"] = "An error occurred while processing the scholarship decision.";
                await LoadApplicantData();
                return Page();
            }
        }


        private async Task LoadApplicantData()
        {
            var applicantName = RouteData.Values["name"]?.ToString();
            if (!string.IsNullOrEmpty(applicantName))
            {
                var applicant = await _primaryContext.Applicants
                    .Include(a => a.Referees)
                    .Include(a => a.ApplicantSports)
                        .ThenInclude(a => a.Sport)
                    .FirstOrDefaultAsync(a => a.Name == applicantName);

                if (applicant != null)
                {
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
                }
            }
        }
    }
}