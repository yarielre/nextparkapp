using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NextPark.Data.Mappings;
using NextPark.Domain.Auth;
using NextPark.Domain.Entities;
using System.Threading;

namespace NextPark.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserEntityMap());
            builder.ApplyConfiguration(new ParkingEntityMap());
            builder.ApplyConfiguration(new OrdersEntityMap());
            builder.ApplyConfiguration(new EventsEntityMap());
            builder.ApplyConfiguration(new CarPlatesEntityMap());
            builder.ApplyConfiguration(new ScheduleEntityMap());
        }

        #region DbSets
        public new DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<CarPlate> CarPlates { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        #endregion

        public virtual void Commit()
        {
            SaveChanges();
        }

        public virtual async void CommitAsync(CancellationToken cancellationToken)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }
}
