using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentApplicationModel.Models
{
    public class ScholarshipType
    {
        [Key]
        public int ScholarshipTypeID { get; set; }

        [Required]
        public string ScholarshipLevelName { get; set; }

        [Required]
        public decimal PaymentAmount { get; set; }

        // Navigation Property
        public ICollection<Scholarship> Scholarships { get; set; }
    }
}
