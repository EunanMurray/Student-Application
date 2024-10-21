using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Data;
using ScholarshipInfoSystem.Models;
using StudentApplicationPages.Data;
using StudentApplicationPages.ViewModels;

namespace StudentApplicationPages.Pages.Applications
{
    public class ApplyModel : PageModel
    {
        private readonly PrimaryContext _context;

        public ApplyModel(PrimaryContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ApplicantViewModel Application { get; set; }


        public SelectList CampusSelectList { get; set; }
        public MultiSelectList SportSelectList { get; set; }

        public void OnGet()
        {
            Application = new ApplicantViewModel();

            // Populate CampusSelectList
            CampusSelectList = new SelectList(_context.Campuses, "CampusID", "CampusName");

            // Populate SportSelectList
            SportSelectList = new MultiSelectList(_context.Sports, "SportID", "SportName");
        }


        public async Task<IActionResult> OnPostAsync()
        {
            // Repopulate the select lists
            CampusSelectList = new SelectList(_context.Campuses, "CampusID", "CampusName");
            SportSelectList = new MultiSelectList(_context.Sports, "SportID", "SportName");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Map ViewModel to Applicant model
            var applicant = new Applicant
            {
                Name = Application.Name,
                Email = Application.Email,
                CAONumber = Application.CAONumber,
                ApplicationStatus = "notReviewed",
                DateOfBirth = Application.DateOfBirth,
                Gender = Application.Gender,
                MobilePhoneNumber = Application.MobilePhoneNumber,
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
                CampusID = Application.CampusID
            };

            // Add Applicant to context
            _context.Applicants.Add(applicant);
            await _context.SaveChangesAsync(); // Save to get the ApplicantID

            // Save ContactDetail
            var contactDetail = new ContactDetail
            {
                ApplicantID = applicant.ApplicantID,
                PhoneNumber = Application.MobilePhoneNumber
            };
            _context.ContactDetails.Add(contactDetail);

            // Save HomeDetail
            var homeDetail = new HomeDetail
            {
                ApplicantID = applicant.ApplicantID,
                Address = Application.Address
            };
            _context.HomeDetails.Add(homeDetail);

            // Save CourseCodes
            if (Application.CourseCodes != null)
            {
                foreach (var code in Application.CourseCodes)
                {
                    var courseCode = new CourseCode
                    {
                        ApplicantID = applicant.ApplicantID,
                        Code = code
                    };
                    _context.Add(courseCode);
                }
            }

            // Save ApplicantSports
            if (Application.SportIDs != null)
            {
                foreach (var sportID in Application.SportIDs)
                {
                    var applicantSport = new ApplicantSport
                    {
                        ApplicantID = applicant.ApplicantID,
                        SportID = sportID
                    };
                    _context.ApplicantSports.Add(applicantSport);
                }
            }

            // Save Referees
            if (Application.Referees != null)
            {
                foreach (var refereeVM in Application.Referees)
                {
                    var referee = new Referee
                    {
                        ApplicantID = applicant.ApplicantID,
                        Name = refereeVM.Name,
                        TitleOrRole = refereeVM.TitleOrRole,
                        PhoneNumber = refereeVM.PhoneNumber,
                        Email = refereeVM.Email
                    };
                    _context.Referees.Add(referee);
                }
            }

            // Save all changes to the database
            await _context.SaveChangesAsync();

            // Redirect to a confirmation page
            return RedirectToPage("Confirmation");
        }

    }
}
