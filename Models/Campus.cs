using System.ComponentModel.DataAnnotations;
using ScholarshipInfoSystem.Models;

namespace ScholarshipInfoSystem.Models
{
    public class Campus
    {
        [Key]
        public int CampusID { get; set; }

        [Required]
        public string CampusName { get; set; }
    }
}
