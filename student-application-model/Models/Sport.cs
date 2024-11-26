using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentApplicationModel.Models
{
    public class Sport
    {
        [Key]
        public int SportID { get; set; }

        [Required]
        public string SportName { get; set; }

        // Navigation Properties
        public ICollection<ApplicantSport> ApplicantSports { get; set; }
        public ICollection<ScholarshipOfferHistory> ScholarshipOfferHistories { get; set; }
        public ICollection<CommitteeMember> CommitteeMembers { get; set; }
    }
}
