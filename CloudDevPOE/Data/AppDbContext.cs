using CloudDevPOE.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudDevPOE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; } = default!;
        public DbSet<CloudDevPOE.Models.Venue> Venue { get; set; } = default!;
        public DbSet<CloudDevPOE.Models.Event> Event { get; set; } = default!;
        public DbSet<CloudDevPOE.Models.Booking> Booking { get; set; } = default!;
    }
}
