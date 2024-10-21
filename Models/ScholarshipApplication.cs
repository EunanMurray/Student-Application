using System.ComponentModel.DataAnnotations;

namespace ScholarshipInfoSystem.Models
{
    public class ScholarshipApplication
    {
        [Key]
        public int ApplicationID { get; set; }

        public int ApplicantID { get; set; }

        public int Year { get; set; }

        public string ApplicationType { get; set; }

        // Navigation Property
        public Applicant Applicant { get; set; }
    }
}
