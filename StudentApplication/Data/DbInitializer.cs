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
            InitializeUserSports(primaryContext, userManager);
            InitializeBudget(primaryContext);
            InitializeHistoricalScholarships(primaryContext);
        }

        private static void InitializeScholarshipTypes(PrimaryContext primaryContext)
        {
            if (!primaryContext.ScholarshipTypes.Any())
            {
                var scholarshipTypes = new ScholarshipType[]
                {
                        new ScholarshipType { ScholarshipLevelName = "Gold", PaymentAmount = 3000 },
                        new ScholarshipType { ScholarshipLevelName = "Silver", PaymentAmount = 1500 },
                        new ScholarshipType { ScholarshipLevelName = "Bronze", PaymentAmount = 500 }
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
                    // Add to PrimaryContext
                    var sport = new Sport
                    {
                        SportName = sportName
                    };
                    primaryContext.Sports.Add(sport);
                    primaryContext.SaveChanges();
                }

                Console.WriteLine($"Added {sportNames.Length} sports.");
            }
        }

        private static void InitializeUserSports(PrimaryContext primaryContext, UserManager<IdentityUser> userManager)
        {
            try
            {
                var testUser = userManager.Users.FirstOrDefault(u => u.Email == "member@example.com");
                if (testUser == null)
                {
                    Console.WriteLine("Test user not found");
                    return;
                }

                var existingUserSport = primaryContext.UserSports
                    .AsNoTracking()
                    .FirstOrDefault(us => us.UserID == testUser.Id);

                if (existingUserSport == null)
                {
                    var soccer = primaryContext.Sports.FirstOrDefault(s => s.SportName == "Soccer");
                    if (soccer != null)
                    {
                        var userSport = new UserSport
                        {
                            UserID = testUser.Id,
                            SportID = soccer.SportID
                        };

                        primaryContext.UserSports.Add(userSport);
                        primaryContext.SaveChanges();
                        Console.WriteLine("Added user sport successfully");
                    }
                    else
                    {
                        Console.WriteLine("Soccer sport not found");
                    }
                }
                else
                {
                    Console.WriteLine("User sport already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InitializeUserSports: {ex.Message}");
            }
        }



        private static async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Committee Member", "Viewer", "Applicant", "ReturningApplicant", "Secretary" };

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
                            FirstName = "Michael",
                            LastName = "O'Connor",
                            CAONumber = "23456789",
                            CollegeYear = 1,
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
                            FirstName = "Sarah",
                            LastName = "Kelly",
                            CAONumber = "34567890",
                            CollegeYear = 3,
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
                            FirstName = "Conor",
                            LastName = "Murphy",
                            CollegeYear = 2,
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
                            FirstName = "Emma",
                            LastName = "Walsh",
                            CollegeYear = 2,
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
                            FirstName = "David",
                            LastName = "Ryan",
                            CollegeYear = 4,
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
                            FirstName = "Aisling",
                            LastName = "Byrne",
                            CAONumber = "78901234",
                            CollegeYear = 1,
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
                            FirstName = "Sean",
                            LastName = "O'Brien",
                            CAONumber = "89012345",
                            CollegeYear = 2,
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
                            FirstName = "Laura",
                            LastName = "Fitzgerald",
                            CAONumber = "90123456",
                            CollegeYear = 1,
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
                        },
                        new Applicant
                        {
                            FirstName = "Patrick",
                            LastName = "O'Sullivan",
                            CAONumber = "01234567",
                            CollegeYear = 1,
                            DateOfBirth = new DateTime(2000, 5, 15),
                            Gender = "Male",
                            PreferredLeisurewearSize = "M",
                            CampusID = 2,
                            SecondarySchoolAttended = "St. Patrick's College",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Passionate about Gaelic football and sports science.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT102" }, new CourseCode { Code = "GAF101" } },
                            CurrentClub = "Dublin GAA Club",
                            PastClubs = "Local GAA Club",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "Local champion in Gaelic football",
                            SportPositionOrCategory = "Midfielder",
                            SportingGoals = "Play for the county team",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 3 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "086-111-2223",
                                Email = "patrick.osullivan@email.com",
                                ParentsPhoneNumber = "086-111-2224",
                                ParentsEmail = "parent.osullivan@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "12 St. Patrick Street, Dublin" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Sean",
                                    TitleOrRole = "GAA Coach",
                                    PhoneNumber = "086-222-3333",
                                    Email = "coach.sean@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Ciara",
                            LastName = "Gallagher",
                            CAONumber = "11234567",
                            CollegeYear = 2,
                            DateOfBirth = new DateTime(1998, 3, 10),
                            Gender = "Female",
                            PreferredLeisurewearSize = "S",
                            CampusID = 1,
                            SecondarySchoolAttended = "Gaelic High School",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Keen on athletics and sports nutrition.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "ATH101" }, new CourseCode { Code = "NUT102" } },
                            CurrentClub = "Galway Athletics Club",
                            PastClubs = "High School Athletics",
                            HighestCompetitionLevel = "Provincial",
                            SportingAchievements = "Won several provincial sprint medals",
                            SportPositionOrCategory = "Sprinter",
                            SportingGoals = "Compete in the Olympics",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 4 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "087-222-3333",
                                Email = "ciara.gallagher@email.com",
                                ParentsPhoneNumber = "087-222-3334",
                                ParentsEmail = "parent.gallagher@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "34 Main Street, Galway" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Liam",
                                    TitleOrRole = "Athletics Coach",
                                    PhoneNumber = "087-333-4444",
                                    Email = "coach.liam@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Liam",
                            LastName = "Brennan",
                            CAONumber = "21234567",
                            CollegeYear = 3,
                            DateOfBirth = new DateTime(1997, 7, 20),
                            Gender = "Male",
                            PreferredLeisurewearSize = "L",
                            CampusID = 3,
                            SecondarySchoolAttended = "Cork Central High",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Interested in strength training and biomechanics.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "STR201" }, new CourseCode { Code = "BIO202" } },
                            CurrentClub = "Cork Weightlifting Club",
                            PastClubs = "Cork Gym",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "Winner of a national lifting contest",
                            SportPositionOrCategory = "Lifter",
                            SportingGoals = "Become a national champion",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 6 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-333-4444",
                                Email = "liam.brennan@email.com",
                                ParentsPhoneNumber = "089-333-4445",
                                ParentsEmail = "parent.brennan@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "78 Fitness Road, Cork" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Fiona",
                                    TitleOrRole = "Strength Coach",
                                    PhoneNumber = "089-444-5555",
                                    Email = "coach.fiona@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Fiona",
                            LastName = "Doyle",
                            CAONumber = "31234567",
                            CollegeYear = 4,
                            DateOfBirth = new DateTime(1996, 12, 5),
                            Gender = "Female",
                            PreferredLeisurewearSize = "M",
                            CampusID = 2,
                            SecondarySchoolAttended = "Limerick Academy",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Avid tennis player aiming to enhance skills.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "TEN101" }, new CourseCode { Code = "SPT303" } },
                            CurrentClub = "Limerick Tennis Club",
                            PastClubs = "Limerick Junior Tennis",
                            HighestCompetitionLevel = "International",
                            SportingAchievements = "Participated in multiple international tournaments",
                            SportPositionOrCategory = "Singles Player",
                            SportingGoals = "Win a Grand Slam title",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 10 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "083-222-3333",
                                Email = "fiona.doyle@email.com",
                                ParentsPhoneNumber = "083-222-3334",
                                ParentsEmail = "parent.doyle@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "22 Tennis Court, Limerick" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Kevin",
                                    TitleOrRole = "Tennis Coach",
                                    PhoneNumber = "083-333-4444",
                                    Email = "coach.kevin@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Cian",
                            LastName = "McCarthy",
                            CAONumber = "41234567",
                            CollegeYear = 1,
                            DateOfBirth = new DateTime(2001, 11, 11),
                            Gender = "Male",
                            PreferredLeisurewearSize = "XL",
                            CampusID = 1,
                            SecondarySchoolAttended = "Dublin Institute",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Passionate about basketball and performance analytics.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT302" }, new CourseCode { Code = "MGT202" } },
                            CurrentClub = "Dublin Dunkers",
                            PastClubs = "Dublin High School Basketball",
                            HighestCompetitionLevel = "Provincial",
                            SportingAchievements = "High school tournament MVP",
                            SportPositionOrCategory = "Point Guard",
                            SportingGoals = "Play in a professional league",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 5 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "085-444-5555",
                                Email = "cian.mccarthy@email.com",
                                ParentsPhoneNumber = "085-444-5556",
                                ParentsEmail = "parent.mccarthy@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "56 Basketball Ave, Dublin" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Emma",
                                    TitleOrRole = "Basketball Coach",
                                    PhoneNumber = "085-555-6666",
                                    Email = "coach.emma@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Aoife",
                            LastName = "Kelly",
                            CAONumber = "51234567",
                            CollegeYear = 2,
                            DateOfBirth = new DateTime(2000, 2, 28),
                            Gender = "Female",
                            PreferredLeisurewearSize = "S",
                            CampusID = 3,
                            SecondarySchoolAttended = "Galway Technical School",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Eager to study sports management and analytics.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT303" }, new CourseCode { Code = "MGT203" } },
                            CurrentClub = "Galway Girls Basketball",
                            PastClubs = "Galway Youth Team",
                            HighestCompetitionLevel = "County",
                            SportingAchievements = "County championship runner-up",
                            SportPositionOrCategory = "Guard",
                            SportingGoals = "Lead the team to victory",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 5 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "087-555-6667",
                                Email = "aoife.kelly@email.com",
                                ParentsPhoneNumber = "087-555-6668",
                                ParentsEmail = "parent.kelly@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "101 Sports Blvd, Galway" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Patrick",
                                    TitleOrRole = "Basketball Coach",
                                    PhoneNumber = "087-666-7777",
                                    Email = "coach.patrick@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Eoin",
                            LastName = "Fitzgerald",
                            CAONumber = "61234567",
                            CollegeYear = 3,
                            DateOfBirth = new DateTime(1998, 6, 18),
                            Gender = "Male",
                            PreferredLeisurewearSize = "M",
                            CampusID = 2,
                            SecondarySchoolAttended = "Limerick College",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Combining sports science with data analytics.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT304" }, new CourseCode { Code = "DAT101" } },
                            CurrentClub = "Limerick Data Racers",
                            PastClubs = "Limerick Sports Club",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "Awarded for data-driven analysis",
                            SportPositionOrCategory = "Striker",
                            SportingGoals = "Innovate sports strategies",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 4 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "083-555-7777",
                                Email = "eoin.fitzgerald@email.com",
                                ParentsPhoneNumber = "083-555-7778",
                                ParentsEmail = "parent.fitzgerald@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "77 Analytics Road, Limerick" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Rory",
                                    TitleOrRole = "Data Coach",
                                    PhoneNumber = "083-666-8888",
                                    Email = "coach.rory@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Niamh",
                            LastName = "O'Brien",
                            CAONumber = "71234567",
                            CollegeYear = 1,
                            DateOfBirth = new DateTime(2002, 1, 22),
                            Gender = "Female",
                            PreferredLeisurewearSize = "XS",
                            CampusID = 1,
                            SecondarySchoolAttended = "Dublin Girls High",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Interests in sports journalism and performance.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "JOU101" }, new CourseCode { Code = "SPT305" } },
                            CurrentClub = "Dublin Reporters",
                            PastClubs = "School Newspaper Team",
                            HighestCompetitionLevel = "Local",
                            SportingAchievements = "School sports award recipient",
                            SportPositionOrCategory = "Midfielder",
                            SportingGoals = "Become a sports analyst",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 7 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "086-777-1111",
                                Email = "niamh.obrien@email.com",
                                ParentsPhoneNumber = "086-777-1112",
                                ParentsEmail = "parent.obrien@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "33 Reporter Lane, Dublin" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Seamus",
                                    TitleOrRole = "Journalism Mentor",
                                    PhoneNumber = "086-888-9999",
                                    Email = "mentor.seamus@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Ronan",
                            LastName = "Walsh",
                            CAONumber = "81234567",
                            CollegeYear = 4,
                            DateOfBirth = new DateTime(1995, 9, 30),
                            Gender = "Male",
                            PreferredLeisurewearSize = "L",
                            CampusID = 3,
                            SecondarySchoolAttended = "Waterford High",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Driven to combine sports with psychology.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "PSY201" }, new CourseCode { Code = "SPT306" } },
                            CurrentClub = "Waterford Minds",
                            PastClubs = "Local Sports Club",
                            HighestCompetitionLevel = "Provincial",
                            SportingAchievements = "Received a sports psychology award",
                            SportPositionOrCategory = "Defender",
                            SportingGoals = "Lead research in sports psychology",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 2 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-666-7777",
                                Email = "ronan.walsh@email.com",
                                ParentsPhoneNumber = "089-666-7778",
                                ParentsEmail = "parent.walsh@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "45 Mindful Street, Waterford" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Brian",
                                    TitleOrRole = "Sports Psychologist",
                                    PhoneNumber = "089-777-8888",
                                    Email = "coach.brian@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Saoirse",
                            LastName = "Murphy",
                            CAONumber = "91234567",
                            CollegeYear = 2,
                            DateOfBirth = new DateTime(2001, 4, 15),
                            Gender = "Female",
                            PreferredLeisurewearSize = "M",
                            CampusID = 2,
                            SecondarySchoolAttended = "Cork Arts and Sports",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Aiming for a career in sports marketing.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "MKT101" }, new CourseCode { Code = "SPT307" } },
                            CurrentClub = "Cork Marketing Club",
                            PastClubs = "Cork Youth Sports",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "Received a marketing innovation award",
                            SportPositionOrCategory = "Forward",
                            SportingGoals = "Become a top sports marketer",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 8 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-888-0000",
                                Email = "saoirse.murphy@email.com",
                                ParentsPhoneNumber = "089-888-0001",
                                ParentsEmail = "parent.murphy@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "60 Market Street, Cork" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Seamus",
                                    TitleOrRole = "Marketing Coach",
                                    PhoneNumber = "089-999-1111",
                                    Email = "coach.seamus@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Cillian",
                            LastName = "Ryan",
                            CAONumber = "02234567",
                            CollegeYear = 3,
                            DateOfBirth = new DateTime(1997, 8, 9),
                            Gender = "Male",
                            PreferredLeisurewearSize = "M",
                            CampusID = 1,
                            SecondarySchoolAttended = "Dublin Sports Institute",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Focused on enhancing athletic performance.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT308" }, new CourseCode { Code = "BIO203" } },
                            CurrentClub = "Dublin Athletics",
                            PastClubs = "Dublin Runners",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "National runner-up in sprinting",
                            SportPositionOrCategory = "Runner",
                            SportingGoals = "Compete in a marathon",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 4 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "087-444-6666",
                                Email = "cillian.ryan@email.com",
                                ParentsPhoneNumber = "087-444-6667",
                                ParentsEmail = "parent.ryan@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "88 Runner Road, Dublin" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Mary",
                                    TitleOrRole = "Athletics Coach",
                                    PhoneNumber = "087-555-7777",
                                    Email = "coach.mary@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Róisín",
                            LastName = "Byrne",
                            CAONumber = "12234567",
                            CollegeYear = 1,
                            DateOfBirth = new DateTime(2002, 7, 3),
                            Gender = "Female",
                            PreferredLeisurewearSize = "XS",
                            CampusID = 3,
                            SecondarySchoolAttended = "Carlow High",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Eager to explore sports broadcasting.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "BRC101" }, new CourseCode { Code = "SPT309" } },
                            CurrentClub = "Carlow Broadcasters",
                            PastClubs = "School Media Club",
                            HighestCompetitionLevel = "Local",
                            SportingAchievements = "Won a local media award",
                            SportPositionOrCategory = "Broadcaster",
                            SportingGoals = "Become a sports commentator",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 7 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-333-2222",
                                Email = "roisin.byrne@email.com",
                                ParentsPhoneNumber = "089-333-2223",
                                ParentsEmail = "parent.byrne@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "10 Broadcaster Lane, Carlow" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Niall",
                                    TitleOrRole = "Media Coach",
                                    PhoneNumber = "089-444-3333",
                                    Email = "coach.niall@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Declan",
                            LastName = "O'Connor",
                            CAONumber = "22234567",
                            CollegeYear = 4,
                            DateOfBirth = new DateTime(1996, 10, 20),
                            Gender = "Male",
                            PreferredLeisurewearSize = "L",
                            CampusID = 2,
                            SecondarySchoolAttended = "Limerick Technical",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Interested in sports analytics and coaching.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "SPT310" }, new CourseCode { Code = "DAT102" } },
                            CurrentClub = "Limerick Analytics",
                            PastClubs = "Limerick Coaching Academy",
                            HighestCompetitionLevel = "Provincial",
                            SportingAchievements = "Received a provincial coach award",
                            SportPositionOrCategory = "Coach",
                            SportingGoals = "Develop innovative coaching methods",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 6 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "083-666-5555",
                                Email = "declan.oconnor@email.com",
                                ParentsPhoneNumber = "083-666-5556",
                                ParentsEmail = "parent.oconnor@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "15 Coaching Lane, Limerick" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Alan",
                                    TitleOrRole = "Analytics Coach",
                                    PhoneNumber = "083-777-8888",
                                    Email = "coach.alan@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Grainne",
                            LastName = "Murphy",
                            CAONumber = "32234567",
                            CollegeYear = 2,
                            DateOfBirth = new DateTime(2001, 3, 12),
                            Gender = "Female",
                            PreferredLeisurewearSize = "M",
                            CampusID = 1,
                            SecondarySchoolAttended = "Dublin Central School",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Passionate about sports rehabilitation.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "RHB101" }, new CourseCode { Code = "SPT311" } },
                            CurrentClub = "Dublin Rehab Club",
                            PastClubs = "Dublin Injury Clinic",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "Won a rehab innovation award",
                            SportPositionOrCategory = "Rehabilitator",
                            SportingGoals = "Establish a sports recovery center",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 5 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "087-666-1111",
                                Email = "grainne.murphy@email.com",
                                ParentsPhoneNumber = "087-666-1112",
                                ParentsEmail = "parent.murphy2@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "22 Recovery Road, Dublin" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Eoin",
                                    TitleOrRole = "Rehab Coach",
                                    PhoneNumber = "087-777-2222",
                                    Email = "coach.eoin@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Eamon",
                            LastName = "O'Leary",
                            CAONumber = "42234567",
                            CollegeYear = 1,
                            DateOfBirth = new DateTime(2002, 12, 1),
                            Gender = "Male",
                            PreferredLeisurewearSize = "XL",
                            CampusID = 3,
                            SecondarySchoolAttended = "Waterford Technical",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Keen on sports event management.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "EVT101" }, new CourseCode { Code = "SPT312" } },
                            CurrentClub = "Waterford Events Club",
                            PastClubs = "Waterford Youth Events",
                            HighestCompetitionLevel = "County",
                            SportingAchievements = "Won an event planning award",
                            SportPositionOrCategory = "Organizer",
                            SportingGoals = "Manage international events",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 3 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-444-2222",
                                Email = "eamon.oleary@email.com",
                                ParentsPhoneNumber = "089-444-2223",
                                ParentsEmail = "parent.oleary@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "30 Event Street, Waterford" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Liam",
                                    TitleOrRole = "Event Coach",
                                    PhoneNumber = "089-555-3333",
                                    Email = "coach.liam2@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Clodagh",
                            LastName = "O'Neill",
                            CAONumber = "52234567",
                            CollegeYear = 3,
                            DateOfBirth = new DateTime(1998, 5, 25),
                            Gender = "Female",
                            PreferredLeisurewearSize = "S",
                            CampusID = 2,
                            SecondarySchoolAttended = "Limerick Girls High",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Aiming for excellence in sports law.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "LAW101" }, new CourseCode { Code = "SPT313" } },
                            CurrentClub = "Limerick Legal Eagles",
                            PastClubs = "School Debate Team",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "Won the school debate championship",
                            SportPositionOrCategory = "Legal Analyst",
                            SportingGoals = "Combine sports and law in a professional career",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 8 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "083-777-3333",
                                Email = "clodagh.oneill@email.com",
                                ParentsPhoneNumber = "083-777-3334",
                                ParentsEmail = "parent.oneill@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "99 Law Street, Limerick" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Patrick",
                                    TitleOrRole = "Legal Mentor",
                                    PhoneNumber = "083-888-4444",
                                    Email = "coach.patrick2@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Finbar",
                            LastName = "Doyle",
                            CAONumber = "62234567",
                            CollegeYear = 2,
                            DateOfBirth = new DateTime(2000, 8, 17),
                            Gender = "Male",
                            PreferredLeisurewearSize = "M",
                            CampusID = 1,
                            SecondarySchoolAttended = "Dublin Sports Academy",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Interested in sports biomechanics.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "BIO204" }, new CourseCode { Code = "SPT314" } },
                            CurrentClub = "Dublin Biomechanics Club",
                            PastClubs = "Dublin High Sports",
                            HighestCompetitionLevel = "Provincial",
                            SportingAchievements = "Awarded for innovative biomechanics research",
                            SportPositionOrCategory = "Forward",
                            SportingGoals = "Revolutionize training techniques",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 4 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "087-777-4444",
                                Email = "finbar.doyle@email.com",
                                ParentsPhoneNumber = "087-777-4445",
                                ParentsEmail = "parent.doyle2@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "44 Bio Road, Dublin" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Mary",
                                    TitleOrRole = "Biomechanics Coach",
                                    PhoneNumber = "087-888-5555",
                                    Email = "coach.mary2@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Orlaith",
                            LastName = "Gallagher",
                            CAONumber = "72234567",
                            CollegeYear = 4,
                            DateOfBirth = new DateTime(1996, 3, 14),
                            Gender = "Female",
                            PreferredLeisurewearSize = "M",
                            CampusID = 3,
                            SecondarySchoolAttended = "Waterford Girls Technical",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Combining sports with digital media.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "MED101" }, new CourseCode { Code = "SPT315" } },
                            CurrentClub = "Waterford Digital Sports",
                            PastClubs = "Waterford High Media",
                            HighestCompetitionLevel = "International",
                            SportingAchievements = "Digital sports innovation award",
                            SportPositionOrCategory = "Defender",
                            SportingGoals = "Innovate sports broadcasting",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 5 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-555-4444",
                                Email = "orlaith.gallagher@email.com",
                                ParentsPhoneNumber = "089-555-4445",
                                ParentsEmail = "parent.gallagher2@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "10 Digital Ave, Waterford" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Fiona",
                                    TitleOrRole = "Media Coach",
                                    PhoneNumber = "089-666-5555",
                                    Email = "coach.fiona2@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Donal",
                            LastName = "Lynch",
                            CAONumber = "82234567",
                            CollegeYear = 1,
                            DateOfBirth = new DateTime(2003, 10, 6),
                            Gender = "Male",
                            PreferredLeisurewearSize = "L",
                            CampusID = 2,
                            SecondarySchoolAttended = "Cork High",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Keen on integrating sports and technology.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "TEC101" }, new CourseCode { Code = "SPT316" } },
                            CurrentClub = "Cork Tech Sports",
                            PastClubs = "Cork High Tech",
                            HighestCompetitionLevel = "County",
                            SportingAchievements = "Tech sports innovation award",
                            SportPositionOrCategory = "Midfielder",
                            SportingGoals = "Develop sports tech solutions",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 7 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-666-3333",
                                Email = "donal.lynch@email.com",
                                ParentsPhoneNumber = "089-666-3334",
                                ParentsEmail = "parent.lynch@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "88 Tech Road, Cork" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Eamon",
                                    TitleOrRole = "Tech Coach",
                                    PhoneNumber = "089-777-4444",
                                    Email = "coach.eamon2@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Sínead",
                            LastName = "Duffy",
                            CAONumber = "92234567",
                            CollegeYear = 3,
                            DateOfBirth = new DateTime(1998, 11, 29),
                            Gender = "Female",
                            PreferredLeisurewearSize = "XS",
                            CampusID = 1,
                            SecondarySchoolAttended = "Dublin Modern School",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Passionate about sports nutrition and health.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "NUT103" }, new CourseCode { Code = "SPT317" } },
                            CurrentClub = "Dublin Nutrition Club",
                            PastClubs = "Dublin Wellness Team",
                            HighestCompetitionLevel = "National",
                            SportingAchievements = "Nutrition award winner",
                            SportPositionOrCategory = "Forward",
                            SportingGoals = "Enhance athlete performance",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 8 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "087-888-9999",
                                Email = "sinead.duffy@email.com",
                                ParentsPhoneNumber = "087-888-0000",
                                ParentsEmail = "parent.duffy@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "77 Health Ave, Dublin" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Orla",
                                    TitleOrRole = "Nutrition Coach",
                                    PhoneNumber = "087-999-1111",
                                    Email = "coach.orla@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Tara",
                            LastName = "Quinn",
                            CAONumber = "03234567",
                            CollegeYear = 2,
                            DateOfBirth = new DateTime(2001, 5, 5),
                            Gender = "Female",
                            PreferredLeisurewearSize = "M",
                            CampusID = 3,
                            SecondarySchoolAttended = "Carlow Institute",
                            PriorThirdLevelAttendance = false,
                            CourseSelectionReasons = "Aiming to excel in sports journalism.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "JOU102" }, new CourseCode { Code = "SPT318" } },
                            CurrentClub = "Carlow News Sports",
                            PastClubs = "Carlow Media Club",
                            HighestCompetitionLevel = "Local",
                            SportingAchievements = "Local journalist award winner",
                            SportPositionOrCategory = "Midfielder",
                            SportingGoals = "Join a major sports network",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 7 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "089-999-2222",
                                Email = "tara.quinn@email.com",
                                ParentsPhoneNumber = "089-999-2223",
                                ParentsEmail = "parent.quinn@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "55 Media Lane, Carlow" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Fionn",
                                    TitleOrRole = "Journalism Coach",
                                    PhoneNumber = "089-888-3333",
                                    Email = "coach.fionn@email.com"
                                }
                            }
                        },
                        new Applicant
                        {
                            FirstName = "Cormac",
                            LastName = "O'Donnell",
                            CAONumber = "13234567",
                            CollegeYear = 4,
                            DateOfBirth = new DateTime(1995, 7, 12),
                            Gender = "Male",
                            PreferredLeisurewearSize = "XL",
                            CampusID = 2,
                            SecondarySchoolAttended = "Limerick Elite School",
                            PriorThirdLevelAttendance = true,
                            CourseSelectionReasons = "Dedicated to sports medicine and research.",
                            CourseCodes = new List<CourseCode> { new CourseCode { Code = "MED102" }, new CourseCode { Code = "SPT319" } },
                            CurrentClub = "Limerick Medical Sports",
                            PastClubs = "Limerick University Team",
                            HighestCompetitionLevel = "International",
                            SportingAchievements = "Awarded a medical sports research grant",
                            SportPositionOrCategory = "Defender",
                            SportingGoals = "Advance sports medicine research",
                            IsDeclarationConfirmed = true,
                            ApplicantSports = new List<ApplicantSport> { new ApplicantSport { SportID = 6 } },
                            ContactDetail = new ContactDetail
                            {
                                PhoneNumber = "083-999-4444",
                                Email = "cormac.odonnell@email.com",
                                ParentsPhoneNumber = "083-999-4445",
                                ParentsEmail = "parent.odonnell@email.com"
                            },
                            HomeDetail = new HomeDetail { Address = "100 Medical Ave, Limerick" },
                            Referees = new List<Referee>
                            {
                                new Referee
                                {
                                    Name = "Coach Brendan",
                                    TitleOrRole = "Medical Coach",
                                    PhoneNumber = "083-888-5555",
                                    Email = "coach.brendan@email.com"
                                }
                            }
                        }
                    };



                primaryContext.Applicants.AddRange(testApplicants);
                primaryContext.SaveChanges();

                Console.WriteLine($"Added {testApplicants.Length} test applicants to PrimaryContext.");
            }


        }

        private static void InitializeHistoricalScholarships(PrimaryContext primaryContext)
        {
            if (!primaryContext.ScholarshipOfferHistories.Any(s => s.OfferDate.Year < DateTime.UtcNow.Year))
            {
                var scholarshipTypes = primaryContext.ScholarshipTypes.ToList();

                var applicants = primaryContext.Applicants
                    .Include(a => a.ApplicantSports)
                    .ToList();

                if (!applicants.Any() || !scholarshipTypes.Any())
                    return;

                var pastYears = new[] { 2023, 2024 };
                var random = new Random();

                foreach (var year in pastYears)
                {
                    for (int i = 0; i < 10 && i < applicants.Count; i++)
                    {
                        var applicant = applicants[i];

                        if (i < 5) 
                        {
                            applicant.DateSubmitted = new DateTime(year, random.Next(1, 12), random.Next(1, 28));
                            primaryContext.Applicants.Update(applicant);
                        }

                        var scholarshipType = scholarshipTypes[random.Next(scholarshipTypes.Count)];

                        var scholarship = new Scholarship
                        {
                            ScholarshipTypeID = scholarshipType.ScholarshipTypeID,
                            OtherDetails = $"Historical scholarship for {year}",
                            hasAccepted = true
                        };

                        primaryContext.Scholarships.Add(scholarship);
                        primaryContext.SaveChanges();

                        var sportID = applicant.ApplicantSports.FirstOrDefault()?.SportID ?? 1;

                        var offerHistory = new ScholarshipOfferHistory
                        {
                            ApplicantID = applicant.ApplicantID,
                            SportID = sportID,
                            CampusID = applicant.CampusID,
                            ScholarshipID = scholarship.ScholarshipID,
                            OfferDate = new DateTime(year, random.Next(1, 12), random.Next(1, 28)),
                            ResponseDate = new DateTime(year, random.Next(1, 12), random.Next(1, 28)),
                            ResponseStatus = "Accepted",
                            Stage = "Completed"
                        };

                        primaryContext.ScholarshipOfferHistories.Add(offerHistory);
                    }
                }

                primaryContext.SaveChanges();
                Console.WriteLine($"Created historical scholarship data for {pastYears.Length} past years");
            }
        }
        private static void InitializeBudget(PrimaryContext primaryContext)
        {
            var currentYear = DateTime.UtcNow.Year.ToString();
            if (!primaryContext.Budgets.Any(b => b.BudgetYear == currentYear))
            {
                var budget = new Budget { BudgetAmount = 80000, BudgetYear = currentYear };
                primaryContext.Budgets.Add(budget);
            }
            var pastYears = new[] { "2023", "2024" };
            foreach (var year in pastYears)
            {
                if (!primaryContext.Budgets.Any(b => b.BudgetYear == year))
                {
                    var pastBudget = new Budget { BudgetAmount = 75000, BudgetYear = year };
                    primaryContext.Budgets.Add(pastBudget);
                }
            }
            primaryContext.SaveChanges();
        }
    }
}