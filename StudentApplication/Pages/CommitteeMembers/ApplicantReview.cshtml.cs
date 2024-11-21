using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using ScholarshipInfoSystem.Data;
using StudentApplicationModel.Data;
using Microsoft.EntityFrameworkCore;

public class ApplicantReviewModel : PageModel
{
    private readonly PrimaryContext _primaryContext;
    private readonly SecondaryContext _secondaryContext;
    private readonly UserManager<IdentityUser> _userManager;

    public ApplicantReviewModel(
        PrimaryContext primaryContext,
        SecondaryContext secondaryContext,
        UserManager<IdentityUser> userManager)
    {
        _primaryContext = primaryContext;
        _secondaryContext = secondaryContext;
        _userManager = userManager;
    }

    public class ApplicantSummaryViewModel
    {
        public int ApplicantId { get; set; }
        public string Name { get; set; }
        public string Sports { get; set; }
        public string Campus { get; set; }
        public DateTime? DateSubmitted { get; set; }
    }

    public List<ApplicantSummaryViewModel> Applicants { get; set; }
    public List<string> AssignedSports { get; set; }

    [Authorize(Roles = RoleNames.CommitteeMember)]
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        // Get sports assigned to this committee member
        var assignedSportIds = await _secondaryContext.CommitteeMemberSports
            .Where(cs => cs.UserId == user.Id)
            .Select(cs => cs.SportId)
            .ToListAsync();

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
                Sports = string.Join(", ", a.ApplicantSports.Select(s => s.Sport.SportName)),
                Campus = a.Campus.CampusName,
                DateSubmitted = a.DateSubmitted
            })
            .OrderByDescending(a => a.DateSubmitted)
            .ToListAsync();

        Applicants = applicants;
        return Page();
    }
}