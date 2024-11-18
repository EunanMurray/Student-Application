using System.ComponentModel.DataAnnotations;

public class CourseCode
{
    [Key]
    public int CourseCodeID { get; set; }

    public int ApplicantID { get; set; }
    public string Code { get; set; }

    public Applicant Applicant { get; set; }
}
