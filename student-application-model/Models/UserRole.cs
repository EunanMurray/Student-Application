using StudentApplicationModel.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class UserRole
{
    [Key]
    public int UserRoleID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int RoleID { get; set; }

    [ForeignKey("UserID")]
    public CommitteeMember User { get; set; } = null!; 

    [ForeignKey("RoleID")]
    public Role Role { get; set; } = null!;
}