using StudentApplicationModel.Models;

namespace student_application_model.Models
{
    public class SportIdentity
    {
        public int SportID { get; set; }
        public string SportName { get; set; }
        public virtual ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();
    }
}