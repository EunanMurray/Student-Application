using System;
using System.Linq;
using System.Threading.Tasks;
using StudentApplicationModel.Models;
using student_application_model.Models;
using Microsoft.AspNetCore.Identity;
using StudentApplicationPages.Data;
using StudentApplicationModel.Data;
using Microsoft.EntityFrameworkCore;

namespace ScholarshipInfoSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PrimaryContext primaryContext, ApplicationDbContext applicationContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            primaryContext.Database.EnsureCreated();
            applicationContext.Database.EnsureCreated();

            InitializeRoles(roleManager).Wait();

            InitializeScholarshipTypes(primaryContext);
            InitializeCampuses(primaryContext);
            InitializeSports(primaryContext);
            InitializeDefaultAdmin(userManager).Wait();
            InitializeDefaultCommitteeMember(userManager).Wait();
            InitializeTestApplicants(primaryContext);
        }

        private static void InitializeScholarshipTypes(PrimaryContext primaryContext)
        {
            if (!primaryContext.ScholarshipTypes.Any())
            {
                var scholarshipTypes = new ScholarshipType[]
                {
                        new ScholarshipType { ScholarshipLevelName = "Gold", PaymentAmount = 10000 },
                        new ScholarshipType { ScholarshipLevelName = "Silver", PaymentAmount = 5000 },
                        new ScholarshipType { ScholarshipLevelName = "Bronze", PaymentAmount = 2500 }
                };

                foreach (var s in scholarshipTypes)
                {
                    primaryContext.ScholarshipTypes.Add(s);
                }
                primaryContext.SaveChanges();
            }
        }

        private static void InitializeCampuses(PrimaryContext primaryContext)
        {
            if (!primaryContext.Campuses.Any())
            {
                var campuses = new Campus[]
                {
                        new Campus { CampusName = "Sligo" },
                        new Campus { CampusName = "Letterkenny" },
                        new Campus { CampusName = "Galway" },
                };

                foreach (var c in campuses)
                {
                    primaryContext.Campuses.Add(c);
                }
                primaryContext.SaveChanges();
            }
        }

        private static void InitializeSports(PrimaryContext primaryContext)
        {
            if (!primaryContext.Sports.Any())
            {
                Console.WriteLine("Adding sports to ApplicationDbContext.");

                var sportNames = new[]
                {
            "Soccer", "Gaelic Football", "Hurling", "Rugby",
            "Basketball", "Athletics", "Swimming", "Cycling",
            "Golf", "Tennis"
        };

                foreach (var sportName in sportNames)
                {
                    // Add to ApplicationDbContext
                    //var identitySport = new IdentitySport { SportName = sportName };
                    //applicationContext.Sports.Add(identitySport);
                    //applicationContext.SaveChanges();

                    // Add to PrimaryContext
                    var sport = new Sport
                    {
                        //SportID = identitySport.SportID
                        SportName = sportName
                    };
                    primaryContext.Sports.Add(sport);
                    primaryContext.SaveChanges();
                }

                Console.WriteLine($"Added {sportNames.Length} sports.");
            }
        }

        private static async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Committee Member", "Viewer" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task InitializeDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }

        private static async Task InitializeDefaultCommitteeMember(UserManager<IdentityUser> userManager)
        {
            const string committeeMemberEmail = "member@example.com";
            const string committeeMemberPassword = "Member123!";

            if (await userManager.FindByEmailAsync(committeeMemberEmail) == null)
            {
                var committeeMember = new IdentityUser
                {
                    UserName = committeeMemberEmail,
                    Email = committeeMemberEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(committeeMember, committeeMemberPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(committeeMember, "Committee Member");
                }
            }
        }

        private static void InitializeTestApplicants(PrimaryContext primaryContext)
        {
            if (!primaryContext.Applicants.Any())
            {
                Console.WriteLine("Adding test applicants to PrimaryContext.");

                var testApplicants = new[]
                {
                        new Applicant
                        {
                            Name = "Michael O'Connor",
                            CAONumber = "23456789",
                            DateOfBirth = new DateTime(1999, 8, 12),
                            Gender = "Male",
                            PreferredLeisurewearSize = "L",
                            CampusID = 1,
                            SecondarySchoolAttended = "Galway Community School",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Interest in sports science and performance analysis.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT101" }, new CourseCode { Code = "SPT202" } },
                            CurrentClub = "Galway Rugby Club",
                            PastClubs = "School Rugby Team",
                            HighestCompetitionLevel = "Provincial",
                            SportingAchievements = "Team Captain, Provincial Champions 2023",
                            SportPositionOrCategory = "Flanker",
                            SportingGoals = "Professional rugby career",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new[] { new ApplicantSport { SportID = 4 } }.ToList(),
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "087-555-1234",
                                Email = "michael.oconnor@email.com",
                                ParentsPhoneNumber = "087-555-5678",
                                ParentsEmail = "parent.oconnor@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "15 Castle Road, Galway" },
                            Referees = new[] { new Referee {
                                Name = "Coach Patrick",
                                TitleOrRole = "Rugby Head Coach",
                                PhoneNumber = "087-555-9012",
                                Email = "patrick.coach@email.com"
                            } }.ToList()
                        },
                        new Applicant
                        {
                            Name = "Sarah Kelly",
                            CAONumber = "34567890",
                            DateOfBirth = new DateTime(2001, 4, 30),
                            Gender = "Female",
                            PreferredLeisurewearSize = "XS",
                            CampusID = 3,
                            SecondarySchoolAttended = "Dublin Academy",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Passionate about basketball and sports management.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT301" }, new CourseCode { Code = "MGT101" } },
                            CurrentClub = "Dublin Wildcats",
                            PastClubs = "Youth Basketball Association",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "National U20 Basketball Team Player",
                            SportPositionOrCategory = "Point Guard",
                            SportingGoals = "Become a professional basketball coach",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new[] { new ApplicantSport { SportID = 5 } }.ToList(),
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "085-123-4567",
                                Email = "sarah.kelly@email.com",
                                ParentsPhoneNumber = "085-765-4321",
                                ParentsEmail = "kelly.parents@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "78 Oak Avenue, Dublin" },
                            Referees = new[] { new Referee {
                                Name = "Coach Lisa",
                                TitleOrRole = "Basketball Coach",
                                PhoneNumber = "085-999-8888",
                                Email = "lisa.coach@email.com"
                            } }.ToList()
                        },
                        new Applicant
                        {
                            Name = "Conor Murphy",
                            CAONumber = "45678901",
                            DateOfBirth = new DateTime(2000, 11, 8),
                            Gender = "Male",
                            PreferredLeisurewearSize = "M",
                            CampusID = 2,
                            SecondarySchoolAttended = "Cork Sports Academy",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Dedicated to swimming and sports therapy.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT401" }, new CourseCode { Code = "BIO101" } },
                            CurrentClub = "Cork Swimming Club",
                            PastClubs = "Dolphin Swimming Academy",
                            HighestCompetitionLevel = "International",
                            SportingAchievements = "National Swimming Championship Finalist",
                            SportPositionOrCategory = "Freestyle Specialist",
                            SportingGoals = "Olympic qualification",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new[] { new ApplicantSport { SportID = 7 } }.ToList(),
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-222-3333",
                                Email = "conor.murphy@email.com",
                                ParentsPhoneNumber = "089-444-5555",
                                ParentsEmail = "murphy.parents@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "25 Marina View, Cork" },
                            Referees = new[] { new Referee {
                                Name = "Coach James",
                                TitleOrRole = "Swimming Head Coach",
                                PhoneNumber = "089-666-7777",
                                Email = "james.coach@email.com"
                            } }.ToList()
                        },
                        new Applicant
                        {
                            Name = "Emma Walsh",
                            CAONumber = "56789012",
                            DateOfBirth = new DateTime(1999, 2, 14),
                            Gender = "Female",
                            PreferredLeisurewearSize = "M",
                            CampusID = 1,
                            SecondarySchoolAttended = "Limerick Sports School",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Passionate about tennis and sports education.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "EDU201" }, new CourseCode { Code = "SPT301" } },
                            CurrentClub = "Limerick Tennis Club",
                            PastClubs = "School Tennis Team",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "Best Server Award 2023",
                            SportPositionOrCategory = "Singles Player",
                            SportingGoals = "National team selection and coaching career",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new[] { new ApplicantSport { SportID = 10 } }.ToList(),
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "083-111-2222",
                                Email = "emma.walsh@email.com",
                                ParentsPhoneNumber = "083-333-4444",
                                ParentsEmail = "walsh.family@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "92 River Walk, Limerick" },
                            Referees = new[] { new Referee {
                                Name = "Coach Sarah",
                                TitleOrRole = "Tennis Coach",
                                PhoneNumber = "083-555-6666",
                                Email = "sarah.tennis@email.com"
                            } }.ToList()
                        },
                        new Applicant
                        {
                            Name = "David Ryan",
                            CAONumber = "67890123",
                            DateOfBirth = new DateTime(2001, 7, 19),
                            Gender = "Male",
                            PreferredLeisurewearSize = "L",
                            CampusID = 3,
                            SecondarySchoolAttended = "Waterford College",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Dedicated to GAA and sports psychology.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "PSY101" }, new CourseCode { Code = "SPT202" } },
                            CurrentClub = "Waterford GAA Club",
                            PastClubs = "Junior GAA Club",
                            HighestCompetitionLevel = "County",
                            SportingAchievements = "County Minor Championship Winner",
                            SportPositionOrCategory = "Midfielder",
                            SportingGoals = "Senior inter-county player",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new[] { new ApplicantSport { SportID = 2 } }.ToList(),
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "086-777-8888",
                                Email = "david.ryan@email.com",
                                ParentsPhoneNumber = "086-999-0000",
                                ParentsEmail = "ryan.parents@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "56 Quay Street, Waterford" },
                            Referees = new[] { new Referee {
                                Name = "Coach Brendan",
                                TitleOrRole = "GAA Coach",
                                PhoneNumber = "086-111-2222",
                                Email = "brendan.gaa@email.com"
                            } }.ToList()
                        },
                        new Applicant
                        {
                            Name = "Aisling Byrne",
                            CAONumber = "78901234",
                            DateOfBirth = new DateTime(2000, 9, 3),
                            Gender = "Female",
                            PreferredLeisurewearSize = "S",
                            CampusID = 2,
                            SecondarySchoolAttended = "Kilkenny Sports Institute",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Passionate about tennis and sports nutrition.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "NUT101" }, new CourseCode { Code = "SPT401" } },
                            CurrentClub = "Kilkenny Tennis Club",
                            PastClubs = "Youth Tennis Academy",
                            HighestCompetitionLevel = "Provincial",
                            SportingAchievements = "Leinster Junior Champion 2022",
                            SportPositionOrCategory = "Singles Player",
                            SportingGoals = "Professional tennis career and coaching",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new[] { new ApplicantSport { SportID = 10 } }.ToList(),
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "085-333-4444",
                                Email = "aisling.byrne@email.com",
                                ParentsPhoneNumber = "085-555-6666",
                                ParentsEmail = "byrne.family@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "17 Castle View, Kilkenny" },
                            Referees = new[] { new Referee {
                                Name = "Coach Maria",
                                TitleOrRole = "Tennis Head Coach",
                                PhoneNumber = "085-777-8888",
                                Email = "maria.tennis@email.com"
                            } }.ToList()
                        },
                        new Applicant
                        {
                            Name = "Sean O'Brien",
                            CAONumber = "89012345",
                            DateOfBirth = new DateTime(1999, 12, 21),
                            Gender = "Male",
                            PreferredLeisurewearSize = "XL",
                            CampusID = 1,
                            SecondarySchoolAttended = "Wexford Academy",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Focus on strength and conditioning coaching.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "STR101" }, new CourseCode { Code = "SPT302" } },
                            CurrentClub = "Wexford Weightlifting",
                            PastClubs = "CrossFit Wexford",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "National Weightlifting Championship Medalist",
                            SportPositionOrCategory = "Weightlifting/Strength Events",
                            SportingGoals = "Open own strength and conditioning facility",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new[] { new ApplicantSport { SportID = 6 } }.ToList(),
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "087-999-0000",
                                Email = "sean.obrien@email.com",
                                ParentsPhoneNumber = "087-111-2222",
                                ParentsEmail = "obrien.parents@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "34 Harbor View, Wexford" },
                            Referees = new[] { new Referee {
                                Name = "Coach Daniel",
                                TitleOrRole = "Strength Coach",
                                PhoneNumber = "087-333-4444",
                                Email = "daniel.strength@email.com"
                            } }.ToList()
                        },
                        new Applicant
                        {
                            Name = "Laura Fitzgerald",
                            CAONumber = "90123456",
                            DateOfBirth = new DateTime(2001, 1, 5),
                            Gender = "Female",
                            PreferredLeisurewearSize = "M",
                            CampusID = 3,
                            SecondarySchoolAttended = "Carlow Sports School",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Passionate about tennis and sports development.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT201" }, new CourseCode { Code = "DEV101" } },
                            CurrentClub = "Carlow Tennis Club",
                            PastClubs = "School Tennis Team",
                            HighestCompetitionLevel = "Provincial",
                            SportingAchievements = "Provincial Team Captain",
                            SportPositionOrCategory = "Doubles Specialist",
                            SportingGoals = "National team selection",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new[] { new ApplicantSport { SportID = 10 } }.ToList(),
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-555-6666",
                                Email = "laura.fitzgerald@email.com",
                                ParentsPhoneNumber = "089-777-8888",
                                ParentsEmail = "fitzgerald.family@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "45 Green Lane, Carlow" },
                            Referees = new[] { new Referee {
                                Name = "Coach Rebecca",
                                TitleOrRole = "Tennis Coach",
                                PhoneNumber = "089-999-0000",
                                Email = "rebecca.tennis@email.com"
                            } }.ToList()
                        }
                    };

                primaryContext.Applicants.AddRange(testApplicants);
                primaryContext.SaveChanges();

                Console.WriteLine($"Added {testApplicants.Length} test applicants to PrimaryContext.");
            }
        }
    }
}