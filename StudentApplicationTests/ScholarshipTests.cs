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
            Assert.That(scholarship.ScholarshipID, Is.EqualTo(1));
            Assert.That(scholarship.ScholarshipTypeID, Is.EqualTo(1));
            Assert.That(scholarship.Applicants, Is.Not.Null);
            Assert.That(scholarship.Applicants.Count, Is.EqualTo(1));
        }
    }
}