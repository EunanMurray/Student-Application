using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholarshipInfoSystem.Models
{
    public class CommitteeMember
    {
        [Key]
        public int MemberID { get; set; }

        public string Name { get; set; }

        // Foreign Key for Sport
        public int? SportID { get; set; }

        // Navigation Property for Sport
        [ForeignKey("SportID")]
        public Sport Sport { get; set; }
    }
}
