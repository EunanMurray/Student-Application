using Microsoft.AspNetCore.Identity;
using StudentApplicationModel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace student_application_model.Models
{
    public class CommitteeMemberSport
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int SportId { get; set; }
    }
}