using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentApplicationModel.Models
{
    public class Sport
    {
        public Sport()
        {
            ApplicantSports = new List<ApplicantSport>();
            ScholarshipOfferHistories = new List<ScholarshipOfferHistory>();
            UserSports = new List<UserSport>();
        }

        public int SportID { get; set; }
        public required string SportName { get; set; }

        public virtual ICollection<ApplicantSport> ApplicantSports { get; set; }
        public virtual ICollection<ScholarshipOfferHistory> ScholarshipOfferHistories { get; set; }
        public virtual ICollection<UserSport> UserSports { get; set; }
    }
}
