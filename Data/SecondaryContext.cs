using Microsoft.EntityFrameworkCore;
using ScholarshipInfoSystem.Models;

namespace ScholarshipInfoSystem.Data
{
    public class SecondaryContext : DbContext
    {
        public SecondaryContext(DbContextOptions<SecondaryContext> options) : base(options)
        {
        }

        // Define DbSets for entities managed by SecondaryContext if any
        // For example, large log tables, archives, etc.
    }
}
