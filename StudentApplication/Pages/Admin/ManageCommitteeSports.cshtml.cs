using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Models;
using student_application_model.Models;
using StudentApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentApplicationModel.Data;

namespace StudentApplication.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageCommitteeSportsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _applicationDb;
        private readonly PrimaryContext _primaryContext;

        public ManageCommitteeSportsModel(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext applicationDb,
            PrimaryContext primaryContext)
        {
            _userManager = userManager;
            _applicationDb = applicationDb;
            _primaryContext = primaryContext;
            CommitteeMembers = new List<CommitteeMemberViewModel>();
            SelectedSports = new List<int>();
        }

        public List<CommitteeMemberViewModel> CommitteeMembers { get; set; }

        [BindProperty]
        public string SelectedUserId { get; set; } = string.Empty;

        [BindProperty]
        public List<int> SelectedSports { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var committeeMembers = await _userManager.GetUsersInRoleAsync("Committee Member");

                Console.WriteLine($"Found {committeeMembers.Count} committee members");

                var sports = await _applicationDb.Sports
                    .AsNoTracking()
                    .ToListAsync();

                Console.WriteLine($"Found {sports.Count} sports");

                var userSports = await _applicationDb.UserSports
                    .AsNoTracking()
                    .ToListAsync();

                CommitteeMembers = new List<CommitteeMemberViewModel>();

                foreach (var member in committeeMembers)
                {
                    var assignedSportIds = userSports
                        .Where(us => us.UserID == member.Id)
                        .Select(us => us.SportID)
                        .ToList();

                    Console.WriteLine($"User {member.UserName} has {assignedSportIds.Count} assigned sports");

                    var committeeMember = await _applicationDb.CommitteeMembers
                        .FirstOrDefaultAsync(cm => cm.UserID == member.Id);

                    var viewModel = new CommitteeMemberViewModel
                    {
                        UserId = member.Id,
                        UserName = member.UserName ?? string.Empty,
                        Email = member.Email ?? string.Empty,
                        Name = committeeMember?.Name ?? member.UserName ?? string.Empty,
                        AssignedSportIds = assignedSportIds,
                        AvailableSports = sports
                    };

                    CommitteeMembers.Add(viewModel);

                    Console.WriteLine($"ViewModel for {viewModel.UserName} has {viewModel.AvailableSports.Count} available sports");
                }

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnGetAsync: {ex}");
                TempData["ErrorMessage"] = $"Error loading committee members: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var selectedUserId = SelectedUserId?.Trim();

                var user = await _userManager.FindByIdAsync(selectedUserId);
                if (user == null || !await _userManager.IsInRoleAsync(user, "Committee Member"))
                {
                    TempData["ErrorMessage"] = "Invalid user or user is not a committee member.";
                    return Page();
                }

                var validSportIds = await _applicationDb.Sports
                    .Where(s => SelectedSports.Contains(s.SportID))
                    .Select(s => s.SportID)
                    .ToListAsync();

                if (validSportIds.Count != SelectedSports.Count)
                {
                    TempData["ErrorMessage"] = "One or more selected sports are invalid.";
                    return Page();
                }

                using var transaction = await _applicationDb.Database.BeginTransactionAsync();
                try
                {
                    var existingAssignments = await _applicationDb.UserSports
                        .Where(us => us.UserID == selectedUserId)
                        .ToListAsync();

                    if (existingAssignments.Any())
                    {
                        _applicationDb.UserSports.RemoveRange(existingAssignments);
                        await _applicationDb.SaveChangesAsync();
                    }

                    foreach (var sportId in validSportIds)
                    {
                        _applicationDb.UserSports.Add(new UserSport
                        {
                            UserID = selectedUserId,
                            SportID = sportId
                        });
                    }

                    await _applicationDb.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Sport assignments updated successfully.";
                    return RedirectToPage();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating sport assignments: {ex.Message}";
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostInitializeIdentitySportsAsync()
        {
            try
            {
                var sports = await _primaryContext.Sports.ToListAsync();
                var existingIdentitySports = await _applicationDb.Sports.Select(s => s.SportName).ToListAsync();

                var newIdentitySports = sports.Where(s => !existingIdentitySports.Contains(s.SportName))
                    .Select(s => new SportIdentity
                    {
                        SportName = s.SportName
                    })
                    .ToList();

                if (newIdentitySports.Any())
                {
                    _applicationDb.Sports.AddRange(newIdentitySports);
                    await _applicationDb.SaveChangesAsync();

                    TempData["SuccessMessage"] = "IdentitySports table initialized successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = "IdentitySports table is already up-to-date.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error initializing IdentitySports table: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}