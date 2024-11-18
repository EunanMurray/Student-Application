using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScholarshipInfoSystem.Models
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

        public ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();
    }
}
