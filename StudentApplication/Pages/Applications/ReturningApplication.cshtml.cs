using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Models;
using StudentApplicationModel.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentApplication.Pages.Applications
{
    [Authorize(Roles = "ReturningApplicant")]
    public class ReturningApplicationModel : PageModel
    {
        private readonly PrimaryContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ReturningApplicationModel> _logger;

        public ReturningApplicationModel(
            PrimaryContext context,
            UserManager<IdentityUser> userManager,
            ILogger<ReturningApplicationModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            ErrorMessages = new List<string>();
        }

        [BindProperty]
        public ReturningApplicantViewModel Application { get; set; }

        public List<string> ErrorMessages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found.");
                return NotFound("User not found.");
            }

            _logger.LogInformation($"Loading previous application data for user: {user.Email}");

            var previousApplication = await _context.Applicants
                .Include(a => a.ContactDetail)
                .Include(a => a.HomeDetail)
                .Include(a => a.ApplicantSports)
                .Include(a => a.Campus)
                .FirstOrDefaultAsync(a => a.ContactDetail.Email == user.Email);

            if (previousApplication == null)
            {
                _logger.LogWarning($"No previous application found for user: {user.Email}");
                return NotFound("Previous application not found.");
            }

            _logger.LogInformation($"Found previous application for: {previousApplication.FirstName} {previousApplication.LastName}");

            Application = new ReturningApplicantViewModel
            {
                FirstName = previousApplication.FirstName,
                LastName = previousApplication.LastName,
                Email = previousApplication.ContactDetail.Email,
                StudentNumber = previousApplication.StudentNumber,
                MobilePhoneNumber = previousApplication.ContactDetail.PhoneNumber,
                Address = previousApplication.HomeDetail?.Address,
                CurrentYear = previousApplication.CollegeYear.HasValue ? previousApplication.CollegeYear.Value + 1 : 2,
                AcademicYearAchievements = "",
                ATURepresentation = "",
                SportingGoals = previousApplication.SportingGoals,
                CourseSelectionReasons = previousApplication.CourseSelectionReasons ?? "Please provide your course details",
                PastClubs = "",
                IsDeclarationConfirmed = false
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Processing renewal application submission");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ErrorMessages.Add(error.ErrorMessage);
                }
                _logger.LogWarning("Model validation failed");
                return Page();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found during form submission");
                    return NotFound("User not found.");
                }

                var previousApplication = await _context.Applicants
                    .Include(a => a.ContactDetail)
                    .Include(a => a.HomeDetail)
                    .Include(a => a.ApplicantSports)
                    .Include(a => a.Campus)
                    .FirstOrDefaultAsync(a => a.ContactDetail.Email == user.Email);

                if (previousApplication == null)
                {
                    _logger.LogWarning($"Previous application not found for user: {user.Email}");
                    return NotFound("Previous application not found.");
                }

                var newApplication = new Applicant
                {
                    FirstName = previousApplication.FirstName,
                    LastName = previousApplication.LastName,
                    DateOfBirth = previousApplication.DateOfBirth,
                    Gender = previousApplication.Gender,
                    StudentNumber = previousApplication.StudentNumber,
                    CAONumber = previousApplication.CAONumber,
                    CollegeYear = previousApplication.CollegeYear.HasValue ? previousApplication.CollegeYear.Value + 1 : 2,
                    CampusID = previousApplication.CampusID,
                    SecondarySchoolAttended = previousApplication.SecondarySchoolAttended,
                    PriorThirdLevelAttendance = previousApplication.PriorThirdLevelAttendance,
                    ApplicationStatus = "returning",
                    DateSubmitted = DateTime.UtcNow,
                    PreferredLeisurewearSize = previousApplication.PreferredLeisurewearSize,
                    CourseSelectionReasons = !string.IsNullOrEmpty(Application.CourseSelectionReasons)
                        ? Application.CourseSelectionReasons
                        : "Course information not provided",
                    IsDeclarationConfirmed = Application.IsDeclarationConfirmed,
                    SportingGoals = !string.IsNullOrEmpty(Application.SportingGoals)
                        ? Application.SportingGoals
                        : "Not provided",
                    SportingAchievements = !string.IsNullOrEmpty(Application.AcademicYearAchievements)
                        ? Application.AcademicYearAchievements
                        : "Not provided",
                    PastClubs = !string.IsNullOrEmpty(Application.PastClubs)
                        ? Application.PastClubs
                        : "Not provided",
                    SportPositionOrCategory = !string.IsNullOrEmpty(Application.ATURepresentation)
                        ? $"ATU Representation: {Application.ATURepresentation}"
                        : "ATU Representation: Not provided",
                    CurrentClub = "ATU",
                    HighestCompetitionLevel = !string.IsNullOrEmpty(previousApplication.HighestCompetitionLevel)
                        ? previousApplication.HighestCompetitionLevel
                        : "Not provided"
                };

                _context.Applicants.Add(newApplication);
                await _context.SaveChangesAsync();

                var contactDetail = new ContactDetail
                {
                    ApplicantID = newApplication.ApplicantID,
                    Email = previousApplication.ContactDetail?.Email,
                    PhoneNumber = !string.IsNullOrEmpty(Application.MobilePhoneNumber)
                        ? Application.MobilePhoneNumber
                        : previousApplication.ContactDetail?.PhoneNumber,
                    ParentsEmail = previousApplication.ContactDetail?.ParentsEmail,
                    ParentsPhoneNumber = previousApplication.ContactDetail?.ParentsPhoneNumber
                };
                _context.ContactDetails.Add(contactDetail);

                var homeDetail = new HomeDetail
                {
                    ApplicantID = newApplication.ApplicantID,
                    Address = !string.IsNullOrEmpty(Application.Address)
                        ? Application.Address
                        : previousApplication.HomeDetail?.Address
                };
                _context.HomeDetails.Add(homeDetail);

                foreach (var sport in previousApplication.ApplicantSports)
                {
                    var applicantSport = new ApplicantSport
                    {
                        ApplicantID = newApplication.ApplicantID,
                        SportID = sport.SportID
                    };
                    _context.ApplicantSports.Add(applicantSport);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Renewal application submitted successfully for: {newApplication.FirstName} {newApplication.LastName}");

                TempData["SuccessMessage"] = "Your renewal application has been submitted successfully.";
                return RedirectToPage("/Applications/Confirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting renewal application");
                ErrorMessages.Add($"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }

        public class ReturningApplicantViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string StudentNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Address { get; set; }
        public int CurrentYear { get; set; }
        public string AcademicYearAchievements { get; set; }
        public string ATURepresentation { get; set; }
        public string SportingGoals { get; set; }
        public string PastClubs { get; set; }
        public string CourseSelectionReasons { get; set; }
        public int? CourseID { get; set; }
        public bool IsDeclarationConfirmed { get; set; }
    }
}