using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApplicationModel.Models
{
    public class ScholarshipOfferHistory
    {
        [Key]
        public int OfferID { get; set; }

        public int ApplicantID { get; set; }
        public int SportID { get; set; }
        public int? CampusID { get; set; }
        public int ScholarshipID { get; set; }

        public DateTime OfferDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string ResponseStatus { get; set; }
        public string Stage { get; set; }

        // Navigation Properties
        public Applicant Applicant { get; set; }
        public Sport Sport { get; set; }
        public Scholarship Scholarship { get; set; }
    }
}
