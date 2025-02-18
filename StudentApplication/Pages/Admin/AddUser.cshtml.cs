using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Data;
using StudentApplicationModel.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentApplication.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AddUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PrimaryContext _primaryContext;
        private readonly ILogger<AddUserModel> _logger;

        public AddUserModel(
            UserManager<IdentityUser> userManager,
            PrimaryContext primaryContext,
            ILogger<AddUserModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("AddUserModel constructor called");
            _userManager = userManager;
            _primaryContext = primaryContext;
        }

        [BindProperty]
        public UserInputModel UserInput { get; set; }

        public SelectList SportsList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("OnGetAsync started");

            try
            {
                // Log current user info
                var user = await _userManager.GetUserAsync(User);
                _logger.LogInformation($"Current user: {user?.Email ?? "No user"}, IsAuthenticated: {User.Identity.IsAuthenticated}");

                if (user == null)
                {
                    _logger.LogWarning("No user found, redirecting to login");
                    return RedirectToPage("/Account/Login");
                }

                // Check if user is in Admin role
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                _logger.LogInformation($"User is admin: {isAdmin}");

                if (!isAdmin)
                {
                    _logger.LogWarning("User is not an admin, access denied");
                    return RedirectToPage("/Account/AccessDenied");
                }

                _logger.LogInformation("Fetching sports from database");
                var sports = await _primaryContext.Sports
                    .OrderBy(s => s.SportName)
                    .Select(s => new SelectListItem
                    {
                        Value = s.SportID.ToString(),
                        Text = s.SportName
                    })
                    .ToListAsync();

                _logger.LogInformation($"Found {sports.Count} sports");
                SportsList = new SelectList(sports, "Value", "Text");

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnGetAsync");
                throw;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync started");
            _logger.LogInformation($"Received input - Email: {UserInput?.Email}, Username: {UserInput?.Username}, Role: {UserInput?.Role}");

            try
            {
                if (UserInput.Role != "Committee Member" && !ModelState.IsValid)
                {
                    ModelState.Remove("UserInput.SelectedSports");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid");
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogWarning($"Model Error: {modelError.ErrorMessage}");
                    }
                    await OnGetAsync();
                    return Page();
                }

                // Check if user exists
                _logger.LogInformation($"Checking if user exists: {UserInput.Email}");
                var existingUser = await _userManager.FindByEmailAsync(UserInput.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning($"User already exists: {UserInput.Email}");
                    ModelState.AddModelError(string.Empty, "User with this email already exists.");
                    await OnGetAsync();
                    return Page();
                }

                // Create new user
                _logger.LogInformation("Creating new user");
                var user = new IdentityUser
                {
                    UserName = UserInput.Username,
                    Email = UserInput.Email,
                    EmailConfirmed = true
                };

                _logger.LogInformation("Attempting to create user in database");
                var result = await _userManager.CreateAsync(user, UserInput.Password);

                if (result.Succeeded)
                {
                    var createdUser = await _userManager.FindByEmailAsync(user.Email);
                    var userPassword = await _userManager.CheckPasswordAsync(createdUser, UserInput.Password);
                    _logger.LogInformation($"Can find created user: {createdUser != null}");
                    _logger.LogInformation($"Password check result: {userPassword}");
                    _logger.LogInformation($"User created successfully: {user.Email}");

                    // Add role
                    _logger.LogInformation($"Attempting to add role: {UserInput.Role}");
                    var roleResult = await _userManager.AddToRoleAsync(user, UserInput.Role);

                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError($"Failed to add role. Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        throw new Exception($"Failed to add role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }

                    // Handle sports for Committee Member
                    if (UserInput.Role == "Committee Member" && UserInput.SelectedSports != null && UserInput.SelectedSports.Any())
                    {
                        _logger.LogInformation($"Adding {UserInput.SelectedSports.Count} sports to user");

                        foreach (var sportId in UserInput.SelectedSports)
                        {
                            _logger.LogInformation($"Adding sport ID: {sportId}");
                            _primaryContext.UserSports.Add(new UserSport
                            {
                                UserID = user.Id,
                                SportID = sportId
                            });
                        }

                        await _primaryContext.SaveChangesAsync();
                        _logger.LogInformation("Sports added successfully");
                    }

                    TempData["SuccessMessage"] = "User created successfully.";
                    _logger.LogInformation("User creation process completed successfully");
                    return RedirectToPage("/Admin/RoleTest");
                }
                else
                {
                    
                    _logger.LogError($"User creation failed. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostAsync");
                TempData["ErrorMessage"] = "An error occurred while creating the user.";
            }

            await OnGetAsync();
            return Page();
        }
    }

    public class UserInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; }

        public List<int> SelectedSports { get; set; }
    }
}