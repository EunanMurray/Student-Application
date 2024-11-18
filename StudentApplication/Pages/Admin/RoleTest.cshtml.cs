using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

    [Authorize]
    public class RoleTestModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public List<UserWithRoleViewModel> Users { get; set; } = new List<UserWithRoleViewModel>();
        public string AssignedSport { get; set; }

        public SelectList RoleSelectList { get; set; }

        public RoleTestModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task OnGetAsync()
        {
            Console.WriteLine("Starting OnGetAsync");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                Console.WriteLine("No user found.");
                return;
            }

            Console.WriteLine($"User found with email: {currentUser.Email}");

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

                Console.WriteLine($"User: {u.UserName} - Role: {currentRole}");
            }

            var rolesList = await _roleManager.Roles.ToListAsync();
            RoleSelectList = new SelectList(rolesList, "Name", "Name");

            Console.WriteLine("RoleSelectList successfully populated.");
            Console.WriteLine("OnGetAsync completed.");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(string email, string selectedRole)
        {
            Console.WriteLine("OnPostAsync called.");
            Console.WriteLine($"Email received: {email}");
            Console.WriteLine($"SelectedRole received: {selectedRole}");

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

            // Remove all current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add the new role if provided
            if (!string.IsNullOrEmpty(selectedRole))
            {
                var roleExists = await _roleManager.RoleExistsAsync(selectedRole);
                if (roleExists)
                {
                    await _userManager.AddToRoleAsync(user, selectedRole);
                    TempData["SuccessMessage"] = $"Role for {email} updated to {selectedRole}.";
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
