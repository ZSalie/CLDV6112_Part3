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
        public DbSet<Venue> Venue { get; set; } = default!;
        public DbSet<Event> Event { get; set; } = default!;
        public DbSet<Booking> Booking { get; set; } = default!;
        public DbSet<EventType> EventType { get; set; } = default!;
    }
}
