using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholarshipInfoSystem.Models
{
    public class ContactDetail
    {
        [Key]
        public int ContactID { get; set; }

        public int ApplicantID { get; set; }

        public string PhoneNumber { get; set; }

        // Navigation Property
        public Applicant Applicant { get; set; }
    }
}
