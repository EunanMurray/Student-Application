using NUnit.Framework;
using StudentApplicationModel.Models;
using System.Collections.Generic;

namespace StudentApplicationTests
{
    [TestFixture]
    public class ScholarshipTests
    {
        [Test]
        public void ScholarshipProperties_ShouldBeSetAndGet()
        {
            // Arrange
            var scholarship = new Scholarship();

            // Act
            scholarship.ScholarshipID = 1;
            scholarship.ScholarshipTypeID = 1;
            scholarship.Applicants = new List<Applicant>();
            scholarship.Applicants.Add(new Applicant());

            // Assert
            Assert.AreEqual(1, scholarship.ScholarshipID);
            Assert.AreEqual(1, scholarship.ScholarshipTypeID);
            Assert.IsNotNull(scholarship.Applicants);
            Assert.AreEqual(1, scholarship.Applicants.Count);
        }
    }
}
