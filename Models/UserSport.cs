using ScholarshipInfoSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class UserSport
{
    [Key]
    public int UserSportID { get; set; }

    [Required]
    public int UserID { get; set; } 

    [Required]
    public int SportID { get; set; }

    // Navigation properties
    [ForeignKey("UserID")]
    public CommitteeMember User { get; set; }

    [ForeignKey("SportID")]
    public Sport Sport { get; set; }
}