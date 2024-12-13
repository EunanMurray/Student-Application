using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Data;
using ScholarshipInfoSystem.Models;
using System.Linq;

namespace StudentApplicationPages.Tests
{
    [TestFixture]
    public class ContextTests
    {
        private PrimaryContext _primaryContext;
        private SecondaryContext _secondaryContext;

        [SetUp]
        public void Setup()
        {
            var primaryOptions = new DbContextOptionsBuilder<PrimaryContext>()
                .UseInMemoryDatabase(databaseName: "PrimaryTestDatabase")
                .Options;
            _primaryContext = new PrimaryContext(primaryOptions);

            var secondaryOptions = new DbContextOptionsBuilder<SecondaryContext>()
                .UseInMemoryDatabase(databaseName: "SecondaryTestDatabase")
                .Options;
            _secondaryContext = new SecondaryContext(secondaryOptions);
        }

        [TearDown]
        public void Teardown()
        {
            _primaryContext.Database.EnsureDeleted();
            _secondaryContext.Database.EnsureDeleted();
            _primaryContext.Dispose();
            _secondaryContext.Dispose();
        }

        [Test]
        public void PrimaryContext_Should_AddAndRetrieve_Applicant()
        {
            // Arrange
            var applicant = new Applicant
            {
                ApplicantID = 1,
                Name = "John Doe",
                CAONumber = "123456", 
                CourseSelectionReasons = "Interest in the field", 
                CurrentClub = "Local Club",
                Gender = "Male", 
                HighestCompetitionLevel = "National", 
                PastClubs = "Club A, Club B", 
                PreferredLeisurewearSize = "M", 
                SecondarySchoolAttended = "High School Name", 
                SportPositionOrCategory = "Midfielder", 
                SportingAchievements = "Won regional championship", 
                SportingGoals = "Compete at the national level" 
            };
            _primaryContext.Applicants.Add(applicant);

            // Act
            _primaryContext.SaveChanges();
            var retrievedApplicant = _primaryContext.Applicants.FirstOrDefault(a => a.ApplicantID == 1);

            // Assert (using constraint-based model)
            Assert.That(retrievedApplicant, Is.Not.Null);
            Assert.That(retrievedApplicant.Name, Is.EqualTo("John Doe"));
        }


        [Test]
        public void SecondaryContext_Should_AddAndRetrieve_Role()
        {
            // Arrange
            var role = new Role { RoleID = 1, Name = "Admin" };
            _secondaryContext.Roles.Add(role);

            // Act
            _secondaryContext.SaveChanges();
            var retrievedRole = _secondaryContext.Roles.FirstOrDefault(r => r.RoleID == 1);

            // Assert (using constraint-based model)
            Assert.That(retrievedRole, Is.Not.Null);
            Assert.That(retrievedRole.Name, Is.EqualTo("Admin"));
        }
    }
}
