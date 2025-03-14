using NUnit.Framework;
using StudentApplicationModel.Models;
namespace StudentApplicationTests
{
    [TestFixture]
    public class ScholarshipApplicationTests
    {
        [Test]
        public void ScholarshipApplicationProperties_ShouldBeSetAndGet()
        {
            // Arrange
            var scholarshipApplication = new ScholarshipApplication();
            // Act
            scholarshipApplication.ApplicationID = 1;
            scholarshipApplication.ApplicantID = 1;
            scholarshipApplication.Year = 2025;
            scholarshipApplication.ApplicationType = "Need-based";
            // Assert
            Assert.That(scholarshipApplication.ApplicationID, Is.EqualTo(1));
            Assert.That(scholarshipApplication.ApplicantID, Is.EqualTo(1));
            Assert.That(scholarshipApplication.Year, Is.EqualTo(2025));
            Assert.That(scholarshipApplication.ApplicationType, Is.EqualTo("Need-based"));
        }
    }
}