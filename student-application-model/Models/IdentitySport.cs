using StudentApplicationModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IdentitySport
{
    public int SportID { get; set; }
    public string SportName { get; set; }
    public virtual ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();
}
