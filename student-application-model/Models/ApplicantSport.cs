using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApplicationModel.Models
{
    public class ApplicantSport
    {
        public int ApplicantID { get; set; }
        public int SportID { get; set; }

        // Navigation Properties
        public Applicant Applicant { get; set; }
        public Sport Sport { get; set; }
    }
}
