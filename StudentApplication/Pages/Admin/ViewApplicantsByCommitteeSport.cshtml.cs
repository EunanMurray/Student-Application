using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplication.ViewModels;
using StudentApplicationModel.Data;
using StudentApplicationModel.Models;
using student_application_model.Models;
using Microsoft.Extensions.Logging;

namespace StudentApplication.Pages.Admin
{
    public class ViewApplicantsByCommitteeSportModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PrimaryContext _primaryContext;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<ViewApplicantsByCommitteeSportModel> _logger;

        public ViewApplicantsByCommitteeSportModel(
            UserManager<IdentityUser> userManager,
            PrimaryContext primaryContext,
            ApplicationDbContext applicationDbContext,
            ILogger<ViewApplicantsByCommitteeSportModel> logger)
        {
            _userManager = userManager;
            _primaryContext = primaryContext;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public List<ApplicantViewModel> Applicants { get; set; }
        public List<Campus> Campuses { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedYear { get; set; }
        public List<string> AvailableYears { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Set default year to current year if not specified
            if (string.IsNullOrEmpty(SelectedYear))
            {
                SelectedYear = DateTime.UtcNow.Year.ToString();
            }

            // Get all years that have applications
            AvailableYears = await _primaryContext.Applicants
    .Select(a => a.DateSubmitted.Year.ToString())
    .Distinct()
    .OrderByDescending(y => y)
    .ToListAsync();

            // Ensure current year is in the list (even if no applications yet)
            string currentYear = DateTime.UtcNow.Year.ToString();
            if (!AvailableYears.Contains(currentYear))
            {
                AvailableYears.Add(currentYear);
                AvailableYears = AvailableYears.OrderByDescending(y => y).ToList();
            }

            // Retrieve the current user
            var currentUser = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"Retrieved current user: {(currentUser != null ? currentUser.UserName : "null")}");

            // Check if the user exists and is a committee member
            if (currentUser == null)
            {
                _logger.LogWarning("Current user is null. Redirecting to home page.");
                return Redirect("/");
            }

            var isCommitteeMember = await _userManager.IsInRoleAsync(currentUser, "Committee Member");
            _logger.LogInformation($"Is user a committee member: {isCommitteeMember}");

            if (!isCommitteeMember)
            {
                _logger.LogInformation("User is not a committee member, redirecting to home page.");
                return Redirect("/");
            }

            Campuses = await _primaryContext.Campuses.ToListAsync();

            // Retrieve the sports assigned to the user
            _logger.LogInformation("Retrieving user sports.");
            var userSports = await _primaryContext.UserSports
                .Where(us => us.UserID == currentUser.Id)
                .Select(us => us.SportID)
                .ToListAsync();

            _logger.LogInformation($"UserSports for user {currentUser.UserName}: {string.Join(", ", userSports)}");

            if (!userSports.Any())
            {
                _logger.LogInformation("User has no assigned sports, displaying error message.");
                TempData["ErrorMessage"] = "You don't have any assigned sports.";
                return RedirectToPage();
            }

            // Retrieve the sport names associated with the user's sports
            _logger.LogInformation("Retrieving sport names for the user's sports.");
            var sportNames = await _primaryContext.Sports
                .Where(s => userSports.Contains(s.SportID))
                .Select(s => s.SportName)
                .ToListAsync();

            _logger.LogInformation($"Sport names: {string.Join(", ", sportNames)}");

            // Build the applicants query
            _logger.LogInformation("Building applicants query.");
            var applicantsQuery = _primaryContext.Applicants
                .Include(a => a.ContactDetail)
                .Include(a => a.HomeDetail)
                .Include(a => a.Referees)
                .Include(a => a.ApplicantSports)
                .ThenInclude(a => a.Sport)
                .Where(a => a.ApplicantSports.Any(a => sportNames.Contains(a.Sport.SportName)))
                .Where(a => a.DateSubmitted.Year.ToString() == "2025");

            _logger.LogInformation($"Applicants query: {applicantsQuery.ToQueryString()}");

            // Map applicants to the view model
            _logger.LogInformation("Executing applicants query.");
            var applicantsList = await applicantsQuery.ToListAsync();

            _logger.LogInformation($"Number of applicants found: {applicantsList.Count}");

            _logger.LogInformation("Mapping applicants to ApplicantViewModel.");
            Applicants = applicantsList.Select(a => new ApplicantViewModel
            {
                FirstName = a.FirstName,
                LastName = a.LastName,
                CollegeYear = a.CollegeYear,
                CampusID = a.CampusID,
                DateOfBirth = a.DateOfBirth,
                SportingDetails = a.SportPositionOrCategory,
                SportingAchievements = a.SportingAchievements,
                SportingGoals = a.SportingGoals,
                PastClubs = a.PastClubs,
                ApplicationStatus = a.ApplicationStatus,
                HighestCompetitionLevel = a.HighestCompetitionLevel,
                Referees = a.Referees.Select(r => new RefereeViewModel
                {
                    Name = r.Name,
                    TitleOrRole = r.TitleOrRole,
                    PhoneNumber = r.PhoneNumber,
                    Email = r.Email
                }).ToList(),
                ScholarshipLevel = ""
            }).ToList();

            _logger.LogInformation($"Number of applicants in view model: {Applicants.Count}");

            var applicantIds = Applicants.Select(a => a.FirstName + " " + a.LastName).ToList();

            var scholarshipOffers = await _primaryContext.ScholarshipOfferHistories
                .Include(s => s.Scholarship)
                    .ThenInclude(s => s.ScholarshipType)
                .Include(s => s.Applicant)
                .Where(s => applicantIds.Contains(s.Applicant.FirstName + " " + s.Applicant.LastName))
                .ToListAsync();

            foreach (var applicant in Applicants)
            {
                var fullName = applicant.FirstName + " " + applicant.LastName;
                var offer = scholarshipOffers.FirstOrDefault(s =>
                    s.Applicant.FirstName + " " + s.Applicant.LastName == fullName);

                if (offer != null)
                {
                    applicant.ScholarshipLevel = offer.Scholarship?.ScholarshipType?.ScholarshipLevelName;
                    applicant.HasScholarshipOffer = true;
                }
            }

            _logger.LogInformation("OnGetAsync completed successfully.");
            return Page();
        }
    }


public class ApplicantViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string? StudentNumber { get; set; }
        public int? CollegeYear { get; set; }

        public int? CampusID { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SportingDetails { get; set; }
        public string SportingAchievements { get; set; }
        public string SportingGoals { get; set; }
        public string PastClubs { get; set; }
        public string ApplicationStatus { get; set; } = "notReviewed";
        public string HighestCompetitionLevel { get; set; }
        public List<RefereeViewModel> Referees { get; set; }
        public string ScholarshipLevel { get; set; }
        public bool HasScholarshipOffer { get; set; }
    }

    public class RefereeViewModel
    {
        public string Name { get; set; }
        public string TitleOrRole { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}