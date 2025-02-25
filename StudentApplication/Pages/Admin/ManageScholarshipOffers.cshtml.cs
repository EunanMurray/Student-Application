using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Data;
using StudentApplicationModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApplication.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageScholarshipOffersModel : PageModel
    {
        private readonly PrimaryContext _primaryContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ManageScholarshipOffersModel> _logger;

        public ManageScholarshipOffersModel(
            PrimaryContext primaryContext,
            UserManager<IdentityUser> userManager,
            ILogger<ManageScholarshipOffersModel> logger)
        {
            _primaryContext = primaryContext;
            _userManager = userManager;
            _logger = logger;
        }

        public class ScholarshipOfferViewModel
        {
            public int OfferId { get; set; }
            public string ApplicantName { get; set; }
            public string ScholarshipLevel { get; set; }
            public DateTime OfferDate { get; set; }
            public string ResponseStatus { get; set; }
            public string Stage { get; set; }
            public string SportName { get; set; }
        }

        public List<ScholarshipOfferViewModel> ScholarshipOffers { get; set; } = new List<ScholarshipOfferViewModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToPage("/Account/Login");
                }

                var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                if (!isAdmin)
                {
                    return RedirectToPage("/Account/AccessDenied");
                }

                ScholarshipOffers = await _primaryContext.ScholarshipOfferHistories
                    .Include(s => s.Applicant)
                    .Include(s => s.Scholarship)
                        .ThenInclude(s => s.ScholarshipType)
                    .Include(s => s.Sport)
                    .OrderByDescending(s => s.OfferDate)
                    .Select(s => new ScholarshipOfferViewModel
                    {
                        OfferId = s.OfferID,
                        ApplicantName = s.Applicant.Name,
                        ScholarshipLevel = s.Scholarship.ScholarshipType.ScholarshipLevelName,
                        OfferDate = s.OfferDate,
                        ResponseStatus = s.ResponseStatus,
                        Stage = s.Stage,
                        SportName = s.Sport.SportName
                    })
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scholarship offers");
                TempData["ErrorMessage"] = "An error occurred while retrieving scholarship offers.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteOfferAsync(int offerId)
        {
            try
            {
                var offer = await _primaryContext.ScholarshipOfferHistories
                    .Include(s => s.Scholarship)
                    .Include(s => s.Applicant)
                    .FirstOrDefaultAsync(s => s.OfferID == offerId);

                if (offer == null)
                {
                    TempData["ErrorMessage"] = "Scholarship offer not found.";
                    return RedirectToPage();
                }

                string applicantName = offer.Applicant.Name;

                // Reset applicant status if this was their only offer
                var applicantOtherOffers = await _primaryContext.ScholarshipOfferHistories
                    .Where(s => s.ApplicantID == offer.ApplicantID && s.OfferID != offerId)
                    .AnyAsync();

                if (!applicantOtherOffers)
                {
                    offer.Applicant.ApplicationStatus = "notReviewed";
                }

                // Delete the scholarship record
                if (offer.Scholarship != null)
                {
                    _primaryContext.Scholarships.Remove(offer.Scholarship);
                }

                // Delete the offer record
                _primaryContext.ScholarshipOfferHistories.Remove(offer);

                await _primaryContext.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Scholarship offer for {applicantName} has been successfully removed.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting scholarship offer");
                TempData["ErrorMessage"] = "An error occurred while deleting the scholarship offer.";
                return RedirectToPage();
            }
        }
    }
}