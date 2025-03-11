using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Data;
using StudentApplicationModel.Models;
using StudentApplicationModel.Data;
using StudentApplicationPages.ViewModels;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StudentApplication.Services;

namespace StudentApplicationPages.Pages.Applications
{
    [Authorize(Roles = "Applicant")]
    public class ApplyModel : PageModel
    {
        private readonly PrimaryContext _context;
        private readonly IEmailService _emailService;

        public ApplyModel(PrimaryContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
            Application = new ApplicantViewModel();
            CampusSelectList = new SelectList(_context.Campuses, "CampusID", "CampusName");
            SportSelectList = new MultiSelectList(_context.Sports, "SportID", "SportName");
            ErrorMessages = new List<string>();
        }

        [BindProperty]
        public ApplicantViewModel Application { get; set; }

        public SelectList CampusSelectList { get; set; }
        public MultiSelectList SportSelectList { get; set; }
        public List<string> ErrorMessages { get; set; }

        public void OnGet()
        {
            CampusSelectList = new SelectList(_context.Campuses, "CampusID", "CampusName");
            SportSelectList = new MultiSelectList(_context.Sports, "SportID", "SportName");
        }

        public async Task<IActionResult> OnPostAsync(string submit)
        {
            if (submit == "submit")
            {
                if (!Application.ValidateAtuStudentDetails())
                {
                    ModelState.AddModelError(string.Empty, "If providing College Year or Student Number, both must be filled.");
                    ErrorMessages.Add("If you're an ATU student, please provide both College Year and Student Number.");
                    return Page();
                }

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Debug.WriteLine("Validation Error: " + error.ErrorMessage);
                        ErrorMessages.Add(error.ErrorMessage);
                    }
                    return Page();
                }

                try
                {
                    // Create and save the Applicant entity first
                    var applicant = new Applicant
                    {
                        FirstName = Application.FirstName,
                        LastName = Application.LastName,
                        CAONumber = Application.CAONumber,
                        DateOfBirth = Application.DateOfBirth,
                        Gender = Application.Gender,
                        PreferredLeisurewearSize = Application.PreferredLeisurewearSize,
                        IsDeclarationConfirmed = Application.IsDeclarationConfirmed,
                        SecondarySchoolAttended = Application.SecondarySchoolAttended,
                        PriorThirdLevelAttendance = Application.PriorThirdLevelAttendance,
                        CourseSelectionReasons = Application.CourseSelectionReasons,
                        SportPositionOrCategory = Application.SportPositionOrCategory,
                        CurrentClub = Application.CurrentClub,
                        PastClubs = Application.PastClubs,
                        HighestCompetitionLevel = Application.HighestCompetitionLevel,
                        SportingAchievements = Application.SportingAchievements,
                        SportingGoals = Application.SportingGoals,
                        CampusID = Application.CampusID,
                        // New fields for ATU student
                        CollegeYear = Application.CollegeYear,
                        StudentNumber = Application.StudentNumber
                    };

                    _context.Applicants.Add(applicant);
                    await _context.SaveChangesAsync(); // Save to generate ApplicantID
                    Debug.WriteLine($"Applicant created with ID: {applicant.ApplicantID}");

                    // Add Contact Detail
                    if (!string.IsNullOrEmpty(Application.MobilePhoneNumber) && !string.IsNullOrEmpty(Application.Email))
                    {
                        var contactDetail = new ContactDetail
                        {
                            ApplicantID = applicant.ApplicantID,
                            PhoneNumber = Application.MobilePhoneNumber,
                            Email = Application.Email,
                            ParentsPhoneNumber = Application.ParentPhoneNumber,
                            ParentsEmail = Application.ParentEmail
                        };
                        _context.ContactDetails.Add(contactDetail);
                        Debug.WriteLine("ContactDetail added for applicant.");
                    }
                    else
                    {
                        Debug.WriteLine("Contact details are missing required fields.");
                    }

                    // Add Home Detail
                    if (!string.IsNullOrEmpty(Application.Address))
                    {
                        var homeDetail = new HomeDetail
                        {
                            ApplicantID = applicant.ApplicantID,
                            Address = Application.Address
                        };
                        _context.HomeDetails.Add(homeDetail);
                        Debug.WriteLine("HomeDetail added for applicant.");
                    }

                    // Add Course Codes if provided
                    if (Application.CourseCodes != null && Application.CourseCodes.Any())
                    {
                        var courseCodes = Application.CourseCodes
                            .Where(code => !string.IsNullOrEmpty(code))
                            .Select(code => new CourseCode
                            {
                                ApplicantID = applicant.ApplicantID,
                                Code = code
                            }).ToList();

                        _context.CourseCodes.AddRange(courseCodes);
                    }

                    // Add ApplicantSports if provided
                    if (Application.SportIDs != null && Application.SportIDs.Any())
                    {
                        var applicantSports = Application.SportIDs
                            .Select(sportID => new ApplicantSport
                            {
                                SportID = sportID,
                                ApplicantID = applicant.ApplicantID
                            }).ToList();

                        _context.ApplicantSports.AddRange(applicantSports);
                    }

                    // Add Referees if provided
                    if (Application.Referees != null && Application.Referees.Any())
                    {
                        var referees = Application.Referees
                            .Select(refereeVM => new Referee
                            {
                                Name = refereeVM.Name,
                                TitleOrRole = refereeVM.TitleOrRole,
                                PhoneNumber = refereeVM.PhoneNumber,
                                Email = refereeVM.Email,
                                ApplicantID = applicant.ApplicantID
                            }).ToList();

                        _context.Referees.AddRange(referees);
                    }

                    // Save all changes to the database
                    await _context.SaveChangesAsync();

                    // Send email to the submitter
                    await _emailService.SendApplicationConfirmationEmailAsync(Application.Email, Application.FirstName);

                    Debug.WriteLine("Data saved successfully.");
                    return RedirectToPage("Confirmation");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred during save: " + ex.Message);
                    ErrorMessages.Add("An error occurred while saving your application. Please try again.");
                    return Page();
                }
            }

            return Page(); // Return to the same page if submit button wasn't clicked or model state is invalid
        }

    }
}
