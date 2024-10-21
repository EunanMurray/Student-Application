using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholarshipInfoSystem.Models
{
    public class HomeDetail
    {
        [Key]
        public int HomeID { get; set; }

        public int ApplicantID { get; set; }

        public string Address { get; set; }

        // Navigation Property
        public Applicant Applicant { get; set; }
    }
}
