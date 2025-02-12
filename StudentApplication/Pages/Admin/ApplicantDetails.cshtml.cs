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

        //Scholarship Review Function for Adminn
        [BindProperty]
        public ScholarshipReviewModel ScholarshipReview { get; set; }

        public class ScholarshipReviewModel
        {
            public string ScholarshipLevel { get; set; }
            public string ReviewNotes { get; set; }
            [Display(Name = "Supporting Documentation")]
            public string SupportingDocs { get; set; }
        }

        public async Task<IActionResult> OnPostScholarshipDecisionAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return Page();
                }

                var isCommitteeMember = await _userManager.IsInRoleAsync(currentUser, "Committee Member");
                if (!isCommitteeMember)
                {
                    TempData["ErrorMessage"] = "Unauthorized access.";
                    return Page();
                }

                var applicantName = RouteData.Values["name"]?.ToString();
                if (string.IsNullOrEmpty(applicantName))
                {
                    TempData["ErrorMessage"] = "Applicant name is missing.";
                    return Page();
                }

                var applicant = await _primaryContext.Applicants
                    .FirstOrDefaultAsync(a => a.Name == applicantName);

                if (applicant == null)
                {
                    TempData["ErrorMessage"] = "Applicant not found.";
                    return Page();
                }

                var review = new ScholarshipReview
                {
                    ApplicantId = applicant.ApplicantID,
                    ReviewerId = currentUser.Id,
                    ScholarshipLevel = ScholarshipReview.ScholarshipLevel,
                    ReviewNotes = ScholarshipReview.ReviewNotes,
                    SupportingDocumentation = ScholarshipReview.SupportingDocs,
                    ReviewDate = DateTime.UtcNow,
                    Status = ScholarshipReview.ScholarshipLevel == "Reject" ? "Rejected" : "Approved"
                };

                _primaryContext.ScholarshipReviews.Add(review);

                // Update applicant status
                applicant.ApplicationStatus = "reviewed";

                await _primaryContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Scholarship decision recorded successfully.";
                return RedirectToPage("/Admin/ViewApplicantsByCommitteeSport");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scholarship decision");
                TempData["ErrorMessage"] = "An error occurred while processing the scholarship decision.";
                return Page();
            }
        }
    }
}

                //ScholarshipReview class
                public class ScholarshipReview
                {
                    public int ScholarshipReviewId { get; set; }
                    public int ApplicantId { get; set; }
                    public string ReviewerId { get; set; }
                    public string ScholarshipLevel { get; set; }
                    public string ReviewNotes { get; set; }
                    public string SupportingDocumentation { get; set; }
                    public DateTime ReviewDate { get; set; }
                    public string Status { get; set; }
                }

                //ScholarshipReviews adding DbSet to the PrimaryContext
                public class PrimaryContext : DbContext
                {
                    public PrimaryContext(DbContextOptions<PrimaryContext> options)
                        : base(options)
                    {
                    }

                    public DbSet<Applicant> Applicants { get; set; }
                    public DbSet<ScholarshipReview> ScholarshipReviews { get; set; }

                    protected override void OnModelCreating(ModelBuilder modelBuilder)
                    {
                        base.OnModelCreating(modelBuilder);
                    }

                }

