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
            Assert.AreEqual(1, scholarshipApplication.ApplicationID);
            Assert.AreEqual(1, scholarshipApplication.ApplicantID);
            Assert.AreEqual(2025, scholarshipApplication.Year);
            Assert.AreEqual("Need-based", scholarshipApplication.ApplicationType);
        }
    }
}
