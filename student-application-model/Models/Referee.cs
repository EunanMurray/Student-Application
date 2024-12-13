using System.ComponentModel.DataAnnotations;

public class Referee
{
    [Key]
    public int RefereeID { get; set; }

    public int ApplicantID { get; set; }

    public string Name { get; set; }
    public string TitleOrRole { get; set; }
    public string PhoneNumber { get; set; }
    [EmailAddress]
    public string Email { get; set; }

    // Navigation Property
    public Applicant Applicant { get; set; }
}
