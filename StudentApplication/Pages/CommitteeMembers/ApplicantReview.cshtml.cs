using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using StudentApplicationPages.Data;
using StudentApplicationModel.Data;
using StudentApplicationModel.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicantReviewModel : PageModel
{
    private readonly PrimaryContext _primaryContext;
    private readonly ApplicationDbContext _applicationDb;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<ApplicantReviewModel> _logger;

    public ApplicantReviewModel(
        PrimaryContext primaryContext,
        ApplicationDbContext applicationDb,
        UserManager<IdentityUser> userManager,
        ILogger<ApplicantReviewModel> logger)
    {
        _primaryContext = primaryContext;
        _applicationDb = applicationDb;
        _userManager = userManager;
        _logger = logger;
    }

    public class ApplicantSummaryViewModel
    {
        public int ApplicantId { get; set; }
        public string Name { get; set; }
        public string Sports { get; set; }
        public string Campus { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public string ApplicationStatus { get; set; }
    }

    public List<ApplicantSummaryViewModel> Applicants { get; set; } = new List<ApplicantSummaryViewModel>();
    public List<string> AssignedSports { get; set; } = new List<string>();

    [Authorize(Roles = RoleNames.CommitteeMember)]
    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                return NotFound("User not found.");
            }

            // Get sports assigned to this committee member from ApplicationDbContext
            var assignedSportIds = await _applicationDb.UserSports
                .Where(us => us.UserID == user.Id)
                .Select(us => us.SportID)
                .ToListAsync();

            if (!assignedSportIds.Any())
            {
                _logger.LogInformation($"No sports assigned to committee member {user.Id}");
                return Page();
            }

            // Get sport names from PrimaryContext
            AssignedSports = await _primaryContext.Sports
                .Where(s => assignedSportIds.Contains(s.SportID))
                .Select(s => s.SportName)
                .ToListAsync();

            // Get applicants who have applied for any of the assigned sports
            var applicants = await _primaryContext.Applicants
                .Include(a => a.ApplicantSports)
                .ThenInclude(a => a.Sport)
                .Include(a => a.Campus)
                .Where(a => a.ApplicantSports.Any(s => assignedSportIds.Contains(s.SportID)))
                .Select(a => new ApplicantSummaryViewModel
                {
                    ApplicantId = a.ApplicantID,
                    Name = a.Name,
                    Sports = string.Join(", ", a.ApplicantSports
                        .Where(s => assignedSportIds.Contains(s.SportID))
                        .Select(s => s.Sport.SportName)),
                    Campus = a.Campus.CampusName,
                    DateSubmitted = a.DateSubmitted,
                    ApplicationStatus = a.ApplicationStatus
                })
                .OrderByDescending(a => a.DateSubmitted)
                .ToListAsync();

            Applicants = applicants;

            _logger.LogInformation($"Retrieved {applicants.Count} applicants for committee member {user.Id}");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applicant data");
            TempData["ErrorMessage"] = "An error occurred while retrieving applicant data.";
            return Page();
        }
    }
}