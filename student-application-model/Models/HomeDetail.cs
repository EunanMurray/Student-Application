using System.ComponentModel.DataAnnotations;

public class HomeDetail
{
    [Key]
    public int HomeID { get; set; }

    public int ApplicantID { get; set; }
    public string Address { get; set; }

    public Applicant Applicant { get; set; }
}
