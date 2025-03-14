using NUnit.Framework;
using StudentApplicationModel.Models;
namespace StudentApplicationTests
{
    [TestFixture]
    public class ScholarshipTypeTests
    {
        [Test]
        public void ScholarshipTypeProperties_ShouldBeSetAndGet()
        {
            // Arrange
            var scholarshipType = new ScholarshipType();
            // Act
            scholarshipType.ScholarshipTypeID = 1;
            scholarshipType.ScholarshipLevelName = "Undergraduate";
            scholarshipType.PaymentAmount = 1000m;
            // Assert
            Assert.That(scholarshipType.ScholarshipTypeID, Is.EqualTo(1));
            Assert.That(scholarshipType.ScholarshipLevelName, Is.EqualTo("Undergraduate"));
            Assert.That(scholarshipType.PaymentAmount, Is.EqualTo(1000m));
        }
    }
}