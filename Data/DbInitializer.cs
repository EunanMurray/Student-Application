using System;
using System.Linq;
using ScholarshipInfoSystem.Models; 
using ScholarshipInfoSystem.Data;   

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

        }
    }
}
