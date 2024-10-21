using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScholarshipInfoSystem.Models
{
    public class CommitteeMember
    {
        [Key]
        public int MemberID { get; set; }

        public string Name { get; set; }

        // Navigation Property
        public ICollection<MemberSport> MemberSports { get; set; }
    }
}
