using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Data;
using ScholarshipInfoSystem.Models;
using System.Linq;

namespace StudentApplicationPages.Tests
{
    [TestFixture]
    public class ContextTests2
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
        public void PrimaryContext_Should_AddAndRetrieve_ApplicantWithAllRequiredFields()
        {
            // Arrange
            var contactDetail = new ContactDetail
            {
                ApplicantID = 1,
                PhoneNumber = "123-456-7890",
                Email = "johndoe@example.com"
            };

            var applicant = new Applicant
            {
                ApplicantID = 1,
                Name = "John Doe",
                CAONumber = "123456",
                ApplicationStatus = "notReviewed",
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = "Male",
                PreferredLeisurewearSize = "M",
                IsDeclarationConfirmed = true,
                SecondarySchoolAttended = "High School Name",
                PriorThirdLevelAttendance = false,
                CourseSelectionReasons = "Interest in the field",
                SportPositionOrCategory = "Midfielder",
                CurrentClub = "Local Club",
                PastClubs = "Club A, Club B",
                HighestCompetitionLevel = "National",
                SportingAchievements = "Won regional championship",
                SportingGoals = "Compete at the national level",
                ContactDetail = contactDetail // Establish the one-to-one relationship
            };

            _primaryContext.Applicants.Add(applicant);

            // Act
            _primaryContext.SaveChanges();
            var retrievedApplicant = _primaryContext.Applicants
                .Include(a => a.ContactDetail)
                .FirstOrDefault(a => a.ApplicantID == 1);

            // Assert
            Assert.That(retrievedApplicant, Is.Not.Null);
            Assert.That(retrievedApplicant.ContactDetail, Is.Not.Null);
            Assert.That(retrievedApplicant.Name, Is.EqualTo("John Doe"));
            Assert.That(retrievedApplicant.ContactDetail.PhoneNumber, Is.EqualTo("123-456-7890"));
            Assert.That(retrievedApplicant.ContactDetail.Email, Is.EqualTo("johndoe@example.com"));
        }
    }
}
