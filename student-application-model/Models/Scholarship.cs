using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApplicationModel.Models
{
    public class Scholarship
    {
        [Key]
        public int ScholarshipID { get; set; }

        public string OtherDetails { get; set; }

        // Foreign Key
        public int ScholarshipTypeID { get; set; }

        // Navigation Property
        [ForeignKey("ScholarshipTypeID")]
        public ScholarshipType ScholarshipType { get; set; }

        public ICollection<Applicant> Applicants { get; set; }
        public ICollection<ScholarshipOfferHistory> ScholarshipOfferHistories { get; set; }
    }
}
