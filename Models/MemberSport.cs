using System.ComponentModel.DataAnnotations.Schema;

namespace ScholarshipInfoSystem.Models
{
    public class MemberSport
    {
        public int MemberID { get; set; }
        public int SportID { get; set; }

        // Navigation Properties
        public CommitteeMember CommitteeMember { get; set; }
        public Sport Sport { get; set; }
    }
}
