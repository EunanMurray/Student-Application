using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentApplicationModel.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApplication.Pages.Applicants
{
    [AllowAnonymous]
    public class AcceptScholarshipModel : PageModel
    {
        private readonly PrimaryContext _primaryContext;
        private readonly ILogger<AcceptScholarshipModel> _logger;

        public AcceptScholarshipModel(
            PrimaryContext primaryContext,
            ILogger<AcceptScholarshipModel> logger)
        {
            _primaryContext = primaryContext;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public string ApplicantName { get; set; }
        public string ScholarshipLevel { get; set; }
        public string StatusMessage { get; set; }
        public bool IsValid { get; set; }
        public bool IsExpired { get; set; }
        public bool IsProcessed { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id <= 0)
            {
                return RedirectToPage("/Error");
            }

            try
            {
                var offerHistory = await _primaryContext.ScholarshipOfferHistories
                    .Include(s => s.Applicant)
                    .Include(s => s.Scholarship)
                        .ThenInclude(s => s.ScholarshipType)
                    .FirstOrDefaultAsync(s => s.OfferID == Id);

                if (offerHistory == null)
                {
                    StatusMessage = "Invalid scholarship offer. Please contact the scholarship office.";
                    IsValid = false;
                    return Page();
                }

                ApplicantName = offerHistory.Applicant.Name;
                ScholarshipLevel = offerHistory.Scholarship.ScholarshipType.ScholarshipLevelName;

                // Check if offer has expired (14 days limit)
                if ((DateTime.UtcNow - offerHistory.OfferDate).TotalDays > 14)
                {
                    StatusMessage = "This scholarship offer has expired. Please contact the scholarship office.";
                    IsExpired = true;
                    return Page();
                }

                // Check if already processed
                if (offerHistory.ResponseStatus != "Pending")
                {
                    StatusMessage = $"This scholarship offer has already been {offerHistory.ResponseStatus.ToLower()}.";
                    IsProcessed = true;
                    return Page();
                }

                IsValid = true;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing scholarship acceptance with ID: {Id}");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAcceptAsync()
        {
            if (Id <= 0)
            {
                return RedirectToPage("/Error");
            }

            try
            {
                var offerHistory = await _primaryContext.ScholarshipOfferHistories
                    .Include(s => s.Scholarship)
                    .FirstOrDefaultAsync(s => s.OfferID == Id);

                if (offerHistory == null)
                {
                    return RedirectToPage("/Error");
                }

                offerHistory.ResponseStatus = "Accepted";
                offerHistory.ResponseDate = DateTime.UtcNow;
                offerHistory.Stage = "Offer Accepted";

                // Update the scholarship record
                offerHistory.Scholarship.hasAccepted = true;

                await _primaryContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "You have successfully accepted the scholarship offer.";
                return RedirectToPage("/Applicants/ScholarshipConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error accepting scholarship with ID: {Id}");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostDeclineAsync()
        {
            if (Id <= 0)
            {
                return RedirectToPage("/Error");
            }

            try
            {
                var offerHistory = await _primaryContext.ScholarshipOfferHistories
                    .FirstOrDefaultAsync(s => s.OfferID == Id);

                if (offerHistory == null)
                {
                    return RedirectToPage("/Error");
                }

                offerHistory.ResponseStatus = "Declined";
                offerHistory.ResponseDate = DateTime.UtcNow;
                offerHistory.Stage = "Offer Declined";

                await _primaryContext.SaveChangesAsync();

                TempData["InfoMessage"] = "You have declined the scholarship offer.";
                return RedirectToPage("/Applicants/ScholarshipConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error declining scholarship with ID: {Id}");
                return RedirectToPage("/Error");
            }
        }
    }
}