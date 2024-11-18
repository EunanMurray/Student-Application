using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ScholarshipInfoSystem.Models
{
    public class CommitteeMember
    {
        [Key]
        public int MemberID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int? SportID { get; set; }

        [ForeignKey("SportID")]
        public Sport Sport { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();
    }
}
