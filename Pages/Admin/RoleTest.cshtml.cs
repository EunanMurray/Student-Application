using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class RoleTestModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SecondaryContext _secondaryContext;

    public List<UserWithRoleViewModel> Users { get; set; }
    public string AssignedSport { get; set; }
    [BindProperty]
    public string NewRole { get; set; }

    public SelectList RoleSelectList { get; set; }

    public RoleTestModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SecondaryContext secondaryContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _secondaryContext = secondaryContext;
    }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (int.TryParse(user.Id, out int userId))
        {
            var userRole = await _secondaryContext.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserID == userId);

            if (userRole != null && userRole.Role.Name == "CommitteeMember")
            {
                var committeeMember = await _secondaryContext.CommitteeMembers
                    .Include(cm => cm.Sport)
                    .FirstOrDefaultAsync(cm => cm.MemberID == userId);

                AssignedSport = committeeMember?.Sport?.SportName;
            }
        }

        if (User.IsInRole("Admin"))
        {
            Users = await _userManager.Users
                .Select(u => new UserWithRoleViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    CurrentRole = _userManager.GetRolesAsync(u).Result.FirstOrDefault() 
                })
                .ToListAsync();

          
            var roles = await _roleManager.Roles.ToListAsync();
            RoleSelectList = new SelectList(roles, "Name", "Name");
        }
    }

    public async Task<IActionResult> OnPostAsync(string userId, string selectedRole)
    {
        if (!User.IsInRole("Admin")) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!string.IsNullOrEmpty(selectedRole))
        {
            await _userManager.AddToRoleAsync(user, selectedRole);
        }

        return RedirectToPage();
    }
}

public class UserWithRoleViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string CurrentRole { get; set; }
}
