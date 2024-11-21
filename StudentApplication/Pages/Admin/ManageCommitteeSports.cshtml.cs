using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Data;
using StudentApplicationModel.Data;
using StudentApplicationModel.Models;
using student_application_model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApplication.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageCommitteeSportsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PrimaryContext _primaryContext;
        private readonly SecondaryContext _secondaryContext;

        public ManageCommitteeSportsModel(
            UserManager<IdentityUser> userManager,
            PrimaryContext primaryContext,
            SecondaryContext secondaryContext)
        {
            _userManager = userManager;
            _primaryContext = primaryContext;
            _secondaryContext = secondaryContext;
            CommitteeMembers = new List<CommitteeMemberViewModel>();
            SelectedSports = new List<int>();
        }

        public class CommitteeMemberViewModel
        {
            public string UserId { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public List<int> AssignedSportIds { get; set; } = new List<int>();
            public List<Sport> AvailableSports { get; set; } = new List<Sport>();
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
                var sports = await _primaryContext.Sports.ToListAsync();
                var committeeSportAssignments = await _secondaryContext.Set<CommitteeMemberSport>().ToListAsync();

                CommitteeMembers = new List<CommitteeMemberViewModel>();

                foreach (var member in committeeMembers)
                {
                    var assignedSportIds = committeeSportAssignments
                        .Where(cs => cs.UserId == member.Id)
                        .Select(cs => cs.SportId)
                        .ToList();

                    CommitteeMembers.Add(new CommitteeMemberViewModel
                    {
                        UserId = member.Id,
                        UserName = member.UserName ?? string.Empty,
                        Email = member.Email ?? string.Empty,
                        AssignedSportIds = assignedSportIds,
                        AvailableSports = sports
                    });
                }

                return Page();
            }
            catch (Exception ex)
            {
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

                // Debug information
                var selectedUserId = SelectedUserId?.Trim();
                Console.WriteLine($"Selected User ID: {selectedUserId}");

                // Verify the user exists in AspNetUsers
                var userExists = await _secondaryContext.Users.AnyAsync(u => u.Id == selectedUserId);
                if (!userExists)
                {
                    TempData["ErrorMessage"] = $"User with ID {selectedUserId} not found in AspNetUsers.";
                    return Page();
                }

                // Verify user roles
                var user = await _userManager.FindByIdAsync(selectedUserId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return Page();
                }

                if (!await _userManager.IsInRoleAsync(user, "Committee Member"))
                {
                    TempData["ErrorMessage"] = $"User {user.Email} is not a committee member.";
                    return Page();
                }

                // Verify sports exist in primary context
                var validSportIds = await _primaryContext.Sports
                    .Where(s => SelectedSports.Contains(s.SportID))
                    .Select(s => s.SportID)
                    .ToListAsync();

                if (validSportIds.Count != SelectedSports.Count)
                {
                    TempData["ErrorMessage"] = "One or more selected sports are invalid.";
                    return Page();
                }

                // Begin transaction
                using var transaction = await _secondaryContext.Database.BeginTransactionAsync();
                try
                {
                    // Remove existing assignments
                    var existingAssignments = await _secondaryContext.Set<CommitteeMemberSport>()
                        .Where(cs => cs.UserId == selectedUserId)
                        .ToListAsync();

                    if (existingAssignments.Any())
                    {
                        _secondaryContext.Set<CommitteeMemberSport>().RemoveRange(existingAssignments);
                        await _secondaryContext.SaveChangesAsync();
                    }

                    // Add new assignments
                    foreach (var sportId in validSportIds)
                    {
                        // Create the new assignment
                        var sql = $@"
                    INSERT INTO CommitteeMemberSports (UserId, SportId)
                    VALUES (@p0, @p1)";

                        await _secondaryContext.Database.ExecuteSqlRawAsync(
                            sql,
                            selectedUserId,
                            sportId);

                        Console.WriteLine($"Added sport {sportId} for user {selectedUserId}");
                    }

                    await transaction.CommitAsync();
                    TempData["SuccessMessage"] = "Sport assignments updated successfully.";
                    return RedirectToPage();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error during transaction: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnPostAsync: {ex}");
                TempData["ErrorMessage"] = $"Error updating sport assignments: {ex.Message}";
                await OnGetAsync();
                return Page();
            }
        }
    }
}