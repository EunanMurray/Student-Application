using System.Collections.Generic;
using StudentApplicationModel.Models;

namespace StudentApplication.ViewModels
{
    public class CommitteeMemberViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<int> AssignedSportIds { get; set; } = new List<int>();
        public List<IdentitySport> AvailableSports { get; set; } = new List<IdentitySport>();
    }
}