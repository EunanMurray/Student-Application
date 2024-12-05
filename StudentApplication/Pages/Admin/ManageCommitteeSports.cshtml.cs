using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Models;
using StudentApplication.ViewModels;
using StudentApplicationModel.Data;
using System;
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

        public ManageCommitteeSportsModel(
            UserManager<IdentityUser> userManager,
            PrimaryContext primaryContext)
        {
            _userManager = userManager;
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
                // Fetch committee members
                var committeeMembers = await _userManager.GetUsersInRoleAsync("Committee Member");

                // Fetch sports and user-sport relationships
                var sports = await _primaryContext.Sports.AsNoTracking().ToListAsync();
                var userSports = await _primaryContext.UserSports.AsNoTracking().ToListAsync();

                // Populate committee member view models
                foreach (var member in committeeMembers)
                {
                    var assignedSportIds = userSports
                        .Where(us => us.UserID == member.Id)
                        .Select(us => us.SportID)
                        .ToList();

                    var viewModel = new CommitteeMemberViewModel
                    {
                        UserId = member.Id,
                        UserName = member.UserName ?? string.Empty,
                        Email = member.Email ?? string.Empty,
                        AssignedSportIds = assignedSportIds,
                        AvailableSports = sports.Select(s => new Sport
                        {
                            SportID = s.SportID,
                            SportName = s.SportName
                        }).ToList()
                    };

                    CommitteeMembers.Add(viewModel);
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

                // Validate selected user
                var user = await _userManager.FindByIdAsync(SelectedUserId?.Trim());
                if (user == null || !await _userManager.IsInRoleAsync(user, "Committee Member"))
                {
                    TempData["ErrorMessage"] = "Invalid user or user is not a committee member.";
                    return Page();
                }

                // Validate selected sports
                var validSportIds = await _primaryContext.Sports
                    .Where(s => SelectedSports.Contains(s.SportID))
                    .Select(s => s.SportID)
                    .ToListAsync();

                if (validSportIds.Count != SelectedSports.Count)
                {
                    TempData["ErrorMessage"] = "One or more selected sports are invalid.";
                    return Page();
                }

                // Update user-sport relationships
                using var transaction = await _primaryContext.Database.BeginTransactionAsync();
                try
                {
                    var existingAssignments = await _primaryContext.UserSports
                        .Where(us => us.UserID == SelectedUserId)
                        .ToListAsync();

                    if (existingAssignments.Any())
                    {
                        _primaryContext.UserSports.RemoveRange(existingAssignments);
                        await _primaryContext.SaveChangesAsync();
                    }

                    foreach (var sportId in validSportIds)
                    {
                        _primaryContext.UserSports.Add(new UserSport
                        {
                            UserID = SelectedUserId,
                            SportID = sportId
                        });
                    }

                    await _primaryContext.SaveChangesAsync();
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
    }
}
