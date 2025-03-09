using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
            _logger.LogInformation($"OnGetAsync started with Id: {Id}");

            if (Id <= 0)
            {
                _logger.LogWarning($"Invalid Id provided: {Id}");
                return RedirectToPage("/Error");
            }

            try
            {
                _logger.LogInformation($"Querying for ScholarshipOfferHistory with Id: {Id}");
                var offerHistory = await _primaryContext.ScholarshipOfferHistories
                    .Include(s => s.Applicant)
                    .Include(s => s.Scholarship)
                        .ThenInclude(s => s.ScholarshipType)
                    .FirstOrDefaultAsync(s => s.OfferID == Id);

                if (offerHistory == null)
                {
                    _logger.LogWarning($"No ScholarshipOfferHistory found with Id: {Id}");
                    StatusMessage = "Invalid scholarship offer. Please contact the scholarship office.";
                    IsValid = false;
                    return Page();
                }

                _logger.LogInformation($"Found offer for applicant: {offerHistory.Applicant?.Name ?? "Unknown"}, ScholarshipID: {offerHistory.ScholarshipID}");

                ApplicantName = offerHistory.Applicant?.Name;
                ScholarshipLevel = offerHistory.Scholarship?.ScholarshipType?.ScholarshipLevelName;

                _logger.LogInformation($"Offer date: {offerHistory.OfferDate}, Current status: {offerHistory.ResponseStatus}");

                if ((DateTime.UtcNow - offerHistory.OfferDate).TotalDays > 14)
                {
                    _logger.LogInformation($"Offer has expired. OfferDate: {offerHistory.OfferDate}, Days passed: {(DateTime.UtcNow - offerHistory.OfferDate).TotalDays}");
                    StatusMessage = "This scholarship offer has expired. Please contact the scholarship office.";
                    IsExpired = true;
                    return Page();
                }

                if (offerHistory.ResponseStatus != "Pending")
                {
                    _logger.LogInformation($"Offer already processed with status: {offerHistory.ResponseStatus}");
                    StatusMessage = $"This scholarship offer has already been {offerHistory.ResponseStatus.ToLower()}.";
                    IsProcessed = true;
                    return Page();
                }

                IsValid = true;
                _logger.LogInformation("Offer is valid and ready for response");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing scholarship acceptance view with ID: {Id}");
                TempData["ErrorMessage"] = $"Error processing scholarship: {ex.Message}";
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAcceptAsync()
        {
            _logger.LogInformation($"OnPostAcceptAsync started with Id: {Id}");

            if (Id <= 0)
            {
                _logger.LogWarning($"Invalid Id provided for accept: {Id}");
                return RedirectToPage("/Error");
            }

            try
            {
                _logger.LogInformation($"Querying for ScholarshipOfferHistory to accept with Id: {Id}");

                var offerHistory = await _primaryContext.ScholarshipOfferHistories
                    .Include(s => s.Applicant) //I am setting it up so that we can set the applicants year directly to 1
                    .Include(s => s.Applicant.ContactDetail) //Adding this as we need to update the applicants role, this is for future applications. 
                    .FirstOrDefaultAsync(s => s.OfferID == Id);

                if (offerHistory == null)
                {
                    _logger.LogWarning($"No ScholarshipOfferHistory found for accept with Id: {Id}");
                    TempData["ErrorMessage"] = "Scholarship offer not found.";
                    return RedirectToPage("/Error");
                }

                _logger.LogInformation($"Found offer to accept. ScholarshipID: {offerHistory.ScholarshipID}, Current status: {offerHistory.ResponseStatus}");

                var scholarship = await _primaryContext.Scholarships
                    .FirstOrDefaultAsync(s => s.ScholarshipID == offerHistory.ScholarshipID);

                if (scholarship == null)
                {
                    _logger.LogWarning($"No Scholarship found with ID: {offerHistory.ScholarshipID}");
                    TempData["ErrorMessage"] = "Scholarship record not found.";
                    return RedirectToPage("/Error");
                }

                _logger.LogInformation("Updating offer and scholarship records");

                offerHistory.ResponseStatus = "Accepted";
                offerHistory.ResponseDate = DateTime.UtcNow;
                offerHistory.Stage = "Offer Accepted";
                scholarship.hasAccepted = true;

                if (offerHistory.Applicant != null && offerHistory.Applicant.CollegeYear == null) // We're checking if the applicant is new, if so set them to a first year applicant
                {
                    _logger.LogInformation("Setting Year to 1 for first-time applicant");
                    offerHistory.Applicant.CollegeYear = 1;
                }

                string applicantEmail = offerHistory.Applicant?.ContactDetail?.Email; // Grabbing the applciants email so we can use it to query their account and change their role. 

                if (!string.IsNullOrEmpty(applicantEmail))
                {
                    _logger.LogInformation($"Attempting to update role for user with email: {applicantEmail}");

                    // Grab the user by using their email
                    var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                    var user = await userManager.FindByEmailAsync(applicantEmail);

                    if (user != null)
                    {
                        _logger.LogInformation($"User found: {user.UserName}");

                        // Checking to ensure our roles exist (just in case)
                        var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
                        bool roleExists = await roleManager.RoleExistsAsync("ReturningApplicant");

                        if (!roleExists)
                        {
                            _logger.LogInformation("Creating ReturningApplicant role");
                            await roleManager.CreateAsync(new IdentityRole("ReturningApplicant"));
                        }

                        // Check to see if the user is already a returning applicant (they shouldn't be here if they are)
                        if (!await userManager.IsInRoleAsync(user, "ReturningApplicant"))
                        {
                            // If they are just an applicant then remove it from them
                            if (await userManager.IsInRoleAsync(user, "Applicant"))
                            {
                                await userManager.RemoveFromRoleAsync(user, "Applicant");
                            }

                            // Add  the ReturningApplicant role instead so we can track them for the future applications
                            var result = await userManager.AddToRoleAsync(user, "ReturningApplicant");

                            if (result.Succeeded)
                            {
                                _logger.LogInformation($"User role updated to ReturningApplicant successfully");
                            }
                            else
                            {
                                _logger.LogWarning($"Failed to update user role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                            }
                        }
                        else
                        {
                            _logger.LogInformation("User is already in ReturningApplicant role");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"No user found with email: {applicantEmail}");
                    }
                }
                else
                {
                    _logger.LogWarning("Unable to find applicant's email");
                }

                // Save changes to make it so that the applicant is accepted
                await _primaryContext.SaveChangesAsync();
                _logger.LogInformation("Database updated successfully for acceptance");

                TempData["SuccessMessage"] = "You have successfully accepted the scholarship offer.";

                _logger.LogInformation("Redirecting to ScholarshipConfirmation page");
                return RedirectToPage("/Admin/ScholarshipConfirmation");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrency error accepting scholarship with ID: {Id}");
                TempData["ErrorMessage"] = "The scholarship offer was modified by another process. Please try again.";
                return RedirectToPage("/Error");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database update error accepting scholarship with ID: {Id}");
                TempData["ErrorMessage"] = $"Database error: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToPage("/Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error accepting scholarship with ID: {Id}");
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostDeclineAsync()
        {
            _logger.LogInformation($"OnPostDeclineAsync started with Id: {Id}");

            if (Id <= 0)
            {
                _logger.LogWarning($"Invalid Id provided for decline: {Id}");
                return RedirectToPage("/Error");
            }

            try
            {
                _logger.LogInformation($"Querying for ScholarshipOfferHistory to decline with Id: {Id}");
                var offerHistory = await _primaryContext.ScholarshipOfferHistories
                    .FirstOrDefaultAsync(s => s.OfferID == Id);

                if (offerHistory == null)
                {
                    _logger.LogWarning($"No ScholarshipOfferHistory found for decline with Id: {Id}");
                    TempData["ErrorMessage"] = "Scholarship offer not found.";
                    return RedirectToPage("/Error");
                }

                _logger.LogInformation($"Found offer to decline. Current status: {offerHistory.ResponseStatus}");

                offerHistory.ResponseStatus = "Declined";
                offerHistory.ResponseDate = DateTime.UtcNow;
                offerHistory.Stage = "Offer Declined";

                await _primaryContext.SaveChangesAsync();
                _logger.LogInformation("Database updated successfully for decline");

                TempData["InfoMessage"] = "You have declined the scholarship offer.";
                _logger.LogInformation("Redirecting to ScholarshipConfirmation page");
                return RedirectToPage("/Admin/ScholarshipConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error declining scholarship with ID: {Id}");
                TempData["ErrorMessage"] = $"Error declining scholarship: {ex.Message}";
                return RedirectToPage("/Error");
            }
        }
    }
}