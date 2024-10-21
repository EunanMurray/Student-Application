using ScholarshipInfoSystem.Models;

public class Applicant
{
    // Existing properties

    // New properties
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string MobilePhoneNumber { get; set; }
    public string PreferredLeisurewearSize { get; set; }
    public bool IsDeclarationConfirmed { get; set; }
    public string SecondarySchoolAttended { get; set; }
    public bool PriorThirdLevelAttendance { get; set; }
    public string CourseSelectionReasons { get; set; }
    public string SportPositionOrCategory { get; set; }
    public string CurrentClub { get; set; }
    public string PastClubs { get; set; }
    public string HighestCompetitionLevel { get; set; }
    public string SportingAchievements { get; set; }
    public string SportingGoals { get; set; }

    // Relationships
    public ICollection<ApplicantSport> ApplicantSports { get; set; }
    public ICollection<Referee> Referees { get; set; }
    public ICollection<CourseCode> CourseCodes { get; set; }
}
