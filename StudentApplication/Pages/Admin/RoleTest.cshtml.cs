using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationPages.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentApplicationModel.Data;

[Authorize]
public class RoleTestModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _applicationDb;
    private readonly PrimaryContext _primaryContext;
    private readonly ILogger<RoleTestModel> _logger;

    public List<UserWithRoleViewModel> Users { get; set; } = new List<UserWithRoleViewModel>();
    public List<string> AssignedSports { get; set; } = new List<string>();
    public SelectList RoleSelectList { get; set; }

    public RoleTestModel(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext applicationDb,
        PrimaryContext primaryContext,
        ILogger<RoleTestModel> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _applicationDb = applicationDb;
        _primaryContext = primaryContext;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("No user found.");
                return;
            }

            if (User.IsInRole(RoleNames.CommitteeMember))
            {
                AssignedSports = await _primaryContext.UserSports
                    .Where(us => us.UserID == currentUser.Id)
                    .Join(_primaryContext.Sports,
                        us => us.SportID,
                        s => s.SportID,
                        (us, s) => s.SportName)
                    .ToListAsync();
            }

            if (User.IsInRole(RoleNames.Admin))
            {
                var allUsers = await _userManager.Users.ToListAsync();

                foreach (var u in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    var currentRole = roles.FirstOrDefault() ?? "None";
                    Users.Add(new UserWithRoleViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        CurrentRole = currentRole
                    });
                }

                var rolesList = await _roleManager.Roles.ToListAsync();
                RoleSelectList = new SelectList(rolesList, "Name", "Name");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnGetAsync");
            TempData["ErrorMessage"] = "An error occurred while loading the page.";
        }
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostAsync(string email, string selectedRole)
    {
        try
        {
            if (!User.IsInRole(RoleNames.Admin))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email cannot be null or empty.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["ErrorMessage"] = $"User with email '{email}' not found.";
                return RedirectToPage();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            if (!string.IsNullOrEmpty(selectedRole))
            {
                var roleExists = await _roleManager.RoleExistsAsync(selectedRole);
                if (roleExists)
                {
                    await _userManager.AddToRoleAsync(user, selectedRole);
                    TempData["SuccessMessage"] = $"Role for {email} updated to {selectedRole}.";

                    if (selectedRole != RoleNames.CommitteeMember)
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
                }
                else
                {
                    TempData["ErrorMessage"] = "Selected role does not exist.";
                }
            }
            else
            {
                TempData["SuccessMessage"] = $"All roles removed from {email}.";
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnPostAsync");
            TempData["ErrorMessage"] = "An error occurred while updating the role.";
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
    public const string Viewer = "Viewer";
}