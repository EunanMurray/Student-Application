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

            // Remove user's sports assignments
            var userSports = await _primaryContext.UserSports
                .Where(us => us.UserID == user.Id)
                .ToListAsync();
            if (userSports.Any())
            {
                _primaryContext.UserSports.RemoveRange(userSports);
                await _primaryContext.SaveChangesAsync();
            }

            // Delete the user
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {email} has been deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
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