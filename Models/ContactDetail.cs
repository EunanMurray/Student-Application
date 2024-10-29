using System.ComponentModel.DataAnnotations;

public class ContactDetail
{
    [Key]
    public int ContactID { get; set; }

    public int ApplicantID { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string ParentsPhoneNumber { get; set; }
    public string ParentsEmail { get; set; }

    public Applicant Applicant { get; set; }
}
