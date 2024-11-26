using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace StudentApplicationModel.Models
{
    public class CommitteeMember
    {
        public int MemberID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}