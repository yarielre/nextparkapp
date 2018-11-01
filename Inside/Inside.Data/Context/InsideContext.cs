using System.Threading;
using Inside.Data.Mappings;
using Inside.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inside.Data.Context
{
    public class InsideContext : IdentityDbContext<User,Role,int>
    {
        public InsideContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users"); // Use ApplicationUser instead
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.ApplyConfiguration(new EventMap());
            modelBuilder.ApplyConfiguration(new ParkingMap());
            modelBuilder.ApplyConfiguration(new ParkingCategoryMap());
            modelBuilder.ApplyConfiguration(new ParkingTypeMap());
            modelBuilder.ApplyConfiguration(new OrderMap());
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public virtual void Commit()
        {
            SaveChanges();
        }

        public virtual async void CommitAsync(CancellationToken cancellationToken)
        {
            await SaveChangesAsync(cancellationToken);
        }

        #region DbSets
        public new DbSet<User> Users { get; set; }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<ParkingCategory> ParkingCategories { get; set; }
        public DbSet<ParkingType> ParkingTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Event> Events { get; set; }
        #endregion
    }
}