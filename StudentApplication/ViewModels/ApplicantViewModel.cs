using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentApplicationPages.ViewModels
{
    public class ApplicantViewModel
    {
        // Personal Details

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        public string Name => $"{FirstName} {LastName}";

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string MobilePhoneNumber { get; set; }

        public string Address { get; set; }




        // Parent/Guardian Details (if under 18)

        public string ParentPhoneNumber { get; set; }

        public string ParentEmail { get; set; }





        // Academic History

        public string SecondarySchoolAttended { get; set; }

        public bool PriorThirdLevelAttendance { get; set; }

        [Required(ErrorMessage = "CAO Number is required.")]
        public string CAONumber { get; set; }

        [Required(ErrorMessage = "Campus selection is required.")]
        public int CampusID { get; set; }

        public List<string> CourseCodes { get; set; }

        public string CourseSelectionReasons { get; set; }

        // Sport Details

        [Required(ErrorMessage = "At least one sport must be selected.")]
        public List<int> SportIDs { get; set; }

        public string SportPositionOrCategory { get; set; }

        public string CurrentClub { get; set; }

        public string PastClubs { get; set; }

        public string HighestCompetitionLevel { get; set; }

        public string SportingAchievements { get; set; }

        public string SportingGoals { get; set; }

        // References

        public List<RefereeViewModel> Referees { get; set; }

        // Leisurewear and Declaration

        [Required(ErrorMessage = "Leisurewear size selection is required.")]
        public string PreferredLeisurewearSize { get; set; }

        [Required(ErrorMessage = "You must confirm the declaration.")]
        [Display(Name = "I confirm that the information provided is accurate.")]
        public bool IsDeclarationConfirmed { get; set; }

 
        public ApplicantViewModel()
        {
            Referees = new List<RefereeViewModel>
            {
                new RefereeViewModel(),
                new RefereeViewModel()
            };
            CourseCodes = new List<string>();
            SportIDs = new List<int>();
        }
    }

    public class RefereeViewModel
    {
      
        [Required(ErrorMessage = "Referee name is required.")]
        public string Name { get; set; }

        public string TitleOrRole { get; set; }


        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
    }
}
