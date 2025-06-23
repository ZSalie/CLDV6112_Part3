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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the primary key for the User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            // Configure the primary key for the Venue entity
            modelBuilder.Entity<Venue>()
                .HasKey(v => v.VenueId);

            // Configure the primary key for the Event entity
            modelBuilder.Entity<Event>()
                .HasKey(e => e.EventId);

            // Configure the primary key for the Booking entity
            modelBuilder.Entity<Booking>()
                .HasKey(b => b.BookingId);

            // Configure the primary key for the EventType entity
            modelBuilder.Entity<EventType>()
                .HasKey(et => et.EventTypeId);

            modelBuilder.Entity<EventType>()
                .HasData(
                    new EventType { EventTypeId = 1, Name = "Comedy" },
                    new EventType { EventTypeId = 2, Name = "Concert" },
                    new EventType { EventTypeId = 3, Name = "Conference" },
                    new EventType { EventTypeId = 4, Name = "eSports" },
                    new EventType { EventTypeId = 5, Name = "Sports" },
                    new EventType { EventTypeId = 6, Name = "Theater" },
                    new EventType { EventTypeId = 7, Name = "Workshop" }
                );
        }
    }
}
