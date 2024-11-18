using System.ComponentModel.DataAnnotations;
using StudentApplicationModel.Models;

namespace StudentApplicationModel.Models
{
    public class Campus
    {
        [Key]
        public int CampusID { get; set; }

        [Required]
        public string CampusName { get; set; }
    }
}
