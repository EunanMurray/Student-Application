using Microsoft.AspNetCore.Identity;
using student_application_model.Models;

namespace StudentApplicationModel.Models
{
    public class UserSport
    {
        public required string UserID { get; set; }
        public int SportID { get; set; }

        public virtual IdentitySport? Sport { get; set; }
        public virtual IdentityUser? User { get; set; }
    }

}