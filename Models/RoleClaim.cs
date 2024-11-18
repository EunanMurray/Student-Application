using ScholarshipInfoSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class RoleClaim
{
    [Key]
    public int RoleClaimID { get; set; }

    [Required]
    public int RoleID { get; set; }

    [Required]
    [MaxLength(50)]
    public string ClaimType { get; set; }

    [Required]
    [MaxLength(50)]
    public string ClaimValue { get; set; }


    [ForeignKey("RoleID")]
    public Role Role { get; set; }
}