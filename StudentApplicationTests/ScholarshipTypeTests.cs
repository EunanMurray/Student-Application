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
            Assert.AreEqual(1, scholarshipType.ScholarshipTypeID);
            Assert.AreEqual("Undergraduate", scholarshipType.ScholarshipLevelName);
            Assert.AreEqual(1000m, scholarshipType.PaymentAmount);
        }
    }
}
