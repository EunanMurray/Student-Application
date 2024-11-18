using System;
using System.Linq;
using StudentApplicationModel.Models; 
using ScholarshipInfoSystem.Data;
using StudentApplicationModel.Data;

namespace ScholarshipInfoSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PrimaryContext context)
        {
            context.Database.EnsureCreated();

   
            if (context.ScholarshipTypes.Any())
            {
                return;   
            }

            var scholarshipTypes = new ScholarshipType[]
            {
                new ScholarshipType { ScholarshipLevelName = "Gold", PaymentAmount = 10000 },
                new ScholarshipType { ScholarshipLevelName = "Silver", PaymentAmount = 5000 },
                new ScholarshipType { ScholarshipLevelName = "Bronze", PaymentAmount = 2500 }
            };

            foreach (var s in scholarshipTypes)
            {
                context.ScholarshipTypes.Add(s);
            }
            context.SaveChanges();

            if (context.Campuses.Any())
            {
                return;   
            }

            var campuses = new Campus[]
            {
                new Campus { CampusName = "Sligo" },
                new Campus { CampusName = "Letterkenny" },
                new Campus { CampusName = "Galway" },
            };  

            foreach (var c in campuses)
            {
                context.Campuses.Add(c);
            }
            context.SaveChanges();

            if (context.Sports.Any())
            {
                return;   
            }

            var sports = new Sport[]
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

            foreach (var s in sports)
            {
                context.Sports.Add(s);
            }
            context.SaveChanges();


        }
    }
}
