using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class RoleTestModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly PrimaryContext _primaryContext;
    private readonly ILogger<RoleTestModel> _logger;

    public RoleTestModel(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        PrimaryContext primaryContext,
        ILogger<RoleTestModel> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _primaryContext = primaryContext;
        _logger = logger;
    }

    public List<UserWithRoleViewModel> Users { get; set; } = new List<UserWithRoleViewModel>();
    public SelectList RoleSelectList { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            var allUsers = await _userManager.Users
                .Where(u => u.Id != currentUser.Id)
                .ToListAsync();

            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                Users.Add(new UserWithRoleViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    CurrentRole = roles.FirstOrDefault() ?? "None"
                });
            }

            var rolesList = await _roleManager.Roles.ToListAsync();
            RoleSelectList = new SelectList(rolesList, "Name", "Name");

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
            TempData["ErrorMessage"] = "An error occurred while loading users.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(string email, string selectedRole)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            if (!string.IsNullOrEmpty(selectedRole))
            {
                var result = await _userManager.AddToRoleAsync(user, selectedRole);
                if (result.Succeeded)
                {
                    if (selectedRole != "Committee Member")
                    {
                        var userSports = await _primaryContext.UserSports
                            .Where(us => us.UserID == user.Id)
                            .ToListAsync();
                        if (userSports.Any())
                        {
                            _primaryContext.UserSports.RemoveRange(userSports);
                            await _primaryContext.SaveChangesAsync();
                        }
                    }
                    TempData["SuccessMessage"] = $"Role updated for {user.Email}";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update role.";
                }
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role");
            TempData["ErrorMessage"] = "An error occurred while updating the role.";
            return RedirectToPage();
        }
    }

    public async Task<IActionResult> OnPostDeleteUserAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage();
            }

            _logger.LogInformation($"Starting deletion process for user: {email}");

            var applicants = await _primaryContext.Applicants
                .Include(a => a.ContactDetail)
                .Where(a => a.ContactDetail.Email == email)
                .ToListAsync();

            foreach (var applicant in applicants)
            {
                _logger.LogInformation($"Found applicant with ID {applicant.ApplicantID} for user {email}");

                var scholarshipOffers = await _primaryContext.ScholarshipOfferHistories
                    .Where(s => s.ApplicantID == applicant.ApplicantID)
                    .ToListAsync();

                if (scholarshipOffers.Any())
                {
                    _logger.LogInformation($"Removing {scholarshipOffers.Count} scholarship offers for applicant {applicant.ApplicantID}");

                    foreach (var offer in scholarshipOffers)
                    {
                        var otherOffersUsingThisScholarship = await _primaryContext.ScholarshipOfferHistories
                            .Where(s => s.ScholarshipID == offer.ScholarshipID && s.ApplicantID != applicant.ApplicantID)
                            .AnyAsync();

                        if (!otherOffersUsingThisScholarship)
                        {
                            var scholarship = await _primaryContext.Scholarships
                                .FirstOrDefaultAsync(s => s.ScholarshipID == offer.ScholarshipID);

                            if (scholarship != null)
                            {
                                _logger.LogInformation($"Removing scholarship with ID {scholarship.ScholarshipID}");
                                _primaryContext.Scholarships.Remove(scholarship);
                            }
                        }
                    }

                    _primaryContext.ScholarshipOfferHistories.RemoveRange(scholarshipOffers);
                }

                var courseCodes = await _primaryContext.CourseCodes
                    .Where(c => c.ApplicantID == applicant.ApplicantID)
                    .ToListAsync();

                if (courseCodes.Any())
                {
                    _logger.LogInformation($"Removing {courseCodes.Count} course codes for applicant {applicant.ApplicantID}");
                    _primaryContext.CourseCodes.RemoveRange(courseCodes);
                }

                var applicantSports = await _primaryContext.ApplicantSports
                    .Where(s => s.ApplicantID == applicant.ApplicantID)
                    .ToListAsync();

                if (applicantSports.Any())
                {
                    _logger.LogInformation($"Removing {applicantSports.Count} sport associations for applicant {applicant.ApplicantID}");
                    _primaryContext.ApplicantSports.RemoveRange(applicantSports);
                }

                var referees = await _primaryContext.Referees
                    .Where(r => r.ApplicantID == applicant.ApplicantID)
                    .ToListAsync();

                if (referees.Any())
                {
                    _logger.LogInformation($"Removing {referees.Count} referees for applicant {applicant.ApplicantID}");
                    _primaryContext.Referees.RemoveRange(referees);
                }

                var homeDetail = await _primaryContext.HomeDetails
                    .FirstOrDefaultAsync(h => h.ApplicantID == applicant.ApplicantID);

                if (homeDetail != null)
                {
                    _logger.LogInformation($"Removing home detail for applicant {applicant.ApplicantID}");
                    _primaryContext.HomeDetails.Remove(homeDetail);
                }

                var contactDetail = await _primaryContext.ContactDetails
                    .FirstOrDefaultAsync(c => c.ApplicantID == applicant.ApplicantID);

                if (contactDetail != null)
                {
                    _logger.LogInformation($"Removing contact detail for applicant {applicant.ApplicantID}");
                    _primaryContext.ContactDetails.Remove(contactDetail);
                }

                _logger.LogInformation($"Removing applicant {applicant.ApplicantID}");
                _primaryContext.Applicants.Remove(applicant);
            }

            var userSports = await _primaryContext.UserSports
                .Where(us => us.UserID == user.Id)
                .ToListAsync();

            if (userSports.Any())
            {
                _logger.LogInformation($"Removing {userSports.Count} sport assignments for user {email}");
                _primaryContext.UserSports.RemoveRange(userSports);
            }

            await _primaryContext.SaveChangesAsync();
            _logger.LogInformation($"Successfully removed all associated data for user {email}");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {email} has been successfully deleted");
                TempData["SuccessMessage"] = $"User {email} has been deleted along with all associated data.";
            }
            else
            {
                _logger.LogError($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user {email} and associated data");
            TempData["ErrorMessage"] = $"An error occurred while deleting the user: {ex.Message}";
            return RedirectToPage();
        }
    }
}

public class UserWithRoleViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string CurrentRole { get; set; }
}

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string CommitteeMember = "Committee Member";
    public const string Secretary = "Secretary";
    public const string Viewer = "Viewer";
    
}