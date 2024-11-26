using System;
using System.Linq;
using System.Threading.Tasks;
using StudentApplicationModel.Models;
using student_application_model.Models;
using Microsoft.AspNetCore.Identity;
using StudentApplicationPages.Data;
using StudentApplicationModel.Data;

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
            InitializeSports(primaryContext, applicationContext);
            InitializeDefaultAdmin(userManager).Wait();
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

        private static void InitializeSports(PrimaryContext primaryContext, ApplicationDbContext applicationContext)
        {
            if (!primaryContext.Sports.Any())
            {
                var sports = new[]
                {
                    new Sport { SportName = "Soccer" },
                    new Sport { SportName = "Gaelic Football" },
                    new Sport { SportName = "Hurling" },
                    new Sport { SportName = "Rugby" },
                    new Sport { SportName = "Basketball" },
                    new Sport { SportName = "Athletics" },
                    new Sport { SportName = "Swimming" },
                    new Sport { SportName = "Cycling" },
                    new Sport { SportName = "Golf" },
                    new Sport { SportName = "Tennis" }
                };

                primaryContext.Sports.AddRange(sports);
                primaryContext.SaveChanges();

                var identitySports = sports.Select(s => new SportIdentity
                {
                    SportID = s.SportID,
                    SportName = s.SportName
                }).ToList();

                applicationContext.Sports.AddRange(identitySports);
                applicationContext.SaveChanges();
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
    }
}